using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Author
    {
        private string shortDesc;
        private string firstName;
        private string lastName;

        [Key]
        public int AuthorId { get; set; }
        public string ShortDesc { get => shortDesc; set => shortDesc = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
    }
}
