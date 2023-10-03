﻿using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using System.ComponentModel;
using UIKit;

namespace Sharpnado.Shades.iOS
{
    public class iOSShadowsRenderer : VisualElementRenderer<Shadows>
    {
        private static int instanceCount;

        private string _tag = nameof(iOSShadowsRenderer);

        private iOSShadowsController _shadowsController;

        private CALayer _shadowsLayer;

        public static void Initialize()
        {
            var preserveRenderer = typeof(iOSShadowsRenderer);
        }

        public override void LayoutSublayersOfLayer(CALayer layer)
        {
            base.LayoutSublayersOfLayer(layer);

            _shadowsController?.OnLayoutSubLayers();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _shadowsController?.Dispose();
                _shadowsController = null;

                _shadowsLayer?.Dispose();
                _shadowsLayer = null;
            }

            instanceCount--;

            InternalLogger.Debug(_tag, () => $"Disposed( disposing: {disposing} ) => {instanceCount} instances");
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Shadows> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                _shadowsController?.Dispose();
                _shadowsController = null;

                _shadowsLayer?.Dispose();
                _shadowsLayer = null;
                return;
            }

            if (_shadowsController == null && Subviews.Length > 0)
            {
                CreateShadowController(Subviews[0], e.NewElement);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Renderer":
                    break;

                case nameof(Element.CornerRadius):
                    _shadowsController?.UpdateCornerRadius(Element.CornerRadius);
                    break;

                case nameof(Element.Shades):
                    _shadowsController?.UpdateShades(Element.Shades);
                    break;
            }
        }

        private void CreateShadowController(UIView shadowSource, Shadows formsElement)
        {
            Layer.BackgroundColor = new CGColor(0, 0, 0, 0);
            Layer.MasksToBounds = false;

            _shadowsLayer = new CALayer { MasksToBounds = false };
            Layer.InsertSublayer(_shadowsLayer, 0);

            _shadowsController = new iOSShadowsController(shadowSource, _shadowsLayer,  formsElement.CornerRadius);
            _shadowsController.UpdateShades(formsElement.Shades);

            instanceCount++;
            InternalLogger.Debug(_tag, () => $"Create ShadowView => {instanceCount} instances");
        }
    }
}
