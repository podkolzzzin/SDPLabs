namespace SDPLabs.Common.Interfaces;

public interface IEventPublisherService
{
  void Publish<T>(T @event) where T : notnull;
}