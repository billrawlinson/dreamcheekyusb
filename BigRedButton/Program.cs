using HidLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace BRB
{
    class Program
    {
        private const int VendorId = 0x1D34;
        private const int ProductId = 0x000D;

        private static HidDevice _device;

        static void Main()
        {
            _device = HidDevices.Enumerate(VendorId, ProductId).FirstOrDefault();

            if (_device != null)
            {
                _device.OpenDevice();

                _device.Inserted += DeviceAttachedHandler;
                _device.Removed += DeviceRemovedHandler;

                _device.MonitorDeviceEvents = true;

                _device.ReadReport(OnReport);

                Console.WriteLine("Button found, press any key to exit.");
                Console.ReadKey();

                _device.CloseDevice();
            }
            else
            {
                Console.WriteLine("Could not find Button.");
                Console.ReadKey();
            }
        }

        private static void OnReport(HidReport report)
        {
            if (!_device.IsConnected) { return; }


            if (report.Data.Length >= 4)
            {
                BigRedButtonMessage message = new BigRedButtonMessage(report.Data);
                if (message.RedPressed)
                {
                    Console.WriteLine("Button Pressed");
                    MediaButtonClick();
                } else if(message.WasOpened){
                    Console.WriteLine("Lid Opened");
                }
                else if (message.WasClosed)
                {
                    Console.WriteLine("Lid Closed");
                }
            }

            _device.ReadReport(OnReport);

        }

        private static void DeviceAttachedHandler()
        {
            Console.WriteLine("Device attached.");
            _device.ReadReport(OnReport);
        }

        private static void DeviceRemovedHandler()
        {
            Console.WriteLine("Device removed.");
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        static void MediaButtonClick()
        {
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

        }

    }
}
