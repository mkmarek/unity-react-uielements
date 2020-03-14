using System.Runtime.InteropServices;
using Unity.Entities;

namespace UnityReactUIElements.Examples
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CounterComponent : IComponentData
    {
        public int count;
    }
}