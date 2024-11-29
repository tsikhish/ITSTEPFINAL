using Domain.DTO;
using Domain;
using Repository;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserInterface
    {
        private readonly AccountJsonRepository _jsonRepository;
        private readonly QuizJsonRepository _quizJsonRepository;
        public UserService(AccountJsonRepository jsonRepository, QuizJsonRepository quizJsonRepository)
        {
            _jsonRepository = jsonRepository;
            _quizJsonRepository = quizJsonRepository;
        }
        public void DisplayTopTenUsers()
        {
            var topUsers = _jsonRepository.TopTenUsers();
            Console.WriteLine("Top 10 Users:");
            foreach (var user in topUsers)
            {
                Console.WriteLine($"{user.FirstName} {user.LastName} ({user.Username}) - Max Score: {user.MaxScore}");
            }
        }
        public void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("Choose one: \n1. Register \n2. Login \n3. Exit");
                if (int.TryParse(Console.ReadLine(), out int chosenNumber) && chosenNumber >= 1 && chosenNumber <= 3)
                {
                    switch (chosenNumber)
                    {
                        case 1:
                            RegisterUserUsingConsole();
                            break;
                        case 2:
                            LoginUserUsingConsole();
                            break;
                        case 3:
                            Console.WriteLine("You exited.");
                            return; 
                        default:
                            Console.WriteLine("Try again");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please choose a number between 1 and 3.");
                }
            }

        }
        public void RegisterUserUsingConsole()
        {
            Console.WriteLine("This is register page");
            Console.WriteLine("Please enter your firstname: ");
            string firstname = Console.ReadLine();
            Console.WriteLine("Please enter your lastname: ");
            string lastname = Console.ReadLine();
            Console.WriteLine("Please enter your username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter your password: ");
            string password = Console.ReadLine();
            Console.WriteLine("Please enter your age: ");
            int age = int.Parse(Console.ReadLine());
            var checkUsername = _jsonRepository.GetUserByUsername(username);
            if (checkUsername != null)
            {
                Console.WriteLine("This username already exists.");
            }
            else
            {
                RegisterUser registerUser = new RegisterUser()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Password = password,
                    UserName = username,
                    Age = age
                };
                addUserTojson(registerUser);
                Console.WriteLine("Now you can login");
                LoginUserUsingConsole();
            }
        }
        public void LoginUserUsingConsole()
        {
            while (true)
            {
                Console.WriteLine("Please enter your username: ");
                var username = Console.ReadLine();
                var checkUsername = _jsonRepository.GetUserByUsername(username);

                if (checkUsername == null)
                {
                    Console.WriteLine("This username doesn't exist. Please try again.");
                }
                else
                {
                    Console.WriteLine($"{username} successfully logged in.");
                    QuizService quiz = new QuizService(_jsonRepository, _quizJsonRepository);
                    quiz.Main(username);
                    return;
                }
            }
        }
        public void addUserTojson(RegisterUser registerUser)
        {
            if (registerUser != null)
            {
                User user = new User()
                {
                    FirstName = registerUser.FirstName,
                    LastName = registerUser.LastName,
                    Password = registerUser.Password,
                    Username = registerUser.UserName,
                    Age = registerUser.Age,
                };
                _jsonRepository.Create(user);
            }
            else Console.WriteLine("Somethings wrong, please try again.");
        }

    }
}
