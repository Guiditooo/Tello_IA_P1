using System;

namespace FlyEngine
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(Vector2 other)
        {
            return Math.Abs(x - other.x);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            return (float)Math.Sqrt(x * x + y * y);
        }
    }
}