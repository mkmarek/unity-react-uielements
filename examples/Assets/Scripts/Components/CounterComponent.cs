using System.Runtime.InteropServices;
using Unity.Entities;
using UnityReactUIElements;

[assembly: JSBindingSystem]
[assembly: JSTypeBinding(typeof(Entity))]

namespace UnityReactUIElements.Examples
{
    [JSBinding]
    public struct SomeNestedStruct
    {
        public int Test;
        //public FixedString128 Str;
        public Entity Entity;
    }

    [JSBinding]
    [StructLayout(LayoutKind.Sequential)]
    public struct CounterComponent : IComponentData
    {
        public int IntTest;
        public short ShortTest;
        public float FloatTest;
        public double DoubleTest;
        public SomeNestedStruct Nested;
    }

    [JSBinding]
    [StructLayout(LayoutKind.Sequential)]
    public struct SomeOtherCddomponent : IComponentData
    {
        public int Bla;
    }
}