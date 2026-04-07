using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Extensions
{
    internal static class ColorExtensions
    {
        public static string ToCssColor(this UnityEngine.Color color)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "#{0:X2}{1:X2}{2:X2}",
                Math.Min(255, (int)(color.r * 255.0f)),
                Math.Min(255, (int)(color.g * 255.0f)),
                Math.Min(255, (int)(color.b * 255.0f)));
        }
    }
}
