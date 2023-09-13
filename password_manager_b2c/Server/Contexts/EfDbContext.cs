using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using password_manager_b2c.Shared;

namespace password_manager_b2c.Server.Contexts;

public partial class EfDbContext : DbContext
{
    private readonly IConfiguration configuration;

    public EfDbContext()
    {
    }

    public EfDbContext(DbContextOptions<EfDbContext> options, IConfiguration configuration)
        : base(options)
    {
        this.configuration = configuration;
    }


    public virtual DbSet<PasswordmanagerAccount> PasswordmanagerAccounts { get; set; }

    // parse the elephantSQL provided string into ASP.net core friendly connection string
    private string getConnectionString()
    {
        // ElephantSQL formatting
        var uriString = configuration.GetConnectionString("cloudConnectionString")!;
        var uri = new Uri(uriString);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
            uri.Host, db, user, passwd, port);
        return connStr;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(getConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("btree_gin")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("citext")
            .HasPostgresExtension("cube")
            .HasPostgresExtension("dblink")
            .HasPostgresExtension("dict_int")
            .HasPostgresExtension("dict_xsyn")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("fuzzystrmatch")
            .HasPostgresExtension("hstore")
            .HasPostgresExtension("intarray")
            .HasPostgresExtension("ltree")
            .HasPostgresExtension("pg_stat_statements")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("pgrowlocks")
            .HasPostgresExtension("pgstattuple")
            .HasPostgresExtension("tablefunc")
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("uuid-ossp")
            .HasPostgresExtension("xml2");

        modelBuilder.Entity<PasswordmanagerAccount>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Userid }).HasName("passwordmanager_accounts_pkey");

            entity.ToTable("passwordmanager_accounts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.LastUpdatedAt).HasColumnName("last_updated_at");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
