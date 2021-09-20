using System.Collections.Generic;
using System.Threading.Tasks;
using TestStore.Models;

namespace TestStore.Services
{
    public interface IParseGood
    {
        public IEnumerable<Good> ParseGood(string htmlData);
    }
}
