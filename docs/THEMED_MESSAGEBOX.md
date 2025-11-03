# ThemedMessageBox - Theme-Aware Dialog System

## Overview

The `ThemedMessageBox` class provides a theme-aware alternative to the built-in `MessageBox` that respects the application's dark mode settings. This ensures a consistent user experience across all dialogs in the application.

## Why ThemedMessageBox?

The built-in Windows `MessageBox` does not respect application-level theming and always displays in the system's default appearance. This creates a jarring experience when the application is in dark mode. `ThemedMessageBox` solves this by:

- Using `AppColors` for consistent theming with the rest of the application
- Adapting automatically to light/dark mode based on `AppColors.IsDarkMode`
- Providing the same API as `MessageBox.Show()` for easy migration
- Following the same design patterns as other custom dialogs in the app

## Usage

### Basic Examples

```csharp
// Simple notification
ThemedMessageBox.Show("Settings saved successfully.", "Success");

// With icon
ThemedMessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

// Yes/No confirmation
var result = ThemedMessageBox.Show(
    "Delete this snippet?", 
    "Confirm Delete", 
    MessageBoxButtons.YesNo, 
    MessageBoxIcon.Question);
if (result == DialogResult.Yes)
{
    // Delete the snippet
}

// With owner window
ThemedMessageBox.Show(this, "Update complete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
```

### Supported Features

#### MessageBoxButtons
- `OK` - Single OK button
- `OKCancel` - OK and Cancel buttons
- `YesNo` - Yes and No buttons
- `YesNoCancel` - Yes, No, and Cancel buttons
- `RetryCancel` - Retry and Cancel buttons
- `AbortRetryIgnore` - Abort, Retry, and Ignore buttons

#### MessageBoxIcon
- `None` - No icon
- `Error` - Error/stop icon (red X)
- `Question` - Question icon (blue ?)
- `Warning` - Warning icon (yellow !)
- `Information` - Information icon (blue i)

#### Return Values
Returns `DialogResult` with values like:
- `DialogResult.OK`
- `DialogResult.Cancel`
- `DialogResult.Yes`
- `DialogResult.No`
- `DialogResult.Retry`
- `DialogResult.Abort`
- `DialogResult.Ignore`

## Implementation Details

### Color Scheme

**Light Mode:**
- Background: SystemColors.Control
- Text: SystemColors.ControlText
- Buttons: Standard Windows button appearance

**Dark Mode:**
- Background: Dark gray (#202020)
- Text: Light gray (#F0F0F0)
- Primary button: Accent color (#0078D7) with white text
- Secondary buttons: Dark background (#2D2D30) with light text
- Borders: Medium gray (#555555)
- Flat button style for better dark mode appearance

### Design Patterns

The `ThemedMessageBox` follows the same patterns as other custom dialogs in the application:

1. **Constructor Pattern**: Private constructor that calls `InitializeDialog()`
2. **Static Show Methods**: Public static methods that create, show, and dispose the dialog
3. **AppColors Usage**: Consistent use of `AppColors` properties for all colors
4. **Flat Style in Dark Mode**: Uses `FlatStyle.Flat` for buttons in dark mode
5. **Accent on Primary Button**: The primary (rightmost) button gets accent color in dark mode

### Code Organization

Located in: `HotkeyTyper/ThemedMessageBox.cs`

Key components:
- Form setup and theming
- Icon selection and rendering
- Message label with auto-sizing
- Button creation and positioning
- Dialog result handling

## Migration from MessageBox

To convert existing code:

```csharp
// Before
MessageBox.Show("Error occurred!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

// After
ThemedMessageBox.Show("Error occurred!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
```

The API is intentionally identical to make migration straightforward.

## Developer Guidelines

### When Adding New Dialogs

**DO:**
- Always use `ThemedMessageBox.Show()` for all message boxes
- Use appropriate `MessageBoxIcon` for the message type
- Provide clear, concise message text
- Use proper button combinations for the action

**DON'T:**
- Never use `MessageBox.Show()` directly
- Don't create custom message boxes for simple notifications
- Avoid very long messages (consider using a custom dialog with scrolling for extensive content)

### Testing Considerations

Test message boxes in both light and dark modes:
1. Set theme to Light in app settings
2. Restart app and verify message box appearance
3. Set theme to Dark in app settings
4. Restart app and verify message box appearance

The message box should be clearly readable and visually consistent with the rest of the application in both modes.

## Related Files

- `ThemedMessageBox.cs` - The message box implementation
- `AppColors.cs` - Color definitions used for theming
- `AboutDialog.cs` - Example of another themed dialog
- `InputDialog.cs` - Example of another themed dialog
- `UpdateDialog.cs` - Example of another themed dialog

## Examples in the Codebase

The application uses `ThemedMessageBox` for various notifications:

1. **Error Messages**: Settings load/save errors, file read errors, typing errors
2. **Warnings**: Invalid snippet names, duplicate names, validation errors
3. **Information**: Update notifications, theme change notifications
4. **Confirmations**: Snippet deletion, yes/no decisions

Browse `Form1.cs` to see real-world usage examples throughout the application.
