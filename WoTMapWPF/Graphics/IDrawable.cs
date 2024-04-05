using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTMapWPF.Graphics
{
    internal interface IDrawable
    {
        public void Draw(Scene scene);
    }
}
