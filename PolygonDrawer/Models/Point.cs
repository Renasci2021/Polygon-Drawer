namespace PolygonDrawer.Models
{
    public readonly struct Vector(double x, double y)
    {
        public double X { get; } = x;
        public double Y { get; } = y;

        public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector operator *(double scalar, Vector a) => new(a.X * scalar, a.Y * scalar);

        public double SquaredLength => X * X + Y * Y;
        public static double DotProduct(Vector a, Vector b) => a.X * b.X + a.Y * b.Y;
        public static double CrossProduct(Vector a, Vector b) => a.X * b.Y - a.Y * b.X;
    }
}