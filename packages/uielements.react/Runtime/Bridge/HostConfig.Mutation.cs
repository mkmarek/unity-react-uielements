using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge
{
    internal partial class HostConfig
    {
        public static void CommitTextUpdate(VisualElement textInstance, string oldText, string newText)
        {
            if (oldText != newText)
            {
                ((TextElement) textInstance).text = newText;
            }
        }

        public static void CommitMount(VisualElement instance, string type, IComponentProps props)
        {

        }

        public static bool CommitUpdate(
            VisualElement instance,
            IComponentProps updatePayload,
            string type,
            IComponentProps oldProps,
            IComponentProps newProps)
        {
            ComponentMapper.ApplyProps(type, instance, updatePayload);

            return true;
        }

        public static void InsertBefore(VisualElement parent, VisualElement child, VisualElement beforeChild)
        {
            var index = parent.IndexOf(beforeChild);
            parent.Insert(index, child);
        }

        public static void InsertInContainerBefore(VisualElement container, VisualElement child, VisualElement beforeChild)
        {
            var index = container.IndexOf(beforeChild);
            container.Insert(index, child);
        }

        public static void RemoveChild(VisualElement parent, VisualElement child)
        {
            parent.Remove(child);
        }

        public static void RemoveChildFromContainer(VisualElement container, VisualElement child)
        {
            container.Remove(child);
        }

        public static void ResetTextContent(VisualElement element)
        {
        }

        public static void HideInstance(VisualElement element)
        {
        }

        public static void HideTextInstance(VisualElement element)
        {
        }

        public static void UnhideInstance(VisualElement element)
        {
        }

        public static void UnhideTextInstance(VisualElement element)
        {
        }
    }
}