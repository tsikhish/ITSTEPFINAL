using Domain;
using Domain.DTO;
using Repository;
using Services.Interfaces;
using static System.Formats.Asn1.AsnWriter;
namespace Services
{
    public class QuizService : IQuizService
    {
        private readonly AccountJsonRepository _accountJsonRepository;
        private readonly QuizJsonRepository _quizJsonRepository;
        public QuizService(AccountJsonRepository accountJsonRepository, QuizJsonRepository quizJsonRepository)
        {
            _accountJsonRepository = accountJsonRepository;
            _quizJsonRepository = quizJsonRepository;
        }
        public void GetAllQuizs()
        {
            var allQuiz = _quizJsonRepository.GetAllQuiz();
            DisplayQuizs(allQuiz,true);
        }
        public void SolvingQuiz(string username)
        {
            var allQuiz = _quizJsonRepository.GetAllQuiz();
            DisplayQuizs(allQuiz,false);
            while (true)
            {
                var input = ValidationsForSolvingQuiz(allQuiz, username);
                if (input != -1)
                {
                    break;
                }
            }
        }
       

        public void Main(string username)
        {
            TextInConsoleBeforeAction(username);
        }
        public void CreateQuiz(string username)
        {
            var user = _accountJsonRepository.GetUserByUsername(username);
            Quiz quiz = new Quiz
            {
                UserId = user.Id,
                QuizQuestions = new List<PossibleAnswers>()
            };
            CreatingEachQuestions(quiz);
            CreatingInJson(user, quiz);
        }
        public void DeleteQuiz(string username)
        {
            var user = _accountJsonRepository.GetUserByUsername(username);
            var quizId = ValidationsForDelete(user);
            if (quizId != -1)
                SavingDeletionToJson(user, quizId);
        }

