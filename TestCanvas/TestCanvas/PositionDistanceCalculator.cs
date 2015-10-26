using System;

namespace TestCanvas
{
    /// <summary>
    ///     This class can be used to calculate the distance
    ///     between two positions.
    /// </summary>
    /// <remarks>
    ///     Author:     Daniel Saidi [daniel.saidi@gmail.com]
    ///     Link:       http://danielsaidi.github.com/nextra
    /// </remarks>
    public class PositionDistanceCalculator : IPositionDistanceCalculator
    {
        private readonly IAngleConverter _angleConverter;


        public PositionDistanceCalculator(IAngleConverter angleConverter)
        {
            _angleConverter = angleConverter;
        }


        public double CalculateDistance(IPosition pos1, IPosition pos2, DistanceUnit unit)
        {
            double r = (unit == DistanceUnit.Miles)
                           ? GeoConstants.EarthRadiusInMiles
                           : GeoConstants.EarthRadiusInKilometers;
            double dLat = _angleConverter.ConvertDegreesToRadians(pos2.X) -
                          _angleConverter.ConvertDegreesToRadians(pos1.X);
            double dLon = _angleConverter.ConvertDegreesToRadians(pos2.Y) -
                          _angleConverter.ConvertDegreesToRadians(pos1.Y);
            double a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                       Math.Cos(_angleConverter.ConvertDegreesToRadians(pos1.X))*
                       Math.Cos(_angleConverter.ConvertDegreesToRadians(pos2.X))*Math.Sin(dLon/2)*
                       Math.Sin(dLon/2);
            double c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = c*r;

            return Math.Round(distance, 2);
        }

        public double CalculateRhumbDistance(IPosition pos1, IPosition pos2, DistanceUnit unit)
        {
            double r = (unit == DistanceUnit.Miles)
                           ? GeoConstants.EarthRadiusInMiles
                           : GeoConstants.EarthRadiusInKilometers;
            double lat1 = _angleConverter.ConvertDegreesToRadians(pos1.X);
            double lat2 = _angleConverter.ConvertDegreesToRadians(pos2.X);
            double dLat = _angleConverter.ConvertDegreesToRadians(pos2.X - pos1.X);
            double dLon = _angleConverter.ConvertDegreesToRadians(Math.Abs(pos2.Y - pos1.Y));

            double dPhi = Math.Log(Math.Tan(lat2/2 + Math.PI/4)/Math.Tan(lat1/2 + Math.PI/4));
            double q = Math.Cos(lat1);
            if (dPhi != 0) q = dLat/dPhi; // E-W line gives dPhi=0
            // if dLon over 180° take shorter rhumb across 180° meridian:
            if (dLon > Math.PI) dLon = 2*Math.PI - dLon;
            double dist = Math.Sqrt(dLat*dLat + q*q*dLon*dLon)*r;

            return dist;
        }
    }
}