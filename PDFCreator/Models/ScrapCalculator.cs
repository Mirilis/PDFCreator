using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFCreator.Models
{
    
    public class ScrapCalculator
    {
        private readonly List<C90dScrapTotals>  productScrap;
        private readonly List<C90dScrapTotals> allScrap;
        public Dictionary<string, string> ScrapValues { get; set; }

        public ScrapCalculator (string product,int line)
        {
            using (var context = new ScrapModelContainer())
            {
                allScrap = context.C90dScrapTotals.ToList();
                ScrapValues = new Dictionary<string, string>();
                if (line ==0)
                {
                    productScrap = allScrap
                        .Where(x => x.Product == product)
                        .GroupBy(a => new { a.Product, a.Reason, a.ID })
                        .Select(b => new
                        {
                            Line = "0",
                            Reason = b.
                            Key.Reason,
                            Product = b.Key.Product,
                            ID = b.Key.ID,
                            Produced = b.Sum(y => y.Produced),
                            Scrapped = b.Sum(z => z.Scrapped)
                        })
                        .ToList()
                        .Select(x => new C90dScrapTotals()
                        {
                            Product = x.Product,
                            Scrapped = x.Scrapped,
                            Produced = x.Produced,
                            Line = x.Line,
                            ID = x.ID,
                            Reason = x.Reason
                        }).ToList();



                }
                else
                {
                    productScrap = allScrap.Where(x => x.Product == product && x.Line == line.ToString()).ToList();
                }
                


                var d = new List<Tuple<double, string, string>>();
                foreach (var item in productScrap)
                {
                    var perc = item.Scrapped.Value / (double)item.Produced.Value;
                    var detail = string.Format("{0}: {1:P2}", item.Reason, perc);
                    d.Add(new Tuple<double, string, string>(perc,item.Reason, detail));
                }
                d = d.OrderByDescending(x => x.Item1).ToList();
                for (int i = 0; i < d.Count(); i++)
                {
                    ScrapValues.Add(string.Format("Defect{0}", i + 1), d[i].Item3);
                }

            }
        }

    }
}