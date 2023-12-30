using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AuditingSystem.Web.Controllers.AuditProcess
{
    public class DepartmentController : Controller
    {
        private readonly IBaseRepository<Department, int> _departmentRepository;
        private readonly IBaseRepository<Company, int> _companyRepository;
        public DepartmentController(
            IBaseRepository<Company, int> companyRepository,
            IBaseRepository<Department, int> departmentRepository)
        {
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var departments = await _departmentRepository.ListAsync(
                c => c.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
                c => c.Company);

            var model = departments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = departments.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(departments.Count() / (double)pageSize);

            string apiurl = "https://onyx3.azurewebsites.net/departments/Getdepartments";
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

                    // Create APIDepartment entities and add them to the repository
                    foreach (var item in json["items"])
                    {
                        int id = (int)item["id"];
                        string name = (string)item["departmentName"];
                        int? companyId = item["companyId"]?.ToObject<int?>() ?? null;
                        string code = (string)item["code"];

                        // Check if the APIDepartment with the given id already exists
                        var existingApiDepartment = await _departmentRepository.FindByAsync(id);
                        if (existingApiDepartment == null)
                        {
                            // Check if companyId is not null
                            if (companyId != null)
                            {
                                // Check if there is a record in the Company table with the same id
                                var existingCompany = await _companyRepository.FindByAsync(companyId.Value);

                                // Create a new APIDepartment and add it to the repository
                                var newApiDepartment = new Department
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    Source = "API",
                                    CompanyId = existingCompany != null ? existingCompany.Id : null,
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _departmentRepository.CreateAsync(newApiDepartment);
                            }
                            else
                            {
                                // Create a new APIDepartment without a CompanyId and add it to the repository
                                var newApiDepartment = new Department
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    Source = "API",
                                    CompanyId = companyId,
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _departmentRepository.CreateAsync(newApiDepartment);
                            }
                        }
                    }
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Add()
        {
            var company = _companyRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.CompanyId = new SelectList(company, "Id", "Name");

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentRepository.FindByAsync(id);

            var company = _companyRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.CompanyId = new SelectList(company, "Id", "Name", department.CompanyId);

            return View(department);
        }
    }
}
