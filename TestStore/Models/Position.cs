namespace TestStore.Models
{
    public class Position
    {
        public int PositionId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int GoodId { get; set; }
        public Good Good { get; set; }
    }
}
