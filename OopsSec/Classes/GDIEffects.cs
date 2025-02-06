using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OopsSec.Classes
{
    public class GDIEffects : IDisposable
    {
        private readonly Random _random = new Random();
        private readonly IntPtr _hdc;
        private readonly IntPtr _memDc;
        private readonly IntPtr _hBitmap;
        private readonly IntPtr _oldBitmap;

        private readonly Rectangle _allScreensBounds;

        public GDIEffects()
        {
            // Get the combined bounds of all screens
            _allScreensBounds = GetAllScreensBounds();

            // Get the device context for the entire screen
            _hdc = Win32Imports.GetDC(IntPtr.Zero);

            // Create a compatible memory device context
            _memDc = Win32Imports.CreateCompatibleDC(_hdc);

            // Create a compatible bitmap for the screen
            _hBitmap = Win32Imports.CreateCompatibleBitmap(_hdc, _allScreensBounds.Width, _allScreensBounds.Height);

            // Select the bitmap into the memory device context
            _oldBitmap = Win32Imports.SelectObject(_memDc, _hBitmap);
        }

        public void InvertScreenColors()
        {
            Win32Imports.BitBlt(
                _hdc, _allScreensBounds.Left, _allScreensBounds.Top, // Destination
                _allScreensBounds.Width, _allScreensBounds.Height, // Screens bounds
                _hdc, _allScreensBounds.Left, _allScreensBounds.Top, // Source
                TernaryRasterOperations.NOTSRCCOPY // Inverts the colors and puts it on screen
            );
        }

        public void ShiftRandomStripDown(int width = 160, int shift = 4)
        {
            // Randomly select a vertical strip of the screen
            int randX = _random.Next(_allScreensBounds.Width) + _allScreensBounds.Left;

            // Adjust the shift based on the screen bounds
            int shiftY = shift + _allScreensBounds.Top;

            // Apply the effect using BitBlt
            Win32Imports.BitBlt(
                _hdc, randX, shiftY, // Destination
                width, _allScreensBounds.Height, // Strip size
                _hdc, randX, _allScreensBounds.Top, // Source
                TernaryRasterOperations.SRCCOPY
            );
        }

        public void FlipScreenHorizontal()
        {
            Win32Imports.StretchBlt(
                _hdc,
                _allScreensBounds.Right, // Destination X: Start from the right edge
                _allScreensBounds.Top,   // Destination Y: Start from the top
                -_allScreensBounds.Width, // Width: Negative to flip horizontally
                _allScreensBounds.Height, // Height: Same as original
                _hdc,
                _allScreensBounds.Left,  // Source X: Start from the left edge
                _allScreensBounds.Top,   // Source Y: Start from the top
                _allScreensBounds.Width, // Source Width: Same as original
                _allScreensBounds.Height, // Source Height: Same as original
                TernaryRasterOperations.SRCCOPY
            );
        }

        public void FlipScreenVertical()
        {
            Win32Imports.StretchBlt(
                _hdc,
                _allScreensBounds.Left,  // Destination X: Start from the left edge
                _allScreensBounds.Bottom, // Destination Y: Start from the bottom
                _allScreensBounds.Width, // Width: Same as original
                -_allScreensBounds.Height, // Height: Negative to flip vertically
                _hdc,
                _allScreensBounds.Left,  // Source X: Start from the left edge
                _allScreensBounds.Top,   // Source Y: Start from the top
                _allScreensBounds.Width, // Source Width: Same as original
                _allScreensBounds.Height, // Source Height: Same as original
                TernaryRasterOperations.SRCCOPY
            );
        }

        private Rectangle GetAllScreensBounds()
        {
            Screen[] allScreens = Screen.AllScreens;
            int minX = allScreens.Min(s => s.Bounds.Left);
            int minY = allScreens.Min(s => s.Bounds.Top);
            int maxX = allScreens.Max(s => s.Bounds.Right);
            int maxY = allScreens.Max(s => s.Bounds.Bottom);

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public void Dispose()
        {
            // Clean up resources
            Win32Imports.SelectObject(_memDc, _oldBitmap); // Restore the old bitmap
            Win32Imports.DeleteObject(_hBitmap); // Delete the bitmap
            Win32Imports.DeleteDC(_memDc); // Delete the memory device context
            Win32Imports.ReleaseDC(IntPtr.Zero, _hdc); // Release the screen device context
        }
    }
}
