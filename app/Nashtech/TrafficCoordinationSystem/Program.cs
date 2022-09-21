using Confluent.Kafka;

Console.WriteLine("---------Traffic Coordination system---------");

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
                    Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
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