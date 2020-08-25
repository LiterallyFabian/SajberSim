using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SajberSim.Colors
{
    class Colors
    {
        // Dark purple used as default comment color in visual novels
        public static Color DarkPurple = new Color(0.2641509f, 0.04859381f, 0.2544991f, 1);
        // Default text color in Unity
        public static Color UnityGray = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1);
        // Dark red
        public static Color NsfwRed = new Color(0.5f, 0, 0, 1);
        // Dark green
        public static Color SfwGreen = new Color(0, 0.5f, 0, 1);
        // Slightly brighter purple used for outlining buttons
        public static Color ButtonOutline = new Color(0.4230333f, 0.07903168f, 0.4528302f, 1);
        // Boring purple-gray-ish used for titles
        public static Color TitleText = new Color(0.3019608f, 0.1568628f, 0.3490196f, 1);
        // Main color used in the logo
        public static Color LogoPurple = new Color(0.6313726f, 0.1254902f, 0.5529412f, 1);
        // Light blue used for ingame buttons and dropdowns
        public static Color IngameBlue = new Color(0.495283f, 0.6032573f, 1, 1);
        // Nuff said
        public static Color Transparent = new Color(0, 0, 0, 0);
        // Nuff said
        public static Color AlmostTransparent = new Color(0, 0, 0, 0.08f);

        public static Color FromRGB(string rgb)
        {
            if (rgb[0] != '#') rgb.Insert(0, "#");
            ColorUtility.TryParseHtmlString(rgb, out Color c);
            return c;
        }
    }
}
