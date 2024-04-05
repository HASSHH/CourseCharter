using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTMapWPF
{
    public class MapFileDefinition
    {
        public string Name { get; set; } = string.Empty;
        public int SampleUnits { get; set; }
        public int SamplePixels { get; set; }
        public string UnitLabel { get; set; } = string.Empty;
        public string ImageMD5 { get; set; } = string.Empty;
        public string ImageExt { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is MapFileDefinition))
                return false;
            MapFileDefinition other = (MapFileDefinition)obj;
            if (Name == other.Name &&
                ImageMD5 == other.ImageMD5 &&
                SamplePixels == other.SamplePixels &&
                SampleUnits == other.SampleUnits &&
                UnitLabel == other.UnitLabel &&
                ImageExt == other.ImageExt)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
