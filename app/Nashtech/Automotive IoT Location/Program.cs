/*
 * The system simulates the position of the cars in circulation.
 * Each vehicle position is simulated by 1 task
 * In order to serve the autopilot system in the future.
 */

using Automotive_IoT_Location;

Console.WriteLine("---------Automovie application---------");
#region Init the car data
const int minimumCoordinate = 10;
Random rd = new Random();
Car carNumber1 = new Car("Car1", "75F-12345",
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate , Y = rd.NextDouble() + minimumCoordinate},
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate },
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate},
    new Coordinates() { X = rd.NextDouble() + minimumCoordinate, Y = rd.NextDouble() + minimumCoordinate });
Car carNumber2 = new Car("Car2", "30A-54321",
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() },
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() },
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() },
    new Coordinates() { X = rd.NextDouble(), Y = rd.NextDouble() });
#endregion

#region Faking the car location in the real time using the Thread for each car

Thread car1 = new Thread(()=> RandomThisVehicleLocation(ref carNumber1));
Thread car2 = new Thread(()=> RandomThisVehicleLocation(ref carNumber2));
car1.Start();
car2.Start();

#endregion



void RandomThisVehicleLocation(ref Car car)
{
    const int delayTime = 1500;
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
        Thread.Sleep(delayTime);
    }
}