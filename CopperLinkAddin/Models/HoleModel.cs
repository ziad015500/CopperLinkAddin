namespace CopperLinkAddin.Models
{
    public class HoleModel
    {
        public string HoleType { get; set; } // "Circle" / "VSlot" / "HSlot"
        public double Diameter { get; set; }
        public double SlotWidth { get; set; }
        public double SlotHeight { get; set; }
        public int Columns { get; set; }
        public double ColumnSpacing { get; set; }
        public int Rows { get; set; }
        public double RowSpacing { get; set; }
        public double EdgeDistanceX { get; set; }
        public double EdgeDistanceY { get; set; }
        public bool CenterAcrossWidth { get; set; }
        public bool Mirror { get; set; }
    }
}