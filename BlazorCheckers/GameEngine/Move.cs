using Newtonsoft.Json;

namespace BlazorCheckers.GameEngine
{
    public class Move
    {
        [JsonProperty]
        public int StartRow { get; }
        [JsonProperty]
        public int StartColumn { get; }
        [JsonProperty]
        public int EndRow { get; }
        [JsonProperty]
        public int EndColumn { get; }

        [JsonConstructor]
        public Move(int startRow, int startColumn, int endRow, int endColumn)
        {
            StartRow = startRow;
            StartColumn = startColumn;
            EndRow = endRow;
            EndColumn = endColumn;
        }

        public Move(Cell startCell, Cell endCell)
            : this(startCell.Row, startCell.Column, endCell.Row, endCell.Column)
        {

        }

        public bool IsEqual(Move other)
        {
            return StartRow == other.StartRow && StartColumn == other.StartColumn &&
                EndRow == other.EndRow && EndColumn == other.EndColumn;
        }
    }
}
