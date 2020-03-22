using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactPopupWindowElement : PopupWindow, IReactElement
    {
        public ReactPopupWindowElement(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
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

            text = props.text;
        }
    }
}