        public void UpdateQuiz(string username)
        {
            var allQuiz = _quizJsonRepository.GetAllQuiz();
            DisplayQuizs(allQuiz, true);
            var quiz = FindQuizInJson(username);
            DisplayQuizs(quiz,true);
            UpdateInJson(quiz);
        }
        private void UpdateInJson(Quiz quiz)
        {
            while (true)
            {
                Console.WriteLine("\nOptions: \n1. Update Quiz \n2. Exit");
                int choice = GetValidatedInput("Enter your choice:", new[] { 1, 2 });
                if (choice == 2)
                {
                    Console.WriteLine("Exiting update process.");
                    break;
                }
                Console.WriteLine("Enter the ID of the question you want to update:");
                int questionId = GetValidatedInput("Enter the question ID:", quiz.QuizQuestions.Select(q => q.Id).ToArray());
                var question = quiz.QuizQuestions.FirstOrDefault(x => x.Id == questionId);
                if (question == null)
                {
                    Console.WriteLine("Question ID not found.");
                    continue;
                }
                Console.WriteLine("Choose what to update: \n1. Question \n2. Possible Answers \n3. Right Answer \n4. Exit");
                int updateOption = GetValidatedInput("Enter your choice:", new[] { 1, 2, 3, 4 });
                if (updateOption == 4)
                {
                    Console.WriteLine("Exiting update for this question.");
                    continue;
                }
                PerformUpdate(question, updateOption);
                _quizJsonRepository.Update(quiz);
                Console.WriteLine("Quiz updated successfully.");
            }
        }
        private Quiz FindQuizInJson(string username)
        {
            while (true)
            {
                var user = _accountJsonRepository.LoadData().FirstOrDefault(x => x.Username == username);
                var quizId = ValidationsForUpdate(user);
                var quiz = _quizJsonRepository.GetQuiz(quizId);
                
                return quiz;
            }
        }
        private void PerformUpdate(PossibleAnswers question, int updateOption)
        {
            switch (updateOption)
            {
                case 1:
                    Console.WriteLine("Enter the updated question:");
                    question.Question = Console.ReadLine();
                    break;
                case 2:
                    Console.WriteLine("Enter the updated possible answers:");
                    for (int i = 0; i < question.PossibleAnswer.Count; i++)
                    {
                        Console.WriteLine($"Answer {i + 1}:");
                        question.PossibleAnswer[i] = Console.ReadLine();
                    }
                    break;
                case 3:
                    Console.WriteLine("Enter the updated right answer index (1-based):");
                    int newAnswerIndex;
                    while (!int.TryParse(Console.ReadLine(), out newAnswerIndex) || newAnswerIndex < 1 || newAnswerIndex > question.PossibleAnswer.Count)
                    {
                        Console.WriteLine($"Invalid input. Please enter a number between 1 and {question.PossibleAnswer.Count}.");
                    }
                    question.RightAnswer = newAnswerIndex;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        private int GetValidatedInput(string prompt, int[] validOptions)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if (int.TryParse(Console.ReadLine(), out int input) && validOptions.Contains(input))
                {
                    return input;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }
        }
        private void DisplayQuizs(List<Quiz> allQuiz,bool showCorrectAnswer = true)
        {
            foreach (var quiz in allQuiz)
            {
                Console.WriteLine($"Quiz ID: {quiz.Id}, User ID: {quiz.UserId}");

                if (quiz.QuizQuestions == null || !quiz.QuizQuestions.Any())
                {
                    Console.WriteLine("This quiz has no questions.\n");
                    continue;
                }

                foreach (var q in quiz.QuizQuestions)
                {
                    Console.WriteLine($"Question: {q.Question}");
                    Console.WriteLine("Possible Answers:");

                    for (int i = 0; i < q.PossibleAnswer.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {q.PossibleAnswer[i]}");
                    }
                    if (showCorrectAnswer)
                    {
                        Console.WriteLine($"Correct Answer: {q.RightAnswer}");
                    }
                }
            }

        }
        private void DisplayQuizWithoutAnswer(Quiz quiz,string username)
        {
            Console.WriteLine("You have 2 minutes to complete the quiz. Timer starts now!");
            var startTime = DateTime.Now;
            var timeLimit = TimeSpan.FromMinutes(2);
            var score = 0;
            foreach (var q in quiz.QuizQuestions)
            {
                CheckTime(startTime,timeLimit,score);
                UpdatingScores(q, score,username);
                CheckTime(startTime, timeLimit,score);
            }
        }
        private void CheckTime(DateTime startTime, TimeSpan timeLimit,int score)
        {
            if (DateTime.Now - startTime > timeLimit)
            {
                Console.WriteLine("Time's up! You could not complete the quiz in 2 minutes.");
                Console.WriteLine($"Final Score: {score}");
                return;
            }

        }
        private void UpdatingScores(PossibleAnswers q,int score,string username)
        {
            Console.WriteLine($"Question: {q.Question}");
            Console.WriteLine("Possible Answers:");
            for (int i = 0; i < q.PossibleAnswer.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {q.PossibleAnswer[i]}");
            }
            while (true)
            {
                Console.Write("Enter your answer: ");
                if (int.TryParse(Console.ReadLine(), out int answer) && answer > 0 && answer <= q.PossibleAnswer.Count)
                {
                    if (answer == q.RightAnswer)
                    {
                        Console.WriteLine("Thats correct, You just got +20 points");
                        score += 20;
                        var existingUser = _accountJsonRepository.GetUserByUsername(username);
                        if (score > existingUser.MaxScore)
                        {
                            existingUser.MaxScore = score;
                            _accountJsonRepository.Update(existingUser);
                        }
                    }
                    else
                    {
                        score -= 20;
                        Console.WriteLine("Incorrect. You just lost 20 points");
                    }
                    break;
                }
                else Console.WriteLine("Invalid input. Please try again");
            }
        }
        private int ValidationsForSolvingQuiz(List<Quiz> allQuiz, string username)
        {
            while (true)
            {
                Console.WriteLine("1.solve quiz \n2.exit");
                if (int.TryParse(Console.ReadLine(), out int exit) && (exit == 1 || exit == 2))
                {
                    if (exit == 2)
                    {
                        Console.WriteLine("You exited");
                        return -1;
                    }
                    if (exit == 1)
                    {
                        Console.WriteLine("Please enter ID of the quiz you want to solve, except of yours");
                        if (int.TryParse(Console.ReadLine(), out int input))
                        {
                            var existingQuiz = _quizJsonRepository.GetQuiz(input);
                            if (existingQuiz == null)
                            {
                                Console.WriteLine("Not Found.");
                            }
                            else if (existingQuiz != null)
                            {
                                var existingUsersQuiz = _accountJsonRepository.GetUserByUsername(username);
                                if (existingUsersQuiz.QuizIds.Contains(existingQuiz.Id))
                                {
                                    Console.WriteLine("You have no permission to solve your quiz");
                                }
                                else
                                {
                                    DisplayQuizWithoutAnswer(existingQuiz, username);
                                    return input;
                                }
                            }
                            else
                                Console.WriteLine("Invalid input. Please try again.");
                        }
                    }
                    Console.WriteLine("Invalid input. Please choose 1 or 2.");
                }

            }
        }

        private void DisplayQuizs(Quiz quiz,bool showCorrectAnswer = true)
        {
            foreach (var q in quiz.QuizQuestions)
            {
                Console.WriteLine($"Question: {q.Question}");
                Console.WriteLine("Possible Answers:");

                for (int i = 0; i < q.PossibleAnswer.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {q.PossibleAnswer[i]}");
                }
                if(showCorrectAnswer)
                    Console.WriteLine($"Correct Answer: {q.RightAnswer}");
            }
        }
        private void SavingDeletionToJson(User user, int quizId)
        {
            _quizJsonRepository.Delete(quizId);
            user.QuizIds.Remove(quizId);
            _accountJsonRepository.Update(user);
            Console.WriteLine($"Quiz with ID {quizId} has been successfully deleted.");

        }
        private int ValidationsForDelete(User user)
        {
            Console.WriteLine("Please enter id of Quiz you want to delete");
            if (!int.TryParse(Console.ReadLine(), out int quizId))
            {
                Console.WriteLine("Invalid input. Please enter a valid numeric Quiz ID.");
                return -1;
            }
            var quiz = _quizJsonRepository.GetQuiz(quizId);
            if (quiz == null)
            {
                Console.WriteLine("This id is not found and cant be deleted");
                return -1;
            }
            if (!user.QuizIds.Contains(quizId))
            {
                Console.WriteLine("You do not have permission to delete this quiz.");
                return -1;
            }
            else return quizId;
        }
        private int ValidationsForUpdate(User user)
        {
            while (true) {
                Console.WriteLine("Enter Id of the quiz you want to update");
                if (!int.TryParse(Console.ReadLine(), out int quizId))
                {
                    Console.WriteLine("Invalid input. Please enter a valid numeric Quiz ID.");
                    continue;
                }
                var quiz = _quizJsonRepository.GetQuiz(quizId);
                if (quiz == null)
                {
                    Console.WriteLine("This id is not found and cant be updated");
                    continue;
                }
                if (!user.QuizIds.Contains(quizId))
                {
                    Console.WriteLine("You do not have permission to update this quiz.");
                    continue;
                }
                return quizId;
            } }
        private void TextInConsoleBeforeAction(string username)
        {
            while (true)
            {
                Console.WriteLine("Main: \n" +
                    "1.Create Quiz \n" +
                    "2.Update Quiz \n" +
                    "3.Delete Quiz \n" +
                    "4.Solving Quiz \n" +
                    "5.Get all quizs \n"+
                    "6.exit");
                ChoosingAction(username);
            }
        }
        private void ChoosingAction(string username)
        {
            Console.WriteLine("Please enter the number of the action:");
            if (int.TryParse(Console.ReadLine(), out int chosenNumber) && chosenNumber >= 1 && chosenNumber <= 6)
            {
                switch (chosenNumber)
                {
                    case 1:
                        CreateQuiz(username);
                        break;
                    case 2:
                        UpdateQuiz(username);
                        break;
                    case 3:
                        DeleteQuiz(username);
                        break;
                    case 4:
                        SolvingQuiz(username);
                        break;
                    case 5:
                        GetAllQuizs();
                        break;
                    case 6:

                        Console.WriteLine("You just exited. Goodbye!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
            }
        }
        private void CreatingInJson(User user, Quiz quiz)
        {
            _quizJsonRepository.Create(quiz);
            var existingQuiz = _quizJsonRepository.LoadData();
            user.QuizIds.Add(quiz.Id);
            _accountJsonRepository.Update(user);
            Console.WriteLine($"Quiz successfully created with ID: {quiz.Id}");
        }
        private void CreatingEachQuestions(Quiz quiz)
        {
            int nextId = quiz.QuizQuestions.Count > 0
                ? quiz.QuizQuestions.Max(q => q.Id) + 1
                : 1;
            for (int i = 1; i <= 5; i++)
            {
                PossibleAnswers possAnsw = new PossibleAnswers
                {
                    Id = nextId++,

                    PossibleAnswer = new List<string>()
                };
                Console.WriteLine($"Please enter your {i} question");
                possAnsw.Question = Console.ReadLine();
                Console.WriteLine("Please write possible answers");
                for (int j = 1; j <= 4; j++)
                {
                    Console.WriteLine($"Answer {j}: ");
                    var answers = Console.ReadLine();
                    possAnsw.PossibleAnswer.Add(answers);
                }
                Console.WriteLine("Now write which one is the correct answer (1-4)");
                int answerIndex;
                while (!int.TryParse(Console.ReadLine(), out answerIndex) || answerIndex < 1 || answerIndex > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                }
                possAnsw.RightAnswer = answerIndex;
                quiz.QuizQuestions.Add(possAnsw);
            }
        }
    }
}