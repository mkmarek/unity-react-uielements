using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge
{
    internal partial class HostConfig
    {
        public static VisualElement GetPublicInstance(VisualElement instance)
        {
            return instance;
        }

        public static HostContext GetRootHostContext(VisualElement rootContainer)
        {
            return new HostContext();
        }

        public static HostContext GetChildHostContext(HostContext parentHostContext, string type, VisualElement rootContainer)
        {
            return parentHostContext;
        }

        public static void PrepareForCommit(VisualElement container)
        {
        }

        public static void ResetAfterCommit(VisualElement container)
        {
        }

        public static VisualElement CreateInstance(string type, IComponentProps props, VisualElement rootContainer)
        {
            var component = ComponentMapper.CreateComponent(type);
            ComponentMapper.ApplyProps(type, component, props);

            return component;
        }

        public static void AppendInitialChild(VisualElement parent, VisualElement child)
        {
            parent.Add(child);
        }

        public static bool FinalizeInitialChildren(
            VisualElement parentInstance,
            string type,
            IComponentProps props,
            VisualElement rootContainer,
            HostContext hostContext)
        {
            return true;
        }

        public static IComponentProps PrepareUpdate(
            VisualElement instance,
            string type,
            IComponentProps oldProps,
            IComponentProps newProps,
            VisualElement rootContainer,
            HostContext hostContext)
        {
            return ComponentMapper.MakePropsDiff(type, oldProps, newProps);
        }

        public static bool ShouldSetTextContent(string type, IComponentProps props)
        {
            return type == "text";
        }

        public static bool ShouldDeprioritizeSubtree(string type, IComponentProps props)
        {
            return false;
        }

        public static VisualElement CreateTextInstance(string text, VisualElement rootContainer, HostContext hostContext)
        {
            return new TextElement()
            {
                text = text
            };
        }

        public static void AppendChild(VisualElement parent, VisualElement child)
        {
            parent.Add(child);
        }

        public static void AppendChildToContainer(VisualElement container, VisualElement child)
        {
            container.Add(child);
        }
    }
}