using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Imaging;
using System.Text;

namespace AuditingSystem.Web.Controllers.AuditProcess
{
    public class IndustryController : Controller
    {
        private readonly IBaseRepository<Industry, int> _industryRepository;
        
        public IndustryController(IBaseRepository<Industry, int> industryRepository)
        {
            _industryRepository = industryRepository;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var industries = await _industryRepository.ListAsync(
                  i => i.IsDeleted == false,
                  q => q.OrderBy(u => u.ParentIndustryId),
                  i => i.ParentIndustry);

            var model = industries.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = industries.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(industries.Count() / (double)pageSize);

            string apiurl = "https://onyx3.azurewebsites.net/Industries/GetIndustries";
            using (HttpClient client = new HttpClient())
            {
                string requestBody = "{}";
                StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiurl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse JSON response using JObject
                    JObject json = JObject.Parse(responseBody);

                    // Create APIIndustry entities and add them to the repository
                    foreach (var item in json["items"])
                    {
                        int id = (int)item["id"];
                        string name = (string)item["industry"];
                        int? parentId = item["parentId"]?.ToObject<int?>() ?? null;
                        string code = (string)item["code"];

                        // Check if the APIIndustry with the given id already exists
                        var existingApiIndustry = await _industryRepository.FindByAsync(id);
                        if (existingApiIndustry == null)
                        {
                            // Check if parentId is not null
                            if (parentId != null)
                            {
                                // Check if there is a record in the ParentIndustry table with the same id
                                var existingParentIndustry = await _industryRepository.FindByAsync(parentId.Value);

                                // Create a new APIIndustry and add it to the repository
                                var newApiIndustry = new Industry
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    Source = "API",
                                    ParentIndustryId = existingParentIndustry != null ? existingParentIndustry.Id : null,
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                    // Set other properties if needed
                                };
                                await _industryRepository.CreateAsync(newApiIndustry);
                            }
                            else
                            {
                                // Create a new APIIndustry without a ParentIndustryId and add it to the repository
                                var newApiIndustry = new Industry
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    Source = "API",
                                    ParentIndustryId = parentId,
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                    // Set other properties if needed
                                };
                                await _industryRepository.CreateAsync(newApiIndustry);
                            }
                        }
                        
                    }
                }
            }

            return View(model);
        }



        public async Task<IActionResult> Add()
        {
            var industries = _industryRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;


            ViewBag.ParentIndustryId = new SelectList(industries, "Id", "Name");

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var industry = await _industryRepository.FindByAsync(id);

            var industries = _industryRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.ParentIndustryId = new SelectList(industries, "Id", "Name", industry.ParentIndustryId);

            return View(industry);
        }
    }
}
