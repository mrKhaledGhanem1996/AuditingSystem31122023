using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AuditingSystem.Web.Controllers.AuditProcess
{
    public class ObjectiveController : Controller
    {
        private readonly IBaseRepository<Activity, int> _activityRepository;
        private readonly IBaseRepository<Objective, int> _objectiveRepository;
        public ObjectiveController(
            IBaseRepository<Activity, int> activityRepository,
            IBaseRepository<Objective, int> objectiveRepository)
        {
            _activityRepository = activityRepository;
            _objectiveRepository = objectiveRepository;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var objectives = await _objectiveRepository.ListAsync(
                  c => c.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  c => c.Activity);

            var model = objectives.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = objectives.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(objectives.Count() / (double)pageSize);

            string apiurl = "https://onyx3.azurewebsites.net/objectives/Getobjectives";
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

                    // Create APIObjective entities and add them to the repository
                    foreach (var item in json["items"])
                    {
                        int id = (int)item["id"];
                        string name = (string)item["objectiveName"];
                        int? activityId = item["activityId"]?.ToObject<int?>() ?? null;
                        string code = (string)item["code"];

                        // Check if the APIObjective with the given id already exists
                        var existingApiObjective = await _objectiveRepository.FindByAsync(id);
                        if (existingApiObjective == null)
                        {
                            // Check if activityId is not null
                            if (activityId != null)
                            {
                                // Check if there is a record in the Activity table with the same id
                                var existingActivity = await _activityRepository.FindByAsync(activityId.Value);

                                // Create a new APIObjective and add it to the repository
                                var newApiObjective = new Objective
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    ActivityId = existingActivity != null ? existingActivity.Id : null,
                                    Source = "API",
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _objectiveRepository.CreateAsync(newApiObjective);
                            }
                            else
                            {
                                // Create a new APIObjective without an ActivityId and add it to the repository
                                var newApiObjective = new Objective
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    ActivityId = activityId,
                                    Source = "API",
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _objectiveRepository.CreateAsync(newApiObjective);
                            }
                        }
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var activity = _activityRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.ActivityId = new SelectList(activity, "Id", "Name");

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var objective = await _objectiveRepository.FindByAsync(id);

            var activity = _activityRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.ActivityId = new SelectList(activity, "Id", "Name", objective.ActivityId);

            return View(objective);
        }
    }
}
