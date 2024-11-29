using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IQuizService
    {
        public void GetAllQuizs();
        public void CreateQuiz(string username);
        public void UpdateQuiz(string username);
        public void DeleteQuiz(string username);    
        public void SolvingQuiz(string username);
        public void Main(string username);
    }
}
