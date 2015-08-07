namespace Core.Events
{
	public interface IOrderReadyForShipment : IOrderId
	{
	}


	public interface IOrderShipped : IOrderId
	{
	}
}
