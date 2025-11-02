using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace HotkeyTyper;

/// <summary>
/// Generates an in-memory application Icon so we don't need to ship a physical .ico file.
/// Provides a simple stylized "HT" badge with a gradient background.
/// Supports multi-resolution and theme-aware variants.
/// </summary>
internal static class IconFactory
{
    /// <summary>
    /// Standard icon sizes for Windows icons (tray, taskbar, Alt+Tab, Explorer).
    /// </summary>
    private static readonly int[] StandardSizes = new[] { 16, 20, 24, 32, 48, 64 };

    /// <summary>
    /// Detect if Windows is using dark theme by querying the registry.
    /// Returns true for light theme, false for dark theme.
    /// Defaults to light theme on error or if key is not found.
    /// </summary>
    private static bool IsLightTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null)
            {
                var value = key.GetValue("SystemUsesLightTheme");
                if (value is int themeValue)
                {
                    return themeValue != 0;
                }
            }
        }
        catch
        {
            // Ignore errors (registry access denied, key not found, etc.)
        }
        return true; // Default to light theme
    }

    /// <summary>
    /// Create a multi-resolution application icon with theme-aware colors.
    /// Returns an Icon containing multiple sizes (16, 20, 24, 32, 48, 64) and the handle of the primary 32x32 representation.
    /// </summary>
    public static Icon Create(out IntPtr nativeHandle)
    {
        bool isLight = IsLightTheme();
        return CreateMultiSize(isLight, out nativeHandle);
    }

    /// <summary>
    /// Create a multi-resolution icon with specified theme.
    /// </summary>
    /// <param name="isLightTheme">True for light theme colors, false for dark theme colors.</param>
    /// <param name="nativeHandle">Output handle for the primary 32x32 icon representation.</param>
    private static Icon CreateMultiSize(bool isLightTheme, out IntPtr nativeHandle)
    {
        var bitmaps = new List<Bitmap>();
        try
        {
            // Generate bitmap for each size
            foreach (int size in StandardSizes)
            {
                bitmaps.Add(CreateSingleSizeBitmap(size, isLightTheme));
            }

            // Build multi-size icon
            Icon multiIcon = CreateIconFromBitmaps(bitmaps);
            
            // Create handle from 32x32 variant for Form.Icon assignment
            var bmp32 = bitmaps.FirstOrDefault(b => b.Width == 32) ?? bitmaps[0];
            nativeHandle = bmp32.GetHicon();
            
            return multiIcon;
        }
        finally
        {
            // Clean up bitmaps
            foreach (var bmp in bitmaps)
            {
                bmp.Dispose();
            }
        }
    }

    /// <summary>
    /// Create a single bitmap of specified size with theme-appropriate colors.
    /// </summary>
    private static Bitmap CreateSingleSizeBitmap(int size, bool isLightTheme)
    {
        var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Choose colors based on theme
            Color gradientStart, gradientEnd, borderColor, textColor;
            if (isLightTheme)
            {
                // Light theme: darker colors on light background
                gradientStart = Color.MediumPurple;
                gradientEnd = Color.DeepSkyBlue;
                borderColor = Color.White;
                textColor = Color.White;
            }
            else
            {
                // Dark theme: lighter colors that stand out on dark background
                gradientStart = Color.FromArgb(200, 162, 235); // Lighter purple
                gradientEnd = Color.FromArgb(135, 206, 250); // Lighter sky blue
                borderColor = Color.FromArgb(240, 240, 240); // Light gray
                textColor = Color.FromArgb(240, 240, 240);
            }

            using var path = new GraphicsPath();
            path.AddEllipse(new Rectangle(0, 0, size - 1, size - 1));
            using (var brush = new LinearGradientBrush(new Rectangle(0, 0, size, size), gradientStart, gradientEnd, 45f))
            {
                g.FillPath(brush, path);
            }
            
            // Scale border width proportionally
            float borderWidth = Math.Max(1f, size / 16f);
            using var borderPen = new Pen(borderColor, borderWidth);
            g.DrawPath(borderPen, path);

            // Draw centered text HT with size-appropriate font
            var text = "HT";
            float fontSize = size * 0.375f; // Scale font to size
            using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var sz = g.MeasureString(text, font);
            float x = (size - sz.Width) / 2f;
            float y = (size - sz.Height) / 2f - (size / 32f); // small vertical tweak
            using var textBrush = new SolidBrush(textColor);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(text, font, textBrush, x, y);
        }
        
        return bmp;
    }

    /// <summary>
    /// Build a multi-resolution .ico from multiple bitmaps.
    /// Based on the ICO file format specification.
    /// </summary>
    private static Icon CreateIconFromBitmaps(List<Bitmap> bitmaps)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        // ICO header
        writer.Write((short)0); // Reserved, must be 0
        writer.Write((short)1); // Type: 1 for .ico
        writer.Write((short)bitmaps.Count); // Number of images
        
        // Calculate offset where image data starts (after directory entries)
        int imageDataOffset = 6 + (bitmaps.Count * 16);
        
        // Write directory entries and collect image data
        var imageDataList = new List<byte[]>();
        foreach (var bmp in bitmaps)
        {
            byte[] imageData = ConvertBitmapToIconImage(bmp);
            imageDataList.Add(imageData);
            
            // Directory entry
            writer.Write((byte)(bmp.Width >= 256 ? 0 : bmp.Width)); // Width (0 means 256)
            writer.Write((byte)(bmp.Height >= 256 ? 0 : bmp.Height)); // Height (0 means 256)
            writer.Write((byte)0); // Color palette (0 for true color)
            writer.Write((byte)0); // Reserved
            writer.Write((short)1); // Color planes
            writer.Write((short)32); // Bits per pixel
            writer.Write((int)imageData.Length); // Size of image data
            writer.Write((int)imageDataOffset); // Offset to image data
            
            imageDataOffset += imageData.Length;
        }
        
        // Write all image data
        foreach (var imageData in imageDataList)
        {
            writer.Write(imageData);
        }
        
        ms.Position = 0;
        return new Icon(ms);
    }

    /// <summary>
    /// Convert a bitmap to PNG format for embedding in ICO file.
    /// Modern ICO files use PNG for better quality.
    /// </summary>
    private static byte[] ConvertBitmapToIconImage(Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
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
