# Introducing HotkeyTyper: The Demo Creator's Best Friend

## The Problem That Started It All

As a developer who creates a lot of video content and demos, I found myself constantly frustrated with one recurring issue: having to repeatedly type out long prompts, code snippets, and explanations during screen recordings. You know the drill - you're in the middle of a great explanation, the recording is going perfectly, and then you have to break the flow to manually type out that lengthy API call or complex configuration. Even worse, you might make typos and have to start over!

I needed a simple solution that would let me pre-configure text snippets and instantly type them out with a hotkey during live demos and recordings. After searching for existing tools and not finding exactly what I needed, I decided to build my own: **HotkeyTyper**.

## What is HotkeyTyper?

HotkeyTyper is a lightweight .NET Windows Forms application that solves this exact problem. It's a simple utility that allows you to:

- Pre-configure any text you want to type repeatedly
- Trigger typing with a global hotkey (`CTRL+SHIFT+1`) from any application
- Type text at human-like speed with realistic timing variations
- Run silently in the system tray for unobtrusive background operation

Perfect for content creators, developers giving demos, educators, or anyone who needs to repeatedly type the same text snippets across different applications.

## Key Features

### 🔥 Global Hotkey Support
Press `CTRL+SHIFT+1` from any Windows application and your predefined text will be typed automatically. No need to switch windows or interrupt your workflow.

### 🤖 Human-like Typing
The app doesn't just paste text instantly - it types each character individually with realistic delays (50-150ms between characters) and random variations to simulate natural human typing speed. This makes it perfect for live demos where you want the typing to look authentic.

### ⚡ Adjustable Typing Speed
Choose from 10 different speed settings, from "Very Slow" (great for following along with tutorials) to "Very Fast" (for when you need to get through text quickly).

### 💾 Persistent Settings
Your predefined text and preferences are automatically saved to a `settings.json` file and restored when you restart the application.

### 🔕 System Tray Integration  
Minimize to the system tray and keep the app running in the background. The hotkey stays active even when the main window is hidden.

### 🎯 Cross-Application Compatibility
Works in any Windows application that accepts text input - from code editors and terminals to web browsers and document editors.

## Building HotkeyTyper with GitHub Copilot and VS Code

One of the most interesting aspects of this project was how I leveraged GitHub Copilot and Visual Studio Code to accelerate the development process. Here's how the development unfolded:

### Setting Up the Project
I started by creating a new .NET Windows Forms project:

```bash
dotnet new winforms -n HotkeyTyper
cd HotkeyTyper
```

Then opened it in VS Code with the C# extension installed. GitHub Copilot immediately started suggesting relevant code completions.

### Building with Copilot Assistance

**Windows API Integration**: When I started typing Windows API declarations for hotkey registration, Copilot suggested the complete `RegisterHotKey` and `UnregisterHotKey` imports along with the necessary constants:

```csharp
// Copilot suggested these Windows API imports automatically
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

[DllImport("user32.dll")]  
private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
```

**System Tray Implementation**: When I added a comment about needing system tray functionality, Copilot generated most of the `InitializeSystemTray()` method, including the context menu setup.

**Settings Persistence**: For JSON serialization, Copilot suggested the complete `AppSettings` class structure and the save/load methods when I started typing the settings-related code.

### Development Commands

Building and running the application was straightforward with .NET CLI:

```bash
# Build the project
dotnet build

# Run the application  
dotnet run

# Build for release
dotnet build --configuration Release
```

The entire development process, from initial concept to working application, took just a few hours thanks to Copilot's intelligent suggestions and VS Code's excellent C# support.

## Technical Implementation Deep Dive

### Global Hotkey Registration
The app uses Windows API calls to register a system-wide hotkey:

```csharp
RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.D1);
```

This registers `CTRL+SHIFT+1` as a global hotkey that works across all applications. The app listens for `WM_HOTKEY` messages in the `WndProc` method to detect when the hotkey is pressed.

### Human-like Typing Simulation
Instead of simply pasting text, the app sends individual keystrokes using `SendKeys.SendWait()`:

```csharp
foreach (char c in predefinedText)
{
    SendKeys.SendWait(c.ToString());
    await Task.Delay(Math.Max(20, random.Next(baseDelay - variation, baseDelay + variation)));
}
```

The delay calculation is based on the user's speed preference:
- Speed 1 (Very Slow): 200-300ms between characters
- Speed 5 (Normal): 50-150ms between characters  
- Speed 10 (Very Fast): 10-50ms between characters

Random variation is added to make the typing pattern feel more natural.

### Settings Persistence
Configuration is stored in JSON format using `System.Text.Json`:

```csharp
public class AppSettings
{
    public string PredefinedText { get; set; } = string.Empty;
    public int TypingSpeed { get; set; } = 5;
}
```

Settings are automatically saved whenever the user updates text or changes typing speed, ensuring preferences persist across sessions.

### System Tray Integration
The app creates a `NotifyIcon` with a context menu for background operation:

```csharp
trayIcon = new NotifyIcon()
{
    Text = "Hotkey Typer - CTRL+SHIFT+1 Active",
    Visible = true,
    Icon = SystemIcons.Application
};
```

Users can minimize to tray and the hotkey remains active, making it perfect for background use during demos.

## Perfect for Content Creators

HotkeyTyper has already proven invaluable for my video content creation workflow. Some use cases where it shines:

- **API Demonstrations**: Pre-configure complex API calls and endpoints
- **Code Examples**: Set up lengthy code snippets for tutorials  
- **Command Line Operations**: Store frequently used CLI commands
- **Configuration Files**: Keep complex configuration examples ready
- **Explanatory Text**: Have detailed explanations ready for complex topics

The human-like typing speed means viewers can follow along naturally, and the global hotkey ensures smooth, uninterrupted demos.

## Get Started Today

HotkeyTyper is open source and available on GitHub. You can:

1. **Download the source**: Clone or download from the repository
2. **Build locally**: Run `dotnet build` to compile the application  
3. **Start using**: Run `dotnet run` or build a release executable
4. **Customize**: Modify the code to fit your specific needs

The application requires Windows 10 or later and .NET 9.0+.

Whether you're a content creator, educator, developer doing demos, or anyone who repeatedly types the same text, HotkeyTyper can streamline your workflow and eliminate those frustrating typing interruptions.

Try it out and let me know how it helps with your demos and content creation!

---

*HotkeyTyper is open source software. Feel free to contribute improvements, report issues, or fork it for your own needs.*