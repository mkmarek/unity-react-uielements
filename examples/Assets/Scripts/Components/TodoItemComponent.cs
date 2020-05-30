using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;

namespace UnityReactUIElements.Examples
{
    [JSBinding]
    [StructLayout(LayoutKind.Sequential)]
    public struct TodoItemComponent : IComponentData
    {
        public FixedString128 Content;
    }
}