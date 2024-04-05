using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WoTMapWPF.CustomControls
{
    public class GLLineStipplePatternControlViewModel : INotifyPropertyChanged
    {
        private short stipplePattern;

        public event PropertyChangedEventHandler? PropertyChanged;

        public GLLineStipplePatternControlViewModel()
        {
            BitList = new List<StippleBit>();
            for (int i = 0; i < 16; i++)
                BitList.Add(new StippleBit(false));
            InitializeBitList();
            foreach (StippleBit bit in BitList)
                bit.PropertyChanged += Bit_PropertyChanged;
        }

        public List<StippleBit> BitList { get; private set; }
        public short StipplePattern { get => stipplePattern; set { stipplePattern = value; OnPropertyChanged("StipplePattern"); } }

        public void InitializeBitList()
        {
            short stipplePattern = (short)App.Current.Resources["LineStipplePattern"];
            byte[] stippleBytes = BitConverter.GetBytes(stipplePattern);
            int bitIndex = 0;
            foreach (byte b in stippleBytes)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool bitValue = (b & (1 << i)) != 0;
                    BitList[bitIndex++].Value = bitValue;
                }
            }
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void Bit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            byte[] stippleBytes = new byte[2];
            int bitIndex = 0;
            int bitValue;
            for (int j = 0; j < stippleBytes.Length; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    bitValue = BitList[bitIndex++].Value ? 1 : 0;
                    stippleBytes[j] = (byte)(stippleBytes[j] & ~(1 << i) | ((int)bitValue << i));
                }
            }
            StipplePattern = BitConverter.ToInt16(stippleBytes, 0);
        }

        public class StippleBit : INotifyPropertyChanged
        {
            private bool value;

            public event PropertyChangedEventHandler? PropertyChanged;

            public StippleBit(bool value)
            {
                Value = value;
            }

            public bool Value { get => value; set { this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value")); } }
        }
    }
}
