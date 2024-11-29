using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Quiz
    {
        public int Id {  get; set; }
        public List<PossibleAnswers>? QuizQuestions {  get; set; }
        public int UserId { get; set; }
    }
}
