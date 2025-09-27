using System.Runtime.InteropServices;

namespace HotkeyTyper;

/// <summary>
/// Sends keystrokes using the low-level Win32 SendInput API for higher reliability than SendKeys.
/// Uses Unicode input to avoid layout / shift-state issues and reduce risk of dropped characters at high speed.
/// </summary>
internal class KeyboardInputSender
{
    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type; // 1 = keyboard
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public nint dwExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private const uint INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_UNICODE = 0x0004;
    private const ushort VK_RETURN = 0x0D;
    private const ushort VK_TAB = 0x09;

    /// <summary>
    /// Sends a single character (or control key) through SendInput.
    /// </summary>
    public bool SendChar(char c)
    {
        switch (c)
        {
            case '\r':
                // Ignore bare CR (handled with LF for new line)
                return true;
            case '\n':
                return SendVirtualKey(VK_RETURN);
            case '\t':
                return SendVirtualKey(VK_TAB);
            default:
                return SendUnicodeChar(c);
        }
    }

    private static bool SendVirtualKey(ushort vk)
    {
        var inputs = new INPUT[2];
        inputs[0] = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT { wVk = vk, wScan = 0, dwFlags = 0, time = 0, dwExtraInfo = 0 }
            }
        };
        inputs[1] = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT { wVk = vk, wScan = 0, dwFlags = KEYEVENTF_KEYUP, time = 0, dwExtraInfo = 0 }
            }
        };
        var sent = SendInput(2, inputs, Marshal.SizeOf<INPUT>());
        return sent == 2;
    }

    private static bool SendUnicodeChar(char c)
    {
        var inputs = new INPUT[2];
        inputs[0] = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT { wVk = 0, wScan = c, dwFlags = KEYEVENTF_UNICODE, time = 0, dwExtraInfo = 0 }
            }
        };
        inputs[1] = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT { wVk = 0, wScan = c, dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP, time = 0, dwExtraInfo = 0 }
            }
        };
        var sent = SendInput(2, inputs, Marshal.SizeOf<INPUT>());
        return sent == 2;
    }
}
