using UnityEngine;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge
{
    internal partial class HostConfig
    {
        public static VisualElement GetPublicInstance(VisualElement instance)
        {
            Debug.Log("GetPublicInstance");
            return instance;
        }

        public static HostContext GetRootHostContext(VisualElement rootContainer)
        {
            Debug.Log("GetRootHostContext");
            return new HostContext();
        }

        public static HostContext GetChildHostContext(HostContext parentHostContext, string type, VisualElement rootContainer)
        {
            Debug.Log("GetChildHostContext");
            return parentHostContext;
        }

        public static void PrepareForCommit(VisualElement container)
        {
            Debug.Log($"PrepareForCommit");
        }

        public static void ResetAfterCommit(VisualElement container)
        {
            Debug.Log($"ResetAfterCommit");
        }

        public static VisualElement CreateInstance(string type, IComponentProps props, VisualElement rootContainer)
        {
            Debug.Log($"CreateInstance {type}");
            return new VisualElement();
        }

        public static void AppendInitialChild(VisualElement parent, VisualElement child)
        {
            Debug.Log($"AppendInitialChild");
            parent.Add(child);
        }

        public static bool FinalizeInitialChildren(
            VisualElement parentInstance,
            string type,
            IComponentProps props,
            VisualElement rootContainer,
            HostContext hostContext)
        {
            Debug.Log($"FinalizeInitialChildren");
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
            Debug.Log($"PrepareUpdate");
            return null;
        }

        public static bool ShouldSetTextContent(string type, IComponentProps props)
        {
            Debug.Log($"ShouldSetTextContent");
            return type == "text";
        }

        public static bool ShouldDeprioritizeSubtree(string type, IComponentProps props)
        {
            Debug.Log($"ShouldDeprioritizeSubtree");
            return false;
        }

        public static VisualElement CreateTextInstance(string text, VisualElement rootContainer, HostContext hostContext)
        {
            Debug.Log($"CreateTextInstance");
            return new TextElement()
            {
                text = text
            };
        }

        public static void AppendChild(VisualElement parent, VisualElement child)
        {
            Debug.Log($"AppendChild");
            parent.Add(child);
        }

        public static void AppendChildToContainer(VisualElement container, VisualElement child)
        {
            Debug.Log($"AppendChildToContainer");
            container.Add(child);
        }
    }
}