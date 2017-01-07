using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPBodyData: GPGregorianTime
    {
        public double eclipticalLongitude { get; set; }
        public double eclipticalLatitude { get; set; }
        public double rightAscession { get; set; }
        public double declination { get; set; }
        public double azimuth { get; set; }
        public double elevation { get; set; }
        public double distance { get; set; }

        public GPBodyData(GPGregorianTime time)
            : base(time)
        {
        }

        public GPBodyData(GPBodyData data):base(data)
        {
            this.eclipticalLongitude = data.eclipticalLongitude;
            this.eclipticalLatitude = data.eclipticalLatitude;
            this.rightAscession = data.rightAscession;
            this.declination = data.declination;
            this.azimuth = data.azimuth;
            this.elevation = data.elevation;
            this.distance = data.distance;
        }

        public override string ToString()
        {
            return String.Format("Longitude:{0}, RA:{1}, Azimuth:{2}, Elevation:{3}, Declination: {4}", eclipticalLongitude, rightAscession, azimuth, elevation, declination);
        }

    }
}
