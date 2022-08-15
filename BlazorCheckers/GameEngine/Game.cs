using Newtonsoft.Json;

namespace BlazorCheckers.GameEngine
{
    public enum GameState
    { 
        InProgress,
        LightPlayerWon,
        DarkPlayerWon
    }

    public class Game
    {
        private readonly List<Direction> allDirections;
        private readonly List<Direction> upDirections;
        private readonly List<Direction> downDirections;

        [JsonProperty]
        public Board Board { get; }
        [JsonProperty]
        public Side CurrentSide { get; private set; }
        [JsonProperty]
        public List<Piece> LostPieces { get; }
        [JsonProperty]
        public List<CaptureSequence> Sequences { get; }
        [JsonProperty]
        public List<Move> RegularMoves { get; }
        [JsonProperty]
        public List<Move> CurrentMoves { get; }
        [JsonProperty]
        public GameState CurrentState { get; private set; }


        [JsonProperty]
        private RuleSet ruleSet;

        public Game(GameMode gameMode)
        {
            Board = new Board();
            LostPieces = new List<Piece>();
            RegularMoves = new List<Move>();
            ruleSet = new RuleSet(gameMode);

            allDirections = Enum.GetValues<Direction>().ToList();
            upDirections = allDirections.Where(d => d.ToString().Contains("Up")).ToList();
            downDirections = allDirections.Where(d => d.ToString().Contains("Down")).ToList();

            Sequences = new List<CaptureSequence>();
            RegularMoves = new List<Move>();
            CurrentMoves = new List<Move>();
            FindValidMoves();
        }

        [JsonConstructor]
        public Game(Board board, Side currentSide, List<Piece> lostPieces, List<CaptureSequence> sequences, 
            List<Move> regularMoves, List<Move> currentMoves, GameState currentState, RuleSet ruleSet)
        {
            Board = board;
            CurrentSide = currentSide;
            LostPieces = lostPieces;
            Sequences = sequences;
            RegularMoves = regularMoves;
            CurrentMoves = currentMoves;
            CurrentState = currentState;
            this.ruleSet = ruleSet;

            allDirections = Enum.GetValues<Direction>().ToList();
            upDirections = allDirections.Where(d => d.ToString().Contains("Up")).ToList();
            downDirections = allDirections.Where(d => d.ToString().Contains("Down")).ToList();
        }

        public void ApplyRegularMove(Move move)
        {
            if (!RegularMoves.Any(m => m.IsEqual(move)))
            {
                return;
            }

            Cell startCell = Board.GetCell(move.StartRow, move.StartColumn);
            Cell endCell = Board.GetCell(move.EndRow, move.EndColumn);

            endCell.Piece = startCell.Piece;
            startCell.Piece = null;
            TryPromotingPiece(endCell);

            EndTurn();
        }

        public void ApplyCaptureMove(Move move)
        {
            var sequenceMoves = new List<Move>(CurrentMoves);
            sequenceMoves.Add(move);

            CaptureSequence? sequence = Sequences
                .FirstOrDefault(s => s.StartsWith(sequenceMoves));
            
            if (sequence is null)
            {
                return;
            }

            Cell capturedCell = sequence.CapturedCells[sequenceMoves.Count - 1];
            Cell startCell = Board.GetCell(move.StartRow, move.StartColumn);
            Cell endCell = Board.GetCell(move.EndRow, move.EndColumn);

            if (capturedCell.Piece is not null)
            {
                LostPieces.Add(capturedCell.Piece);
                capturedCell.Piece = null;
            }

            endCell.Piece = startCell.Piece;
            startCell.Piece = null;
            TryPromotingPiece(endCell);

            CurrentMoves.Add(move);

            bool hasNextMoves = Sequences.Any(s => s.StartsWith(sequenceMoves) && 
                s.Moves.Count > sequenceMoves.Count);

            if (!hasNextMoves)
            {
                EndTurn();
            }
        }

        private void TryPromotingPiece(Cell cell)
        {
            if (cell.Piece is not null && cell.Piece.Rank == PieceRank.Pawn && 
                Board.IsCrowningCell(cell, CurrentSide))
            {
                cell.Piece = new Piece(CurrentSide, PieceRank.King);
            }
        }

        private void EndTurn()
        {
            CurrentSide = CurrentSide == Side.Light ? Side.Dark : Side.Light;
            CurrentMoves.Clear();
            FindValidMoves();
            UpdateGameState();
        }

        private void UpdateGameState()
        {
            if ((RegularMoves.Count == 0 && Sequences.Count == 0) ||
                !Board.Cells.Any(c => c.Piece is not null && c.Piece.Side == CurrentSide))
            {
                CurrentState = GetSideLosingState(CurrentSide);
            }
        }

        public void EndGameEarly(Side gaveUpSide)
        {
            CurrentState = GetSideLosingState(gaveUpSide);
        }

