using System.ComponentModel;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Sharpnado.Shades.Droid;

public class ShadowsHandler : ViewHandler<Shadows, FrameLayout>
{
    public static IPropertyMapper<Shadows, ShadowsHandler> Mapper = new PropertyMapper<Shadows, ShadowsHandler>();
    public static CommandMapper<Shadows, ShadowsHandler> CommandMapper = new();

    private static int instanceCount;

    private ShadowView _shadowView;
    private string _tag = nameof(ShadowsHandler);
    private LayoutChangeListener _listner;

    protected Shadows Element => VirtualView;

    public ShadowsHandler() : base(Mapper, CommandMapper)
    {
        _listner = new LayoutChangeListener(OnLayout);
    }

    protected override void ConnectHandler(FrameLayout platformView)
    {

        base.ConnectHandler(platformView);
        platformView.AddOnLayoutChangeListener(_listner);
        VirtualView.PropertyChanged += OnElementPropertyChanged;

        if (!_shadowView.IsNullOrDisposed())
        {
            _shadowView.RemoveFromParent();
            _shadowView.Dispose();
        }
    }

    protected override void DisconnectHandler(FrameLayout platformView)
    {
        base.DisconnectHandler(platformView);

        if (!_shadowView.IsNullOrDisposed())
        {
            _shadowView.RemoveFromParent();
            _shadowView.Dispose();
        }

        instanceCount--;

        InternalLogger.Debug(_tag, () => $"Disposed( => {instanceCount} instances");
    }

    protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "Renderer":
                var content = PlatformView.GetChildAt(0);
                if (content == null)
                    return;

                if (!string.IsNullOrWhiteSpace(Element.StyleId))
                    _tag += $" | {Element.StyleId}@{Element.InstanceNumber}";

                if (_shadowView == null)
                {
                    _shadowView = new ShadowView(Context, content, Context.ToPixels(Element.CornerRadius));
                    _shadowView.UpdateShades(Element.Shades);

                    Element.WeakCollectionChanged += _shadowView.ShadesSourceCollectionChanged;

                    PlatformView.AddView(_shadowView, 0);

                    instanceCount++;
                    InternalLogger.Debug(_tag, () => $"Create ShadowView => {instanceCount} instances");
                }

                break;

            case nameof(Element.CornerRadius):
                _shadowView.UpdateCornerRadius(Context.ToPixels(Element.CornerRadius));
                break;

            case nameof(Element.Shades):
                _shadowView.UpdateShades(Element.Shades);
                break;
        }
    }


    protected void OnLayout(bool changed, int l, int t, int r, int b)
    {
        InternalLogger.Debug(_tag, () => $"OnLayout( {l}l, {t}t, {r}r, {b}b )");

        var children = PlatformView.GetChildAt(1);
        if (children == null)
        {
            return;
        }

        _shadowView?.Layout(children.MeasuredWidth, children.MeasuredHeight);
    }

    protected override FrameLayout CreatePlatformView() => new(Context);
}