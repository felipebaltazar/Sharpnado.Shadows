﻿using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Sharpnado.Shades;

public class Shade : Element
{
    public static readonly BindableProperty OffsetProperty = BindableProperty.Create(
        nameof(Offset),
        typeof(Point),
        typeof(Shade),
        defaultValueCreator: _ => DefaultOffset);

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(Shade),
        defaultValueCreator: _ => DefaultColor);

    public static readonly BindableProperty OpacityProperty = BindableProperty.Create(
        nameof(Opacity),
        typeof(double),
        typeof(Shade),
        defaultValue: DefaultOpacity);

    public static readonly BindableProperty BlurRadiusProperty = BindableProperty.Create(
        nameof(BlurRadius),
        typeof(double),
        typeof(Shade),
        DefaultBlurRadius);

    private const double DefaultBlurRadius = 12f;

    private const double DefaultOpacity = 0.24f;

    private static readonly Color DefaultColor = new Color(0, 0, 0, Convert.ToSingle(DefaultOpacity, CultureInfo.InvariantCulture));

    private static readonly Point DefaultOffset = new Point(0, 8);

    readonly WeakEventManager _weakPropertyChangedSource = new ();

    public event EventHandler<PropertyChangedEventArgs> WeakPropertyChanged
    {
        add => _weakPropertyChangedSource.AddEventHandler(value);
        remove => _weakPropertyChangedSource.RemoveEventHandler(value);
    }

    public Point Offset
    {
        get => (Point)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    public double BlurRadius
    {
        get => (double)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    public static bool IsShadeProperty(string propertyName)
    {
        return propertyName == nameof(Offset)
               || propertyName == nameof(Color)
               || propertyName == nameof(Opacity)
               || propertyName == nameof(BlurRadius);
    }

    public override string ToString() =>
        $"{{ Offset: {Offset}, Color: {{A={Color.Alpha}, R={Colors.Red}, G={Colors.Green}, B={Colors.Blue}}}, Opacity: {Opacity}, BlurRadius: {BlurRadius} }}";

    public Shade Clone()
    {
        return new Shade { BlurRadius = BlurRadius, Color = Color, Offset = Offset, Opacity = Opacity };
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        _weakPropertyChangedSource.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(WeakPropertyChanged));
    }
}