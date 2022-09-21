using Confluent.Kafka;
using GeoCoordinatePortable;
using Newtonsoft.Json;
using Vehicle;

Console.WriteLine("---------Traffic Coordination system---------");

List<Car> allVehicles = new List<Car>();
GetVehicleLocation();


double GetMinimumDistanceBetweenOtherCar(Car car1, Car car2)
{
    if (car1.Location != null && car2.Location != null)
        return new GeoCoordinate(car1.Location.X, car1.Location.Y).GetDistanceTo(
            new GeoCoordinate(car2.Location.X, car2.Location.Y));
    return -1;
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
                    {
                        Console.WriteLine(
                            $"Vehicle {vehicle.Name} - {vehicle.Plate} at: X = '{vehicle.Location?.X}' Y= '{vehicle.Location?.Y}'");
                        int indexVehicle = allVehicles.FindIndex(item => item.Plate == vehicle.Plate);
                        if (indexVehicle != -1)
                            allVehicles[indexVehicle].Location = vehicle.Location;
                        else
                            allVehicles.Add(vehicle);
                        if (allVehicles.Count > 1)
                        {
                            for (int item = 0; item < allVehicles.Count - 1; item++)
                            {
                                Console.WriteLine(
                                    $"Vehicle {allVehicles[item].Plate} <-> {allVehicles[item + 1].Plate} is: {Math.Round(GetMinimumDistanceBetweenOtherCar(allVehicles[item], allVehicles[item + 1]))}");
                            }
                        }
                    }
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