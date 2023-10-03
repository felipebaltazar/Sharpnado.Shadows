﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Sharpnado.Shades
{
    [AcceptEmptyServiceProvider]
    public class NeumorphismShadesExtension : IMarkupExtension<IReadOnlyCollection<Shade>>
    {
        private static readonly Color LowerShadeColor = Color.FromArgb("#19000000");

        private readonly ReadOnlyCollection<Shade> _neumorphismShades;

        private readonly Shade _upperShade;
        private readonly Shade _lowerShade;

        public NeumorphismShadesExtension()
        {
            _upperShade = new Shade
                {
                    BlurRadius = 15,
                    Opacity = 1,
                    Offset = new Point(-10, -10),
                    Color = Colors.White,
                };

            _lowerShade = new Shade
                {
                    BlurRadius = 15,
                    Opacity = 1,
                    Offset = new Point(10, 10),
                    Color = LowerShadeColor,
                };

            _neumorphismShades = new ReadOnlyCollection<Shade>(new List<Shade> { _upperShade, _lowerShade });
        }

        public Point UpperOffset
        {
            get => _upperShade.Offset;
            set => _upperShade.Offset = value;
        }

        public Point LowerOffset
        {
            get => _lowerShade.Offset;
            set => _lowerShade.Offset = value;
        }

        public IReadOnlyCollection<Shade> ProvideValue(IServiceProvider serviceProvider)
        {
            return _neumorphismShades;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<IReadOnlyCollection<Shade>>).ProvideValue(serviceProvider);
        }
    }
}
