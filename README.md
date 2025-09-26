# Hotkey Typer

A tiny Windows Forms utility that types a predefined block of text into the currently focused application using a global hotkey. It's perfect for demos, tutorials, or any workflow where you need to reproduce long prompts quickly and naturally.

Key features:

- Global hotkey (default: CTRL+SHIFT+1)
- Human-like typing with randomized delays
- Persistent settings stored in `settings.json`
- System tray integration for unobtrusive background operation

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
2. Enter your desired text in the main window and click "Update Text" to save.
3. Switch to any application and press CTRL+SHIFT+1 to have Hotkey Typer type the configured snippet.
4. Minimize to tray to keep the app running in the background.

Notes

- Some applications or elevated processes may block automated input; run Hotkey Typer with matching elevation if needed.
- Because the app simulates keystrokes, security tools may flag it. Only run it in trusted environments.

See `blogs/announce-hotkeytyper.md` for a longer announcement and implementation notes.
