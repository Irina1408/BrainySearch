using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Data.Models
{
    public class Lecture
    {
        public Lecture()
        {
            KeyWords = new List<Word>();
            InitialInfos = new List<InitialInfo>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Theme { get; set; }

        public string Text { get; set; }

        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Word> KeyWords { get; set; }

        public virtual ICollection<InitialInfo> InitialInfos { get; set; }
    }
}
