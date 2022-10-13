using System.Threading;

namespace MakoIoT.Device.Displays.Led
{
    /// <summary>
    /// Logical RGB pixel
    /// </summary>
    public class RgbPixel
    {
        private const int DefaultSteps = 100;
        private const int DefaultDelay = 10;

        private readonly IPixelDriver _driver;
        
        /// <summary>
        /// Pixel color
        /// </summary>
        public Color Color { get; private set; }

        public RgbPixel(Color color, IPixelDriver driver)
        {
            Color = color;
            _driver = driver;
            _driver.SetPixel(Color);
        }

        public RgbPixel(IPixelDriver driver) : this(new Color(0, 0, 0), driver)
        {
        }

        /// <summary>
        /// Sets pixel's color
        /// </summary>
        /// <param name="color">The color</param>
        public void SetColor(Color color)
        {
            if (Equals(color, Color)) return;
            Color = color;
            _driver.SetPixel(color);
        }

        /// <summary>
        /// Transitions the pixel to new color in a separate thread
        /// </summary>
        /// <param name="color">The new color</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="steps">Number of transition steps</param>
        /// <param name="delay">Delay between each transition step</param>
        /// <returns>The new Thread or null if color is the same as current color</returns>
        public Thread TransitionAsync(Color color, CancellationToken cancellationToken, int steps = DefaultSteps, int delay = DefaultDelay)
        {
            if (Equals(color, Color)) return null;

            double dr = (double)(color.R - Color.R) / (double)steps;
            double dg = (double)(color.G - Color.G) / (double)steps;
            double db = (double)(color.B - Color.B) / (double)steps;

            var thread = new Thread(() => TransitionInternal(cancellationToken, steps, delay, dr, dg, db));

            thread.Start();

            return thread;
        }

        /// <summary>
        /// Transitions the pixel to new color
        /// </summary>
        /// <param name="color">The new color</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="steps">Number of transition steps</param>
        /// <param name="delay">Delay between each transition step</param>
        public void Transition(Color color, CancellationToken cancellationToken, int steps = DefaultSteps, int delay = DefaultDelay)
        {
            if (Equals(color, Color)) return;

            double dr = (double)(color.R - Color.R) / (double)steps;
            double dg = (double)(color.G - Color.G) / (double)steps;
            double db = (double)(color.B - Color.B) / (double)steps;

            TransitionInternal(cancellationToken, steps, delay, dr, dg, db);
        }

        /// <summary>
        /// Fades out to black and transitions the pixel to new color in a separate thread
        /// </summary>
        /// <param name="color">The new color</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="steps">Number of transition steps</param>
        /// <param name="delay">Delay between each transition step</param>
        /// <returns>The new Thread or null if color is the same as current color</returns>
        public Thread FadeTransitionAsync(Color color, CancellationToken cancellationToken, int steps = DefaultSteps, int delay = DefaultDelay)
        {
            var thread = new Thread(() =>
                {
                    TransitionAsync(new Color(0, 0, 0), cancellationToken, steps, delay)?.Join();
                    if (cancellationToken.IsCancellationRequested) return;
                    TransitionAsync(color, cancellationToken, steps, delay)?.Join();
                }
            );

            thread.Start();

            return thread;
        }

        /// <summary>
        /// Fades out and then back to current color in a separate thread
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="steps">Number of transition steps</param>
        /// <param name="delay">Delay between each transition step</param>
        /// <returns>The new Thread or null if color is the same as current color</returns>
        public Thread BlinkSmoothAsync(CancellationToken cancellationToken, int steps = DefaultSteps, int delay = DefaultDelay)
        {
            return FadeTransitionAsync(new Color(Color), cancellationToken, steps, delay);
        }

        private void TransitionInternal(CancellationToken cancellationToken, int steps, int delay, double dr, double dg,
            double db)
        {
            double cr = Color.R;
            double cg = Color.G;
            double cb = Color.B;

            for (int i = 0; i < steps && !cancellationToken.IsCancellationRequested; i++)
            {
                cr += dr;
                cg += dg;
                cb += db;

                SetColor(new Color((byte)cr, (byte)cg, (byte)cb));

                cancellationToken.WaitHandle.WaitOne(delay, false);
            }
        }
    }
}
