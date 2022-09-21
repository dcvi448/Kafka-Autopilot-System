using GeoCoordinatePortable;

namespace CarLocation;

public class Car : Location
{
    public string Name { get; set; }
    public string Plate { get; set; }

    public Location VehicleLocation { get; set; }

    public Car(string name, string plate, Coordinates upperLeftCorner, Coordinates upperRightCorner,
        Coordinates bottomLeftCorner, Coordinates bottomRightCorner)
    {
        Name = name;
        Plate = plate;
        VehicleLocation = new Location()
        {
            UpperLeftCorner = upperLeftCorner,
            UpperRightCorner = upperRightCorner,
            BottomLeftCorner = bottomLeftCorner,
            BottomRightCorner = bottomRightCorner
        };
    }

    public double GetMinimumDistanceBetweenOtherCar(Car other)
    {
        List<double> distanceCorner = new List<double>();
        var distanceUpperLeft =
            new GeoCoordinate(UpperLeftCorner.X, UpperLeftCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.VehicleLocation.UpperLeftCorner.X,
                    other.VehicleLocation.UpperLeftCorner.Y));
        var distanceUpperRight =
            new GeoCoordinate(UpperRightCorner.X, UpperRightCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.VehicleLocation.UpperRightCorner.X,
                    other.VehicleLocation.UpperRightCorner.Y));
        var distanceBottomLeft =
            new GeoCoordinate(BottomLeftCorner.X, BottomLeftCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.VehicleLocation.BottomLeftCorner.X,
                    other.VehicleLocation.BottomLeftCorner.Y));
        var distanceBottomRight =
            new GeoCoordinate(BottomRightCorner.X, BottomRightCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.VehicleLocation.BottomRightCorner.X,
                    other.VehicleLocation.BottomRightCorner.Y));
        distanceCorner.AddRange(
            new[] { distanceUpperLeft, distanceUpperRight, distanceBottomLeft, distanceBottomRight });
        return distanceCorner.Min();
    }
}