using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;

namespace NxEditor.EpdPlugin.Models.Common;

public abstract class TextEditorBase<T> : Editor<T, TextEditorView>, IEditorInterface where T : TextEditorBase<T>
{
    protected TextEditorBase(IFileHandle handle) : base(handle) { }

    public override void OnSelected()
    {
        View.TextEditor.Focus();
        base.OnSelected();
    }

    public override Task Undo()
    {
        View.TextEditor.Undo();
        return Task.CompletedTask;
    }

    public override Task Redo()
    {
        View.TextEditor.Redo();
        return Task.CompletedTask;
    }

    public override Task SelectAll()
    {
        View.TextEditor.SelectAll();
        return Task.CompletedTask;
    }

    public override Task Cut()
    {
        View.TextEditor.Cut();
        return Task.CompletedTask;
    }

    public override Task Copy()
    {
        View.TextEditor.Copy();
        return Task.CompletedTask;
    }

    public override Task Paste()
    {
        View.TextEditor.Paste();
        return Task.CompletedTask;
    }

    public override Task Find()
    {
        View.TextEditor.Focus();
        StatusModal.Set("Use key binding Ctrl+F", isWorkingStatus: false, temporaryStatusTime: 3);
        return Task.CompletedTask;
    }

    public override Task FindAndReplace()
    {
        View.TextEditor.Focus();
        StatusModal.Set("Use key binding Ctrl+H", isWorkingStatus: false, temporaryStatusTime: 3);
        return Task.CompletedTask;
    }

    public override void Cleanup()
    {
        View.TextMateInstallation.Dispose();
        base.Cleanup();
    }
}
