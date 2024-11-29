using Domain;
using Domain.DTO;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    internal interface IUserInterface
    {
        public void RegisterUserUsingConsole();
        public void LoginUserUsingConsole();
        public void DisplayTopTenUsers();
        public void addUserTojson(RegisterUser registerUser);
    }
}
