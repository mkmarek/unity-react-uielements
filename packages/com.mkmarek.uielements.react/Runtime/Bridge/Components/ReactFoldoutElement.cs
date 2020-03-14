using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactFoldoutElement : Foldout, IReactElement
    {
        public ReactFoldoutElement(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
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

            if (!string.IsNullOrWhiteSpace(props.value))
            {
                bool.TryParse(props.value, out var tmpValue);
                value = tmpValue;
            }
        }
    }
}
