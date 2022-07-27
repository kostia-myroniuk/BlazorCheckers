namespace BlazorCheckers.GameEngine
{
    public class Piece
    {
        public bool CanMoveBackwards { get; }
        public bool CanCaptureBackwards { get; }
        public bool CanFly { get; }
        public Side Side { get; }
        public PieceRank Kind { get; }

        public Piece(Side side, PieceRank kind)
        {
            Side = side;
            Kind = kind;

            if (kind == PieceRank.Pawn)
            {
                CanMoveBackwards = false;
                CanCaptureBackwards = false;
                CanFly = false;
            }
            else
            {
                CanMoveBackwards = true;
                CanCaptureBackwards = true;
                CanFly = true;
            }
        }
    }
}
