using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TestStore.Models;

namespace TestStore.Services
{
    public class ParseGoodRozetka : IParseGood
    {
        public IEnumerable<Good> ParseGood(string htmlData)
        {
            var culture = new CultureInfo("en-US");
            var result = new List<Good>();

            HtmlDocument doc = new HtmlDocument();

            byte[] byteArray = Encoding.UTF8.GetBytes(htmlData);
            var ms = new MemoryStream(byteArray);

            doc.Load(ms);
            var root = doc.DocumentNode;
            var catalogGrid = root.SelectSingleNode("//ul[@class='catalog-grid']");
            var catalogGridContent = catalogGrid.SelectNodes("//li[@class='catalog-grid__cell  catalog-grid__cell_type_slim']");

            decimal price;
            var description = string.Empty;
            
            for (var i = 0; i < 2; i++)
            {
                description = catalogGridContent[i]
                      .SelectSingleNode("//app-goods-tile-default")
                      .SelectSingleNode("//div[@class='goods-tile']")
                      .SelectSingleNode("//a[@class='goods-tile__heading']")
                      .Attributes["Title"].Value;
                   
                 
                price = Convert.ToDecimal(catalogGridContent[i]
                     .SelectSingleNode("//app-goods-tile-default")
                     .SelectSingleNode("//div[@class='goods-tile']")
                    .SelectSingleNode("//div[@class='goods-tile__price goods-tile__price_color_red']")
                    .SelectSingleNode("//p")
                    .SelectSingleNode("//span[@class='goods-tile__price-value']").InnerText.Replace("&nbsp;","."), culture);

                var good = new Good()
                {
                    Description = description,
                    Price = Convert.ToDecimal(price)
                };

                result.Add(good);
            }

            return result;
        }
    }
}
