using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Book : ICloneable
    {
        private string title;
        private int publishYear;

        [Key]
        public Guid BookId { get; set; } = Guid.NewGuid();
        public Guid Author { get; set; }
        public string Member { get; set; }
        public string Title { get => title; set => title = value; }
        public int PublishYear { get => publishYear; set => publishYear = value; }

        public ICloneable Clone()
        {
            Book clone = new Book();
            clone.Title = Title;
            clone.PublishYear = PublishYear;
            clone.Author = Author;
            clone.Member = Member;

            return clone;
        }
    }
}
