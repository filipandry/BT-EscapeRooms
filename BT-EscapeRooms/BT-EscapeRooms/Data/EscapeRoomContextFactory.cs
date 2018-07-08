using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Data
{
    class EscapeRoomContextFactory : IDesignTimeDbContextFactory<EscapeRoomContext>
    {
        public IConfigurationRoot Configuration { get; }

        public EscapeRoomContextFactory()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public EscapeRoomContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EscapeRoomContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            return new EscapeRoomContext(optionsBuilder.Options);
        }

        public EscapeRoomContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EscapeRoomContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            return new EscapeRoomContext(optionsBuilder.Options);
        }

        public EscapeRoomContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EscapeRoomContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            return new EscapeRoomContext(optionsBuilder.Options);
        }
    }
}
