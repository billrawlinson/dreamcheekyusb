using System;

namespace BRB
{
    public class BigRedButtonMessage
    {

        private const int DepressAction = 0x0800;

        private const int PressAction = 0x16;
        private const int OpenAction = 0x17;
        private const int CloseAction = 0x15;

        private readonly ushort _message;
        private readonly byte _buttonsPressed;

        public BigRedButtonMessage(byte[] message)
        {
            if (message != null && message.Length == 8)
            {
                var value = new byte[4];
                Array.Copy(message, 0, value, 0, 2);
                _message = BitConverter.ToUInt16(value, 0);
            }
            else throw new InvalidCastException("Cannot convert big red button message to 32 bit integer.");
            _buttonsPressed = GetButtonsPressed(this);
        }

        public bool Depress { get { return (DepressAction == _message); } }
        public bool RedPressed { get { return (PressAction == _message); } }
        public bool IsOpen { get { return (OpenAction == _message); } }
        public bool IsClosed { get { return (CloseAction == _message); } }


        private static byte GetButtonsPressed(BigRedButtonMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            byte buttonsPressed = 0;

            if (message.RedPressed) { buttonsPressed++;}
            return buttonsPressed;
        }

    }
}
