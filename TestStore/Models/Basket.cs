using System.Collections.Generic;

namespace TestStore.Models
{
    public class Basket
    {
        public IEnumerable<Good> goods { get; set; }
    }
}
