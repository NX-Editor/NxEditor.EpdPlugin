# RESTBl/RSTB Editor v3

- [How the editor works](#how-the-editor-works)
- [How to use the editor](#how-to-use-the-editor)
- [How the RESTBL/RSTB file works](#how-the-restbl-rstb-file-works)

# How the editor works

The editor applies changelog (RCL) files to the loaded RESTBL file and exports the result.

The RCL files make editing groups of entries easy while keeping the safety of explicit edits.

## What is an RCL file?

RCL stands for **R**estbl **C**hange **L**og.

These files store a set of changes made to the RESTBL without the need for the original RESTBL file to compare against. It also makes it possible to retain the names of custom entries, which would otherwise become lost in the hash table.

The syntax for these files is pretty straightforward.

### Change

Changes are denoted with an asterisk (`*`), plus (`+`), or minus (`-`) symbol followed by the entry name and value. Each prefix does something slightly different.

- `*` modifies an existing entry
- `+` adds a new entry
- `-` removes an entry

Use each prefix as follows:

```diff
* Pack/Actor/Player.pack = 1157920
```

```diff
+ My/Own/Entry = 65123
```

```diff
- Actor/Player.engine__actor__ActorParam.bgyml
```

**Note:** The last prefix (`-`) does not require a value since it's just getting removed.

### Comments

The `#` symbol denotes a comment. These can be placed at the beginning of a new line to add a note to your RCL file.<br>
**Note:** Comments must be on a single line; comments after another expression are unsupported.

Example:
```diff
# This Works
+ Entry = 0 # this does not work
```

### Groups

In addition to comments, groups can get added for additional organization, which can get denoted with three hyphens (`---`).

Example:
```diff
--- Group Name ---
```

*(The three hyphens (`---`) at the end are optional for style)*

---

# How to use the editor

The editor has three main areas: The RCL editor, the RCL list, and the names list.

## RCL Editor

The RCL editor is the main text area in the top left of the editor; it contains the contents of the currently selected RCL file.
Above this text area is the RCL file name and the actions for the current RCL file.

Moving from left to right - the actions are as follows:

### Format RCL File (Paint Brush Icon)

This action will format the current RCL contents, primarily used for locating the current values of existing entries.
To best use this feature, copy an entry from the names list in the bottom left and paste it into the RCL editor.
When you click **Format**, any lines that have no prefix (`*`, `+`, `-`, or `#`) will be looked up in the open RESTBL/RSTB file and added to the right of the name.<br>
Example: `TexToGo/Armor_001_Head_Alb.txtg` > `* TexToGo/Armor_001_Head_Alb.txtg = 66400`.

### Save RCL File (Floppy Disk Icon)

This action saves the current RCL contents to disk.
  
### Delete RCL File (Trash Can Icon)

This action permanently deletes the current RCL file.
  
### Add New RCL File (Plus Icon)

This action creates a new blank RCL file with a default name (`changelog-#`).

### Export RCL File (Arrow Out of Bracket Icon)

Opens a file dialog to save the current RCL file to your PC.

### RESTBL/RSTB Info

This action opens a dialog directing to this document.

## RCL File List

To the right of the RCL editor is the **RCL File List** - this contains a list of all the RCL files in local storage.

On each entry there is a red/green dot. When this dot is green, the RCL file is *enabled*, and the contents will be applied when saving the open RESTBL/RSTB file.
When the dot is red, the RCL file gets skipped.

## Names List

Just below the main RCL editor is the names list. This text area will either be chock-full of file names or blank.
If it's blank, check **RESTBL Strings** path in the RESTBL settings (you'll need to re-open the editor after changing this).

The **Names List** is just used to copy/paste entries into the RCL editor, then you can edit the size value by applying the RCL file.

---

# How the RESTBL/RSTB file works

The RESTBL/RSTB file stores the minimum required allocation sizes for most files in the game it's used in (I.e., BotW, TotK, and a few others).

The allocation sizes get organized by file name in two different tables.

The first and primary table is the **Hash Table**, which stores every file name as a 32-bit hash to save space and make searching faster.
However, as you may have guessed when condensing a verbose file name to a 4-character (32-bit) entry, you will have some duplicate keys.

That is where the second table, commonly referred to as the **Name Table** or **Collision Table**, will be used.
It stores the entries with the full file name, but only if the hashed version is already in the **Hash Table** with a different file name. _(Hence the name **“Collision Table”**)_

## String Table

Because most entries get stored as an irreversible hash ([CRC32](https://en.wikipedia.org/wiki/Cyclic_redundancy_check)), we need the original file names so they can get read by humans.

That is the reason behind the string table; it stores every file name to match with a hash in the **Hash Table**.