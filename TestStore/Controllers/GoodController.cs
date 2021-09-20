using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestStore.Models;
using TestStore.Services;

namespace TestStore.Controllers
{
    [ApiController]
    public class GoodController : ControllerBase
    {
        ApplicationContext db;
        IParseGood _parseGood;
        // private const string URL = "https://rozetka.com.ua/mobile-phones/c80003/filter/";
        private const string URL = "https://all.biz/";

        public GoodController(ApplicationContext context, IParseGood parseGood)
        {
            db = context;
            _parseGood = parseGood;
        }

        [HttpGet("/api/getgoods")]
        public async Task<IActionResult> GetGoods()
        {
            IEnumerable<Good> goods = await db.Goods.ToListAsync();

            if (goods != null)
            {
                return Ok(goods);
            }

            return BadRequest(new { errorText = "Нет ни одного товара" });
        }

        [HttpGet("/api/getgood/{goodid}")]
        public async Task<IActionResult> GetGood(int goodId)
        {
            Good good = await db.Goods.FirstOrDefaultAsync(g => g.GoodId == goodId);

            if (good != null)
            {
                return Ok(good);
            }

            return BadRequest(new { errorText = "Товар не найден" });
        }

        [HttpPost("/api/newgood/")]
        [Authorize]
        public async Task<IActionResult> NewUser([FromBody] Good good)
        {
            if (good != null)
            {
                db.Goods.Add(good);

                await db.SaveChangesAsync();

                return Ok(good);
            }

            return BadRequest(new { errorText = "Некорректные исходные данные" });
        }

        [HttpPut("/api/updategood/")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] Good good)
        {
            if (good == null)
            {
                return BadRequest(new { errorText = "Некорректные данные." });
            }
            if (!db.Goods.Any(g => g.GoodId == good.GoodId))
            {
                return NotFound(new { errorText = "Товар не найден." });
            }

            db.Update(good);
            await db.SaveChangesAsync();

            return Ok(good);
        }

        [HttpDelete("/api/deletegood/{goodid}")]
        [Authorize]
        public async Task<IActionResult> DeleteGood(int goodId)
        {
            Good good = db.Goods.FirstOrDefault(g => g.GoodId == goodId);

            if (good == null)
            {
                return NotFound(new { errorText = "Товар не найден." });
            }

            db.Goods.Remove(good);
            await db.SaveChangesAsync();

            return Ok(new { okText = "Товар удален" });
        }

        [HttpGet("/api/addgood/")]
     //   [Authorize]
        public async Task<IActionResult> AddGood()
        {
            var goods = new List<Good>();
            var request = new HttpRequestMessage(HttpMethod.Get, URL);

            request.Headers.Add("Accept", "text/html, application/xhtml+xml, application/xml, application/json;q=0.9, */*;q=0.8");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");

            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
               var htmlData = await response.Content.ReadAsStringAsync();

                 goods = _parseGood.ParseGood(htmlData).ToList();

                foreach (var good in goods)
                {
                    db.Add(good);
                }

                await db.SaveChangesAsync();
            }

            return Ok(goods);
        }
    }
}
