using System;
using System.Collections.Generic;
using UnityEngine;
using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactTextFieldElement : TextField, IReactElement
    {
        private class OnTextChangeEventModel
        {
            public string previousValue;
            public string value;

            public OnTextChangeEventModel(string value, string previousValue)
            {
                this.value = value;
                this.previousValue = previousValue;
            }
        }

        private ReactRenderer renderer;
        private JsToNativeBridgePayload.BridgeMessage.ComponentProps previousProps;
        private Queue<string> queuedTextChanges = new Queue<string>();

        public ReactTextFieldElement(ReactRenderer renderer, JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            this.renderer = renderer;

            if (props != null)
            {
                this.UpdateProps(props);
            }
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            if (evt.eventTypeId == MouseUpEvent.TypeId())
            {
                renderer.AddMessageToBuffer(NativeToJsBridgePayload.BridgeMessage.CreateEventCallbackMessage(
                    this.name, "onClick"));
            }
            else if (evt.eventTypeId == MouseOverEvent.TypeId())
            {
                renderer.AddMessageToBuffer(NativeToJsBridgePayload.BridgeMessage.CreateEventCallbackMessage(
                    this.name, "onMouseOver"));
            }
            else if (evt.eventTypeId == MouseOutEvent.TypeId())
            {
                renderer.AddMessageToBuffer(NativeToJsBridgePayload.BridgeMessage.CreateEventCallbackMessage(
                    this.name, "onMouseOut"));
            }
            else if (evt.eventTypeId == ChangeEvent<string>.TypeId())
            {
                var e = evt as ChangeEvent<string>;

                queuedTextChanges.Enqueue(e.newValue);

                renderer.AddMessageToBuffer(NativeToJsBridgePayload.BridgeMessage.CreateEventCallbackMessage(
                    this.name, "onChange",
                    JsonUtility.ToJson(new OnTextChangeEventModel(e.newValue, e.previousValue))));

            }

            base.ExecuteDefaultAction(evt);
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

            var expectsTextChange = false;

            if (previousProps != null && props.text != previousProps.text && queuedTextChanges.Count > 0)
            {
                var textChange = queuedTextChanges.Dequeue();

                if (textChange == props.text) expectsTextChange = true;
            }

            if (!expectsTextChange && value != props.text)
            {
                SetValueWithoutNotify(props.text);
            }

            previousProps = props;
        }
    }
}
