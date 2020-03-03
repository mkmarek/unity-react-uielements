﻿using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactButtonElement : Button, IReactElement
    {
        private ReactRenderer renderer;

        public ReactButtonElement(ReactRenderer renderer, BridgePayload.BridgeMessage.ComponentProps props)
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
            } else if (evt.eventTypeId == MouseOverEvent.TypeId())
            {
                renderer.handlesToInvoke.Enqueue((this.name, "onMouseOver"));
            } else if (evt.eventTypeId == MouseOutEvent.TypeId())
            {
                renderer.handlesToInvoke.Enqueue((this.name, "onMouseOut"));
            }
        }

        public void UpdateProps(BridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props.style != null)
            {
                StyleMapper.AssignStyleProps(props.style, this);
            }

            text = props.text;
        }
    }
}
