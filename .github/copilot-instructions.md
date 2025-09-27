# Copilot Instructions for Hotkey Typer

These notes give an AI agent the minimum project-specific context to contribute productively. Keep edits focused and consistent with existing patterns.

The code lives https://github.com/jamesmontemagno/app-hotkeytyper

## Overview
Hotkey Typer is a single WinForms app (project: `HotkeyTyper`, tests: `HotkeyTyper.Tests`) targeting `net10.0-windows`. It registers a global hotkey (CTRL+SHIFT+1) and, when pressed, types a configured text snippet (or file contents) into the currently focused window with human-like timing. Settings persist in `settings.json` beside the executable. A small test suite validates `EscapeForSendKeys` behavior.

## Execution & Workflows
- Run app (development): `dotnet run --project HotkeyTyper/HotkeyTyper.csproj`
- Build all: `dotnet build TypeBack.sln`
- Run tests: `dotnet test`
- Generated runtime artifacts: `HotkeyTyper/bin/Debug/net10.0-windows/` (includes `settings.json` after first save and exported `HotkeyTyper.ico` if not present).

## Architecture & Key Components
- `Program.cs`: Standard WinForms bootstrap (`ApplicationConfiguration.Initialize(); Application.Run(new Form1());`).
- `Form1.cs`: Core logic: settings load/save, UI event wiring, global hotkey registration in `OnHandleCreated`, typing orchestration, mode toggles (code mode, file mode), status updates.
- `KeyboardInputSender.cs`: Low-level Unicode keystroke injection via `SendInput` (preferred) with fallbacks to `SendKeys` only when character escaping is required.
- `IconFactory.cs`: Dynamically generates an in-memory icon; exports a copy (`HotkeyTyper.ico`) only if absent. Manages native handle lifetime via `Destroy`.
- `LimitedTrackBar.cs`: Owner-drawn track bar with optional `SoftMax` (used to cap speed in code mode and visually gray out disallowed ticks).
- Tests (`EscapeForSendKeysTests.cs`): Assert escaping rules for `SendKeys` special characters and CR/LF handling.

## Typing Pipeline (Hotkey -> Output)
1. WM_HOTKEY handled in `WndProc` triggers `StartTyping()` if no active typing task.
2. Captures foreground window handle, delays 250ms to allow key release, restores focus.
3. Iterates characters: uses `KeyboardInputSender.SendChar` (handles newline/tab/Unicode); if that returns false, falls back to `SendKeys.SendWait(EscapeForSendKeys(c))`.
4. Delay per char: `baseDelay = 310 - speed*30` plus random jitter; enforced minimum (`MinFastDelayMs = 35`) at high speeds; contextual pause (`ContextualPauseMs = 140`) after certain boundary chars followed by a space at speeds ≥7.
5. Cancellation via CTS (`btnStop`). UI status updated on start, cancel, finish.

## Modes & Settings
Stored in JSON (`AppSettings` in `Form1.cs`): `PredefinedText`, `TypingSpeed (1-10)`, `HasCode`, `LastNonCodeSpeed`, `UseFileSource`, `FileSourcePath`.
- Code Mode: Limits effective speed to 8 (capped in UI & logic); retains prior non-code speed in `LastNonCodeSpeed` for restoration.
- File Mode: Ignores textbox; loads file content just-in-time; truncates >50,000 chars (status reflects truncation).

## Hotkey Handling Nuance
Registration moved to `OnHandleCreated` to survive WinForms handle recreation (e.g., toggling `ShowInTaskbar` when minimizing to tray). Always unregister prior ID before re-registering. Failures surface both in label & tray balloon tip.

## Character Escaping Rules
`EscapeForSendKeys(char)` maps: `+ ^ % ~ ( )` -> wrapped with braces, `{` -> `{{}`, `}` -> `{}}`, `\n` -> `{ENTER}`, `\r` ignored (avoid double enters in CRLF), `\t` -> `{TAB}`. Extend both method and test when adding new special cases.

## UI Patterns
- Status feedback uses color (Green = ready/success, DarkOrange = in-progress/heuristic notices, Red = error/cancel).
- Tooltips updated dynamically for speed slider when code mode toggles.
- Track bar custom paint; speed labels map to buckets (Very Slow / Slow / Normal / Fast / Very Fast). Keep any new speed-related labels consistent.

## Memory / Handle Hygiene
- Icon handle from `IconFactory.Create` must be destroyed via `IconFactory.Destroy` in `Dispose`.
- Global hotkey unregistered in `Dispose` even if interim re-registrations occurred.

## Extension Guidelines (Follow These Patterns)
- Adding another global hotkey: define new ID constant, register in `OnHandleCreated`, branch in `WndProc` (avoid heavy logic directly inside `WndProc`).
- New typing heuristics: confine constants near existing (`MinFastDelayMs`, `ContextualPauseMs`) and document reasoning in a concise comment.
- New persisted setting: add property to `AppSettings`, adjust load/save, then update `UpdateUIFromSettings()` / related event handlers.
- Additional special character handling: modify `EscapeForSendKeys` and expand the XUnit `[Theory]` data set.

## What NOT To Change Without Reason
- Delay computation formula & min fast delay (tuned for reliability).
- Hotkey re-registration pattern in `OnHandleCreated`.
- Ignoring `\r` (prevents double-enter on CRLF input).

## Quick Checklist Before PR
- `dotnet build` and `dotnet test` pass.
- If modifying escaping: added/updated test cases.
- No leaked native icon handles (verify `Dispose` path still calls `IconFactory.Destroy`).

(Provide feedback if any section is unclear or if additional internal patterns emerge that should be documented.)