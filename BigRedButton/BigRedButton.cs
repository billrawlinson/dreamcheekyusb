using HidLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;



namespace DreamCheekyUSB {
	public class BigRedButton  {
        #region Constant and readonly values
        public HidDevice _device;
        private bool _attached;

        new public const int DEFAULT_VENDOR_ID = 0x1D34;
		new public const int DEFAULT_PRODUCT_ID = 0x000D;

		//Default for USB Big Red Button
		new public static class Messages {
			public const byte LID_CLOSED = 0x15;
			public const byte BUTTON_PRESSED = 0x16;
			public const byte LID_OPEN = 0x17;
		}

		public const string PID = "000d";
        public static readonly byte[] CmdStatus = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };

        #endregion

        private AutoResetEvent WriteEvent = new AutoResetEvent(false);
        private System.Timers.Timer t = new System.Timers.Timer(5);
        //Timer for checking USB status every 100ms
        private Action Button_Callback;
        private bool LidOpen;
        protected byte ActivatedMessage;




        public BigRedButton() : this(0)
        {
			ActivatedMessage = Messages.BUTTON_PRESSED;
		}

		public BigRedButton(int deviceIndex = 0) : this(DEFAULT_VENDOR_ID, DEFAULT_PRODUCT_ID, deviceIndex) {
			ActivatedMessage = Messages.BUTTON_PRESSED;
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

            ActivatedMessage = Messages.BUTTON_PRESSED;
		}

		public BigRedButton(string devicePath) {
            _device = HidDevices.Enumerate(DEFAULT_VENDOR_ID, DEFAULT_PRODUCT_ID).FirstOrDefault(a => a.DevicePath == devicePath);
            if (_device == null)
            {
                throw new Exception(String.Format("Cannot find USB HID Device with path={0}", devicePath));
            }
            ActivatedMessage = Messages.BUTTON_PRESSED;
            ActivatedMessage = Messages.BUTTON_PRESSED;
		}

        /// <summary>
        /// Private init function for constructors.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public bool Run()
        {
            ButtonState = false;
            _device.OpenDevice();
            _device.Inserted += DeviceAttachedHandler;
            _device.Removed += DeviceRemovedHandler;


            _device.MonitorDeviceEvents = true;
            
            _device.ReadReport(OnReport);
            //Device is valid
            Trace.WriteLine("Init HID device: " + _device.Description + "\r\n");
            return true;
        }

        public void Stop()
        {
            _device.CloseDevice();
        }

        private void DeviceAttachedHandler()
        {
            _attached = true;
            Console.WriteLine("Big Red Button attached.");
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
            //t.Enabled = true;
            //t.Start();
        }


	}
}