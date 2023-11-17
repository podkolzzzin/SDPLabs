// See https://aka.ms/new-console-template for more information

using SDPLabs.Common;
using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Repositories;

Console.WriteLine("This APP will randomly publishes CarMoved events to all cars in database with Random delays!");

var publisher = new EventPublisherService("localhost", "user", "password");
await using var ctx = new SDPLabsDbContext();
var carsRepository = new CarRepository(ctx);
var cars = await carsRepository.GetAllAsync();
var random = Random.Shared;
while (true)
{
  foreach (var car in cars)
  {
    if (random.NextDouble() > 0.3)
    {
      var evt = new CarMoved(car.Id, random.NextDouble() * 50 + 50, DateTime.Now);
      publisher.Publish(evt);
      Console.WriteLine($"Published event: {evt}");
    }
  }
}