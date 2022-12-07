using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OilCaseApi.resources;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OilCaseApi.Models;

public class ApplicationContext : IdentityDbContext<User, IdentityRole, string>
{
    public static string? ConnectionString;
    public virtual DbSet<Cell> Cells { get; set; }

    public virtual DbSet<LithologicalModel> LithologicalModel { get; set; }
    public virtual DbSet<LogName> LogNames { get; set; }
    public virtual DbSet<BoreholeLog> BoreholeLogs { get; set; }
    public virtual DbSet<Seismic> Seismic { get; set; }

    public virtual DbSet<TrajectoryPoint> TrajectoryPoints { get; set; }
    public virtual DbSet<BoreholeStatus> BoreholeStatus { get; set; }
    public virtual DbSet<BoreholeStatusHistory> BoreholeStatusHistories { get; set; }
    public virtual DbSet<ObjectOfArrangement> ObjectsOfArrangement { get; set; }
    
    public virtual DbSet<PurchasedBoreholeExploration> PurchasedBoreholeExplorations { get; set; }
    public virtual DbSet<PurchasedBoreholeProduction> PurchasedBoreholeProductions{ get; set; }
    public virtual DbSet<PurchasedObjectOfArrangement> PurchasedObject { get; set; }
    public virtual DbSet<PurchasedSeismic> PurchasedSeismic { get; set; }
    public virtual DbSet<PurchasedLogName> PurchasedLogName { get; set; }
    public virtual DbSet<WellTop> WellTops{ get; set; }


    public virtual DbSet<Team> Team { get; set; }
    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<TeamActionLogs> TeamActionLogs { get; set; }


    public ApplicationContext()
    {
        Database.EnsureCreated();
    }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Team>().HasIndex(x => x.Name).IsUnique();

        builder.Entity<BoreholeLog>()
            .HasKey(x => x.Id).IsClustered(false);

        builder.Entity<BoreholeLog>()
            .HasIndex(v => v.CellId)
            .IsClustered();

        builder.Entity<BoreholeLog>()
            .HasOne(typeof(LogName), "LogName")
            .WithMany()
            .HasForeignKey("LogNameId")
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<PurchasedBoreholeExploration>()
            .HasOne(typeof(Team), "Team")
            .WithMany()
            .HasForeignKey("TeamId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<PurchasedBoreholeProduction>()
            .HasOne(typeof(Team), "Team")
            .WithMany()
            .HasForeignKey("TeamId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Seismic>()
            .HasKey(x => x.Id).IsClustered(false);

        builder.Entity<Seismic>()
            .HasIndex(v => v.LithologicalModelId)
            .IsClustered();

        builder.Entity<PurchasedObjectOfArrangement>()
            .HasKey(x => x.Id).IsClustered(false);

        builder.Entity<PurchasedObjectOfArrangement>()
            .HasIndex(v => v.TeamId)
            .IsClustered();

        builder.Entity<TrajectoryPoint>()
            .HasKey(x => x.Id).IsClustered(false);

        builder.Entity<TrajectoryPoint>()
            .HasIndex(v => v.PurchasedBoreholeId)
            .IsClustered();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (ConnectionString != null) optionsBuilder.UseSqlServer(ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
}