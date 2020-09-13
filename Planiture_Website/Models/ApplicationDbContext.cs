using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Planiture_Website.Controllers;
using Planiture_Website.Models;

namespace Planiture_Website.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Investment_Info> UserInvestment { get; set; }
        public DbSet<Account_Info> UserAccount { get; set; }
        public DbSet<CusTransaction> UserTransaction { get; set; }
        public DbSet<Feedback> UserFeedback { get; set; }
        public DbSet<ConfigFile> ConfigFiles { get; set; }
        public DbSet<Token> UserToken { get; set; }
        public DbSet<ActivePlans> ActivePlans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(p => p.MemberSince)
                .HasDefaultValueSql("getdate()");

            builder.Entity<CusTransaction>()
                .Property(p => p.Date)
                .HasDefaultValueSql("getdate()");

            builder.Entity<ActivePlans>()
                .Property(p => p.DateAdded)
                .HasDefaultValueSql("getdate()");

        }
    }
}
