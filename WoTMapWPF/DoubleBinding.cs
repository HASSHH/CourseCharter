﻿using System.Windows.Data;

namespace WoTMapWPF
{
    public class DoubleBinding : MultiBinding
    {
        public DoubleBinding(BindingBase b1, BindingBase b2)
        {
            Bindings.Add(b1);
            Bindings.Add(b2);
        }
    }
}
