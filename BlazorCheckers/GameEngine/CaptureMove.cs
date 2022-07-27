namespace BlazorCheckers.GameEngine
{
    public class CaptureMove : IEquatable<CaptureMove>
    {
        public Position Start { get; }
        public Position Target { get; }
        public Position End { get; }

        public CaptureMove(Position start, Position target, Position end)
        {
            Start = start;
            Target = target;
            End = end;
        }

        public override bool Equals(object obj) => this.Equals(obj as CaptureMove);

        public bool Equals(CaptureMove p)
        {
            if (p is null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }
            if (this.GetType() != p.GetType())
            {
                return false;
            }
            return (Start == p.Start) && (Target == p.Target) && (End == p.End);
        }

        public override int GetHashCode() => (Start, End).GetHashCode();

        public static bool operator ==(CaptureMove lhs, CaptureMove rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CaptureMove lhs, CaptureMove rhs) => !(lhs == rhs);
    }
}
