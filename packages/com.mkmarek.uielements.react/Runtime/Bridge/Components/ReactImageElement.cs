using UnityEngine;
using UnityReactUIElements.Bridge.Styles;
using UnityEngine.UIElements;

namespace UnityReactUIElements.Bridge.Components
{
    public class ReactImageElement : Image, IReactElement
    {
        public ReactImageElement(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props != null)
            {
                this.UpdateProps(props);
            }
        }

        public void UpdateProps(JsToNativeBridgePayload.BridgeMessage.ComponentProps props)
        {
            if (props.style != null)
            {
                StyleMapper.AssignStyleProps(props.style, this);
            }

            switch (props.pickingMode)
            {
                case "ignore": pickingMode = PickingMode.Ignore;
                    break;
                case "position": pickingMode = PickingMode.Position;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(props.image))
            {
                var texture = Resources.Load<Texture2D>(props.image);

                if (texture != null)
                {
                    image = texture;
                }
                else
                {
                    var renderTexture = Resources.Load<RenderTexture>(props.image);

                    if (renderTexture != null)
                    {
                        image = renderTexture;
                    }
                    else
                    {
                        vectorImage = Resources.Load<VectorImage>(props.image);
                    }
                }
            }

            if (props.sourceRect != null && props.sourceRect.IsSet())
            {
                sourceRect = props.sourceRect.ToRect();
            }

            if (props.uv != null && props.uv.IsSet())
            {
                uv = props.uv.ToRect();
            }

            if (!string.IsNullOrWhiteSpace(props.scaleMode))
            {
                switch (props.scaleMode)
                {
                    case "scale-and-crop":
                        scaleMode = ScaleMode.ScaleAndCrop; break;
                    case "scale-to-fit":
                        scaleMode = ScaleMode.ScaleToFit; break;
                    case "stretch-to-fill":
                        scaleMode = ScaleMode.StretchToFill; break;
                }
            }

            if (!string.IsNullOrWhiteSpace(props.tintColor))
            {
                tintColor = StyleMapper.ParseColor(props.tintColor);
            }
        }
    }
}
