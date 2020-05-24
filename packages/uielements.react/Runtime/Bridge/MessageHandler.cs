using System;
using System.Collections.Generic;
using System.IO;
using ChakraHost.Hosting;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityReactUIElements.Bridge.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge
{
    public class MessageHandler
    {
        private readonly ReactRenderer renderer;

        public MessageHandler(ReactRenderer renderer)
        {
            this.renderer = renderer;
        }

        private VisualElement FindElement(string name)
        {
            var queue = new Stack<VisualElement>();

            queue.Push(renderer.visualTree);

            while (queue.Count > 0)
            {
                var element = queue.Pop();

                if (element.name == name) return element;

                var children = element.Children();
                foreach (var child in children)
                {
                    queue.Push(child);
                }
            }

            throw new InvalidOperationException($"Element {name} not found");
        }

        public void SendMessage(NativeToJsBridgePayload payload)
        {
            unsafe
            {
                var bridgeFunction = Globals.GetNativeToJsBridgeFunction();

                var data = UnsafeUtility.Malloc(
                    UnsafeUtility.SizeOf<NativeToJsBridgePayload>(),
                    UnsafeUtility.AlignOf<NativeToJsBridgePayload>(),
                    Allocator.TempJob);

                UnsafeUtility.CopyStructureToPtr(ref payload, data);

                Native.ThrowIfError(Native.JsCreateExternalObject((IntPtr)data, null, out var value));

                Native.JsCallFunction(bridgeFunction, new[]
                {
                    JavaScriptValue.Null,
                    value

                }, 2, out var result);

                UnsafeUtility.Free(data, Allocator.TempJob);
            }
        }

        public void HandleMessage(string message)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Debug.Log(message);

            var payload = JsonUtility.FromJson<JsToNativeBridgePayload>(message);
            var temporaryComponentStorage = new Dictionary<string, VisualElement>();

            foreach (var deserialized in payload.messages)
            {
                switch (deserialized.operation)
                {
                    case "create":
                        if (deserialized.isContainer)
                        {
                            renderer.visualTree.name = deserialized.id;
                        }
                        else
                        {
                            var element = CreateElement(deserialized);
                            element.name = deserialized.id;
                            temporaryComponentStorage.Add(deserialized.id, element);
                        }

                        break;
                    case "add-child":
                        var parent = temporaryComponentStorage.ContainsKey(deserialized.parent)
                            ? temporaryComponentStorage[deserialized.parent]
                            : FindElement(deserialized.parent);

                        var child = temporaryComponentStorage.ContainsKey(deserialized.child)
                            ? temporaryComponentStorage[deserialized.child]
                            : FindElement(deserialized.child);

                        parent.Add(child);
                        break;
                    case "insert-child":
                        var parent2 = temporaryComponentStorage.ContainsKey(deserialized.parent)
                            ? temporaryComponentStorage[deserialized.parent]
                            : FindElement(deserialized.parent);

                        var child2 = temporaryComponentStorage.ContainsKey(deserialized.child)
                            ? temporaryComponentStorage[deserialized.child]
                            : FindElement(deserialized.child);

                        var index = deserialized.index;

                        parent2.Insert(index, child2);
                        break;
                    case "remove-child":
                        var parent3 = temporaryComponentStorage.ContainsKey(deserialized.parent)
                            ? temporaryComponentStorage[deserialized.parent]
                            : FindElement(deserialized.parent);

                        var child3 = temporaryComponentStorage.ContainsKey(deserialized.child)
                            ? temporaryComponentStorage[deserialized.child]
                            : FindElement(deserialized.child);

                        parent3.Remove(child3);

                        if (!temporaryComponentStorage.ContainsKey(child3.name))
                            temporaryComponentStorage.Add(child3.name, child3);
                        break;
                    case "update-props":

                        if (deserialized.props != null)
                        {
                            var elementToUpdate = temporaryComponentStorage.ContainsKey(deserialized.id)
                                ? temporaryComponentStorage[deserialized.id]
                                : FindElement(deserialized.id);

                            if (elementToUpdate is IReactElement reactElement)
                            {
                                reactElement.UpdateProps(deserialized.props);
                            }
                        }

                        break;
                    case "register-component-query":
                        ReactUIElementsQueryRegistry.RegisterComponentQuery(deserialized.id, deserialized.queryName);
                        break;
                    case "remove-component-query":
                        ReactUIElementsQueryRegistry.RemoveComponentQuery(deserialized.id);
                        break;
                    case "update-component-data-via-hook":
                        ReactUIElementsQueryRegistry.UpdateComponentData(
                            deserialized.id, deserialized.componentIndex, deserialized.index, deserialized.data);
                        break;
                    case "create-entity-with-components":
                        EntityFactory.CreateWithComponents(deserialized.data);
                        break;
                    case "remove-entity":
                        EntityFactory.RemoveEntity(deserialized.index, deserialized.components);
                        break;
                }
            }

            // Debug.Log($"Elapsed: {sw.ElapsedMilliseconds}");
        }

        private VisualElement CreateElement(JsToNativeBridgePayload.BridgeMessage deserialized)
        {
            switch (deserialized.type)
            {
                case "element":
                    return new ReactVisualElement(renderer, deserialized.props);
                case "text":
                    return new ReactTextElement(deserialized.props);
                case "box":
                    return new ReactBoxElement(deserialized.props);
                case "label":
                    return new ReactLabelElement(deserialized.props);
                case "button":
                    return new ReactButtonElement(renderer, deserialized.props);
                case "foldout":
                    return new ReactFoldoutElement(deserialized.props);
                case "image":
                    return new ReactImageElement(deserialized.props);
                case "listview":
                    return new ReactListViewElement(deserialized.props);
                case "minmaxslider":
                    return new ReactMinMaxSliderElement(deserialized.props);
                case "popupwindow":
                    return new ReactPopupWindowElement(deserialized.props);
                case "repeatbutton":
                    return new ReactRepeatButtonElement(deserialized.props);
                case "scroller":
                    return new ReactScrollerElement(deserialized.props);
                case "scrollview":
                    return new ReactScrollViewElement(deserialized.props);
                case "slider":
                    return new ReactSliderElement(deserialized.props);
                case "sliderint":
                    return new ReactSliderIntElement(deserialized.props);
                case "templatecontainer":
                    return new ReactTemplateContainerElement(deserialized.props);
                case "textfield":
                    return new ReactTextFieldElement(renderer, deserialized.props);
                case "toggle":
                    return new ReactToggleElement(deserialized.props);
                default:
                    throw new InvalidDataException($"Unknown element type {deserialized.type}");
            }
        }
    }
}
