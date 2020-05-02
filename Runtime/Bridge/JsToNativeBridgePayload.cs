using System;
using UnityEngine;

namespace UnityReactUIElements.Bridge
{
    [Serializable]
    public class JsToNativeBridgePayload
    {
        public BridgeMessage[] messages;

        [Serializable]
        public class BridgeMessage
        {
            // Base fields
            public string operation;
            public string id;
            public int index;

            // Fields for "create" operations
            public string type;
            public bool isContainer;
            public ComponentProps props;

            // Fields for "add-child" and "remove-child" operations
            public string parent;
            public string child;

            //fields for register-component-query
            public string queryName;

            //fields for update-component-data-via-hook
            public int componentIndex;
            public string data;

            //fields for remove-entity
            public string[] components;

            [Serializable]
            public class RectProp
            {
                public float x;
                public float y;
                public float width;
                public float height;

                public bool IsSet()
                {
                    return x != 0 || y != 0 || width != 0 || height != 0;
                } 

                public Rect ToRect()
                {
                    return new Rect(x, y, width, height);
                }
            }

            [Serializable]
            public class ComponentProps
            {
                public ComponentStyle style;
                public string pickingMode;
                public string text;
                public string value;
                public string image;
                public bool onChange;
                public RectProp sourceRect;
                public RectProp uv;
                public string scaleMode;
                public string tintColor;
            }

            [Serializable]
            public class ComponentStyle
            {
                public string flexDirection;
                public string alignContent;
                public string alignItems;
                public string alignSelf;
                public string justifyContent;
                public string width;
                public string height;
                public string marginBottom;
                public string marginLeft;
                public string marginRight;
                public string marginTop;
                public string paddingBottom;
                public string paddingLeft;
                public string paddingRight;
                public string paddingTop;
                public string backgroundColor;
                public string backgroundImage;
                public string unityBackgroundImageTintColor;
                public string unityBackgroundScaleMode;
                public string color;
                public string fontSize;
                public string borderColor;
                public string borderTopWidth;
                public string borderLeftWidth;
                public string borderBottomWidth;
                public string borderRightWidth;
                public string borderTopColor;
                public string borderLeftColor;
                public string borderBottomColor;
                public string borderRightColor;
                public string unityTextAlign;
                public string minWidth;
                public string minHeight;
                public string maxWidth;
                public string maxHeight;
                public string position;
                public string top;
                public string bottom;
                public string left;
                public string right;
                public string flexBasis;
                public string flexGrow;
                public string flexShrink;
                public string flexWrap;
            }
        }
    }
}
