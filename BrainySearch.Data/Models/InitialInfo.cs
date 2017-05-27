using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Data.Models
{
    public class InitialInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string SourceLink { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public Guid LectureId { get; set; }

        [ForeignKey(nameof(LectureId))]
        public virtual Lecture Lecture { get; set; }
    }
}
