using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreBackend
{
    public class UserService
    {
        Dictionary<string, IUser> LoggedInMembers;
        static BookstoreDbContext DbContext; // Maybe using block? DEFINITELY USING BLCOK

        private string hashToken(string username)
        {
            using(SHA256 sha256 = SHA256.Create())
            {
                var computedHash = sha256.ComputeHash(
                Encoding.Unicode.GetBytes(username + "biber"));
                return Convert.ToBase64String(computedHash);
            }
        }

        public UserService()
        {
            LoggedInMembers = new Dictionary<string, IUser>();
        }

        public bool IsMemberLoggedIn(string token)
        {
            return LoggedInMembers.ContainsKey(token);
        }
        public bool IsMemberAdmin(IUser member)
        {
            return DbContext.Admins.FirstOrDefault(m => m.Username == member.Username) != null;
        }

        public string LoginUser(string username, string password)
        {
            IUser user = DbContext.Admins.FirstOrDefault(m => m.Username == username);
            if (user != null)
            {
                if (user.Password == password)
                {
                    string token = hashToken(user.Username);
                    if (IsMemberLoggedIn(token)) throw new Exception("User already logged in!");
                    LoggedInMembers[token] = user;
                    return token;
                } else throw new Exception("Wrong Password!");
            } throw new Exception("Wrong Username!");
        }
    }
}
