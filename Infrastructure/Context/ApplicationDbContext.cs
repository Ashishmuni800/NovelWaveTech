﻿using Domain.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<PasswordChangeHistory> PasswordChangeHistory { get; set; }
        public DbSet<GenerateCaptchaCode> GenerateCaptchaCode { get; set; }
        public DbSet<AuthorizationData> AuthorizationData { get; set; }
    }
}
