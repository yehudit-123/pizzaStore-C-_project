using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class PizzaStoreContext : DbContext
{
    public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options)
        : base(options) { }

    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Pizza> Pizzas { get; set; }
    public virtual DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId).HasColumnName("custumerId");
            entity.Property(e => e.Gender).HasColumnName("gender").HasMaxLength(10);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(20);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(10);
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(50);
        });

        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.HasKey(e => e.PizzaId);
            entity.Property(e => e.PizzaId).HasColumnName("pizzaId");
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(10);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatingDate)
                  .HasColumnName("creatingDate")
                  .HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CustomerId).HasColumnName("custumerId");
            entity.Property(e => e.PizzaId).HasColumnName("pizzaId");
            entity.Property(e => e.Feedback).HasColumnName("feedback").HasMaxLength(50);
            entity.Property(e => e.IsClose).HasColumnName("isClose");
            entity.Property(e => e.ReminderDate).HasColumnName("ReminderDate");
            entity.Property(e => e.ReminderSent).HasColumnName("reminderSent");

            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId);

            entity.HasOne(o => o.Pizza)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(o => o.PizzaId);
        });
    }
}