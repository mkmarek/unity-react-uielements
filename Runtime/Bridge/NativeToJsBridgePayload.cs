using System;

namespace UnityReactUIElements.Bridge
{
    [Serializable]
    public class NativeToJsBridgePayload
    {
        public BridgeMessage[] messages;

        [Serializable]
        public class BridgeMessage
        {
            // Base fields
            public string operation;

            public string componentId;

            public string callbackName;

            public string hookId;

            public string data;

            public static BridgeMessage CreateEventCallbackMessage(string componentId, string callbackName, string data = null)
            {
                return new BridgeMessage()
                {
                    operation = "event-callback",
                    componentId = componentId,
                    callbackName = callbackName,
                    data = data
                };
            }

            public static BridgeMessage CreateUpdateComponentDataHookMessage(string hookId, string data)
            {
                return new BridgeMessage()
                {
                    operation = "update-component-data-hook",
                    hookId = hookId,
                    data = data
                };
            }
        }

        public static NativeToJsBridgePayload Create(params BridgeMessage[] messages)
        {
            return new NativeToJsBridgePayload()
            {
                messages = messages
            };
        }
    }
}
