using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class PossibleAnswers
    {
        public int Id {  get; set; }
        public string? Question {  get; set; }
        public List<string>? PossibleAnswer {  get; set; }
        public int RightAnswer {  get; set; }

    }
}
