﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NetLock_RMM_User_Process.Windows.Mouse
{
    internal class MouseControl
    {
        // P/Invoke für SetCursorPos
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        // P/Invoke für mouse_event
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        // Konstanten für die Mausereignisse
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        // P/Invoke für Monitorinformationen
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfo
        {
            public uint Size;
            public Rect Monitor;
            public Rect Work;
            public uint Flags;
        }

        public delegate bool MonitorEnumDelegate(nint hMonitor, nint hdcMonitor, ref Rect lprcMonitor, nint dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(nint hdc, nint lprcClip, MonitorEnumDelegate lpfnEnum, nint dwData);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfo lpmi);

        // Methode zum Abrufen aller Bildschirme
        public static Rect[] GetAllScreens()
        {
            var screens = new List<Rect>();

            EnumDisplayMonitors(nint.Zero, nint.Zero, (nint hMonitor, nint hdcMonitor, ref Rect lprcMonitor, nint dwData) =>
            {
                MonitorInfo mi = new MonitorInfo();
                mi.Size = (uint)Marshal.SizeOf(mi);
                if (GetMonitorInfo(hMonitor, ref mi))
                {
                    screens.Add(mi.Monitor);
                }
                return true;
            }, nint.Zero);

            return screens.ToArray();
        }

        // Customized method for moving the mouse on the correct screen
        public static async Task MoveMouse(int x, int y, int screenIndex)
        {
            try
            {
                var screens = GetAllScreens();

                if (screenIndex >= screens.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(screenIndex), "Ungültiger Bildschirmindex.");
                }

                var screen = screens[screenIndex];

                // Calculate the absolute coordinates for the specified screen
                int absoluteX = screen.Left + x;
                int absoluteY = screen.Top + y;

                // Place the mouse pointer on the calculated absolute coordinates
                SetCursorPos(absoluteX, absoluteY);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to move mouse: {ex.Message}");
            }
        }

        public static async Task LeftClickMouse()
        {
            try
            {
                // Simulate a mouse click
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0); // Press left mouse button
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);   // Release left mouse button
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to left click mouse: {ex.Message}");
            }
        }

        // Right click
        public static async Task RightClickMouse()
        {
            try
            {
                // Simulate a right mouse click
                mouse_event(0x0008, 0, 0, 0, 0); // Press the right mouse button
                mouse_event(0x0010, 0, 0, 0, 0); // Release the right mouse button
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to right click mouse: {ex.Message}");
            }
        }

        public static async Task LeftMouseDown()
        {
            try
            {
                // Press (hold) the left mouse button
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to left mouse down: {ex.Message}");
            }
        }

        public static async Task LeftMouseUp()
        {
            try
            {
                // Release left mouse button
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to left mouse up: {ex.Message}");
            }
        }

    }

}
