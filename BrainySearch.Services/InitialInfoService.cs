namespace BrainySearch.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BrainySearch.Data.Models;
    using Data;

    public interface IInitialInfoService : IBaseService<InitialInfo>
    {

    }

    public class InitialInfoService : BaseService<InitialInfo>, IInitialInfoService
    {
        public InitialInfoService(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
