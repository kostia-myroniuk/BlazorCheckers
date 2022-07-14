namespace ConsoleCheckers.Game
{
    public class Piece
    {
        public bool CanMoveBackwards { get; }
        public bool CanCaptureBackwards { get; }
        public bool CanFly { get; }
        public Side Side { get; }
        public PieceKind Kind { get; }

        public Piece(Side side, PieceKind kind)
        {
            Side = side;
            Kind = kind;

            if (kind == PieceKind.Pawn)
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
