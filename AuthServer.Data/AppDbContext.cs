using AuthServer.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    public class AppDbContext:IdentityDbContext<AppUser,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }


        // Entitylere özellik atamak için (Required, MaxValue gibi) OnModelCreating metodunda tanımlıyoruz. Her entity için ayrı Configuration dosyaları oluşturduk
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }

    }
}


/*
 
 
 */