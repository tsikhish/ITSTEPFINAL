using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain
{
    public class User 
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int Age {  get; set; }
        public int MaxScore { get;set; }
        public List<int> QuizIds { get; set; } = new List<int>();
    }
}
