using System.Collections.Generic;

namespace TestStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; }
        public List<Position> Positions { get; set; }
        public Order()
        {
            Positions = new List<Position>();
        }
    }
}
