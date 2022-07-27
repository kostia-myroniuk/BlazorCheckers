using System.Diagnostics.CodeAnalysis;

namespace BlazorCheckers.GameEngine
{
    public struct Position : IEquatable<Position>
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public void AdjustInDirection(Direction direction)
        {
            Row += (direction == Direction.UpLeft || direction == Direction.UpRight) ? -1 : 1;
            Column += (direction == Direction.UpLeft || direction == Direction.DownLeft) ? -1 : 1;
        }

        public Position Adjusted(Direction direction, int amount = 1)
        {
            int row = Row + (DirectionIsUp(direction) ? -amount : amount);
            int column = Column + (DirectionIsLeft(direction) ? -amount : amount);
            return new Position(row, column);
        }

        private bool DirectionIsUp(Direction dir)
        {
            return dir == Direction.UpLeft || dir == Direction.UpRight;
        }

        private bool DirectionIsLeft(Direction dir)
        {
            return dir == Direction.UpLeft || dir == Direction.DownLeft;
        }

        public override bool Equals(object? obj) => obj is Position other && this.Equals(other);

        public bool Equals(Position p) => Row == p.Row && Column == p.Column;

        public override int GetHashCode() => (Row, Column).GetHashCode();

        public static bool operator ==(Position lhs, Position rhs) => lhs.Equals(rhs);

        public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);
    }
}
