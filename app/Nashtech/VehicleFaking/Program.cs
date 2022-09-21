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

Car carNumber1 = new Car("Car1", "75F-12345");
Car carNumber2 = new Car("Car2", "30A-54321");

#endregion

#region Faking the car location in the real time using the Thread for each car

carNumber1.StartEngine();
Thread th1 = new Thread(() => carNumber1.Running());
th1.Start();

carNumber2.StartEngine();
Thread th2 = new Thread(() => carNumber2.Running());
th2.Start();

#endregion


