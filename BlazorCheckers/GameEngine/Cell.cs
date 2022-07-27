using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCheckers.GameEngine
{
    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        public Piece? Piece { get; set; }

        public CellColor Color => (Row % 2 == Column % 2) ? CellColor.Light : CellColor.Dark;
        public bool IsFree => Piece == null;

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Position GetPosition()
        {
            return new Position(Row, Column);
        }
    }
}
