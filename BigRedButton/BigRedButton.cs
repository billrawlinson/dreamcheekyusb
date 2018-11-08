using HidLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;



namespace BRB {
	public class BigRedButton : IDisposable  {
        #region Constant and readonly values

        private const int DEFAULT_VENDOR_ID = 0x1D34;
		private const int DEFAULT_PRODUCT_ID = 0x000D;

		//Default for USB Big Red Button
		public const string PID = "000d";
        public static readonly byte[] CmdStatus = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };

        #endregion

        private HidDevice _device;
        private Action Button_Callback;
        private bool _attached;

        public BigRedButton() : this(0)
        {
		}

		public BigRedButton(int deviceIndex = 0) : this(DEFAULT_VENDOR_ID, DEFAULT_PRODUCT_ID, deviceIndex) {
		}

		public BigRedButton(int vendorID, int productID, int deviceIndex = 0){
            _device = HidDevices.Enumerate(DEFAULT_VENDOR_ID).FirstOrDefault();

            if (_device == null)
            {
                throw new Exception(String.Format("Cannot find USB HID Device with VendorID=0x{0:X4} and ProductID=0x{1:X4}", vendorID, productID));
            }
            else
            {
            }

		}

		public BigRedButton(string devicePath) {
            _device = HidDevices.Enumerate(DEFAULT_VENDOR_ID, DEFAULT_PRODUCT_ID).FirstOrDefault(a => a.DevicePath == devicePath);
            if (_device == null)
            {
                throw new Exception(String.Format("Cannot find USB HID Device with path={0}", devicePath));
            }
		}

        public void Run()
        {
            ButtonState = false;
            _device.OpenDevice();

            _device.Inserted += DeviceAttachedHandler;
            _device.Removed += DeviceRemovedHandler;
            _device.MonitorDeviceEvents = true;

            _device.ReadReport(OnReport);

            //Device is valid
            Trace.WriteLine("Init HID device: " + _device.Description + "\r\n");
        }
        
        private void DeviceAttachedHandler()
        {
            _attached = true;
            Console.WriteLine("Big Red Button attached.");
            _device.ReadReport(OnReport);
        }

        private void DeviceRemovedHandler()
        {
            _attached = false;
            Console.WriteLine("Big Red Button removed.");
        }

        private void OnReport(HidReport report)
        {
            if (_attached == false) { return; }


            if (report.Data.Length >= 4)
            {
                BigRedButtonMessage message = new BigRedButtonMessage(report.Data);
                if (message.RedPressed && !ButtonState && Button_Callback != null) {
                    ButtonState = true;
                    Button_Callback();
                } else
                {
                    ButtonState = false;
                }
            }

            _device.ReadReport(OnReport);

        }
        public bool ButtonState { get; private set; }


        public void RegisterCallback(Action callback)
        {
            Button_Callback = callback;
        }

        public void Dispose()
        {
            if(_device != null && _device.IsOpen)
            {
                _device.CloseDevice();
                _device = null;
            }
        }
    }
}