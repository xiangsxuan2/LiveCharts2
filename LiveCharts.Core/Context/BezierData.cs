



namespace LiveChartsCore.Context
{
    public class BezierData
    {
        public ICartesianCoordinate TargetCoordinate { get; set; }
        public float X0 { get; set; }
        public float Y0 { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}

