using ElasticSearch.API.Model;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearch.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ElasticController : ControllerBase
    {
        private readonly IElasticClient _client;
        private string index = "people";

        public ElasticController(IElasticClient client) 
        {
            _client = client;
        }


        /// <summary>
        /// Function to Add
        /// </summary>
        /// <param name="person"></param>
        /// <returns>Success message of indexing</returns>
        [HttpPost("add-person")]
        public IActionResult AddPerson(Person person)
        {
            var indexResponse = _client.Index(person, idx => idx.Index(index));
            return Ok(indexResponse);
        }


        /// <summary>
        /// Function to search Person model by name or surname
        /// </summary>
        /// <param name="text">name or surname</param>
        /// <returns>Matching List<person> response</returns>
        [HttpGet("/search-person/{text}")]
        public IActionResult CreateIndex(string text)
        {
            var searchResponse = _client.Search<Person>(s => s
                .Index(index)
                .Query(q => q
                    .Bool(b => b
                    .Should(sh => sh
                        .Match(m => m
                            .Field(f => f.Name)
                            .Query(text)
                        ),
                        sh => sh
                        .Match(m => m
                            .Field(f => f.Surname)
                            .Query(text)
                            )
                        )
                    )
                )
            );

            
            if (searchResponse.IsValid)
            {
                var results = searchResponse.Documents;
                return Ok(results);
            }

            return BadRequest();
        }

    }
}
