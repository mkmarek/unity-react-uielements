namespace UnityReactUIElements.Bridge.Components
{
    public interface IReactElement
    {
        void UpdateProps(JsToNativeBridgePayload.BridgeMessage.ComponentProps props);
    }
}
