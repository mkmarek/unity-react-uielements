﻿using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Styles
{
    public static class StyleMapper
    {
        public static void AssignStyleProps(JsToNativeBridgePayload.BridgeMessage.ComponentStyle style, VisualElement element)
        {
            if (!string.IsNullOrWhiteSpace(style.backgroundColor))
                element.style.backgroundColor = ParseStyleColor(style.backgroundColor);

            if (!string.IsNullOrWhiteSpace(style.flexBasis))
                element.style.flexBasis = ParseStyleLength(style.flexBasis);

            if (!string.IsNullOrWhiteSpace(style.flexGrow))
                element.style.flexGrow = ParseStyleFloat(style.flexGrow);

            if (!string.IsNullOrWhiteSpace(style.flexShrink))
                element.style.flexShrink = ParseStyleFloat(style.flexShrink);

            if (!string.IsNullOrWhiteSpace(style.flexWrap))
                element.style.flexWrap = ParseWrapStyleEnum(style.flexWrap);

            if (!string.IsNullOrWhiteSpace(style.backgroundImage))
                element.style.backgroundImage = ParseStyleBackground(style.backgroundImage);

            if (!string.IsNullOrWhiteSpace(style.unityBackgroundImageTintColor))
                element.style.unityBackgroundImageTintColor = ParseStyleColor(style.unityBackgroundImageTintColor);

            if (!string.IsNullOrWhiteSpace(style.unityBackgroundScaleMode))
                element.style.unityBackgroundScaleMode = ParseScaleModeStyleEnum(style.unityBackgroundScaleMode);

            if (!string.IsNullOrWhiteSpace(style.color))
                element.style.color = ParseStyleColor(style.color);

            if (!string.IsNullOrWhiteSpace(style.width))
                element.style.width = ParseStyleLength(style.width);

            if (!string.IsNullOrWhiteSpace(style.height))
                element.style.height = ParseStyleLength(style.height);

            if (!string.IsNullOrWhiteSpace(style.minWidth))
                element.style.minWidth = ParseStyleLength(style.minWidth);

            if (!string.IsNullOrWhiteSpace(style.minHeight))
                element.style.minHeight = ParseStyleLength(style.minHeight);

            if (!string.IsNullOrWhiteSpace(style.maxWidth))
                element.style.maxWidth = ParseStyleLength(style.maxWidth);

            if (!string.IsNullOrWhiteSpace(style.maxHeight))
                element.style.maxHeight = ParseStyleLength(style.maxHeight);

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

            if (!string.IsNullOrWhiteSpace(style.position))
                element.style.position = ParsePositionStyleEnum(style.position);

            if (!string.IsNullOrWhiteSpace(style.top))
                element.style.top = ParseStyleLength(style.top);

            if (!string.IsNullOrWhiteSpace(style.bottom))
                element.style.bottom = ParseStyleLength(style.bottom);
            
            if (!string.IsNullOrWhiteSpace(style.left))
                element.style.left = ParseStyleLength(style.left);

            if (!string.IsNullOrWhiteSpace(style.right))
                element.style.right = ParseStyleLength(style.right);
        }

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

                if (ReactRenderer.Current.colorsInLinearColorSpace) {
                    return col.linear;
                }

                return col;
            }

            if (hexInString.Length == 8)
            {
                Color col = new Color32(
                    (byte)(color >> 24 & 0xFF),
                    (byte)(color >> 16 & 0xFF),
                    (byte)((color >> 8) & 0xFF),
                    (byte)(color & 0xFF));

                if (ReactRenderer.Current.colorsInLinearColorSpace) {
                    return col.linear;
                }

                return col;
            }

            throw new FormatException($"Color can't have {hexInString.Length} digits");
        }

        private static StyleBackground ParseStyleBackground(string background)
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

        private static StyleEnum<Wrap> ParseWrapStyleEnum(string wrap)
        {
            var keyword = GetStyleKeyword(wrap);

            if (keyword.HasValue) return new StyleEnum<Wrap>(keyword.Value);

            switch (wrap)
            {
                case "nowrap": return new StyleEnum<Wrap>(Wrap.NoWrap);
                case "wrap": return new StyleEnum<Wrap>(Wrap.Wrap);
                case "wrap-reverse": return new StyleEnum<Wrap>(Wrap.WrapReverse);
            }

            return default;
        }

        private static StyleEnum<Position> ParsePositionStyleEnum(string position)
        {
            var keyword = GetStyleKeyword(position);

            if (keyword.HasValue) return new StyleEnum<Position>(keyword.Value);

            switch (position)
            {
                case "relative": return new StyleEnum<Position>(Position.Relative);
                case "absolute": return new StyleEnum<Position>(Position.Absolute);
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

        private static StyleEnum<ScaleMode> ParseScaleModeStyleEnum(string scaleMode)
        {
            var keyword = GetStyleKeyword(scaleMode);

            if (keyword.HasValue) return new StyleEnum<ScaleMode>(keyword.Value);

            switch (scaleMode)
            {
                case "scale-and-crop": return new StyleEnum<ScaleMode>(ScaleMode.ScaleAndCrop);
                case "scale-to-fit": return new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit);
                case "stretch-to-fill": return new StyleEnum<ScaleMode>(ScaleMode.StretchToFill);
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
                return new StyleColor(ParseColor(styleBackgroundColor));
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
