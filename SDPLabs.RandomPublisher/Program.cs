// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SDPLabs.Common.Events;
using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Repositories;

var ctx = new SDPLabsDbContext();
var repository = new CarRepository(ctx);

var factory = new ConnectionFactory()
{
  HostName = "localhost", // Change to the appropriate RabbitMQ server address if needed
  UserName = "user",
  Password = "password"
};

var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.ExchangeDeclare(nameof(CarMovedEvent), ExchangeType.Fanout);

while (true)
{
  var cars = await repository.GetAllAsync();
  foreach (var car in cars)
  {
    if (Random.Shared.NextDouble() > 0.7)
    {
      PublishMovement(car);
    }
  }
  await Task.Delay(1000);
}

void PublishMovement(Car car)
{
  var carMovedEvent = new CarMovedEvent(car.Id, Random.Shared.NextDouble() * 100 + 10);
  var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(carMovedEvent));
  channel.BasicPublish(nameof(CarMovedEvent), "", null, body);
  Console.WriteLine($"Published {carMovedEvent}");
}