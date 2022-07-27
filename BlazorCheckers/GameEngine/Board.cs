using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCheckers.GameEngine
{
    public class Board
    {
        public const int SIZE = 8;
        public const int FILLED_ROWS = 3;

        public List<List<Cell>> Cells { get; set; } = new List<List<Cell>>();

        public Board()
        {
            Cells = new List<List<Cell>>();
            for (int row = 0; row < SIZE; row++)
            {
                Cells.Add(new List<Cell>());
                for (int col = 0; col < SIZE; col++)
                {
                    Cell cell = new Cell(row, col);
                    if (cell.Color == CellColor.Dark)
                    {
                        if (row < FILLED_ROWS)
                        {
                            cell.Piece = new Piece(Side.Light, PieceRank.Pawn);
                        }
                        else if (row >= SIZE - FILLED_ROWS)
                        {
                            cell.Piece = new Piece(Side.Dark, PieceRank.Pawn);
                        }
                    }
                    Cells[row].Add(cell);
                }
            }
        }

        public Board(List<List<Cell>> cells)
        {
            Cells = cells;
        }

        public List<Cell> GetCells(List<Position> coordinates)
        {
            List<Cell> cells = new List<Cell>();
            foreach (var coordinate in coordinates)
            {
                cells.Add(Cells[coordinate.Row][coordinate.Column]);
            }
            return cells;
        }

        public Cell GetCell(int row, int column)
        {
            return Cells[row][column];
        }

        public bool IsCrowningCell(Cell cell, Side side)
        {
            return (cell.Row == 0 && side == Side.Dark) ||
                (cell.Row == SIZE - 1 && side == Side.Light);
        }

        public Cell? GetCell(Position position)
        {
            if (!PositionIsValid(position))
            {
                return null;
            }
            return GetCell(position.Row, position.Column);
        }

        public bool PositionIsValid(Position position)
        {
            return position.Row >= 0 && position.Row < Board.SIZE &&
                position.Column >= 0 && position.Column < Board.SIZE;
        }
    }
}
