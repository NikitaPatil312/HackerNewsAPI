using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HackerNewsNikita.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HackerNewsNikita.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoriesController : ControllerBase
    {
        
        private readonly IConfigurationRoot _config;

        public StoriesController(IConfiguration config)
        {
            _config = (IConfigurationRoot)config;
        }

        [HttpGet]
        [Route("{noOfStories}")]
        public async Task<ActionResult> GetBestStories([FromRoute]int noOfStories)
        {
            return await CallHackNewsAPI(noOfStories);
        }

        private async Task<ActionResult> CallHackNewsAPI(int noOfStories)
        {
            try
            {
                List<StoryModel> stories = new List<StoryModel>();
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = _config["APIEndpoints:BestStoriesAPI"]!;
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                    {

                        return NoContent();
                    }
                    var resultContent = await response.Content.ReadFromJsonAsync<List<int>>();
                    if (noOfStories > resultContent?.Count)
                    {
                        return NotFound(new JsonResult("Stories ask count is bigger than total count of best stories"));
                    }
                    var filteredList = resultContent!.Take(noOfStories);

                    foreach (var singleRecord in filteredList)
                    {
                        string apiForStoryByID = _config["APIEndpoints:StoryFromID"]!;
                        HttpResponseMessage bestStoryData = await client.GetAsync(string.Format(apiForStoryByID, singleRecord));
                        if (!response.IsSuccessStatusCode)
                        {
                            continue;
                        }
                        var storyData = JsonConvert.DeserializeObject<StoryModel>(bestStoryData.Content.ReadAsStringAsync().Result);
                        stories.Add(storyData!);

                    }
                    
                    return new JsonResult(stories.OrderByDescending(x=>x.score)); 
                }

                 
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

