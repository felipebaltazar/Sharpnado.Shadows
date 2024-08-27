using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sharpnado.Shades;

public class Shadows : ContentView
{
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(int),
        typeof(Shadows),
        DefaultCornerRadius);

    public static readonly BindableProperty ShadesProperty = BindableProperty.Create(
        nameof(Shades),
        typeof(IEnumerable<Shade>),
        typeof(Shadows),
        defaultValueCreator: (bo) => new ObservableCollection<Shade> { new Shade { Parent = (Shadows)bo } },
        validateValue: (bo, v) => v is IEnumerable<Shade>,
        propertyChanged: ShadesPropertyChanged,
        coerceValue: CoerceShades);

    private const int DefaultCornerRadius = 0;

    private static int instanceCount = 0;

    readonly WeakEventManager _weakCollectionChangedSource = new ();


    public Shadows()
    {
        InstanceNumber = ++instanceCount;
    }

    public event EventHandler<NotifyCollectionChangedEventArgs> WeakCollectionChanged
    {
        add => _weakCollectionChangedSource.AddEventHandler(value);
        remove => _weakCollectionChangedSource.RemoveEventHandler(value);
    }

    public int InstanceNumber { get; }

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public IEnumerable<Shade> Shades
    {
        get => (IEnumerable<Shade>)GetValue(ShadesProperty);
        set => SetValue(ShadesProperty, value);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        foreach (var shade in Shades)
            shade.BindingContext = BindingContext;
    }

    private static object CoerceShades(BindableObject bindable, object value)
    {
        if (value is not ReadOnlyCollection<Shade> readonlyCollection)
            return value;

        return new ReadOnlyCollection<Shade>(readonlyCollection.Select(s => s.Clone()).ToList());
    }

    private static void ShadesPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var shadows = (Shadows)bindable;
        var enumerableShades = (IEnumerable<Shade>)newvalue;

        if (oldvalue != null)
        {
            if (oldvalue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= shadows.OnShadeCollectionChanged;

            foreach (var shade in enumerableShades)
            {
                shade.Parent = null;
                shade.BindingContext = null;
            }
        }

        foreach (var shade in enumerableShades)
        {
            shade.Parent = shadows;
            shade.BindingContext = shadows.BindingContext;
        }

        if (newvalue is INotifyCollectionChanged newCollection)
            newCollection.CollectionChanged += shadows.OnShadeCollectionChanged;
    }

    private void OnShadeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                {
                    foreach (Shade newShade in e.NewItems)
                    {
                        newShade.Parent = this;
                        newShade.BindingContext = BindingContext;
                        _weakCollectionChangedSource.HandleEvent(this, e, nameof(WeakCollectionChanged));
                    }
                }
                break;

            case NotifyCollectionChangedAction.Reset:
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (Shade oldShade in e.OldItems)
                    {
                        oldShade.Parent = null;
                        oldShade.BindingContext = null;
                        _weakCollectionChangedSource.HandleEvent(this, e, nameof(WeakCollectionChanged));
                    }
                }
                break;
        }
    }
}