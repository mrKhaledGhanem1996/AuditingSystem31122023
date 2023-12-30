using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AuditingSystem.Web.Controllers.AuditProcess
{
    public class ActivityController : Controller
    {
        private readonly IBaseRepository<Activity, int> _activityRepository;
        private readonly IBaseRepository<Function, int> _functionRepository;
        public ActivityController(
            IBaseRepository<Activity, int> activityRepository,
            IBaseRepository<Function, int> functionRepository)
        {
            _activityRepository = activityRepository;
            _functionRepository = functionRepository;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var activities = await _activityRepository.ListAsync(
                c => c.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
                c => c.Function);

            var model = activities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = activities.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(activities.Count() / (double)pageSize);

            string apiurl = "https://onyx3.azurewebsites.net/activities/Getactivities";
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
                        string name = (string)item["activityName"];
                        int? functionId = item["functionId"]?.ToObject<int?>() ?? null;
                        string code = (string)item["code"];

                        // Check if the APIIndustry with the given id already exists
                        var existingApiActivity = await _activityRepository.FindByAsync(id);

                        if (existingApiActivity == null)
                        {
                            // Check if functionId is not null
                            if (functionId != null)
                            {
                                // Check if there is a record in the Function table with the same id
                                var existingFunction = await _functionRepository.FindByAsync(functionId.Value);

                                // Create a new APIIndustry and add it to the repository
                                var newApiactivity = new Activity
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    FunctionId = existingFunction != null ? existingFunction.Id : null,
                                    Source = "API",
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _activityRepository.CreateAsync(newApiactivity);
                            }
                            else
                            {
                                // Create a new APIIndustry without a FunctionId and add it to the repository
                                var newApiactivity = new Activity
                                {
                                    Id = id,
                                    Name = name,
                                    Code = code,
                                    FunctionId = null,
                                    Source = "API",
                                    CreatedBy = "Admin",
                                    CreationDate = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false
                                };
                                await _activityRepository.CreateAsync(newApiactivity);
                            }
                        }
                        //else
                        //{
                        //    // Update existing APIActivity and set the LastApiUpdateDate
                        //    existingApiActivity.Name = name;
                        //    existingApiActivity.Code = code;
                        //    existingApiActivity.FunctionId = existingFunction != null ? existingFunction.Id : null;
                        //    existingApiActivity.UpdatedBy = "Admin";
                        //    existingApiActivity.UpdatedDate = DateTime.Now;

                        //    await _activityRepository.UpdateAsync(existingApiActivity);
                        //}
                    }

                }
            }
            return View(model);
        }


        public async Task<IActionResult> Add()
        {
            var function = _functionRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.FunctionId = new SelectList(function, "Id", "Name");

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _activityRepository.FindByAsync(id);

            var function = _functionRepository.ListAsync(
                  u => u.IsDeleted == false,
                  q => q.OrderBy(u => u.Id),
                  null).Result;

            ViewBag.FunctionId = new SelectList(function, "Id", "Name", activity.FunctionId);

            return View(activity);
        }
    }
}
