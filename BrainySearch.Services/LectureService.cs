namespace BrainySearch.Services
{
    using Data;
    using Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ILectureService : IBaseService<Lecture>
    {

    }

    public class LectureService : BaseService<Lecture>, ILectureService
    {
        public LectureService(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
