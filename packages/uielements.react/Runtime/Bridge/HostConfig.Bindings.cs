using System;
using ChakraHost.Hosting;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge
{
    internal partial class HostConfig
    {
        public static JavaScriptValue Create()
        {
            var prototype = JavaScriptValue.CreateObject();

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("isPrimaryRenderer"),
                JavaScriptValue.True,
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("supportsMutation"),
                JavaScriptValue.True,
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("getPublicInstance"),
                JavaScriptValue.CreateFunction("getPublicInstance", new JavaScriptNativeFunction(GetPublicInstance)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("getRootHostContext"),
                JavaScriptValue.CreateFunction("getRootHostContext", new JavaScriptNativeFunction(GetRootHostContext)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("getChildHostContext"),
                JavaScriptValue.CreateFunction("getChildHostContext", new JavaScriptNativeFunction(GetChildHostContext)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("createInstance"),
                JavaScriptValue.CreateFunction("createInstance", new JavaScriptNativeFunction(CreateInstance)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("prepareForCommit"),
                JavaScriptValue.CreateFunction("prepareForCommit", new JavaScriptNativeFunction(PrepareForCommit)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("resetAfterCommit"),
                JavaScriptValue.CreateFunction("resetAfterCommit", new JavaScriptNativeFunction(ResetAfterCommit)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("appendInitialChild"),
                JavaScriptValue.CreateFunction("appendInitialChild", new JavaScriptNativeFunction(AppendInitialChild)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("finalizeInitialChildren"),
                JavaScriptValue.CreateFunction("finalizeInitialChildren", new JavaScriptNativeFunction(FinalizeInitialChildren)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("prepareUpdate"),
                JavaScriptValue.CreateFunction("prepareUpdate", new JavaScriptNativeFunction(PrepareUpdate)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("shouldSetTextContent"),
                JavaScriptValue.CreateFunction("shouldSetTextContent", new JavaScriptNativeFunction(ShouldSetTextContent)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("shouldDeprioritizeSubtree"),
                JavaScriptValue.CreateFunction("shouldDeprioritizeSubtree", new JavaScriptNativeFunction(ShouldDeprioritizeSubtree)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("createTextInstance"),
                JavaScriptValue.CreateFunction("createTextInstance", new JavaScriptNativeFunction(CreateTextInstance)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("appendChild"),
                JavaScriptValue.CreateFunction("appendChild", new JavaScriptNativeFunction(AppendChild)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("appendChildToContainer"),
                JavaScriptValue.CreateFunction("appendChildToContainer", new JavaScriptNativeFunction(AppendChildToContainer)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("commitTextUpdate"),
                JavaScriptValue.CreateFunction("commitTextUpdate", new JavaScriptNativeFunction(CommitTextUpdate)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("commitMount"),
                JavaScriptValue.CreateFunction("commitMount", new JavaScriptNativeFunction(CommitMount)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("commitUpdate"),
                JavaScriptValue.CreateFunction("commitUpdate", new JavaScriptNativeFunction(CommitUpdate)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("insertBefore"),
                JavaScriptValue.CreateFunction("insertBefore", new JavaScriptNativeFunction(InsertBefore)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("insertInContainerBefore"),
                JavaScriptValue.CreateFunction("insertInContainerBefore", new JavaScriptNativeFunction(InsertInContainerBefore)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("removeChild"),
                JavaScriptValue.CreateFunction("removeChild", new JavaScriptNativeFunction(RemoveChild)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("removeChildFromContainer"),
                JavaScriptValue.CreateFunction("removeChildFromContainer", new JavaScriptNativeFunction(RemoveChildFromContainer)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("resetTextContent"),
                JavaScriptValue.CreateFunction("resetTextContent", new JavaScriptNativeFunction(ResetTextContent)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("hideInstance"),
                JavaScriptValue.CreateFunction("hideInstance", new JavaScriptNativeFunction(HideInstance)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("hideTextInstance"),
                JavaScriptValue.CreateFunction("hideTextInstance", new JavaScriptNativeFunction(HideTextInstance)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("unhideInstance"),
                JavaScriptValue.CreateFunction("unhideInstance", new JavaScriptNativeFunction(UnhideInstance)),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("unhideTextInstance"),
                JavaScriptValue.CreateFunction("unhideTextInstance", new JavaScriptNativeFunction(UnhideTextInstance)),
                true);

            return prototype;
        }

        private static JavaScriptValue GetPublicInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(GetPublicInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var instance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                var result = GetPublicInstance(instance);

                return result.ToJavaScriptValue();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue GetRootHostContext(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(GetRootHostContext)))
                return JavaScriptValue.Undefined;
            try
            {
                var rootContainer = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                var result = GetRootHostContext(rootContainer);

                return result.ToJavaScriptValue();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue GetChildHostContext(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<HostContext>(1, nameof(GetChildHostContext)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(GetChildHostContext), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(3, nameof(GetChildHostContext)))
                return JavaScriptValue.Undefined;

            try
            {
                var parentHostContext = arguments[1].ObjectFromJavaScriptValue<HostContext>();
                var type = arguments[2].ToString();
                var rootContainer = arguments[3].ObjectFromJavaScriptValue<VisualElement>();

                var result = GetChildHostContext(parentHostContext, type, rootContainer);

                return result.ToJavaScriptValue();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue CreateInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithType(1, nameof(CreateInstance), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(CreateInstance), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(3, nameof(CreateInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var type = arguments[1].ToString();
                var props = ComponentMapper.CreateProps(type, arguments[2]);
                var rootContainer = arguments[3].ObjectFromJavaScriptValue<VisualElement>();

                var result = CreateInstance(type, props, rootContainer);

                return result.ToJavaScriptValue();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue PrepareForCommit(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(PrepareForCommit)))
                return JavaScriptValue.Undefined;

            try
            {
                var container = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                PrepareForCommit(container);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue ResetAfterCommit(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(ResetAfterCommit)))
                return JavaScriptValue.Undefined;

            try
            {
                var container = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                ResetAfterCommit(container);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue AppendInitialChild(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(AppendInitialChild)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(AppendInitialChild)))
                return JavaScriptValue.Undefined;

            try
            {
                var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

                AppendInitialChild(parent, child);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue AppendChild(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(AppendChild)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(AppendChild)))
                return JavaScriptValue.Undefined;

            try
            {
                var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

                AppendChild(parent, child);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue AppendChildToContainer(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(AppendChild)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(AppendChild)))
                return JavaScriptValue.Undefined;

            try
            {
                var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

                AppendChildToContainer(parent, child);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue FinalizeInitialChildren(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(FinalizeInitialChildren)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(FinalizeInitialChildren), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(3, nameof(FinalizeInitialChildren), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(4, nameof(FinalizeInitialChildren)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<HostContext>(5, nameof(FinalizeInitialChildren)))
                return JavaScriptValue.Undefined;

            try
            {
                var parentInstance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var type = arguments[2].ToString();
                var props = ComponentMapper.CreateProps(type, arguments[3]);
                var rootContainer = arguments[4].ObjectFromJavaScriptValue<VisualElement>();
                var hostContext = arguments[5].ObjectFromJavaScriptValue<HostContext>();

                FinalizeInitialChildren(parentInstance, type, props, rootContainer, hostContext);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue PrepareUpdate(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(PrepareUpdate)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(PrepareUpdate), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(3, nameof(PrepareUpdate), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(4, nameof(PrepareUpdate), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(5, nameof(PrepareUpdate)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<HostContext>(6, nameof(PrepareUpdate)))
                return JavaScriptValue.Undefined;

            try
            {
                var instance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var type = arguments[2].ToString();
                var oldProps = ComponentMapper.CreateProps(type, arguments[3]);
                var newProps = ComponentMapper.CreateProps(type, arguments[4]);
                var rootContainer = arguments[5].ObjectFromJavaScriptValue<VisualElement>();
                var hostContext = arguments[6].ObjectFromJavaScriptValue<HostContext>();

                var props = PrepareUpdate(instance, type, oldProps, newProps, rootContainer, hostContext);

                return props.ToJavaScriptValue();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue ShouldSetTextContent(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithType(1, nameof(ShouldSetTextContent), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(ShouldSetTextContent), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;

            try
            {
                var type = arguments[1].ToString();
                var props = ComponentMapper.CreateProps(type, arguments[2]);

                var result = ShouldSetTextContent(type, props);

                return JavaScriptValue.FromBoolean(result);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue ShouldDeprioritizeSubtree(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithType(1, nameof(ShouldDeprioritizeSubtree), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(ShouldDeprioritizeSubtree), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;

            try
            {
                var type = arguments[1].ToString();
                var props = ComponentMapper.CreateProps(type, arguments[2]);

                var result = ShouldDeprioritizeSubtree(type, props);

                return JavaScriptValue.FromBoolean(result);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue CreateTextInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithType(1, nameof(CreateTextInstance), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(CreateTextInstance)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<HostContext>(3, nameof(CreateTextInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var text = arguments[1].ToString();
                var rootContainer = arguments[2].ObjectFromJavaScriptValue<VisualElement>();
                var hostContext = arguments[3].ObjectFromJavaScriptValue<HostContext>();

                var result = CreateTextInstance(text, rootContainer, hostContext);

                return result.ToJavaScriptValue();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue CommitTextUpdate(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(CommitTextUpdate)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(CommitTextUpdate), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(3, nameof(CommitTextUpdate), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;

            try
            {
                var visualElement = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var oldText = arguments[2].ToString();
                var newText = arguments[3].ToString();

                CommitTextUpdate(visualElement, oldText, newText);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue CommitMount(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(CommitMount)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(2, nameof(CommitMount), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(3, nameof(CommitMount), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;

            try
            {
                var visualElement = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var type = arguments[2].ToString();
                var props = ComponentMapper.CreateProps(type, arguments[3]);

                CommitMount(visualElement, type, props);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue CommitUpdate(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(CommitUpdate)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<IComponentProps>(2, nameof(CommitUpdate)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(3, nameof(CommitUpdate), JavaScriptValueType.String))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(4, nameof(CommitUpdate), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithType(5, nameof(CommitUpdate), JavaScriptValueType.Object))
                return JavaScriptValue.Undefined;

            try
            {
                var instance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var type = arguments[3].ToString();
                var updatePayload = arguments[2].ObjectFromJavaScriptValue<IComponentProps>();
                var oldProps = ComponentMapper.CreateProps(type, arguments[4]);
                var newProps = ComponentMapper.CreateProps(type, arguments[5]);

                var result = CommitUpdate(instance, updatePayload, type, oldProps, newProps);

                return JavaScriptValue.FromBoolean(result);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue InsertBefore(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(InsertBefore)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(InsertBefore)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(3, nameof(InsertBefore)))
                return JavaScriptValue.Undefined;

            try
            {
                var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();
                var beforeChild = arguments[3].ObjectFromJavaScriptValue<VisualElement>();

                InsertBefore(parent, child, beforeChild);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue InsertInContainerBefore(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(InsertInContainerBefore)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(InsertInContainerBefore)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(3, nameof(InsertInContainerBefore)))
                return JavaScriptValue.Undefined;

            try
            {
                var container = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();
                var beforeChild = arguments[3].ObjectFromJavaScriptValue<VisualElement>();

                InsertInContainerBefore(container, child, beforeChild);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue RemoveChild(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(RemoveChild)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(RemoveChild)))
                return JavaScriptValue.Undefined;

            try
            {
                var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

                RemoveChild(parent, child);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue RemoveChildFromContainer(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(RemoveChildFromContainer)))
                return JavaScriptValue.Undefined;
            if (!arguments.ValidateWithExternalData<VisualElement>(2, nameof(RemoveChildFromContainer)))
                return JavaScriptValue.Undefined;

            try
            {
                var container = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
                var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

                RemoveChildFromContainer(container, child);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue ResetTextContent(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(ResetTextContent)))
                return JavaScriptValue.Undefined;

            try
            {
                var element = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                ResetTextContent(element);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue HideInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(HideInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var element = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                HideInstance(element);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue HideTextInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(HideTextInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var element = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                HideTextInstance(element);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue UnhideInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(UnhideInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var element = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                UnhideInstance(element);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }

        private static JavaScriptValue UnhideTextInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (!arguments.ValidateWithExternalData<VisualElement>(1, nameof(UnhideTextInstance)))
                return JavaScriptValue.Undefined;

            try
            {
                var element = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

                UnhideTextInstance(element);

                return JavaScriptValue.Undefined;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return JavaScriptValue.Undefined;
            }
        }
    }
}