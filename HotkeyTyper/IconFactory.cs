using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace HotkeyTyper;

/// <summary>
/// Generates an in-memory application Icon so we don't need to ship a physical .ico file.
/// Provides a simple stylized "HT" badge with a gradient background.
/// </summary>
internal static class IconFactory
{
    /// <summary>
    /// Create the primary application icon (32x32) and return both the Icon and underlying handle
    /// so the caller can destroy it during disposal to avoid leaking a GDI handle.
    /// </summary>
    public static Icon Create(out IntPtr nativeHandle)
    {
        using var bmp = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = new GraphicsPath();
            path.AddEllipse(new Rectangle(0, 0, 31, 31));
            using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 32, 32), Color.MediumPurple, Color.DeepSkyBlue, 45f))
            {
                g.FillPath(brush, path);
            }
            using var borderPen = new Pen(Color.White, 2f);
            g.DrawPath(borderPen, path);

            // Draw centered text HT
            var text = "HT";
            using var font = new Font("Segoe UI", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            var sz = g.MeasureString(text, font);
            float x = (32 - sz.Width) / 2f;
            float y = (32 - sz.Height) / 2f - 1f; // small vertical tweak
            using var textBrush = new SolidBrush(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(text, font, textBrush, x, y);
        }

        nativeHandle = bmp.GetHicon();
        // Important: Icon.FromHandle does not own the handle; caller must DestroyIcon later.
        return Icon.FromHandle(nativeHandle);
    }

    /// <summary>
    /// Save the provided icon (previously returned by Create) to the specified path if it does not already exist.
    /// Returns true if saved, false if skipped or on failure.
    /// </summary>
    public static bool TryExportIcon(Icon icon, string path)
    {
        try
        {
            if (File.Exists(path)) return false; // do not overwrite user customization
            using var fs = File.Create(path);
            icon.Save(fs); // writes ICO format
            return true;
        }
        catch
        {
            return false;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    /// <summary>
    /// Helper to safely destroy a previously created icon handle obtained from Create().
    /// </summary>
    public static void Destroy(ref Icon? icon, ref IntPtr handle)
    {
        try
        {
            if (handle != IntPtr.Zero)
            {
                DestroyIcon(handle);
                handle = IntPtr.Zero;
            }
        }
        catch
        {
            // Ignore cleanup errors.
        }
        finally
        {
            icon = null; // underlying handle already destroyed
        }
    }
}
