using System.Runtime.InteropServices;
using Unity.Entities;
using UnityReactUIElements;

[assembly: JSBindingSystem]

namespace UnityReactUIElements.Examples
{
    [JSBinding]
    [StructLayout(LayoutKind.Sequential)]
    public struct CounterComponent : IComponentData
    {
        public int Count;
    }
}