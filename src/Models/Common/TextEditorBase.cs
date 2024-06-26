﻿using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;

namespace NxEditor.EpdPlugin.Models.Common;

public abstract class TextEditorBase(IEditorFile handle) : Editor<TextEditorView>(handle), IEditorInterface, ISearchableEditor
{
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
        View.TextEditor.SearchPanel.IsReplaceMode = false;
        View.TextEditor.SearchPanel.Open();
        return Task.CompletedTask;
    }

    public override Task FindAndReplace()
    {
        View.TextEditor.Focus();
        View.TextEditor.SearchPanel.IsReplaceMode = true;
        View.TextEditor.SearchPanel.Open();
        return Task.CompletedTask;
    }

    public override void Cleanup()
    {
        View.TextMateInstallation.Dispose();
        base.Cleanup();
    }

    public int Find(SearchContext context)
    {
        if (context.IsReplacing) {
            View.TextEditor.Text = View.TextEditor.Text
                .Replace(context.FindArgument, context.ReplaceArgument);
            return 1;
        }

        Console.WriteLine(View.TextEditor.Text.Length);
        return View.TextEditor.Text.Contains(context.FindArgument, context.StringComparison) switch {
            true => 1,
            _ => 0,
        };
    }
}
