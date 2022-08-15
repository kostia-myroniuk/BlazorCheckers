using Newtonsoft.Json;

namespace BlazorCheckers.GameEngine
{
    public class CaptureSequence
    {
        [JsonProperty]
        public List<Move> Moves { get; }
        [JsonProperty]
        public List<Cell> CapturedCells { get; }

        public CaptureSequence()
        {
            Moves = new List<Move>();
            CapturedCells = new List<Cell>();
        }

        [JsonConstructor]
        public CaptureSequence(List<Move> moves, List<Cell> capturedCells)
        {
            Moves = moves;
            CapturedCells = capturedCells;
        }

        public void AddCaptureMove(Move move, Cell capturedCell)
        {
            Moves.Add(move);
            CapturedCells.Add(capturedCell);
        }

        public CaptureSequence Copy()
        {
            return new CaptureSequence(new List<Move>(Moves), new List<Cell>(CapturedCells));
        }

        public bool StartsWith(List<Move> startMoves)
        {
            if (startMoves.Count > Moves.Count)
            {
                return false;
            }
            for (int i = 0; i < startMoves.Count; i++)
            {
                if (!startMoves[i].IsEqual(Moves[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
