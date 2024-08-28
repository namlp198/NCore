using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReadCodeGUI.Models
{
    public class DbTestContext : DbContext
    {
        public DbSet<ExcelTemplateModel> ExcelTemplate { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DbTest.db");
        }
    }
}
