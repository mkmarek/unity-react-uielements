using System;

namespace UnityReactUIElements.Bridge
{
    [Serializable]
    public class BridgePayload
    {
        public BridgeMessage[] messages;

        [Serializable]
        public class BridgeMessage
        {
            // Base fields
            public string operation;

            // Fields for "create" operations
            public string type;
            public bool isContainer;
            public string id;
            public int index;
            public ComponentProps props;

            // Fields for "add-child" and "remove-child" operations
            public string parent;
            public string child;

            [Serializable]
            public class ComponentProps
            {
                public ComponentStyle style;
                public string text;
                public string value;
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
                public string backgroundColor;
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
            }
        }
    }
}
