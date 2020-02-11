namespace eda.core
{
  public interface IOrderItem : IHaveItem
  {
    string Description { get; set; }
    int Quantity { get; set; }
    double Price { get; set; }
  }
}
