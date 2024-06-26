﻿using AuthServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x=>x.Price).HasColumnType("decimal(18,2)");
            builder.Property(x=>x.UserId).IsRequired();
        }
    }
}

/*
 
Configurations'larda kullanıcıya bir measj dönülmez. Burada tanımlanan özellikler veritabanı için oluşturulur. Configurations ile belirlediğin özellikleri FluentValidation ile kullanıcıya mesajı verilir. 

 */