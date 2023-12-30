using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AuditingSystem.Web.Controllers.AuditProcess
{
    public class PracticeController : Controller
    {
        private readonly IBaseRepository<Tasks, int> _tasksRepository;
        private readonly IBaseRepository<Practice, int> _practiceRepository;
        public PracticeController(
            IBaseRepository<Tasks, int> tasksRepository,
            IBaseRepository<Practice, int> practiceRepository)
        {
            _tasksRepository = tasksRepository;
            _practiceRepository = practiceRepository;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var practices = await _practiceRepository.ListAsync(
                c => c.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
                c => c.Task);

            var model = practices.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = practices.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(practices.Count() / (double)pageSize);

            string apiurl = "https://onyx3.azurewebsites.net/practices/Getpractices";
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

                    // Create APIPractice entities and add them to the repository
                    foreach (var item in json["items"])
                    {
                        int id = (int)item["id"];
                        string name = (string)item["practiceName"];
                        int? taskId = item["taskId"]?.ToObject<int?>() ?? null;
                        string code = (string)item["code"];

                        // Check if the APIPractice with the given id already exists
                        var existingApiPractice = await _practiceRepository.FindByAsync(id);

                        if (existingApiPractice == null)
                        {
                            // Check if taskId is not null
                            if (taskId != null)
                            {
                                // Check if there is a record in the Task table with the same taskId
                                var existingTask = await _tasksRepository.FindByAsync(taskId.Value);

                                if (existingTask == null)
                                {
                                    // Create a new APIPractice and add it to the repository
                                    var newApiPractice = new Practice
                                    {
                                        Id = id,
                                        Name = name,
                                        Code = code,
                                        TaskId = null,
                                        Source = "API",
                                        CreatedBy = "Admin",
                                        CreationDate = DateTime.Now,
                                        UpdatedBy = "Admin",
                                        UpdatedDate = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    await _practiceRepository.CreateAsync(newApiPractice);
                                }
                                else
                                {
                                    // Log or handle the case where the associated Task doesn't exist
                                }
                            }
                            else
                            {
                                // Create a new APIPractice without a TaskId and add it to the repository
                                var newApiPractice = new Practice
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    TaskId = taskId,
                                    Source = "API",
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _practiceRepository.CreateAsync(newApiPractice);
                            }
                        }
                    }
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Add()
        {
            var task = _tasksRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.TaskId = new SelectList(task, "Id", "Name");

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var practice = await _practiceRepository.FindByAsync(id);

            var task = _tasksRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.TaskId = new SelectList(task, "Id", "Name", practice.TaskId);

            return View(practice);
        }
    }
}
