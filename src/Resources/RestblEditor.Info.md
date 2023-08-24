# RESTBl/RSTB Editor v3

- [How the editor works](#how-the-editor-works)
- [How to use the editor](#how-to-use-the-editor)
- [How the RESTBL/RSTB file works](#how-the-restbl-rstb-file-works)

# How the editor works

The editor works by applying changelog (RCL) files to the laoded RESTBL file and exporting the result.

This is done to editing groups of entries easy, while still keeping the safety of explicit edits.

## What is an RCL file?

RCL stands for **R**estbl **C**hange **L**og.

These store a set of changes made to the RESTBL, without the need for the orignal RESTBL file to compare agains. It also makes it possible to retain the names of custom entries, which would otherwise be lost to the hash table.

The syntax for these files is pretty straight forward.

### Change

Changes are denoted with an asterisk (`*`), plus (`+`), or minus (`-`) symbol followed by the entry name and value. Each symbol does something slightly different.

- `*` modifies an existing entry
- `+` adds a new entry
- `-` removes an entry

Each can be used as follows:

```diff
* Pack/Actor/Player.pack = 1157920
```

```diff
+ My/Own/Entry = 65123
```

```diff
- Actor/Player.engine__actor__ActorParam.bgyml
```

**Note:** The last type (`-`) does not require a value, since the entry is just being removed.

### Comments

The `#` symbol denotes a comment. These can be placed at the beginning of a new line to add a note to your RCL file.<br>
**Note:** Comments must be on there own line, a comment after another expressions is not supported.

Example:
```diff
# This Works
+ Entry = 0 # this does not work
```

### Groups

In addition to comments, groups can also be made for additional organization.

These are denoted with three hyphens (`---`).

Example:
```diff
--- Group Name ---
```

*(The three hyphens (`---`) at the end are optional for style)*

# How to use the editor

...

# How the RESTBL/RSTB file works

...