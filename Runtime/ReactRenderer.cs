using System;
using System.Collections;
using System.Collections.Generic;
using ChakraHost.Hosting;
using UnityReactUIElements.Bridge;
using Unity.UIElements.Runtime;
using UnityEngine;

namespace UnityReactUIElements
{
    [RequireComponent(typeof(UIElementsEventSystem))]
    public class ReactRenderer : PanelRenderer
    {
        public readonly Queue<string> messagesToHandle = new Queue<string>();
        public readonly Queue<(string, string)> handlesToInvoke = new Queue<(string, string)>();

        private MessageHandler messageHandler;
        private UIElementsEventSystem eventSystem;
        private JsModuleRuntime runtime;

        public static ReactRenderer Current { get; private set; }

        [SerializeField]
#pragma warning disable 649
        private JSFileObject _root;
#pragma warning restore 649

        private new void Awake()
        {
            base.Awake();

            if (Current != null)
            {
                throw new Exception("It's possible to have only one react renderer instance in the game'");
            }

            Current = this;
        }

        private new void Start()
        {
            base.Start();

            this.runtime = new JsModuleRuntime();
            this.messageHandler = new MessageHandler(this);
            this.eventSystem = GetComponent<UIElementsEventSystem>();

            this.runtime.RunModule(_root);

            StartCoroutine(HandleMessages());
            StartCoroutine(HandleEvents());
        }

        private IEnumerator HandleEvents()
        {
            var callback = Globals.GetInvokeCallbackFunction();

            while (true)
            {
                if (handlesToInvoke.Count > 0)
                {
                    yield return new WaitForFixedUpdate();

                    var handle = handlesToInvoke.Dequeue();

                    Native.JsCallFunction(callback, new[]
                    {
                        JavaScriptValue.Null,
                        JavaScriptValue.FromString(handle.Item1),
                        JavaScriptValue.FromString(handle.Item2)
                    }, 3, out var result);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator HandleMessages()
        {
            while (true)
            {
                if (messagesToHandle.Count > 0)
                {
                    this.enabled = false;
                    this.eventSystem.enabled = false;

                    yield return new WaitForFixedUpdate();

                    var message = messagesToHandle.Dequeue();

                    messageHandler.HandleMessage(message);

                    this.enabled = true;
                    this.eventSystem.enabled = true;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private new void OnDestroy()
        {
            base.OnDestroy();

            StopAllCoroutines();

            runtime.Dispose();
        }
    }
}