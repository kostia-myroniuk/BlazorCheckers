using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCheckers.Game
{
    public class Game
    {
        private readonly Dictionary<Side, Side> oppositePlayers =
            new Dictionary<Side, Side>()
            {
                { Side.Light, Side.Dark },
                { Side.Dark, Side.Light }
            };

        public Board Board { get; }
        public Side CurrentPlayer { get; private set; }

        public Game()
        {
            Board = new Board();
        }

        public void MakeMove(List<Cell> moveCells)
        {
            if (moveCells.Count == 0)
            {
                return;
            }

            Cell startCell = moveCells[0];
            if (startCell.Piece == null || startCell.Piece.Side != CurrentPlayer)
            {
                return;
            }

            List<List<Cell>> validMoves = FindValidMovesFromCell(startCell);
            if (validMoves.Contains(moveCells))
            {
                CurrentPlayer = oppositePlayers[CurrentPlayer];
            }
        }

        private List<List<Cell>> FindAllValidMoves()
        {
            List<List<Cell>> validMoves = new List<List<Cell>>();
            for (int row = 0; row < Board.SIZE; row++)
            {
                for (int col = 0; col < Board.SIZE; col++)
                {
                    if (Board.Cells[row][col].Piece?.Side == CurrentPlayer)
                    {
                        validMoves.AddRange(FindValidMovesFromCell(Board.Cells[row][col]));
                    }
                }
            }
            return validMoves;
        }

        private List<List<Cell>> FindValidMovesFromCell(Cell startCell)
        {
            List<List<Cell>> moves = new List<List<Cell>>();
            if (startCell.Piece is null)
            {
                return moves;
            }

            List<List<Cell>> diagonals = new List<List<Cell>>();

            int rowChange = startCell.Piece.Side == Side.Light ? -1 : 1;

            diagonals.Add(GetCellsDiagonal(startCell, rowChange, -1));
            diagonals.Add(GetCellsDiagonal(startCell, rowChange, 1));
            if (startCell.Piece.CanMoveBackwards)
            {
                diagonals.Add(GetCellsDiagonal(startCell, -rowChange, -1));
                diagonals.Add(GetCellsDiagonal(startCell, -rowChange, 1));
            }

            foreach (List<Cell> diagonal in diagonals)
            {
                if (!startCell.Piece.CanFly)
                {
                    if (diagonal.Count > 0 && diagonal[0].Piece == null)
                    {
                        moves.Add(new List<Cell>() { startCell, diagonal[0] });
                    }
                }
                else
                {
                    foreach (var diagonalCell in diagonal)
                    {
                        if (diagonalCell.Piece == null)
                        {
                            moves.Add(new List<Cell>() { startCell, diagonalCell });
                        }
                    }
                }
            }

            return moves;
        }

        private List<Cell> GetCellsDiagonal(Cell startCell, int rowChange, int colChange)
        {
            List<Cell> cells = new List<Cell>();
            int row = startCell.Row + rowChange;
            int col = startCell.Column + colChange;
            for ( ; CoordinatesAreValid(row, col); row += rowChange, col += colChange)
            {
                cells.Add(Board.Cells[row][col]);
            }
            return cells;
        }

        private bool CoordinatesAreValid(int row, int column)
        {
            return row >= 0 && row < Board.SIZE &&
                column >= 0 && column < Board.SIZE;
        }
    }
}
