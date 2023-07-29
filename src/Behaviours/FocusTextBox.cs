using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace NxEditor.EpdPlugin.Behaviours;

public class FocusTextBox : Behavior, IAction
{
    public object? Execute(object? sender, object? parameter)
    {
        if (sender is TextBox textBox) {
            return textBox?.Focus();
        }

        return false;
    }
}
