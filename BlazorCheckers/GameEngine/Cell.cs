using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCheckers.Game
{
    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        public CellColor Color => (Row % 2 == Column % 2) ? CellColor.Light : CellColor.Dark;
        public Piece? Piece { get; set; } 

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
