using Newtonsoft.Json;

namespace BlazorCheckers.GameEngine
{
    public class Board
    {
        public const int Size = 8;
        public const int PawnRows = 3;

        private readonly Dictionary<Direction, (int, int)> directionOffsets =
            new Dictionary<Direction, (int, int)>()
            {
                { Direction.UpLeft, (-1, -1) },
                { Direction.UpRight, (-1, 1) },
                { Direction.DownLeft, (1, -1) },
                { Direction.DownRight, (1, 1) }
            };

        [JsonProperty]
        public List<Cell> Cells { get; }

        public Board()
        {
            Cells = new List<Cell>();
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    Cells.Add(new Cell(row, col));
                }
            }

            var darkCells = Cells.Where(c => c.Color == CellColor.Dark);

            List<Cell> lightPawnCells = darkCells.Where(c => c.Row < PawnRows).ToList();
            lightPawnCells.ForEach(c => c.Piece = new Piece(Side.Light, PieceRank.Pawn));

            List<Cell> darkPawnCells = darkCells.Where(c => c.Row >= Size - PawnRows).ToList();
            darkPawnCells.ForEach(c => c.Piece = new Piece(Side.Dark, PieceRank.Pawn));
        }

        [JsonConstructor]
        public Board(List<Cell> cells)
        {
            Cells = cells;
        }

        public List<Cell> GetCellsRotated(Side side)
        {
            if (side == Side.Dark)
            {
                return Cells;
            }

            var cellsRotated = new List<Cell>();
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    cellsRotated.Add(GetCell(Size - row - 1, Size - col - 1));
                }
            }
            return cellsRotated;
        }

        public Cell GetCell(int row, int column)
        {
            return Cells[row * Size + column];
        }

        public Cell? GetNextCell(Cell startCell, Direction direction)
        {
            int row = startCell.Row + directionOffsets[direction].Item1;
            int column = startCell.Column + directionOffsets[direction].Item2;

            if (!IsValidPosition(row, column))
            {
                return null;
            }
            return GetCell(row, column);
        }

        public List<Cell> GetDiagonal(Cell startCell, Direction direction, int? maxSize = null)
        {
            List<Cell> diagonal = new List<Cell>();
            Cell? cell = GetNextCell(startCell, direction);
            
            while (cell is not null)
            {
                diagonal.Add(cell);
                cell = GetNextCell(cell, direction);
                if (maxSize != null && diagonal.Count >= maxSize)
                {
                    break;
                }
            }
            return diagonal;
        }

        public bool IsValidPosition(int row, int column)
        {
            return row >= 0 && row < Size && column >= 0 && column < Size;
        }

        public bool IsCrowningCell(Cell cell, Side side)
        {
            return (cell.Row == 0 && side == Side.Dark) ||
                (cell.Row == Size - 1 && side == Side.Light);
        }
    }
}
