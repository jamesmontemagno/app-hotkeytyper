# Hotkey Typer

A tiny Windows Forms utility that types a predefined block of text into the currently focused application using a global hotkey. It's perfect for demos, tutorials, or any workflow where you need to reproduce long prompts quickly and naturally.

Key features:

- **Configurable global hotkey** (default: CTRL+SHIFT+1) - customize to your preference
- Human-like typing with randomized delays
- Persistent settings stored in `settings.json`
- System tray integration for unobtrusive background operation
	- Hotkey now remains active after minimizing to tray (handle recreation is auto-handled)
- Custom multi-resolution, theme-aware icon generated at runtime
	- Automatically detects Windows light/dark theme and adjusts icon colors
	- Supports multiple resolutions (16, 20, 24, 32, 48, 64px) for crisp display at all DPI scales
	- First run exports multi-size `HotkeyTyper.ico` beside the executable; replace it with your own if desired

Repository:

https://github.com/jamesmontemagno/app-hotkeytyper

Requirements

- Windows 10 or later
- .NET SDK (10.0 or later)

Clone, build, and run (PowerShell):

```powershell
git clone https://github.com/jamesmontemagno/app-hotkeytyper.git
cd app-hotkeytyper/HotkeyTyper
dotnet build
dotnet run
```

Usage

1. Start the app (see above).
2. Enter your desired text in the main window and click "Save" to save.
3. (Optional) Customize the global hotkey by clicking in the "Hotkey" field and pressing your desired key combination (e.g., CTRL+ALT+T).
4. Switch to any application and press your configured hotkey to have Hotkey Typer type the configured snippet.
5. Minimize to tray to keep the app running in the background.
	- The global hotkey remains active even after minimizing or toggling taskbar visibility.

## Configuring the Hotkey

The hotkey section allows you to customize the global shortcut:

- **Change hotkey**: Click in the Hotkey field and press your desired combination (e.g., Ctrl+Alt+F5)
- **Requirements**: 
  - Must include at least one modifier key (Ctrl, Shift, Alt, or Win)
  - Must include a primary key (letter, number, or function key)
- **Clear**: Clears the current hotkey (you'll need to set a new one to use the app)
- **Restore Default**: Reverts to Ctrl+Shift+1
- **Conflict handling**: If your chosen hotkey is already in use by another application, the app will notify you and revert to the previous working hotkey

The hotkey configuration is saved in `settings.json` and persists across restarts.

Notes

- Some applications or elevated processes may block automated input; run Hotkey Typer with matching elevation if needed.
- Because the app simulates keystrokes, security tools may flag it. Only run it in trusted environments.
 - The tray + taskbar icon is generated at runtime with multiple resolutions and theme-aware colors (see `IconFactory.cs`). The icon automatically adapts to Windows light/dark theme. To supply your own static icon, add an .ico file to the project and set `<ApplicationIcon>YourIcon.ico</ApplicationIcon>` in the csproj, then assign it to the tray `NotifyIcon` in `Form1`.
 - A copy of the generated multi-resolution icon is saved as `HotkeyTyper.ico` (only if the file does not already exist) for convenience.

## Typing source code & special characters

Hotkey Typer uses `SendKeys` under the hood. Certain characters have special meaning in `SendKeys` ( `+ ^ % ~ ( ) { }` ).
Previously, entering raw C# (e.g. `public void Concat<T>(params ReadOnlySpan<T> items)`) could trigger the runtime error:

```
Group delimiters are not balanced.
```

This happened because parentheses are grouping delimiters in `SendKeys` and must be escaped to be typed literally.

The app now automatically escapes these characters for you. You can paste or type source code containing:

- Parentheses: `(` `)`
- Braces: `{` `}`
- Plus / caret / percent / tilde: `+ ^ % ~`
- Tabs and newlines (converted to real Tab / Enter key presses)

If you notice any character that still fails to type correctly, open an issue with the exact snippet.

## For Developers

### Theme-Aware Message Boxes

The application uses a custom `ThemedMessageBox` class instead of the built-in `MessageBox` to respect the system dark mode theme. 

**Important:** Always use `ThemedMessageBox.Show()` instead of `MessageBox.Show()` when displaying message boxes throughout the application. This ensures consistent theming and a better user experience in both light and dark modes.

Usage:
```csharp
// Simple message
ThemedMessageBox.Show("Message text", "Title");

// With icon and buttons
ThemedMessageBox.Show("Delete this item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

// With owner window
ThemedMessageBox.Show(this, "Update available!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
```

See `blogs/announce-hotkeytyper.md` for a longer announcement and implementation notes.
