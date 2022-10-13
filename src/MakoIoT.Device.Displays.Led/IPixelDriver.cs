namespace MakoIoT.Device.Displays.Led
{
    /// <summary>
    /// Interface for hardware driver to set pixel color
    /// </summary>
    public interface IPixelDriver
    {
        /// <summary>
        /// Sets hardware pixel color
        /// </summary>
        /// <param name="color">The color</param>
        void SetPixel(Color color);
    }
}
