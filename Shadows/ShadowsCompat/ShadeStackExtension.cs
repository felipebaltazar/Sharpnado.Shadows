using System.Collections.ObjectModel;

namespace Sharpnado.Shades;

[ContentProperty(nameof(Items))]
[AcceptEmptyServiceProvider]
public class ShadeStackExtension : IMarkupExtension<ObservableCollection<Shade>>
{
    public List<Shade> Items { get; } = [];

    public ObservableCollection<Shade> ProvideValue(IServiceProvider serviceProvider) 
        => Items == null ? [] : new ObservableCollection<Shade>(Items);

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) 
        => (this as IMarkupExtension<IReadOnlyCollection<Shade>>).ProvideValue(serviceProvider);
}