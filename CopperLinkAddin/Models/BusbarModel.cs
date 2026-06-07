namespace CopperLinkAddin.Models
{
    public class BusbarModel
    {
        public int ShapeType { get; set; }
        public double Thickness { get; set; }
        public double Width { get; set; }
        public double BendRadius { get; set; }
        public double D1 { get; set; }
        public double D2 { get; set; }
        public double D3 { get; set; }
        public string Material { get; set; }
        public string SavePath { get; set; }
        public string FileName { get; set; }
        public HoleModel Face1Holes { get; set; }
        public HoleModel Face2Holes { get; set; }
    }
}