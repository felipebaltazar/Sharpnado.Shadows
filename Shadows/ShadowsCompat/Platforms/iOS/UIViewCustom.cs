using CoreAnimation;
using UIKit;

namespace Sharpnado.Shades.iOS;

public class UIViewCustom(Action<CALayer> onSublayers) : UIView
{
    public override void LayoutSublayersOfLayer(CALayer layer)
    {
        base.LayoutSublayersOfLayer(layer);
        onSublayers(layer);
    }
}