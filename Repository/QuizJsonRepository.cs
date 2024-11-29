using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository
{
    public class QuizJsonRepository
    {
        private readonly string _filePath;
        private readonly List<Quiz> _quizs;
        public QuizJsonRepository(string filePath)
        {
            _filePath = filePath;
            _quizs = LoadData();
        }
        public List<Quiz> GetAllQuiz() => _quizs.ToList();
        public Quiz GetQuiz(int id) => _quizs.FirstOrDefault(x => x.Id == id);
        public void Create(Quiz quiz)
        {
            quiz.Id = _quizs.Any() ? _quizs.Max(quizz => quizz.Id) + 1 : 1;
            _quizs.Add(quiz);
            SaveData();
        }
        public void Delete(int id)
        {
            var quiz = _quizs.FirstOrDefault(x => x.Id == id);
            if (quiz != null)
            {
                _quizs.Remove(quiz);
                SaveData();
            }
        }
        public void Update(Quiz quiz)
        {
            var index = _quizs.FindIndex(x=>x.Id== quiz.Id);
            if (index >= 0)
            {
                _quizs[index] = quiz;
            }
            SaveData();
        }
        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_quizs, new JsonSerializerOptions { WriteIndented = true });

            using (var writer = new StreamWriter(_filePath, false))
                writer.Write(json);
        }

        public List<Quiz> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<Quiz>();

            using (var reader = new StreamReader(_filePath))
            {
                var json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<List<Quiz>>(json);
            }
        }
    }
}
