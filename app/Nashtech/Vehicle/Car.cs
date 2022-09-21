using Confluent.Kafka;
using GeoCoordinatePortable;
using Newtonsoft.Json;

namespace Vehicle;

public class Car : Location, IAction
{
    public string Name { get; set; }
    public string Plate { get; set; }
    public bool IsRunning { get; set; }
    public Car(string name, string plate)
    {
        Name = name;
        Plate = plate;
        IsRunning = false;
        GetVehicleLocationFromSensor();
    }

    public void GetVehicleLocationFromSensor()
    {
        const int minimumCoordinate = 10;
        Random rd = new Random();
        UpperLeftCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        UpperRightCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        BottomLeftCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        BottomRightCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        SendToTrafficCoordinationSystem();
    }

    public double GetMinimumDistanceBetweenOtherCar(Car other)
    {
        List<double> distanceCorner = new List<double>();
        var distanceUpperLeft =
            new GeoCoordinate(UpperLeftCorner.X, UpperLeftCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.UpperLeftCorner.X,
                    other.UpperLeftCorner.Y));
        var distanceUpperRight =
            new GeoCoordinate(UpperRightCorner.X, UpperRightCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.UpperRightCorner.X,
                    other.UpperRightCorner.Y));
        var distanceBottomLeft =
            new GeoCoordinate(BottomLeftCorner.X, BottomLeftCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.BottomLeftCorner.X,
                    other.BottomLeftCorner.Y));
        var distanceBottomRight =
            new GeoCoordinate(BottomRightCorner.X, BottomRightCorner.Y).GetDistanceTo(
                new GeoCoordinate(other.BottomRightCorner.X,
                    other.BottomRightCorner.Y));
        distanceCorner.AddRange(
            new[] { distanceUpperLeft, distanceUpperRight, distanceBottomLeft, distanceBottomRight });
        return distanceCorner.Min();
    }

    public void Running()
    {
        IsRunning = true;
        while (IsRunning)
        {
            this.GetVehicleLocationFromSensor();
            Thread.Sleep(1000);
        }
    }

    public void StopEngine()
    {
        IsRunning = false;
    }

    public void StartEngine()
    {
        IsRunning = true;
        this.GetVehicleLocationFromSensor();
    }
    
    
    #region Send To Traffic Coordination System via Kafka
    private void SendToTrafficCoordinationSystem()
    {
        const int delayTime = 1000;
        var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

        Action<DeliveryReport<Null, string>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

        using (var p = new ProducerBuilder<Null, string>(conf).Build())
        {
            p.Produce("traffic-system", new Message<Null, string> { Value = JsonConvert.SerializeObject(this) }, handler);
            p.Flush(TimeSpan.FromMilliseconds(delayTime));
        }
    }
    #endregion
}