namespace XamJam.Wall
{
    /// <summary>
    /// Interface for any algorithm that would like to determine how to fill up a wall (defined by screenWidth and screenHeight) with items.
    /// </summary>
    public interface WallSizer
    {
        WallSize Size(double screenWidth, double screenHeight);
    }
}