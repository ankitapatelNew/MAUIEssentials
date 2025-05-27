#if EXPORTFONTS
// Unused: Export the font to ensure it is visibility
namespace MAUIEssentials.Resources

[assembly: ExportFont("NotoNaskhArabic-Bold.ttf", Alias = "NakshBold")]
[assembly: ExportFont("NotoNaskhArabic-Medium.ttf", Alias = "NakshMedium")]
[assembly: ExportFont("NotoNaskhArabic-Regular.ttf", Alias = "NakshRegular")]
[assembly: ExportFont("Roboto-Bold.ttf", Alias = "RobotoBold")]
[assembly: ExportFont("Roboto-Light.ttf", Alias = "RobotoLight")]
[assembly: ExportFont("Roboto-Medium.ttf", Alias = "RobotoMedium")]
[assembly: ExportFont("Roboto-Regular.ttf", Alias = "RobotoRegular")]
[assembly: ExportFont("OpenSans-Regular.ttf", Alias = "OpenSansRegular")]
[assembly: ExportFont("OpenSans-Semibold.ttf", Alias = "OpenSansSemibold")]
#endif

namespace MAUIEssentials.Resources
{
    /// <summary>
    /// Provides a font description for loading a font stored as an embedded resource.
    /// </summary>
    public sealed class FontResource
    {
        static FontResource()
        {
            List<FontResource> defaults = new List<FontResource>()
            {
                new ("NotoNaskhArabic-Bold.ttf", "NakshBold"),
                new ("NotoNaskhArabic-Medium.ttf", "NakshMedium"),
                new ("NotoNaskhArabic-Regular.ttf", "NakshRegular"),
                new ("Roboto-Bold.ttf", "RobotoBold"),
                new ("Roboto-Light.ttf", "RobotoLight"),
                new ("Roboto-Medium.ttf", "RobotoMedium"),
                new ("Roboto-Regular.ttf", "RobotoRegular"),
                new ("OpenSans-Regular.ttf", "OpenSansRegular"),
                new ("OpenSans-Semibold.ttf", "OpenSansSemibold"),
            };
            Defaults = defaults;
        }

        static public readonly IEnumerable<FontResource> Defaults;

        /// <summary>
        /// Loads the <see cref="FontResource"/> instances into a <see cref="IFontCollection"/>.
        /// </summary>
        /// <param name="fonts">The <see cref="IFontCollection"/> to populate.</param>
        /// <param name="fontResources">An <see cref="IEnumerable{FontResource}"/> for iterating the fonts to load.</param>
        public static void Load(IFontCollection fonts, IEnumerable<FontResource> fontResources)
        {
            foreach (FontResource desc in fontResources)
            {
                fonts.AddFont(desc.FileName, desc.Alias);
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="fileName">The embedded resource's file name.</param>
        /// <param name="alias">The alias to use for the font. </param>
        public FontResource(string fileName, string alias)
        {
            FileName = fileName;
            Alias = alias;
        }

        /// <summary>
        /// Gets the filename for the embedded resource font.
        /// </summary>
        public string FileName
        {
            get;
        }

        /// <summary>
        /// Gets the alias for the font.
        /// </summary>
        public string Alias
        {
            get;
        }
    }
}
