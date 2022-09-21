using Confluent.Kafka;
using GeoCoordinatePortable;
using Newtonsoft.Json;

namespace Vehicle;

public class Car : IAction
{
    public string? Name { get; set; }
    public string? Plate { get; set; }
    public bool IsRunning { get; set; }
    public Coordinates? Location { get; set; }

    public Car()
    {
        IsRunning = false;
    }

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
        this.Location = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        SendToTrafficCoordinationSystem();
    }

    public void Running()
    {
        IsRunning = true;
        while (IsRunning)
        {
            this.GetVehicleLocationFromSensor();
            Thread.Sleep(2000);
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
        const int delayTime = 2000;
        var conf = new ProducerConfig { BootstrapServers = "127.0.0.1:9092" };

        Action<DeliveryReport<Null, string>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"{this.Name} - {this.Plate} => {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

        using (var p = new ProducerBuilder<Null, string>(conf).Build())
        {
            p.Produce("traffic-system", new Message<Null, string> { Value = JsonConvert.SerializeObject(this) },
                handler);
            p.Flush(TimeSpan.FromMilliseconds(delayTime));
        }
    }

    #endregion
}