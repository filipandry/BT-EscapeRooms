using BT_EscapeRooms.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT_EscapeRooms.Data.Configuration;

namespace BT_EscapeRooms.Data
{
    public class EscapeRoomContext : DbContext
    {
        public EscapeRoomContext(DbContextOptions<EscapeRoomContext> options) : base(options)
        {
        }

        public DbSet<Score> Scores { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Score>().Configure();
        }
    }
}