        private GameState GetSideLosingState(Side side)
        {
            return side == Side.Light ? GameState.DarkPlayerWon : GameState.LightPlayerWon;
        }

        private void FindValidMoves()
        {
            Sequences.Clear();
            RegularMoves.Clear();

            List<Cell> currentSideCells = Board.Cells.Where(c => c.Piece?.Side == CurrentSide).ToList();
            currentSideCells.ForEach(c => Sequences.AddRange(GetCaptureSequences(c)));

            if (Sequences.Count == 0)
            {
                currentSideCells.ForEach(c => RegularMoves.AddRange(GetRegularMoves(c)));
            }
        }

        private List<Move> GetRegularMoves(Cell startCell)
        {
            if (startCell.Piece == null || startCell.Piece.Side != CurrentSide)
            {
                return new List<Move>();
            }

            var reachableCells = new List<Cell>();
            bool canFly = startCell.Piece.Rank == PieceRank.King && ruleSet.KingsFly;
            bool canMoveBackwards = startCell.Piece.Rank == PieceRank.King;
            int? maxSize = canFly ? null : 1;

            foreach (var direction in GetDirections(startCell.Piece.Side, canMoveBackwards))
            {
                List<Cell> diagonal = Board.GetDiagonal(startCell, direction, maxSize);
                reachableCells.AddRange(diagonal.TakeWhile(c => c.Piece is null));
            }

            return reachableCells.Select(c => new Move(startCell, c)).ToList();
        }

        private List<CaptureSequence> GetCaptureSequences(Cell startCell)
        {
            if (startCell.Piece == null || startCell.Piece.Side != CurrentSide)
            {
                return new List<CaptureSequence>();
            }

            var sequences = new List<CaptureSequence>();
            var sequenceStack = new Stack<(CaptureSequence, Piece)>();

            sequenceStack.Push((new CaptureSequence(), startCell.Piece));

            while (sequenceStack.Count > 0)
            {
                CaptureSequence topSequence = sequenceStack.Peek().Item1;
                Piece topPiece = sequenceStack.Peek().Item2;
                sequenceStack.Pop();

                bool canFly = topPiece.Rank == PieceRank.King && ruleSet.KingsFly;
                bool canMoveBackwards = topPiece.Rank == PieceRank.King || ruleSet.PawnsCaptureBackwards;
                int? maxSize = canFly ? null : 2;
                
                Cell lastCell = GetCaptureSequenceLastCell(topSequence, startCell);
                bool foundNewMoves = false;

                foreach (var direction in GetDirections(CurrentSide, canMoveBackwards))
                {
                    List<Cell> diagonal = Board.GetDiagonal(lastCell, direction, maxSize)
                        .SkipWhile(c => c.Piece is null).ToList();
                    
                    if (diagonal.Count < 2 || diagonal[0].Piece?.Side == CurrentSide)
                    {
                        continue;
                    }

                    Cell capturedCell = diagonal[0];
                    if (topSequence.CapturedCells.Contains(capturedCell))
                    {
                        continue;
                    }

                    List<Cell> endCells = diagonal.Skip(1).TakeWhile(c => c.Piece == null).ToList();

                    foreach (var endCell in endCells)
                    {
                        var nextMove = new Move(lastCell, endCell);
                        CaptureSequence nextSequence = topSequence.Copy();
                        nextSequence.AddCaptureMove(nextMove, capturedCell);

                        var nextPiece = topPiece;
                        if (ruleSet.PawnsPromoteDuringMove && topPiece.Rank == PieceRank.Pawn && 
                            Board.IsCrowningCell(endCell, CurrentSide))
                        {
                            nextPiece = new Piece(CurrentSide, PieceRank.Pawn);
                        }

                        sequenceStack.Push((nextSequence, nextPiece));
                        foundNewMoves = true;
                    }
                }

                if (!foundNewMoves && topSequence.Moves.Count > 0)
                {
                    sequences.Add(topSequence);
                }
            }

            if (ruleSet.MaximalSequenceChosen)
            {
                CaptureSequence? maxSequence = sequences.MaxBy(s => s.Moves.Count);
                if (maxSequence is null)
                {
                    return new List<CaptureSequence>();
                }

                return sequences.Where(s => s.Moves.Count == maxSequence.Moves.Count).ToList();
            }

            return sequences;
        }

        private Cell GetCaptureSequenceLastCell(CaptureSequence sequence, Cell startCell)
        {
            if (sequence.Moves.Count == 0)
            {
                return startCell;
            }
            
            Move lastMove = sequence.Moves.Last();
            return Board.GetCell(lastMove.EndRow, lastMove.EndColumn);
        }

        private List<Direction> GetDirections(Side side, bool canMoveBackwards)
        {
            if (canMoveBackwards)
            {
                return allDirections;
            }
            return side == Side.Light ? downDirections : upDirections;
        }

        public static Side GetOppositeSide(Side side)
        {
            return side == Side.Light ? Side.Dark : Side.Light;
        }
    }
}
