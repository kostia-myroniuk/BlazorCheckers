namespace BlazorCheckers.GameEngine
{
    public class RegularMove : IEquatable<RegularMove>
    {
        public Position Start { get; }
        public Position End { get; }

        public RegularMove(Position start, Position end)
        {
            Start = start;
            End = end;
        }

        public override bool Equals(object obj) => this.Equals(obj as RegularMove);

        public bool Equals(RegularMove p)
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
            return (Start == p.Start) && (End == p.End);
        }

        public override int GetHashCode() => (Start, End).GetHashCode();

        public static bool operator ==(RegularMove lhs, RegularMove rhs)
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

        public static bool operator !=(RegularMove lhs, RegularMove rhs) => !(lhs == rhs);
    }
}
