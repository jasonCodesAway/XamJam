namespace XamJam.Util
{
    public struct SizeInt
    {
        public int Width { get; }

        public int Height { get; }

        private readonly int hashCode;

        public SizeInt(int width, int height)
        {
            Width = width;
            Height = height;
            hashCode = Width * 3 + Height * 7;
        }

        public override bool Equals(object that)
        {
            return that is SizeInt &&
                ((SizeInt)that).hashCode == hashCode &&
                ((SizeInt)that).Width == Width &&
                ((SizeInt)that).Height == Height;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Width} x {Height}";
        }
    }
}
