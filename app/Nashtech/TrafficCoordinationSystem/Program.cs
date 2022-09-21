using Confluent.Kafka;
using GeoCoordinatePortable;
using Newtonsoft.Json;
using Vehicle;

Console.WriteLine("---------Traffic Coordination system---------");

GetVehicleLocation();


double GetMinimumDistanceBetweenOtherCar(Car car1, Car car2)
{
    List<double> distanceCorner = new List<double>();
    var distanceUpperLeft =
        new GeoCoordinate(car1.UpperLeftCorner.X, car1.UpperLeftCorner.Y).GetDistanceTo(
            new GeoCoordinate(car2.UpperLeftCorner.X,car2.UpperLeftCorner.Y));
    var distanceUpperRight =
        new GeoCoordinate(car1.UpperLeftCorner.X, car1.UpperLeftCorner.Y).GetDistanceTo(
            new GeoCoordinate(car2.UpperRightCorner.X, car2.UpperRightCorner.Y));
    var distanceBottomLeft =
        new GeoCoordinate(car1.UpperLeftCorner.X, car1.UpperLeftCorner.Y).GetDistanceTo(
            new GeoCoordinate(car2.BottomLeftCorner.X,car2.BottomLeftCorner.Y));
    var distanceBottomRight =
        new GeoCoordinate(car1.UpperLeftCorner.X, car1.UpperLeftCorner.Y).GetDistanceTo(
            new GeoCoordinate(car2.BottomRightCorner.X,car2.BottomRightCorner.Y));
    distanceCorner.AddRange(
        new[] { distanceUpperLeft, distanceUpperRight, distanceBottomLeft, distanceBottomRight });
    return distanceCorner.Min();
}

void GetVehicleLocation()
{
    var conf = new ConsumerConfig
    {
        GroupId = "vehicle-consumer-group",
        BootstrapServers = "localhost:9092",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
    {
        c.Subscribe("traffic-system");

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            while (true)
            {
                try
                {
                    var cr = c.Consume(cts.Token);
                    var vehicle = JsonConvert.DeserializeObject<Car>(cr.Value);
                    if (vehicle != null)
                        Console.WriteLine($"Vehicle {vehicle.Name} - {vehicle.Plate} at: X = '{vehicle.BottomLeftCorner.X}' Y= '{vehicle.BottomLeftCorner.Y}'");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            c.Close();
        }
    }
}