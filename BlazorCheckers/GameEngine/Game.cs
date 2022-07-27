using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCheckers.GameEngine
{
    public class Game
    {
        private readonly Dictionary<Side, Side> oppositePlayers =
            new Dictionary<Side, Side>()
            {
                { Side.Light, Side.Dark },
                { Side.Dark, Side.Light }
            };

        private readonly List<Direction> upDirections =
            new List<Direction>() 
            { 
                Direction.UpLeft, Direction.UpRight 
            };
        private readonly List<Direction> downDirections =
            new List<Direction>()
            { 
                Direction.UpLeft, Direction.UpRight, 
                Direction.DownLeft, Direction.DownRight 
            };
        private readonly List<Direction> allDirections =
            new List<Direction>()
            {
                Direction.DownLeft, Direction.DownRight
            };

        public Board Board { get; }
        public Side CurrentPlayer { get; private set; }
        public List<List<Cell>> ValidMoves { get; private set; } = new List<List<Cell>>();
        public List<RegularMove> RegularMoves { get; private set; } = new List<RegularMove>();
        public List<List<CaptureMove>> CaptureMoves { get; private set; } = new List<List<CaptureMove>>();

        private List<CaptureMove> currentMove = new List<CaptureMove>();
        private List<Piece> lostPieces = new List<Piece>();

        public Game()
        {
            Board = new Board();
            CalculateValidMoves();
        }

        private void CalculateValidMoves()
        {
            CaptureMoves.Clear();
            RegularMoves.Clear();

            for (int row = 0; row < Board.SIZE; row++)
            {
                for (int col = 0; col < Board.SIZE; col++)
                {
                    if (Board.Cells[row][col].Piece?.Side == CurrentPlayer)
                    {
                        CaptureMoves.AddRange(GetCaptureMoves(Board.Cells[row][col]));
                        RegularMoves.AddRange(GetRegularMoves(Board.Cells[row][col]));
                    }
                }
            }

            if (CaptureMoves.Count > 0)
            {
                RegularMoves.Clear();
            }

            System.Diagnostics.Debug.WriteLine("Capture moves:");
            foreach (var sequence in CaptureMoves)
            {
                foreach (var move in sequence)
                {
                    System.Diagnostics.Debug.Write($"({move.Start.Row} {move.Start.Column} -> {move.Target.Row} {move.Target.Column} -> {move.End.Row} {move.End.Column}) ");
                }
                System.Diagnostics.Debug.WriteLine("");
            }

            System.Diagnostics.Debug.WriteLine("Regular moves:");
            foreach (var move in RegularMoves)
            {
                System.Diagnostics.Debug.WriteLine($"{move.Start.Row} {move.Start.Column} -> {move.End.Row} {move.End.Column}");
            }
        }

        private List<List<CaptureMove>> GetCaptureMoves(Cell startCell)
        {
            var moves = new List<List<CaptureMove>>();
            if (startCell.Piece == null)
            {
                return moves;
            }

            var moveStack = new Stack<(Piece, Cell, List<CaptureMove>)>();
            moveStack.Push(new(startCell.Piece, startCell, new List<CaptureMove>()));

            while (moveStack.Count > 0)
            {
                var topItem = moveStack.Pop();
                var movePiece = topItem.Item1;
                var fromCell = topItem.Item2;
                var move = topItem.Item3;
                
                bool foundNewMoves = false;
                foreach (var direction in allDirections)
                {
                    var targetPosition = fromCell.GetPosition().Adjusted(direction, 1);
                    var nextPosition = fromCell.GetPosition().Adjusted(direction, 2);

                    while (Board.PositionIsValid(nextPosition))
                    {
                        var targetCell = Board.GetCell(targetPosition);
                        var nextCell = Board.GetCell(nextPosition);
                        if (CanCapture(targetCell, nextCell, startCell.Piece.Side, move))
                        {
                            var nextMove = new List<CaptureMove>(move);
                            nextMove.Add(new CaptureMove(fromCell.GetPosition(),
                                targetCell.GetPosition(), nextCell.GetPosition()));
                            var nextMovePiece = movePiece;
                            if (Board.IsCrowningCell(nextCell, startCell.Piece.Side) &&
                                nextMovePiece.Kind == PieceRank.Pawn)
                            {
                                nextMovePiece = new Piece(startCell.Piece.Side, PieceRank.King);
                            }

                            moveStack.Push(new(nextMovePiece, nextCell, nextMove));
                            foundNewMoves = true;
                        }
                        else if (targetCell.Piece != null)
                        {
                            break;
                        }

                        if (!startCell.Piece.CanFly)
                        {
                            break;
                        }

                        targetPosition.AdjustInDirection(direction);
                        nextPosition.AdjustInDirection(direction);
                    }
                }

                if (!foundNewMoves)
                {
                    moves.Add(move);
                }
            }

            return moves;
        }

        private bool CanCapture(Cell target, Cell next, Side side, List<CaptureMove> move)
        {
            return target.Piece != null && target.Piece.Side != side &&
                !move.Any(cm => cm.Target == target.GetPosition());
        }

        private List<RegularMove> GetRegularMoves(Cell startCell)
        {
            List<RegularMove> moves = new List<RegularMove>();
            if (startCell.Piece == null)
            {
                return moves;
            }

            foreach (var direction in GetDirectionsForPiece(startCell.Piece))
            {
                Position position = startCell.GetPosition().Adjusted(direction);
                
                for (Cell? cell = Board.GetCell(position); cell != null && cell.Piece == null; 
                    position = position.Adjusted(direction))
                {
                    moves.Add(new RegularMove(startCell.GetPosition(), cell.GetPosition()));
                    if (!startCell.Piece.CanFly)
                    {
                        break;
                    }
                }
            }
            return moves;
        }

        private List<Direction> GetDirectionsForPiece(Piece piece)
        {
            if (piece.Kind == PieceRank.King)
            {
                return allDirections;
            }
            return piece.Side == Side.Light ? downDirections : upDirections;
        }

        public void TryPromoting(Cell endCell)
        {
            if (endCell.Piece?.Kind == PieceRank.Pawn &&
                Board.IsCrowningCell(endCell, CurrentPlayer))
            {
                endCell.Piece = new Piece(CurrentPlayer, PieceRank.King);
            }
        }

        public void ApplyRegularMove(RegularMove move)
        {
            Cell? startCell = Board.GetCell(move.Start);
            Cell? endCell = Board.GetCell(move.End);

            if (startCell is null || endCell is null ||
                !RegularMoves.Contains(move))
            {
                return;
            }

            endCell.Piece = startCell.Piece;
            TryPromoting(endCell);
            startCell.Piece = null;

            CurrentPlayer = oppositePlayers[CurrentPlayer];
            currentMove.Clear();
            CalculateValidMoves();
        }

        public CaptureMove? GetCaptureMove(RegularMove move)
        {
            foreach (var validMove in CaptureMoves)
            {
                if (validMove.Count < currentMove.Count)
                {
                    continue;
                }
                bool startEqually = true;
                for (int i = 0; i < currentMove.Count; i++)
                {
                    if (currentMove[i] != validMove[i])
                    {
                        startEqually = false;
                        break;
                    }
                }
                if (startEqually && validMove.Count >= currentMove.Count + 1 &&
                    validMove[currentMove.Count].Start == move.Start &&
                    validMove[currentMove.Count].End == move.End)
                {
                    return validMove[currentMove.Count];
                }
            }
            return null;
        }

        public void ApplyCaptureMove(CaptureMove move)
        {
            var resultingMove = new List<CaptureMove>(currentMove);
            resultingMove.Add(move);
            bool fits = false;
            foreach (var validMove in CaptureMoves)
            {
                if (validMove.Count < resultingMove.Count)
                {
                    continue;
                }
                bool startEqually = true;
                for (int i = 0; i < resultingMove.Count; i++)
                {
                    if (resultingMove[i] != validMove[i])
                    {
                        startEqually = false;
                        break;
                    }
                }
                if (startEqually)
                {
                    fits = true;
                    break;
                }
            }

            Cell? startCell = Board.GetCell(move.Start);
            Cell? targetCell = Board.GetCell(move.Target);
            Cell? endCell = Board.GetCell(move.End);

            if (startCell is null || targetCell is null ||
                endCell is null || !fits)
            {
                return;
            }

            endCell.Piece = startCell.Piece;
            TryPromoting(endCell);
            startCell.Piece = null;
            if (targetCell.Piece != null)
            {
                lostPieces.Add(targetCell.Piece);
                targetCell.Piece = null;
            }

            if (CaptureMoves.Contains(resultingMove))
            {
                CurrentPlayer = oppositePlayers[CurrentPlayer];
                currentMove.Clear();
                CalculateValidMoves();
            }
        }
    }
}
