namespace BrainySearch.Services
{
    using Data;
    using Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IKeyWordService : IBaseService<KeyWord>
    {
        List<KeyWord> GetAllByLectureIds(Guid[] lectureIds);

        List<KeyWord> GetAllByLectureId(Guid lectureId);
    }

    public class KeyWordService : BaseService<KeyWord>, IKeyWordService
    {
        public KeyWordService(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public List<KeyWord> GetAllByLectureIds(Guid[] lectureIds)
        {
            return applicationDbContext.KeyWords.Where(item => lectureIds.Contains(item.LectureId)).ToList();
        }

        public List<KeyWord> GetAllByLectureId(Guid lectureId)
        {
            return applicationDbContext.KeyWords.Where(item => item.LectureId == lectureId).ToList();
        }
    }
}
