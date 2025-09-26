# Hotkey Typer

A simple .NET Windows Forms application that allows you to type predefined text using a global hotkey. Perfect for quickly inserting frequently used text snippets, signatures, or any other text you need to type repeatedly.

## Features

- **Global Hotkey**: Press `CTRL+SHIFT+1` from any application to type your predefined text
- **Human-like Typing**: Text is typed at a realistic human speed (50-150ms between characters) with random variation
- **Persistent Settings**: Your predefined text is automatically saved and restored when you restart the application
- **Simple UI**: Easy-to-use interface for configuring your text
- **System Tray Integration**: Minimize to system tray to keep the app running in the background
- **Cross-Application**: Works in any Windows application that accepts text input

## Requirements

- Windows 10 or later
- .NET 9.0 or later

## Installation & Usage

### Building from Source

1. Clone or download this repository
2. Open a terminal/command prompt and navigate to the project directory
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

### Using the Application

1. **Configure Your Text**: 
   - Enter your desired text in the large text area
   - Click "Update Text" to save your changes

2. **Use the Hotkey**:
   - Press `CTRL+SHIFT+1` in any application
   - Your predefined text will be typed automatically at human speed

3. **Background Operation**:
   - Click "Minimize to Tray" to keep the app running in the background
   - Right-click the system tray icon to show the app or exit

## How It Works

The application uses Windows API calls to register a global system hotkey that works across all applications. When the hotkey is pressed:

1. The app detects the hotkey press through Windows message handling
2. It sends individual keystrokes to the currently active window
3. Characters are typed with random delays (50-150ms) to simulate natural human typing speed

## Settings Storage

Your predefined text is automatically saved to a `settings.json` file in the application directory. This ensures your configuration persists between application restarts.

## Technical Details

- **Framework**: .NET 10.0 with Windows Forms
- **Global Hotkey**: Uses Windows API `RegisterHotKey`/`UnregisterHotKey`
- **Text Input**: Uses `SendKeys.SendWait()` with timing delays
- **Settings**: JSON serialization for configuration persistence
- **System Tray**: Built-in NotifyIcon component

## Security Considerations

This application uses Windows API calls to register global hotkeys and send keystrokes. Some antivirus software may flag applications that use these APIs. This is normal behavior for legitimate automation tools.

## Troubleshooting

### Hotkey Not Working
- Make sure no other application is using the `CTRL+SHIFT+1` combination
- Try restarting the application
- Run as administrator if needed

### Text Not Typing in Certain Applications
- Some applications (like those running as administrator) may require the typer to also run as administrator
- Certain secure applications may block automated input

### Settings Not Saving
- Check that the application has write permissions to its directory
- The settings file (`settings.json`) should be created automatically

## Contributing

Feel free to submit issues, fork the repository, and create pull requests for any improvements.

## License

This project is open source. Feel free to use and modify as needed.