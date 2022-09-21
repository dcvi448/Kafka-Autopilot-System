/*
 * The system simulates the position of the cars in circulation.
 * Each vehicle position is simulated by 1 task
 * In order to serve the autopilot system in the future.
 */

using Vehicle;
using Confluent.Kafka;
using Newtonsoft.Json;

Console.WriteLine("---------Vehicle position detection system---------");

#region Init the car data

const int minimumCoordinate = 10;
Random rd = new Random();
Car carNumber1 = new Car("Car1", "75F-12345",
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate },
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate },
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate },
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate });
Car carNumber2 = new Car("Car2", "30A-54321",
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() },
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() },
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() },
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() });

#endregion

#region Faking the car location in the real time using the Thread for each car

Thread car1 = new Thread(() => RandomThisVehicleLocation(ref carNumber1));
Thread car2 = new Thread(() => RandomThisVehicleLocation(ref carNumber2));
car1.Start();
car2.Start();

#endregion

#region Send To Traffic Coordination System via Kafka

void SendToTrafficCoordinationSystem(Car car)
{
    const int delayTime = 1000;
    var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

    Action<DeliveryReport<Null, string>> handler = r =>
        Console.WriteLine(!r.Error.IsError
            ? $"Delivered message to {r.TopicPartitionOffset}"
            : $"Delivery Error: {r.Error.Reason}");

    using (var p = new ProducerBuilder<Null, string>(conf).Build())
    {
        p.Produce("traffic-system", new Message<Null, string> { Value = JsonConvert.SerializeObject(car) }, handler);
        p.Flush(TimeSpan.FromMilliseconds(delayTime));
    }
}

#endregion

void RandomThisVehicleLocation(ref Car car)
{
    const int delayTime = 2000;
    while (true)
    {
        car.UpperLeftCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        car.UpperRightCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        car.BottomLeftCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        car.BottomRightCorner = new Coordinates()
            { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate };
        SendToTrafficCoordinationSystem(car);
        Thread.Sleep(delayTime);
    }
}