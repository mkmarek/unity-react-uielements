using System;
using System.Collections;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Unity.Entities;
using UnityReactUIElements.Bridge;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements
{
    [RequireComponent(typeof(UIElementsEventSystem))]
    public class ReactRenderer : PanelRenderer
    {
        private readonly Queue<string> jsToNativeMessages = new Queue<string>();
        private readonly Queue<NativeToJsBridgePayload.BridgeMessage> nativeToJsMessages = new Queue<NativeToJsBridgePayload.BridgeMessage>();

        private MessageHandler messageHandler;
        private UIElementsEventSystem eventSystem;
        private JsModuleRuntime runtime;

        public static ReactRenderer Current { get; private set; }

#pragma warning disable 649
        [SerializeField]
        private JSFileObject _root;

        [SerializeField]
        internal bool colorsInLinearColorSpace;

        [SerializeField] public bool isMouseOverElement;
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
            StartCoroutine(CheckforJsErrors());
            StartCoroutine(CheckForMouseOverElement());

            visualTree.pickingMode = PickingMode.Ignore;
        }

        public void RunModule(string[] modulesToReload)
        {
            this.runtime.RunModule(_root, modulesToReload);
        }

        public void AddMessageToBuffer(NativeToJsBridgePayload.BridgeMessage message)
        {
            nativeToJsMessages.Enqueue(message);
        }

        public void AddMessageToBuffer(string message)
        {
            jsToNativeMessages.Enqueue(message);
        }

        private IEnumerator CheckForMouseOverElement()
        {
            while (true)
            {
                var position =
                    new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height) *
                    this.visualTree.layout.size;

                // the position needs to be inverted vertically
                position.y = this.visualTree.layout.size.y - position.y;

                var picked = panel.Pick(position);

                // Should also ignore panel elements
                if (picked?.parent != null)
                {
                    isMouseOverElement = true;
                }
                else
                {
                    isMouseOverElement = false;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator CheckforJsErrors()
        {
            while (true)
            {
                runtime.CheckForError();

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator HandleEvents()
        {
            var messageBuffer = new List<NativeToJsBridgePayload.BridgeMessage>();

            while (true)
            {
                while (nativeToJsMessages.Count > 0 && messageBuffer.Count < 32)
                {
                    var message = nativeToJsMessages.Dequeue();
                    messageBuffer.Add(message);
                }

                if (messageBuffer.Count > 0)
                {
                    messageHandler.SendMessage(NativeToJsBridgePayload.Create(messageBuffer.ToArray()));
                    messageBuffer.Clear();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator HandleMessages()
        {
            while (true)
            {
                if (jsToNativeMessages.Count > 0)
                {
                    this.enabled = false;
                    this.eventSystem.enabled = false;

                    while (jsToNativeMessages.Count > 0)
                    {
                        var message = jsToNativeMessages.Dequeue();

                        messageHandler.HandleMessage(message);
                    }

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