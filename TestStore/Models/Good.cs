using System.Collections.Generic;

namespace TestStore.Models
{
    public class Good
    {
        public int GoodId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<Position> Positions { get; set; }
        public Good()
        {
            Positions = new List<Position>();
        }
    }
}
