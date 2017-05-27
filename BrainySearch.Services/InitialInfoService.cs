﻿namespace BrainySearch.Services
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
        List<InitialInfo> GetAllByLectureIds(Guid[] lectureIds);

        List<InitialInfo> GetAllByLectureId(Guid lectureId);
    }

    public class InitialInfoService : BaseService<InitialInfo>, IInitialInfoService
    {
        public InitialInfoService(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
        
        public List<InitialInfo> GetAllByLectureIds(Guid[] lectureIds)
        {
            return applicationDbContext.InitialInfos.Where(item => lectureIds.Contains(item.LectureId)).ToList();
        }

        public List<InitialInfo> GetAllByLectureId(Guid lectureId)
        {
            return applicationDbContext.InitialInfos.Where(item => item.LectureId == lectureId).ToList();
        }
    }
}
