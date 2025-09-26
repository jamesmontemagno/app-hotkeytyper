---
title: Announcing Hotkey Typer — type long prompts with a hotkey
date: 2025-09-26
author: James Montemagno
---

## The problem I wanted to solve

When I'm recording demos or making videos I often need to reproduce long prompts or blocks of text on-screen. Manually retyping them every time is slow and error-prone, and pasting from the clipboard can feel jerky and reveal private content. I wanted a tiny, reliable app that would let me press a global hotkey and have the application type a predefined, human-like block of text into whatever app currently has focus — perfect for demos, tutorials, and repeatable workflows.

Enter Hotkey Typer: a small Windows Forms utility that types back long prompts using a single global hotkey.

<iframe width="560" height="315" src="https://www.youtube.com/embed/W_FuXY4DGrk?si=5eOASvGlGrnlhAsm" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>

## What Hotkey Typer does

- Register a global hotkey (defaults to CTRL+SHIFT+1) that works across applications
- Type a predefined text snippet into the currently focused window with human-like timing
- Save your snippet and typing speed to a `settings.json` file for persistence
- Minimize to the system tray so it runs unobtrusively in the background

This is intentionally minimal — it solves one problem very well: quickly and reliably retyping long prompts during recordings.

## Quick feature tour

- Global hotkey: Press CTRL+SHIFT+1 from any running application to trigger typing.
- Human-like typing: Characters are sent one-by-one with randomized delays so the result looks natural on screen.
- Typing speed slider: Choose from Very Slow through Very Fast (1–10) to simulate different typing paces.
- Persistent settings: Your text and speed are saved to `settings.json` in the app directory.
- System tray: Minimize the UI and keep the app running in the tray with a context menu to show or exit.

## How I built it (and how GitHub Copilot + VS Code helped)

This little app was built as a Windows Forms app targeting .NET 10. I used Visual Studio Code as my editor and relied on GitHub Copilot to accelerate routine code — generating method bodies, serialization snippets, and the interop signatures for the Windows API calls.

Why Copilot was helpful here:

- Saved time writing boilerplate: form initialization, settings load/save, and JSON serialization were scaffolded quickly.
- Suggested idiomatic C# snippets: small things like the randomized delay logic and the speed-to-label mapping were easier to iterate on.
- Encouraged small, testable functions: I split concerns into LoadSettings/SaveSettings, InitializeSystemTray, TypePredefinedText, and so on.

I used VS Code for quick editing and testing, and dotnet CLI for building and running the app.

## How to build and run

Clone the repo and run it with the .NET SDK installed (Windows 10+):

```powershell
git clone https://github.com/jamesmontemagno/app-hotkeytyper.git
cd app-hotkeytyper/HotkeyTyper
dotnet build
dotnet run
```

The app will start and register the default hotkey. Enter your desired text, click Update Text, then switch to any application and press CTRL+SHIFT+1 to have the app type your snippet.

If you want the app to always run in the background, minimize it to the tray.

## Implementation notes — how some features work

Here are a few implementation details for anyone curious or wanting to extend the app.

- Global hotkey registration

  The app uses the Windows API functions `RegisterHotKey` and `UnregisterHotKey` via P/Invoke. The main form registers a hotkey during construction and handles the hotkey event in `WndProc` by checking for `WM_HOTKEY` and the registered id.

  Example P/Invoke signatures used in `Form1.cs`:

  ```csharp
  [DllImport("user32.dll")]
  private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

  [DllImport("user32.dll")]
  private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  private const int WM_HOTKEY = 0x0312;
  private const int HOTKEY_ID = 1;
  ```

- Typing the text

  When the hotkey fires the app calls an async `TypePredefinedText()` method. It uses `SendKeys.SendWait()` to send characters one-by-one to the foreground window and `Task.Delay()` with a randomized interval to simulate human typing. A short initial delay ensures the hotkey release doesn't interfere with the first character.

  A simplified version of `TypePredefinedText()` looks like:

  ```csharp
  private async void TypePredefinedText()
  {
      SendKeys.Flush();
      await Task.Delay(250); // ensure hotkey is released

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
  ```

- Timing and realism

  Typing speed is an integer from 1 (slowest) to 10 (fastest). The app converts that value into a base delay and adds a bit of random variation per character. This creates a natural rhythm instead of a machine-gun paste.

- Settings persistence

  User configuration (the predefined text and the typing speed) are serialized to `settings.json` in the application directory using `System.Text.Json`. The app loads settings on startup and writes them whenever the user updates the snippet or changes speed.

  The simple settings DTO used for JSON (and its defaults):

  ```csharp
  public class AppSettings
  {
      public string PredefinedText { get; set; } = string.Empty;
      public int TypingSpeed { get; set; } = 5;
  }
  ```

- System tray

  A `NotifyIcon` with a `ContextMenuStrip` lets the app hide in the tray and provide quick actions (Show / Exit). The form's `Dispose` method unregisters the hotkey and disposes of the tray icon to keep the system state clean.

## Extending Hotkey Typer — ideas

- Support multiple named snippets and a configurable hotkey per snippet
- Add a CLI mode for scripting or remote triggering
- Use the Windows Input Injection APIs for applications that block `SendKeys`
- Add an option to send the entire string as a clipboard paste (with clipboard restoration) for faster input when acceptable

## Notes and security

Because Hotkey Typer sends keystrokes to other applications, some security software may flag it as automation. This is expected for tools that simulate input — distribute and run it only in environments you control.

Also, typing into elevated apps may require Hotkey Typer to run with the same elevation level.

## Links

- Source code: https://github.com/jamesmontemagno/app-hotkeytyper

If you'd like, I can follow up with a short demo video and a guide showing advanced configuration (multiple snippets, custom hotkeys, and CLI triggers).

---

Thanks for reading — hope Hotkey Typer makes your demos smoother!
