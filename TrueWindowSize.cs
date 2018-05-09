using System;
using System.Drawing;
using System.Runtime.InteropServices;

public static class TrueWindowSize
{
    public static Rectangle GetWindowRectangle(IntPtr handle)
    {
        if (Environment.OSVersion.Version.Major < 6)
        {
            return GetWindowRect(handle);
        }
        else
        {
            return DWMWA_EXTENDED_FRAME_BOUNDS(handle, out Rectangle rectangle) ? rectangle : GetWindowRect(handle);
        }
    }

    private static Rectangle GetWindowRect(IntPtr handle)
    {
        GetWindowRect(handle, out Rect rect);
        return rect.ToRectangle();
    }

    [DllImport(@"dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out Rect pvAttribute, int cbAttribute);

    private enum Dwmwindowattribute
    {
        DwmwaExtendedFrameBounds = 9
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(Left, Top, Right, Bottom);
        }
    }

    private static bool DWMWA_EXTENDED_FRAME_BOUNDS(IntPtr handle, out Rectangle rectangle)
    {
        var result = DwmGetWindowAttribute(handle, (int)Dwmwindowattribute.DwmwaExtendedFrameBounds,
            out Rect rect, Marshal.SizeOf(typeof(Rect)));
        rectangle = rect.ToRectangle();
        return result >= 0;
    }

    [DllImport(@"user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
}