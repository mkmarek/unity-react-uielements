using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge
{
    public static class StyleMapper
    {
        public static Color ParseColor(string hexColor)
        {
            var hexInString = hexColor.Substring(1);
            var color = int.Parse(hexInString, NumberStyles.HexNumber);

            if (hexInString.Length == 6)
            {
                Color col = new Color32(
                    (byte)(color >> 16 & 0xFF),
                    (byte)((color >> 8) & 0xFF),
                    (byte)(color & 0xFF),
                    0xFF);

                return col;
            }

            if (hexInString.Length == 8)
            {
                Color col = new Color32(
                    (byte)(color >> 24 & 0xFF),
                    (byte)(color >> 16 & 0xFF),
                    (byte)((color >> 8) & 0xFF),
                    (byte)(color & 0xFF));

                return col;
            }

            throw new FormatException($"Color can't have {hexInString.Length} digits");
        }

        public static StyleBackground ParseStyleBackground(string background)
        {
            var keyword = GetStyleKeyword(background);

            if (keyword.HasValue) return new StyleBackground(keyword.Value);

            var texture = Resources.Load<Texture2D>(background);

            if (texture != null)
            {
                return new StyleBackground(texture);
            }

            var vectorImage = Resources.Load<VectorImage>(background);

            if (vectorImage != null)
            {
                return new StyleBackground(vectorImage);
            }

            return new StyleBackground();
        }

        public static StyleInt ParseStyleInt(string style)
        {
            var keyword = GetStyleKeyword(style);

            if (keyword.HasValue) return new StyleInt(keyword.Value);

            int.TryParse(style, out var result);

            return new StyleInt(result);
        }

        public static StyleEnum<T> ParseStyleEnum<T>(string style)
            where T : struct, IConvertible
        {
            var keyword = GetStyleKeyword(style);

            if (keyword.HasValue) return new StyleEnum<T>(keyword.Value);

            Enum.TryParse<T>(style, out var result);

            return result;
        }

        public static StyleFloat ParseStyleFloat(string styleFloat)
        {
            var keyword = GetStyleKeyword(styleFloat);

            if (keyword.HasValue) return new StyleFloat(keyword.Value);
            if (float.TryParse(styleFloat, out var length)) return new StyleFloat(length);

            return default;
        }

        public static StyleLength ParseStyleLength(string styleWidth)
        {
            var keyword = GetStyleKeyword(styleWidth);

            if (keyword.HasValue) return new StyleLength(keyword.Value);
            if (float.TryParse(styleWidth, out var length)) return new StyleLength(length);

            if (styleWidth.EndsWith("%"))
                return new StyleLength(new Length(
                    float.Parse(styleWidth.Substring(0, styleWidth.Length - 1)), LengthUnit.Percent));
            if (styleWidth.EndsWith("px"))
                return new StyleLength(new Length(
                    float.Parse(styleWidth.Substring(0, styleWidth.Length - 2)), LengthUnit.Pixel));

            return default;
        }

        public static StyleColor ParseStyleColor(string styleBackgroundColor)
        {
            var keyword = GetStyleKeyword(styleBackgroundColor);

            if (keyword.HasValue) return new StyleColor(keyword.Value);

            if (styleBackgroundColor.StartsWith("#"))
            {
                return new StyleColor(ParseColor(styleBackgroundColor));
            }

            return default;
        }

        public static StyleKeyword? GetStyleKeyword(string style)
        {
            switch (style)
            {
                case "undefined": return StyleKeyword.Undefined;
                case "null": return StyleKeyword.Null;
                case "auto": return StyleKeyword.Auto;
                case "none": return StyleKeyword.None;
                case "initial": return StyleKeyword.Initial;
            }

            return null;
        }

        public static StyleCursor ParseStyleCursor(string stringValue)
        {
            var keyword = GetStyleKeyword(stringValue);

            if (keyword.HasValue) return new StyleCursor(keyword.Value);

            return new StyleCursor();
        }

        public static StyleFont ParseStyleFont(string stringValue)
        {
            var keyword = GetStyleKeyword(stringValue);

            if (keyword.HasValue) return new StyleFont(keyword.Value);

            return new StyleFont(Resources.Load<Font>(stringValue));
        }
    }
}
