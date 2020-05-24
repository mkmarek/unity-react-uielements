﻿using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactTextElement : TextElement, IReactElement
    {
        public ReactTextElement(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props != null)
            {
                this.UpdateProps(props);
            }
        }

        public void UpdateProps(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props.style != null)
            {
                StyleMapper.AssignStyleProps(props.style, this);
            }

            switch (props.pickingMode)
            {
                case "ignore": pickingMode = PickingMode.Ignore;
                    break;
                case "position": pickingMode = PickingMode.Position;
                    break;
            }

            pickingMode = PickingMode.Ignore;
            text = props.text;
        }
    }
}
