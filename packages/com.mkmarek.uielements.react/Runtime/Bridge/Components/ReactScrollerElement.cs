using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactScrollerElement : Scroller, IReactElement
    {
        public ReactScrollerElement(BridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props != null)
            {
                this.UpdateProps(props);
            }
        }

        public void UpdateProps(BridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props.style != null)
            {
                StyleMapper.AssignStyleProps(props.style, this);
            }
        }
    }
}
