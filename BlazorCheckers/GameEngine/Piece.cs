namespace BlazorCheckers.GameEngine
{
    public class Piece
    {
        public Side Side { get; }
        public PieceRank Rank { get; }

        public Piece(Side side, PieceRank rank)
        {
            Side = side;
            Rank = rank;
        }
    }
}
