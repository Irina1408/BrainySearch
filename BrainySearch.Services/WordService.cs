namespace BrainySearch.Services
{
    using Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWordService : IBaseService<Word>
    {

    }

    public class WordService : BaseService<Word>, IWordService
    {
        public WordService(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
