using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace ElasticSearch.API.Controllers
{

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

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


        [HttpPost("add-person")]
        public IActionResult AddPeople(Person person)
        {
            var indexResponse = _client.Index(person, idx => idx.Index(index));
            return Ok(indexResponse);
        }


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
