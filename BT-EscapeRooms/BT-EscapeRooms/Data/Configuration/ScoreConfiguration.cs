using BT_EscapeRooms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Data.Configuration
{
    public static class ScoreConfiguration
    {
        public static void Configure(this EntityTypeBuilder<Score> entity)
        {
            
            entity.ToTable("Scores");

            // ID
            entity.HasKey(t => t.ID);
            entity.HasIndex(t => t.Username);

            // String length
            entity.Property(t => t.Username).HasMaxLength(50);

            // Required
            entity.Property(t => t.Username).IsRequired();
            entity.Property(t => t.Date).IsRequired();
            entity.Property(t => t.Difficulty).IsRequired();
            entity.Property(t => t.Points).IsRequired();
        }
    }
}
