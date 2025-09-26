# Hotkey Typer — Type long prompts with a single hotkey

I was making demo videos and live streams and kept wanting an easy way to "type back" long prompts, commands, and notes without having to retype them or copy/paste from another app. I needed a small, reliable tool I could invoke from anywhere with a single keyboard shortcut while recording. So I made Hotkey Typer.

Hotkey Typer is a tiny Windows utility that lets you register a global hotkey (CTRL+SHIFT+1 by default) to automatically type a predefined block of text into whichever window is active. It's perfect for demos, tutorials, livestreams, or any time you need to paste long, repetitive text quickly and reliably.

## What problem it solves

When recording demos or making videos, repeatedly typing long prompts or commands can be disruptive and slow. Copy/pasting from a notes app can also be fiddly and may accidentally reveal other content. Hotkey Typer solves this by letting you configure a single phrase (or multi-line text) and trigger it with a global hotkey — no switching windows, no clipboard shenanigans.

## Key features

- Global hotkey to type a pre-configured block of text (CTRL+SHIFT+1 by default)
- Multiline, scrollable text editor for the predefined text
- Adjustable typing speed (1 = slowest, 10 = fastest)
- System tray support with a context menu (Show / Exit)
- Saves settings to a JSON file (`settings.json`) so your text and speed persist
- Simple, dependency-free WinForms app — single executable

## Implementation overview

The app is built using .NET and Windows Forms (WinForms). I used Visual Studio Code with GitHub Copilot to help speed development and iterate on UI code. The core pieces are:

- `Form1` — main window and UI
- Global hotkey registration using the Win32 `RegisterHotKey` API
- Typing via `SendKeys.SendWait` with small delays to ensure reliability
- Settings persisted to `settings.json` using `System.Text.Json`
- System tray icon via `NotifyIcon` and `ContextMenuStrip`

### How the hotkey works

Windows exposes a `RegisterHotKey` API that lets an application receive a message when a specified global hotkey is pressed. I register `CTRL+SHIFT+1` and handle `WM_HOTKEY` in the window procedure (`WndProc`) to detect the hotkey press and then call the typing routine.

### How typing is implemented

To avoid clipboard interference and to ensure the target application receives keystrokes, the app uses `SendKeys.SendWait` to send each character in the configured text. There's a small, configurable delay between characters based on the selected typing speed. Random variation is added to the delay to make the typing feel more natural and avoid races with the receiving app.

### Settings

Settings are serialized to a `settings.json` file next to the executable. The file contains the predefined text and typing speed so that the app restores your preferences when restarted.

## How I built and ran it

Clone the repo:

    git clone https://github.com/jamesmontemagno/app-hotkeytyper.git
    cd app-hotkeytyper/HotkeyTyper

Build and run with the .NET CLI:

    dotnet build
    dotnet run --project HotkeyTyper.csproj

Or build a release executable:

    dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

## Notable implementation snippets

- Registering the hotkey and handling `WM_HOTKEY`:

```csharp
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

protected override void WndProc(ref Message m)
{
    if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
    {
        TypePredefinedText();
    }
    base.WndProc(ref m);
}
```

- Typing the text with delays:

```csharp
private async void TypePredefinedText()
{
    try
    {
        SendKeys.Flush();
        await Task.Delay(250);

        int baseDelay = 310 - (typingSpeed * 30);
        int variation = Math.Max(10, baseDelay / 3);
        Random random = new Random();

        foreach (char c in predefinedText)
        {
            SendKeys.SendWait(c.ToString());
            SendKeys.Flush();
            await Task.Delay(Math.Max(20, random.Next(Math.Max(10, baseDelay - variation), baseDelay + variation)));
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error typing text: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

## Where to find the project

The full source code is on GitHub: https://github.com/jamesmontemagno/app-hotkeytyper

## Future improvements

- Add a UI to change the global hotkey
- Make the app DPI-aware and responsive (table layouts / anchoring)
- Support multiple saved snippets and quick-selection
- Add a safety toggle so typing only occurs when a specific window is focused

## Closing

Hotkey Typer is intentionally tiny and focused — built to solve a single problem I hit while recording demos. If you want to contribute, file issues or PRs on the repo: https://github.com/jamesmontemagno/app-hotkeytyper

If you'd like, I can also:

- Add a quick tutorial GIF showing how to set the text and trigger the hotkey
- Add unit tests around settings serialization
- Wire up a feature to manage multiple snippets from the tray menu

Thanks for reading — hope this helps make your demo workflow a little smoother!