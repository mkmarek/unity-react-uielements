using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityReactUIElements.Bridge
{
    [Serializable]
    public struct NativeToJsBridgePayload
    {
        public FixedList4096<BridgeMessage> messages;
        public int count;

        [Serializable]
        public struct BridgeMessage
        {
            // Base fields
            public FixedString128 operation;

            public FixedString128 componentId;

            public FixedString128 callbackName;

            public FixedString128 hookId;

            public FixedString128 data;

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
            var fixedList = new FixedList4096<BridgeMessage>();
            var size = UnsafeUtility.SizeOf<BridgeMessage>();

            for (var i =0; i < messages.Length; i++)
            {
                fixedList.Add(messages[i]);
            }

            return new NativeToJsBridgePayload()
            {
                messages = fixedList,
                count = messages.Length
            };
        }
    }
}
