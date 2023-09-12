using ADO_EF.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_EF.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.Department> Departments { get; set; } = null!;
        public DbSet<Entity.Manager> Managers { get; set; } = null!;

        public DataContext() : base() {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(  // настройка подключения к БД из пакета SqlServer - драйверы MS SQL
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ado-ef;Integrated Security=True"
            );                            // строка для подключения - к несуществующей(или пустой) БД
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //налаштування самої бд - відношень (реляцій) між даними
            //їх обмеження (розміру) та унікальність, а також сідування
            //(від англ seed - зерно) - заповнення начальними даними
            modelBuilder.Entity<Manager>()
                .HasOne(m => m.MainDep)
                .WithMany()
                .HasForeignKey(m => m.IdMainDep)
                .HasPrincipalKey(d => d.Id);
        
            modelBuilder.Entity<Manager>()
                .HasOne(m => m.SecDep)
                .WithMany(d => d.MainManagers)
                .HasForeignKey(m => m.IdSecDep)
                .HasPrincipalKey(m => m.Id);

            modelBuilder.Entity<Manager>()
                .HasOne()
                .WithMany()
                .HasForeignKey(m => m.IdChief)
                .HasPrincipalKey();

            modelBuilder.Entity<Manager>()
                .HasOne(m => m.ChiefDep)
                .WithMany()
                .HasForeignKey(m => m.IdChief)
                .HasPrincipalKey(d => d.Id);

            modelBuilder.Entity<Manager>()
                .HasIndex(m => m.Login)
                .IsUnique();
        }
    }
}
