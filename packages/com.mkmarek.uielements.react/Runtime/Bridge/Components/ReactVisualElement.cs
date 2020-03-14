using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactVisualElement : VisualElement, IReactElement
    {
        private ReactRenderer renderer;

        public ReactVisualElement(ReactRenderer renderer, JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            this.renderer = renderer;

            this.UpdateProps(props);
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);

            if (evt.eventTypeId == MouseUpEvent.TypeId())
            {
                renderer.AddMessageToBuffer(NativeToJsBridgePayload.BridgeMessage.CreateEventCallbackMessage(
                    this.name, "onClick"));
            }
        }

        public void UpdateProps(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props.style != null)
            {
                StyleMapper.AssignStyleProps(props.style, this);
            }
        }
    }
}
