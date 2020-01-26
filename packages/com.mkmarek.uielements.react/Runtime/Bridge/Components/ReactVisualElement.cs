using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactVisualElement : VisualElement, IReactElement
    {
        private ReactRenderer renderer;

        public ReactVisualElement(ReactRenderer renderer, BridgePayload.BridgeMessage.ComponentProps props)
        {
            this.renderer = renderer;

            this.UpdateProps(props);
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);

            if (evt.eventTypeId == MouseUpEvent.TypeId())
            {
                renderer.handlesToInvoke.Enqueue((this.name, "onClick"));
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
