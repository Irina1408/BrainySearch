using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Data.Models
{
    public class Word
    {
        public Word()
        {
            Lectures = new List<Lecture>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required, Column("Word")]
        public string WordText { get; set; }

        public virtual ICollection<Lecture> Lectures { get; set; }
    }
}
