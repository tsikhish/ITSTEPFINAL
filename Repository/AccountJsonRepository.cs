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
    public class AccountJsonRepository
    {
        private readonly string _filePath;
        private readonly List<User> _users;
        public AccountJsonRepository(string filePath)
        {
            _filePath = filePath;
            _users = LoadData();
        }
        public List<User> TopTenUsers()
        {
            var topUsers = _users
                     .OrderByDescending(x => x.MaxScore) 
                     .Take(10)  
                     .ToList();  
            return topUsers;
        }
        public User GetUserByUsername(string username)
        {
            var existingUsername = _users.FirstOrDefault(x => x.Username == username);
            if (existingUsername != null) return existingUsername;
            return null;
        }
        public void Create(User user)
        {
            user.Id = _users.Any() ? _users.Max(account => account.Id) + 1 : 1;
            _users.Add(user);
            SaveData();
        }
        
        public void Update(User user)
        {
            var index = _users.FindIndex(x => x.Id == user.Id);
            if (index >= 0)
            {
                _users[index] = user;
                SaveData();
            }
        }
        private void SaveData()
        {
            string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public List<User> LoadData()
        {
            var jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData);

        }
    }
}
