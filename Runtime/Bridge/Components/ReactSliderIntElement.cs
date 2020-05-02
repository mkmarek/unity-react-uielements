using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactSliderIntElement : SliderInt, IReactElement
    {
        public ReactSliderIntElement(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
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
        }
    }
}
