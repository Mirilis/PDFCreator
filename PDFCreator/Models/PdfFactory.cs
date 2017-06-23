using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PDFCreator.Models
{
    public enum ProcessCardClass
    {
        CoremakeCB22,
        Coremake321,
        CoremakeLaempe,
        Coremake106,
        CoreAssembly,
        MoldingOsborn,
        MoldingSinto,
        MeltingOsborn,
        MeltingSinto,
        CleaningOsborn,
        CleaningSinto,
        Finishing
    }

    internal class PdfFactory
    {
        #region Public Constructors

        public PdfFactory(int id)
        {
            GeneratedPdf = PdfResult(id);
        }

        #endregion Public Constructors

        #region Public Methods

        private void addImage(PdfStamper stamper, AcroFields form, String fieldName, String path, int ID)
        {
            var imgspace = form.GetFieldPositions(fieldName);
            iTextSharp.text.Rectangle rect = imgspace[0].position;
            Image image;
            try
            {
                image = System.Drawing.Image.FromFile(path);
            }
            catch (Exception)
            {
                path = Path.GetDirectoryName(path) + @"/error.png";
                image = System.Drawing.Image.FromFile(path);
            }
            
            var h = (int)(rect.Height * 2);
            var w = (int)(rect.Width * 2);
            if (image.Width != w)
            {
                var re = ((Bitmap)image)
                .ResizeBitmap(h, w)
                .CropAtRect(new System.Drawing.Size(w, h));
                using (var ms = new MemoryStream())
                {
                    var errortxt = string.Empty;
                    re.Save(ms, ImageFormat.Png);
                    image.Dispose();
                    re.Dispose();
                    var imgs = System.Drawing.Image.FromStream(ms);
                    try
                    {
                        
                        var n = Guid.NewGuid().ToString();
                        path = Path.GetDirectoryName(path) + @"/" + n + ".png";
                        if (!File.Exists(path))
                        {
                            imgs.Save(Path.GetDirectoryName(path) + @"/" + n + ".png");
                            using (var context = new ProcessCardsEntities())
                            {
                                context.DataPoints.Where(x => x.ID == ID).First().Value = n + ".png";
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            errortxt = "path existed!";
                            n = Guid.NewGuid().ToString();
                            path = Path.GetDirectoryName(path) + @"/" + n + ".png";
                            if (!File.Exists(path))
                            {
                                imgs.Save(Path.GetDirectoryName(path) + @"/" + n + ".png");
                                using (var context = new ProcessCardsEntities())
                                {
                                    context.DataPoints.Where(x => x.ID == ID).First().Value = n + ".png";
                                    context.SaveChanges();
                                }
                            }
                        }
                        ms.Dispose();
                        
                    }
                    catch(Exception e)
                    {
                        throw new Exception(errortxt+ Environment.NewLine + path + Environment.NewLine + e.ToString());
                        throw;
                    }
                    
                }
            }
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(path);

            img.ScaleToFit(rect);
            img.BorderWidth = 2;
            img.SetAbsolutePosition(imgspace[0].position.Left, imgspace[0].position.Bottom);
            PdfContentByte cb = stamper.GetOverContent((int)imgspace[0].page);
            cb.AddImage(img);
        }

        public byte[] GeneratedPdf { get; set; }

        private string GetRevisionHistory(List<List<DataPoint>> rL)
        {
            var s = new StringBuilder();
            var p = rL;

            var count = p.Count();
            if (p.Count() > 6)
            {
                count = 11;
            }
            for (int i = 0; i < count - 1; i++)
            {
                if (p[i].Count() > 1)
                {
                    var date = p[i].First().ApprovedDate;
                    var key = p[i].First().Key;
                    var currentvalue = p[i].First().Value;
                    var oldValue = p[i][1].Value;
                    var val = string.Format("{0}: Changed {1} from [{2}] to [{3}]", date.ToShortDateString(), key, oldValue, currentvalue);
                    s.Append(val + Environment.NewLine);
                }
            }
            return s.ToString();
        }

        private Dictionary<string, string> GetTemplatePaths()
        {
            throw new NotFiniteNumberException();
        }

        private PdfReader GetCorrectTemplate(int ProcessCardID)
        {
            ProcessCardClass switchVar;
            using (var context = new ProcessCardsEntities())
            {
                switchVar = (ProcessCardClass)context.ProcessCards.Where(x => x.ID == ProcessCardID).First().ProcessCardClass;
            }
            var p = HttpContext.Current.Server.MapPath("/content/templates");
            var temps = Directory.GetFiles(p);
            var pathbook = new Dictionary<string, string>();
            pathbook.Add("CoremakeCB22", string.Empty);
            pathbook.Add("CoremakeLaempe", string.Empty);
            pathbook.Add("Coremake321", string.Empty);
            pathbook.Add("Coremake106", string.Empty);
            pathbook.Add("CoreAssembly", string.Empty);
            pathbook.Add("MoldingOsborn", string.Empty);
            pathbook.Add("MoldingSinto", string.Empty);
            pathbook.Add("MeltingOsborn", string.Empty);
            pathbook.Add("MeltingSinto", string.Empty);
            pathbook.Add("CleaningOsborn", string.Empty);
            pathbook.Add("CleaningSinto", string.Empty);
            pathbook.Add("Finishing", string.Empty);

            var pathtemp = pathbook.Select(x => x.Key).ToList();

            foreach (var item in pathtemp)
            {
                foreach (var f in temps)
                {
                    if (f.Contains(item))
                    {
                        pathbook[item] = Path.GetFileName(f);
                    }
                }
            }

            try
            {
                switch (switchVar)
                {
                    case ProcessCardClass.CoremakeCB22:
                        return new PdfReader(p + @"/" + pathbook["CoremakeCB22"]);

                    case ProcessCardClass.Coremake321:
                        return new PdfReader(p + @"/" + pathbook["Coremake321"]);

                    case ProcessCardClass.CoremakeLaempe:
                        return new PdfReader(p + @"/" + pathbook["CoremakeLaempe"]);

                    case ProcessCardClass.Coremake106:
                        return new PdfReader(p + @"/" + pathbook["Coremake106"]);

                    case ProcessCardClass.CoreAssembly:
                        return new PdfReader(p + @"/" + pathbook["CoreAssembly"]);

                    case ProcessCardClass.MoldingOsborn:
                        return new PdfReader(p + @"/" + pathbook["MoldingOsborn"]);

                    case ProcessCardClass.MoldingSinto:
                        return new PdfReader(p + @"/" + pathbook["MoldingSinto"]);

                    case ProcessCardClass.MeltingOsborn:
                        return new PdfReader(p + @"/" + pathbook["MeltingOsborn"]);

                    case ProcessCardClass.MeltingSinto:
                        return new PdfReader(p + @"/" + pathbook["MeltingSinto"]);

                    case ProcessCardClass.CleaningOsborn:
                        return new PdfReader(p + @"/" + pathbook["CleaningOsborn"]);

                    case ProcessCardClass.CleaningSinto:
                        return new PdfReader(p + @"/" + pathbook["CleaningSinto"]);

                    case ProcessCardClass.Finishing:
                        return new PdfReader(p + @"/" + pathbook["Finishing"]);

                    default:
                        break;
                }
            }
            catch (Exception)
            {
                return new PdfReader(p + "/blank.pdf");
            }
            return null;
        }

        private List<List<DataPoint>> GetFieldsList(int ProcessCardID)
        {
            var fieldsList = new List<List<DataPoint>>();
            using (var context = new ProcessCardsEntities())
            {
                var data = context.ProcessCards.Include("DataPoints").Where(x => x.ID == ProcessCardID).First();
                var product = data.ProductName;
                int line = 0;
                if (((ProcessCardClass)data.ProcessCardClass).ToString().Contains("Osborn"))
                {
                    line = 1;
                }
                else if (((ProcessCardClass)data.ProcessCardClass).ToString().Contains("Sinto"))
                {
                    line = 2;
                }

                var datapoints = data.DataPoints.OrderByDescending(x => x.ApprovedDate);

                var keyList = datapoints.Select(x => x.Key).Distinct().ToList();
                foreach (var item in keyList)
                {
                    fieldsList.Add(datapoints.Where(x => x.Key == item).OrderByDescending(x => x.ApprovedDate).ToList());
                }

                var count = 1;
                var date = datapoints.First().ApprovedDate;

                foreach (var item in fieldsList)
                {
                    count += item.Count - 1;
                }
                var s = GetRevisionHistory(fieldsList);
                fieldsList.Add(new List<DataPoint>() { new DataPoint() { Approver = null, Key = "RevisionNumber", Value = count.ToString() } });
                fieldsList.Add(new List<DataPoint>() { new DataPoint() { Approver = null, Key = "RevisionDate", Value = date.ToShortDateString() } });
                fieldsList.Add(new List<DataPoint>() { new DataPoint() { Approver = null, Key = "RevisionHistory", Value = s } });
                fieldsList.AddRange(GetScrapHistory(product,line));
                fieldsList.Add(new List<DataPoint>() { new DataPoint() { Approver = null, Key = "LineNumber", Value = line.ToString()} });
            }
            return fieldsList;
        }

        private List<List<DataPoint>> GetScrapHistory(string product, int line)
        {
            var tmp = new List<List<DataPoint>>();
            var s = new ScrapCalculator(product,line);
            foreach (var item in s.ScrapValues.Take(5))
            {
                tmp.Add(new List<DataPoint>() { new DataPoint() { Approver = null, Key = item.Key, Value = item.Value } } );
            }
            return tmp;
        }

        private byte[] PdfResult(int ProcessCardID)
        {
            byte[] result;

            var ms = new MemoryStream();
            var fs = GetFieldsList(ProcessCardID);
            var reader = GetCorrectTemplate(ProcessCardID);

            var stamper = new PdfStamper(reader, ms);
            stamper.FormFlattening = true;
            var af = stamper.AcroFields;

            //var sb = new StringBuilder();
            //foreach (var item in af.Fields)
            //{
            //    sb.Append(item.Key.ToString() +", ");
            //}
            //sb.Append(Environment.NewLine);
            var path = HttpContext.Current.Server.MapPath("/content/pdfImages");
            //File.WriteAllText(path + @"/fields.txt", sb.ToString());

            foreach (var item in fs)
            {
                if (af.GetField(item.First().Key) != null)
                {
                    if (item.First().Type != "image")
                    {
                        if (item.First().Value != "blank")
                        {
                            af.SetField(item.First().Key, item.First().Value.ToUpper());
                        }
                        stamper.PartialFormFlattening(item.First().Key);
                    }
                    else
                    {
                        if (item.First().Value != "blank")
                        {
                            addImage(stamper, af, item.First().Key, path + @"/" + item.First().Value, item.First().ID);
                            stamper.PartialFormFlattening(item.First().Key);
                        }
                       
                    }
                }
            }

            stamper.Close();
            reader.Close();
            result = ms.ToArray();
            return result;
        }
    }

    #endregion Public Methods
}