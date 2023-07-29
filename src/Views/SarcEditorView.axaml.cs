using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NxEditor.EpdPlugin.Models.Sarc;
using NxEditor.EpdPlugin.ViewModels;

namespace NxEditor.EpdPlugin.Views;

public partial class SarcEditorView : UserControl
{
    public SarcEditorView()
    {
        InitializeComponent();

        DropClient.AddHandler(DragDrop.DragEnterEvent, DragEnterEvent);
        DropClient.AddHandler(DragDrop.DropEvent, DragDropEvent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        DropClient.Focus();
    }

    public void TreeViewDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is SarcEditorViewModel vm) {
            vm.Edit();
        }
    }

    public void RenameClientAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is SarcFileNode node) {
            node.SetRenameClient(tb);
        }
    }

    public void RenameLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SarcEditorViewModel vm && sender is TextBox tb && tb.DataContext is SarcFileNode node) {
            node.EndRename(vm);
        }
    }

    public void RenameKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is SarcFileNode node
            && e.Key is Key.Escape or Key.Tab or Key.Enter
            && DataContext is SarcEditorViewModel vm) {

            node.EndRename(vm);
            e.Handled = true;

            DropClient.Focus(NavigationMethod.Pointer);
        }
    }

    public void RenameDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is TextBlock tb && tb.DataContext is SarcFileNode node) {
            node.BeginRename();
        }

        e.Handled = true;
    }

    public void FieldKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is TextBox tb) {
            if (e.Key == Key.Escape) {
                tb.IsVisible = false;
            }
            else if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.Shift) {
                if (DataContext is SarcEditorViewModel vm) {
                    vm.FindNext(clearSelection: true, findLast: true);
                }
            }
            else if (e.Key == Key.Enter) {
                if (DataContext is SarcEditorViewModel vm) {
                    vm.FindNext(clearSelection: true);
                }
            }
            else if (e.Key == Key.F && e.KeyModifiers == KeyModifiers.Control) {
                if (DataContext is SarcEditorViewModel vm) {
                    vm.Find();
                }
            }
            else if (e.Key == Key.H && e.KeyModifiers == KeyModifiers.Control) {
                if (DataContext is SarcEditorViewModel vm) {
                    vm.FindAndReplace();
                }
            }
            else if (e.Key == Key.Tab) {
                (tb.Name == nameof(FindField) ? ReplaceField : FindField).Focus();
            }
            else {
                return;
            }

            e.Handled = true;
        }
    }

    public void DragDropEvent(object? sender, DragEventArgs e)
    {
        if (e.Data.GetFiles() is IEnumerable<IStorageItem> paths) {
            foreach (var path in paths.Select(x => x.Path.LocalPath)) {
                if (DataContext is SarcEditorViewModel vm) {
                    object? src = e.Source;
                    SarcFileNode? node = null;
                    while (src is not TreeView && src is Control control) {
                        if (src is TreeViewItem target) {
                            node = target.DataContext as SarcFileNode;
                            break;
                        }

                        src = control.Parent;
                    }

                    node = node?.IsFile == true ? node?.Parent : node;

                    if (File.Exists(path)) {
                        vm.ImportFile(path, File.ReadAllBytes(path), parentNode: node);
                    }
                    else {
                        vm.ImportFolder(path, importTopLevel: true, parentNode: node);
                    }
                }
            }
        }

        e.Handled = true;
    }

    public void DragEnterEvent(object? sender, DragEventArgs e)
    {
        e.Handled = true;
    }
}