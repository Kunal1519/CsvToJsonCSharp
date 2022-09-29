using CsvToJson.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CsvToJson.Controllers
{
    public class HomeController : Controller
    {
        public string word4 { get; set; }
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index(FileModel obj,IFormFile file)
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;
            try
            {
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(wwwRootPath, @"files\Csv");
                    var extension = Path.GetExtension(file.FileName);
                        if (obj.fileUrl != null)
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, obj.fileUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath) != null)
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileStreams = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStreams);
                        }
                        obj.fileUrl = @"\files\Csv\" + fileName + extension;
                        string filepath = Path.Combine(upload + @"\"+ fileName );

                        var csv = new List<string[]>();
                        var lines = System.IO.File.ReadAllLines(filepath);
                        string[] word;
                    
                    foreach (string line in lines)
                    {

                        if(line.Contains("\""))
                        {

                           // word = line.Split("\"");
                            word = line.Split(",");
                            string trimword="";
                            int count = 0;
                            foreach (string word2 in word)
                            {
                                if (word2.Contains("\""))
                                {
                                    string word3 = word2.TrimEnd('\"').TrimStart('\"');
                                    if (trimword == "")
                                    {
                                        trimword = word3;
                                    }
                                    else
                                    {
                                        trimword = trimword + "," + word3;
                                    }

                                    count = count + 1;
                                    if (count == 2)
                                    {
                                        if(word4==null)
                                        {
                                            word4 = trimword;
                                        }
                                        else
                                        {
                                            word4 = word4 + "." + trimword;
                                        }
                                        trimword = "";
                                        count = 0;
                                    }
                                }
                                
                                else if (word4 == null)
                                {
                                    word4 = word2;
                                }
                                else
                                {
                                    word4 = word4 + "." + word2;
                                }
                            }
                            csv.Add(word4.Split("."));
                            word4 = null;
                        }
                        else
                        {
                            csv.Add(line.Split(','));
                        }
                        
                    }
                        var properties = lines[0].Split(',');

                        var listObjResult = new List<Dictionary<string, string>>();
                        
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var objResult = new Dictionary<string, string>();
                            for (int j = 0; j < properties.Length; j++)
                                objResult.Add(properties[j], csv[i][j]);

                            listObjResult.Add(objResult);
                        }

                        string jsonfileContent = JsonConvert.SerializeObject(listObjResult);
                        var jsonpath = Path.Combine(wwwRootPath, @"files\Json");
                        System.IO.File.WriteAllText(@"wwwroot\files\Json\" +file.Name + ".json", jsonfileContent);
                }
            }
            catch
            {
                return View();
            }
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
    }
}