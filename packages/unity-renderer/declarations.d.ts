declare module JSX {

    type StyleKeyword = 'undefined' | 'null' | "auto" | 'none' | 'initial';
    type Align = 'auto' | 'flex-start' | 'center' | 'flex-end' | 'stretch';
    type PickingMode = 'position' | 'ignore';
    type DisplayStyle = 'flex' | 'none';
    type FlexDirection = 'column' | 'column-reverse' | 'row' | 'row-reverse';
    type Wrap = 'no-wrap' | 'wrap' | 'wrap-reverse';
    type Justify = 'flex-start' | 'center' | 'flex-end' | 'space-between' | 'space-around';
    type Overflow = 'visible' | 'hidden';
    type Position = 'relative' | 'absolute';
    type ScaleMode = 'stretch-to-fit' | 'scale-and-crop' | 'scale-to-fit';
    type FontStyle = 'normal' | 'bold' | 'italic' | 'bold-and-italic';
    type OverflowClipBox = 'padding-box' | 'content-box';
    type TextAnchor = 'upper-left' | 'upper-center' | 'upper-right' | 'middle-left' | 'middle-center'
        | 'middle-right' | 'lower-left' | 'lower-center' | 'lower-right';
    type Visibility = 'visible' | 'hidden';
    type WhiteSpace = 'normal' | 'no-wrap';

    type Color = string;

    interface Rect {
        position: Vector2;
        size: Vector2;
    }

    interface Vector2 {
        x: number,
        y: number
    }

    interface Cursor {
        hotspot: Vector2;
        texture: string;
    }

    type StyleColor = StyleKeyword | Color;
    type StyleBackground = StyleKeyword | string;
    type StyleLength = StyleKeyword | string;
    type StyleEnum<T> = StyleKeyword | T;
    type StyleFloat = StyleKeyword | number;
    type StyleCursor = StyleKeyword | Cursor;
    type StyleFont = StyleKeyword | string;
    type StyleInt = StyleKeyword | number;

    interface IStyle {
        alignContent?: StyleEnum<Align>;
        alignItems?: StyleEnum<Align>;
        alignSelf?: StyleEnum<Align>;
        backgroundColor?: StyleColor;
        backgroundImage?: StyleBackground;
        borderBottomColor?: StyleColor;
        borderBottomLeftRadius?: StyleLength;
        borderBottomRightRadius?: StyleLength;
        borderBottomWidth?: StyleFloat;
        borderColor?: StyleColor;
        borderLeftColor?: StyleColor;
        borderLeftWidth?: StyleFloat;
        borderRightColor?: StyleColor;
        borderRightWidth?: StyleFloat;
        borderTopColor?: StyleColor;
        borderTopLeftRadius?: StyleLength;
        borderTopRightRadius?: StyleLength;
        borderTopWidth?: StyleFloat;
        bottom?: StyleLength;
        color?: StyleColor;
        cursor?: StyleCursor;
        display?: StyleEnum<DisplayStyle>;
        flexBasis?: StyleLength;
        flexDirection?: StyleEnum<FlexDirection>;
        flexGrow?: StyleFloat;
        flexShrink?: StyleFloat;
        flexWrap?: StyleEnum<Wrap>;
        fontSize?: StyleLength;
        height?: StyleLength;
        justifyContent?: StyleEnum<Justify>;
        left?: StyleLength;
        marginBottom?: StyleLength;
        marginLeft?: StyleLength;
        marginRight?: StyleLength;
        marginTop?: StyleLength;
        maxHeight?: StyleLength;
        maxWidth?: StyleLength;
        minHeight?: StyleLength;
        minWidth?: StyleLength;
        opacity?: StyleFloat;
        overflow?: StyleEnum<Overflow>;
        paddingBottom?: StyleLength;
        paddingLeft?: StyleLength;
        paddingRight?: StyleLength;
        paddingTop?: StyleLength;
        position?: StyleEnum<Position>;
        right?: StyleLength;
        top?: StyleLength;
        unityBackgroundImageTintColor?: StyleColor;
        unityBackgroundScaleMode?: StyleEnum<ScaleMode>;
        unityFont?: StyleFont;
        unityFontStyleAndWeight?: StyleEnum<FontStyle>;
        unityOverflowClipBox?: StyleEnum<OverflowClipBox>;
        unitySliceBottom?: StyleInt;
        unitySliceLeft?: StyleInt;
        unitySliceRight?: StyleInt;
        unitySliceTop?: StyleInt;
        unityTextAlign?: StyleEnum<TextAnchor>;
        visibility?: StyleEnum<Visibility>;
        whiteSpace?: StyleEnum<WhiteSpace>;
        width?: StyleLength;
    }

    interface Focusable {
        canGrabFocus?: boolean;
        delegatesFocus?: boolean;
        focusable?: boolean;
        tabIndex?: number;
    }

    interface VisualElement extends Focusable {
        cacheAsBitmap?: boolean;
        childCount?: number;
        contentRect?: Rect;
        customStyle?: any;
        enabledInHierarchy?: boolean;
        enabledSelf?: boolean;
        layout?: Rect;
        localBound?: Rect;
        name?: string;
        pickingMode?: PickingMode;
        style?: IStyle;
        tooltip?: string;
        userData?: any;
        viewDataKey?: string;
        visible?: boolean;
        
        onClick?: () => void;
    }

    type Key = string | number;

    interface ElementProps extends VisualElement {
        key?: Key;
        children?: React.ReactNode;
    }

    interface IntrinsicElements {
        element: ElementProps;
    }
}

declare module NodeJS {
    interface Process {
        natives: {
            bridge: (payload: string) => void;
            invokeCallback: (elementId: string, callback: string) => void;
        }
    }
}