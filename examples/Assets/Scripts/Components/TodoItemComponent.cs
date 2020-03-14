using System.Runtime.InteropServices.ComTypes;
using Unity.Entities;

namespace UnityReactUIElements.Examples
{
    public unsafe struct TodoItemComponent : IComponentData
    {
        public fixed byte data[128];
    }
}