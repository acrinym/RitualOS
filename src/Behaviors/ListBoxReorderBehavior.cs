using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace RitualOS.Behaviors;

/// <summary>
/// Enables simple drag-and-drop reordering of ListBox items. Attach with
/// <c>behaviors:ListBoxReorderBehavior.Enabled="True"</c> in XAML.
/// </summary>
public static class ListBoxReorderBehavior
{
    public static readonly AttachedProperty<bool> EnabledProperty =
        AvaloniaProperty.RegisterAttached<ListBox, bool>("Enabled", typeof(ListBoxReorderBehavior));

    static ListBoxReorderBehavior()
    {
        EnabledProperty.Changed.AddClassHandler<ListBox>((lb, e) =>
        {
            var enabled = (bool)e.NewValue!;
            if (enabled)
                Attach(lb);
            else
                Detach(lb);
        });
    }

    public static bool GetEnabled(ListBox element) => element.GetValue(EnabledProperty);
    public static void SetEnabled(ListBox element, bool value) => element.SetValue(EnabledProperty, value);

    private static void Attach(ListBox listBox)
    {
        listBox.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        listBox.AddHandler(DragDrop.DragOverEvent, OnDragOver);
        listBox.AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private static void Detach(ListBox listBox)
    {
        listBox.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        listBox.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
        listBox.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private static async void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not ListBox lb) return;
        if (lb.SelectedItem == null) return;
        var data = new DataObject();
        data.Set("ritual-step", lb.SelectedItem);
        await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
    }

    private static void OnDragOver(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains("ritual-step")) return;
        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }

    private static void OnDrop(object? sender, DragEventArgs e)
    {
        if (sender is not ListBox lb) return;
        if (!e.Data.Contains("ritual-step")) return;
        var source = e.Data.Get("ritual-step");
        var target = (e.Source as Control)?.DataContext;
        if (source == null || target == null || ReferenceEquals(source, target)) return;
        if (lb.Items is IList items)
        {
            var sourceIndex = items.IndexOf(source);
            var targetIndex = items.IndexOf(target);
            if (sourceIndex >= 0 && targetIndex >= 0 && sourceIndex != targetIndex)
            {
                items.RemoveAt(sourceIndex);
                items.Insert(targetIndex, source);
                lb.SelectedItem = source;
            }
        }
        e.Handled = true;
    }
}
