using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCheckers.Game
{
    public class Board
    {
        public const int SIZE = 8;
        public const int FILLED_ROWS = 3;

        public List<List<Cell>> Cells { get; set; }

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
                            cell.Piece = new Piece(Side.Light, PieceKind.Pawn);
                        }
                        else if (row >= SIZE - FILLED_ROWS)
                        {
                            cell.Piece = new Piece(Side.Dark, PieceKind.Pawn);
                        }
                    }
                    Cells[row].Add(cell);
                }
            }
        }
    }
}
