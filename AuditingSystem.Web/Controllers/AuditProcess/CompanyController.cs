using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AuditingSystem.Web.Controllers.AuditProcess
{
    public class CompanyController : Controller
    {
        private readonly IBaseRepository<Company, int> _companyRepository;
        private readonly IBaseRepository<Industry, int> _industryRepository;
        private readonly HttpClient _httpClient;
        public CompanyController(
            IBaseRepository<Company, int> companyRepository,
            IBaseRepository<Industry, int> industryRepository,
            HttpClient httpClient)
        {
            _companyRepository = companyRepository;
            _industryRepository = industryRepository;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var companies = await _companyRepository.ListAsync(
                c => c.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
                c => c.Industry);

            var model = companies.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = companies.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(companies.Count() / (double)pageSize);

            string apiurl = "https://onyx3.azurewebsites.net/companies/Getcompanies";
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

                    // Create APICompany entities and add them to the repository
                    foreach (var item in json["items"])
                    {
                        int id = (int)item["id"];
                        string name = (string)item["companyName"];
                        int? industryId = item["industryId"]?.ToObject<int?>() ?? null;
                        string code = (string)item["code"];
                        string address = (string)item["address"];
                        string contactNo = (string)item["contactNumber"];
                        string email = (string)item["email"];

                        // Check if the APICompany with the given id already exists
                        var existingApiCompany = await _companyRepository.FindByAsync(id);
                        if (existingApiCompany == null)
                        {
                            // Check if industryId is not null
                            if (industryId != null)
                            {
                                // Check if there is a record in the Industry table with the same id
                                var existingIndustry = await _industryRepository.FindByAsync(industryId.Value);

                                // Create a new APICompany and add it to the repository
                                var newApiCompany = new Company
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    Source = "API",
                                    IndustryId = existingIndustry != null ? existingIndustry.Id : null,
                                    Address = address,
                                    ContactNo = contactNo,
                                    Email = email,
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _companyRepository.CreateAsync(newApiCompany);
                            }
                            else
                            {
                                // Create a new APICompany without an IndustryId and add it to the repository
                                var newApiCompany = new Company
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    Source = "API",
                                    IndustryId = industryId,
                                    Address = address,
                                    ContactNo = contactNo,
                                    Email = email,
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _companyRepository.CreateAsync(newApiCompany);
                            }
                        }
                        
                    }
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Add()
        {
            var industry = _industryRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.IndustryId = new SelectList(industry, "Id", "Name");

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var company = await _companyRepository.FindByAsync(id);

            var industry = _industryRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.IndustryId = new SelectList(industry, "Id", "Name", company.IndustryId);

            return View(company);
        }


    }
}
