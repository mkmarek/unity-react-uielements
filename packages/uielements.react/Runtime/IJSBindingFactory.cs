using ChakraHost.Hosting;
using Unity.Entities;

namespace UnityReactUIElements
{
    public interface IJSBindingFactory
    {
        string Name { get; }
        ComponentType ReadComponentType { get; }
        int ComponentSize { get; }

        JavaScriptValue CreateConstructor();
        unsafe JavaScriptValue CreateJsObjectForNative(void* ptr, bool finalize);
    }
}
