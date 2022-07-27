/* Author: Perry Lee
* Submission: Capture Screen (Add Screenshot Capability to Programs)
* Date of Submission: 12/29/03
*/

// If you have any questions regarding functions (methods) imported from
// GDI32.dll and User32.dll refer to 'msdn.microsoft.com'

using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace AutomateDesktop
{
    internal class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(int hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, int hdcSrc, int nXSrc, int nYSrc, int dwRop);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight); [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(int hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);
        [DllImport("GDI32.dll")]
        public static extern int SelectObject(int hdc, int hgdiobj);
        class User32
        {
            [DllImport("User32.dll")]
            public static extern int GetDesktopWindow();
            [DllImport("User32.dll")]
            public static extern int GetWindowDC(int hWnd);
            [DllImport("User32.dll")]
            public static extern int ReleaseDC(int hWnd, int hDC);
        }
        internal class GetImageFromScreen
        {
            public void CaptureScreen(string fileName, ImageFormat imageFormat)
            {
                int hdcSrc = User32.GetWindowDC(User32.GetDesktopWindow()); // Get a handle to the desktop window
                int hdcDest = GDI32.CreateCompatibleDC(hdcSrc); // Create a memory device context
                int hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, // Create a bitmap and place it in the memory DC
                GDI32.GetDeviceCaps(hdcSrc, 8), GDI32.GetDeviceCaps(hdcSrc, 10));
                // GDI32.GetDeviceCaps(hdcSrc,8) returns the width of the desktop window
                // GDI32.GetDeviceCaps(hdcSrc,10) returns the height of the desktop window
                GDI32.SelectObject(hdcDest, hBitmap); // Required to create a color bitmap

                GDI32.BitBlt(hdcDest, 0, 0, GDI32.GetDeviceCaps(hdcSrc, 8), // Copy the on-screen image into the memory DC
                    GDI32.GetDeviceCaps(hdcSrc, 10), hdcSrc, 0, 0, 0x00CC0020);

                //SaveImageAs(hBitmap, fileName, imageFormat); // Save the screen-capture to the specified file using the designated image format
                Automater.SeeScreen(hBitmap);

                Cleanup(hBitmap, hdcSrc, hdcDest); // Free system resources
            }

            private void Cleanup(int hBitmap, int hdcSrc, int hdcDest)
            {
                // Release the device context resources back to the system
                User32.ReleaseDC(User32.GetDesktopWindow(), hdcSrc);
                GDI32.DeleteDC(hdcDest);
                GDI32.DeleteObject(hBitmap);
            }

            private void SaveImageAs(int hBitmap, string fileName, ImageFormat imageFormat)
            {
                // Create a bitmap from the Windows handle
                Bitmap image = new Bitmap(System.Drawing.Image.FromHbitmap(new IntPtr(hBitmap)),
                System.Drawing.Image.FromHbitmap(new IntPtr(hBitmap)).Width,
                System.Drawing.Image.FromHbitmap(new IntPtr(hBitmap)).Height);
                //image.Save(fileName, imageFormat);
            }
        }
    }
}
