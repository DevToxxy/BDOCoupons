using BDOCoupons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.IO;

using AngleSharp;
using AngleSharp.Html.Parser;
namespace BDOCoupons.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string url = "https://incendar.com/bdo_pearl_abyss_coupon_codes.php";
            var response = CallUrl(url).Result;
            var linkList = ParseHtml(response);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
           
            return await response;
        }
        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            
            var rows = htmlDoc.DocumentNode.Descendants("input").Where(node => node.GetAttributeValue("id", "")
                .Contains("incendar")).ToList();

            List<string> couponCodeList = new List<string>(); //coupon codes sorted from newest

            foreach (var row in rows)
            {
                couponCodeList.Add(row.GetAttributeValue("value", ""));
            }

            var couponsDesc = htmlDoc.DocumentNode.Descendants("table")
                .Where((node) => !node.HasClass("headTable")).ToList();

            var tr = couponsDesc[0].Descendants("tr").ToList();


            var td = tr[0].Descendants("td").ToList();


            List<string> dateList = new List<string>();
            foreach (var tableDate in td)
            {
                dateList.Add(tableDate.InnerText);
            }

            int i = 0;
            List<string> trueTableDateList = new List<string>(); //coupon expiration dates sorted from newest

            foreach (var tabledata in dateList)
            {
                if(i % 3 == 0)
                {
                    if(i == 0)
                    {
                        trueTableDateList.Add(tabledata.Substring(0,5)); //hardcoded: change if owner of incendar also changes format
                    }
                    else
                    {
                        trueTableDateList.Add(tabledata);
                    }
                }
                i++;
            }
            return null;

        }

    }
}
