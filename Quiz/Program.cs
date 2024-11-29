using Domain;
using Domain.DTO;
using Repository;
using Services;
using System.Text.Json;
class Program
{
    static void Main(string[] args)
    {
        string accountFilePath = @"C:\Users\Mari\source\repos\Quiz\Quiz\userData.json";
        string quizFilePath = @"C:\Users\Mari\source\repos\Quiz\Quiz\quizData.json";
        var account = new AccountJsonRepository(accountFilePath);
        var quiz = new QuizJsonRepository(quizFilePath);
        var userService = new UserService(account, quiz);
        userService.MainMenu();
    }
}