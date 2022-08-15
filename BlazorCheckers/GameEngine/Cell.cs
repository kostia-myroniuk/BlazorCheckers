namespace BlazorCheckers.GameEngine
{
    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        public Piece? Piece { get; set; }

        public CellColor Color => (Row % 2 == Column % 2) ? CellColor.Light : CellColor.Dark;

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
