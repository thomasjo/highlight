using System.Globalization;

// Replacement for System.Drawing.Color which is not available on non-Windows platforms
namespace Highlight
{
    public class Color
    {
        public string Name { get; set; } // Either Hex or Web Color Name
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public Color(string name, int r, int g, int b)
        {
            Name = name;
            R = r;
            G = g;
            B = b;
        }

        public static Color Empty = new Color("#FFFFFF", 0, 0, 0);

        public static Color FromName(string name)
        {
            // Todo: Add extended colors from https://en.wikipedia.org/wiki/Web_colors#HTML_color_names
            if (name.IndexOf("#") == -1)
            {
                name = name switch
                {
                    "white" => "#FFFFFF",
                    "silver" => "#C0C0C0",
                    "gray" => "#808080",
                    "black" => "#000000",
                    "red" => "#FF0000",
                    "maroon" => "#800000",
                    "yellow" => "#FFFF00",
                    "olive" => "#808000",
                    "lime" => "#00FF00",
                    "green" => "#008000",
                    "aqua" => "#00FFFF",
                    "teal" => "#008080",
                    "blue" => "#0000FF",
                    "navy" => "#000080",
                    "fuchsia" => "#FF00FF",
                    "purple" => "#800080",
                    _ => throw new System.Exception("Unknown color name: " + name),
                };
            }

            int red = int.Parse(name.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            int green = int.Parse(name.Substring(3, 2), NumberStyles.AllowHexSpecifier);
            int blue = int.Parse(name.Substring(5, 2), NumberStyles.AllowHexSpecifier);

            return new Color(name, red, green, blue);
        }
    }
}
