using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Admin : IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
