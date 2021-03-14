using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LisergyServer.Database
{
    public class ServicesContext : DbContext
    {
        public DbSet<GameServiceEntity> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={ Path.Combine(Directory.GetCurrentDirectory(), "Data\\services.db")}");
    }
}
