using BrainySearch.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Data
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System.Configuration;
    using System.Data.Entity;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString)
        {
        }

        public ApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<InitialInfo> InitialInfos { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<KeyWord> KeyWords { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
