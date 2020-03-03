using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Styles
{
    public static class StyleMapper
    {
        public static void AssignStyleProps(BridgePayload.BridgeMessage.ComponentStyle style, VisualElement element)
        {
            if (!string.IsNullOrWhiteSpace(style.backgroundColor))
                element.style.backgroundColor = ParseStyleColor(style.backgroundColor);

            if (!string.IsNullOrWhiteSpace(style.color))
                element.style.color = ParseStyleColor(style.color);

            if (!string.IsNullOrWhiteSpace(style.width))
                element.style.width = ParseStyleLength(style.width);

            if (!string.IsNullOrWhiteSpace(style.height))
                element.style.height = ParseStyleLength(style.height);

            if (!string.IsNullOrWhiteSpace(style.marginBottom))
                element.style.marginBottom = ParseStyleLength(style.marginBottom);

            if (!string.IsNullOrWhiteSpace(style.marginLeft))
                element.style.marginLeft = ParseStyleLength(style.marginLeft);

            if (!string.IsNullOrWhiteSpace(style.marginRight))
                element.style.marginRight = ParseStyleLength(style.marginRight);

            if (!string.IsNullOrWhiteSpace(style.marginTop))
                element.style.marginTop = ParseStyleLength(style.marginTop);

            if (!string.IsNullOrWhiteSpace(style.paddingBottom))
                element.style.paddingBottom = ParseStyleLength(style.paddingBottom);

            if (!string.IsNullOrWhiteSpace(style.paddingLeft))
                element.style.paddingLeft = ParseStyleLength(style.paddingLeft);

            if (!string.IsNullOrWhiteSpace(style.paddingRight))
                element.style.paddingRight = ParseStyleLength(style.paddingRight);

            if (!string.IsNullOrWhiteSpace(style.paddingTop))
                element.style.paddingTop = ParseStyleLength(style.paddingTop);

            if (!string.IsNullOrWhiteSpace(style.alignItems))
                element.style.alignItems = ParseAlignStyleEnum(style.alignItems);

            if (!string.IsNullOrWhiteSpace(style.alignContent))
                element.style.alignContent = ParseAlignStyleEnum(style.alignContent);

            if (!string.IsNullOrWhiteSpace(style.alignItems))
                element.style.alignItems = ParseAlignStyleEnum(style.alignItems);

            if (!string.IsNullOrWhiteSpace(style.alignSelf))
                element.style.alignSelf = ParseAlignStyleEnum(style.alignSelf);

            if (!string.IsNullOrWhiteSpace(style.justifyContent))
                element.style.justifyContent = ParseJustifyStyleEnum(style.justifyContent);

            if (!string.IsNullOrWhiteSpace(style.flexDirection))
                element.style.flexDirection = ParseFlexDirectionStyleEnum(style.flexDirection);

            if (!string.IsNullOrWhiteSpace(style.fontSize))
                element.style.fontSize = ParseStyleLength(style.fontSize);

            if (!string.IsNullOrWhiteSpace(style.borderColor))
                element.style.borderColor = ParseStyleColor(style.borderColor);

            if (!string.IsNullOrWhiteSpace(style.borderBottomWidth))
                element.style.borderBottomWidth = ParseStyleFloat(style.borderBottomWidth);

            if (!string.IsNullOrWhiteSpace(style.borderLeftWidth))
                element.style.borderLeftWidth = ParseStyleFloat(style.borderLeftWidth);

            if (!string.IsNullOrWhiteSpace(style.borderTopWidth))
                element.style.borderTopWidth = ParseStyleFloat(style.borderTopWidth);

            if (!string.IsNullOrWhiteSpace(style.borderRightWidth))
                element.style.borderRightWidth = ParseStyleFloat(style.borderRightWidth);

            if (!string.IsNullOrWhiteSpace(style.borderBottomColor))
                element.style.borderBottomColor = ParseStyleColor(style.borderBottomColor);

            if (!string.IsNullOrWhiteSpace(style.borderLeftColor))
                element.style.borderLeftColor = ParseStyleColor(style.borderLeftColor);

            if (!string.IsNullOrWhiteSpace(style.borderTopColor))
                element.style.borderTopColor = ParseStyleColor(style.borderTopColor);

            if (!string.IsNullOrWhiteSpace(style.borderRightColor))
                element.style.borderRightColor = ParseStyleColor(style.borderRightColor);

            if (!string.IsNullOrWhiteSpace(style.unityTextAlign))
                element.style.unityTextAlign = ParseTextAlign(style.unityTextAlign);
        }

        private static StyleEnum<TextAnchor> ParseTextAlign(string styleUnityTextAlign)
        {
            var keyword = GetStyleKeyword(styleUnityTextAlign);

            if (keyword.HasValue) return new StyleEnum<TextAnchor>(keyword.Value);

            switch (styleUnityTextAlign)
            {
                case "upper-left": return new StyleEnum<TextAnchor>(TextAnchor.UpperLeft);
                case "upper-center": return new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);
                case "upper-right": return new StyleEnum<TextAnchor>(TextAnchor.UpperRight);
                case "middle-left": return new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                case "middle-center": return new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                case "middle-right": return new StyleEnum<TextAnchor>(TextAnchor.MiddleRight);
                case "lower-left": return new StyleEnum<TextAnchor>(TextAnchor.LowerLeft);
                case "lower-center": return new StyleEnum<TextAnchor>(TextAnchor.LowerCenter);
                case "lower-right": return new StyleEnum<TextAnchor>(TextAnchor.LowerRight);
            }

            return default;
        }

        private static StyleEnum<Justify> ParseJustifyStyleEnum(string styleJustifyContent)
        {
            var keyword = GetStyleKeyword(styleJustifyContent);

            if (keyword.HasValue) return new StyleEnum<Justify>(keyword.Value);

            switch (styleJustifyContent)
            {
                case "space-between": return new StyleEnum<Justify>(Justify.SpaceBetween);
                case "flex-start": return new StyleEnum<Justify>(Justify.FlexStart);
                case "center": return new StyleEnum<Justify>(Justify.Center);
                case "flex-end": return new StyleEnum<Justify>(Justify.FlexEnd);
                case "space-around": return new StyleEnum<Justify>(Justify.SpaceAround);
            }

            return default;
        }

        private static StyleEnum<FlexDirection> ParseFlexDirectionStyleEnum(string styleFlexDirection)
        {
            var keyword = GetStyleKeyword(styleFlexDirection);

            if (keyword.HasValue) return new StyleEnum<FlexDirection>(keyword.Value);

            switch (styleFlexDirection)
            {
                case "column": return new StyleEnum<FlexDirection>(FlexDirection.Column);
                case "column-reverse": return new StyleEnum<FlexDirection>(FlexDirection.ColumnReverse);
                case "row": return new StyleEnum<FlexDirection>(FlexDirection.Row);
                case "row-reverse": return new StyleEnum<FlexDirection>(FlexDirection.RowReverse);
            }

            return default;
        }

        private static StyleEnum<Align> ParseAlignStyleEnum(string styleAlignItems)
        {
            var keyword = GetStyleKeyword(styleAlignItems);

            if (keyword.HasValue) return new StyleEnum<Align>(keyword.Value);

            switch (styleAlignItems)
            {
                case "auto": return new StyleEnum<Align>(Align.Auto);
                case "flex-start": return new StyleEnum<Align>(Align.FlexStart);
                case "center": return new StyleEnum<Align>(Align.Center);
                case "flex-end": return new StyleEnum<Align>(Align.FlexEnd);
                case "stretch": return new StyleEnum<Align>(Align.Stretch);
            }

            return default;
        }

        private static StyleFloat ParseStyleFloat(string styleFloat)
        {
            var keyword = GetStyleKeyword(styleFloat);

            if (keyword.HasValue) return new StyleFloat(keyword.Value);
            if (float.TryParse(styleFloat, out var length)) return new StyleFloat(length);

            return default;
        }

        private static StyleLength ParseStyleLength(string styleWidth)
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

        private static StyleColor ParseStyleColor(string styleBackgroundColor)
        {
            var keyword = GetStyleKeyword(styleBackgroundColor);

            if (keyword.HasValue) return new StyleColor(keyword.Value);

            if (styleBackgroundColor.StartsWith("#"))
            {
                var hexInString = styleBackgroundColor.Substring(1);
                var color = int.Parse(hexInString, NumberStyles.HexNumber);

                if (hexInString.Length == 6)
                {
                    return new StyleColor(new Color32(
                        (byte)(color >> 16 & 0xFF),
                        (byte)((color >> 8) & 0xFF),
                        (byte)(color & 0xFF),
                        0xFF));
                }

                if (hexInString.Length == 8)
                {
                    return new StyleColor(new Color32(
                        (byte)(color >> 24 & 0xFF),
                        (byte)(color >> 16 & 0xFF),
                        (byte)((color >> 8) & 0xFF),
                        (byte)(color & 0xFF)));
                }

                throw new FormatException($"Color can't have {hexInString.Length} digits");
            }

            return default;
        }

        private static StyleKeyword? GetStyleKeyword(string style)
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
    }
}
