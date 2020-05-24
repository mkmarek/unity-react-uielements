using System;
using ChakraHost.Hosting;
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
                JavaScriptValue.CreateFunction(GetPublicInstance),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("getRootHostContext"),
                JavaScriptValue.CreateFunction(GetRootHostContext),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("getChildHostContext"),
                JavaScriptValue.CreateFunction(GetChildHostContext),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("createInstance"),
                JavaScriptValue.CreateFunction(CreateInstance),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("prepareForCommit"),
                JavaScriptValue.CreateFunction(PrepareForCommit),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("resetAfterCommit"),
                JavaScriptValue.CreateFunction(ResetAfterCommit),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("appendInitialChild"),
                JavaScriptValue.CreateFunction(AppendInitialChild),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("finalizeInitialChildren"),
                JavaScriptValue.CreateFunction(FinalizeInitialChildren),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("prepareUpdate"),
                JavaScriptValue.CreateFunction(PrepareUpdate),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("shouldSetTextContent"),
                JavaScriptValue.CreateFunction(ShouldSetTextContent),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("shouldDeprioritizeSubtree"),
                JavaScriptValue.CreateFunction(ShouldDeprioritizeSubtree),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("createTextInstance"),
                JavaScriptValue.CreateFunction(CreateTextInstance),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("appendChild"),
                JavaScriptValue.CreateFunction(AppendChild),
                true);

            prototype.SetProperty(
                JavaScriptPropertyId.FromString("appendChildToContainer"),
                JavaScriptValue.CreateFunction(AppendChildToContainer),
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
            if (arguments.Length < 2) throw new InvalidOperationException("GetPublicInstance method expects at least one argument");

            var instance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

            var result = GetPublicInstance(instance);

            return result.ToJavaScriptValue();
        }

        private static JavaScriptValue GetRootHostContext(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 2) throw new InvalidOperationException("GetRootHostContext method expects at least one argument");

            var rootContainer = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

            var result = GetRootHostContext(rootContainer);

            return result.ToJavaScriptValue();
        }

        private static JavaScriptValue GetChildHostContext(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 4) throw new InvalidOperationException("GetChildHostContext method expects at least 3 arguments");

            var parentHostContext = arguments[1].ObjectFromJavaScriptValue<HostContext>();
            var type = arguments[2].ToString();
            var rootContainer = arguments[3].ObjectFromJavaScriptValue<VisualElement>();

            var result = GetChildHostContext(parentHostContext, type, rootContainer);

            return result.ToJavaScriptValue();
        }

        private static JavaScriptValue CreateInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 2) throw new InvalidOperationException("CreateInstance method expects at least 3 arguments");

            var type = arguments[1].ToString();

            var result = CreateInstance(type, null, null);

            return result.ToJavaScriptValue();
        }

        private static JavaScriptValue PrepareForCommit(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 2) throw new InvalidOperationException("PrepareForCommit method expects at least 1 argument");

            var container = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

            PrepareForCommit(container);

            return JavaScriptValue.Undefined;
        }

        private static JavaScriptValue ResetAfterCommit(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 2) throw new InvalidOperationException("ResetAfterCommit method expects at least 1 argument");

            var container = arguments[1].ObjectFromJavaScriptValue<VisualElement>();

            ResetAfterCommit(container);

            return JavaScriptValue.Undefined;
        }

        private static JavaScriptValue AppendInitialChild(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 3) throw new InvalidOperationException("AppendInitialChild method expects at least 2 arguments");

            var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
            var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

            AppendInitialChild(parent, child);

            return JavaScriptValue.Undefined;
        }

        private static JavaScriptValue AppendChild(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 3) throw new InvalidOperationException("AppendInitialChild method expects at least 2 arguments");

            var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
            var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

            AppendChild(parent, child);

            return JavaScriptValue.Undefined;
        }

        private static JavaScriptValue AppendChildToContainer(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 3) throw new InvalidOperationException("AppendInitialChild method expects at least 2 arguments");

            var parent = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
            var child = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

            AppendChildToContainer(parent, child);

            return JavaScriptValue.Undefined;
        }

        private static JavaScriptValue FinalizeInitialChildren(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 6) throw new InvalidOperationException("FinalizeInitialChildren method expects at least 5 arguments");

            var parentInstance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
            var type = arguments[2].ToString();
            //var child = arguments[3].ObjectFromJavaScriptValue<VisualElement>();
            var rootContainer = arguments[4].ObjectFromJavaScriptValue<VisualElement>();
            var hostContext = arguments[5].ObjectFromJavaScriptValue<HostContext>();

            FinalizeInitialChildren(parentInstance, type, null, rootContainer, hostContext);

            return JavaScriptValue.Undefined;
        }

        private static JavaScriptValue PrepareUpdate(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 7) throw new InvalidOperationException("PrepareUpdate method expects at least 6 arguments");

            var instance = arguments[1].ObjectFromJavaScriptValue<VisualElement>();
            var type = arguments[2].ToString();
            //var oldProps = arguments[3].ObjectFromJavaScriptValue<VisualElement>();
            //var newProps = arguments[4].ObjectFromJavaScriptValue<VisualElement>();
            var rootContainer = arguments[5].ObjectFromJavaScriptValue<VisualElement>();
            var hostContext = arguments[6].ObjectFromJavaScriptValue<HostContext>();

            var props = PrepareUpdate(instance, type, null, null, rootContainer, hostContext);

            return JavaScriptValue.Null;
        }

        private static JavaScriptValue ShouldSetTextContent(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 3) throw new InvalidOperationException("ShouldSetTextContent method expects at least 2 arguments");

            var type = arguments[1].ToString();
            //var props = arguments[3].ObjectFromJavaScriptValue<VisualElement>();

            var result = ShouldSetTextContent(type, null);

            return JavaScriptValue.FromBoolean(result);
        }

        private static JavaScriptValue ShouldDeprioritizeSubtree(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 3) throw new InvalidOperationException("ShouldSetTextContent method expects at least 2 arguments");

            var type = arguments[1].ToString();
            //var props = arguments[2].ObjectFromJavaScriptValue<VisualElement>();

            var result = ShouldDeprioritizeSubtree(type, null);

            return JavaScriptValue.FromBoolean(result);
        }

        private static JavaScriptValue CreateTextInstance(
            JavaScriptValue callee,
            bool isconstructcall,
            JavaScriptValue[] arguments,
            ushort argumentcount,
            IntPtr callbackdata)
        {
            if (arguments.Length < 3) throw new InvalidOperationException("ShouldSetTextContent method expects at least 2 arguments");

            var text = arguments[1].ToString();
            var rootContainer = arguments[2].ObjectFromJavaScriptValue<VisualElement>();
            var hostContext = arguments[3].ObjectFromJavaScriptValue<HostContext>();

            var result = CreateTextInstance(text, rootContainer, hostContext);

            return result.ToJavaScriptValue();
        }
    }
}