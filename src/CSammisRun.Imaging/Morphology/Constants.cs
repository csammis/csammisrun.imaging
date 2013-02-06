namespace CSammisRun.Imaging.Morphology
{
    /// <summary>
    /// A container for constants relating to 1bpp image morphology
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The value of a pixel that is colored in a 1bpp image
        /// </summary>
        public const byte PIXEL_VALUE_INK = 0x00;
        /// <summary>
        /// The value of a pixel that is whitespace in a 1bpp image
        /// </summary>
        public const byte PIXEL_VALUE_WHITESPACE = 0xFF;
    }
}
