﻿using System;
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

            visualTree.pickingMode = PickingMode.Ignore;
            visualTree.RegisterCallback<MouseEnterEvent>(x => isMouseOverElement = true);
            visualTree.RegisterCallback<MouseLeaveEvent>(x => isMouseOverElement = false);
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
                while (nativeToJsMessages.Count > 0)
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