using ChakraHost.Hosting;

namespace UnityReactUIElements
{
    public interface IJSBindingFactory
    {
        string Name { get; }
        JavaScriptValue CreateConstructor();
        unsafe JavaScriptValue CreateJsObjectForNative(void* ptr);
    }
}
