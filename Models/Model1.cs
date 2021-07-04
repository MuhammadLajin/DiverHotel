using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DiverHotel.Models
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<MealPlan> MealPlans { get; set; }
        public virtual DbSet<RateSeason> RateSeasons { get; set; }
        public virtual DbSet<ReservationGuest> ReservationGuests { get; set; }
        public virtual DbSet<ReservationMealPlan> ReservationMealPlans { get; set; }
        public virtual DbSet<ReservationRoom> ReservationRooms { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Type> Types { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MealPlan>()
                .HasMany(e => e.ReservationGuests)
                .WithOptional(e => e.MealPlan1)
                .HasForeignKey(e => e.MealPlan);

            modelBuilder.Entity<MealPlan>()
                .HasMany(e => e.ReservationMealPlans)
                .WithRequired(e => e.MealPlan)
                .HasForeignKey(e => e.MealID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RateSeason>()
                .HasMany(e => e.ReservationRooms)
                .WithRequired(e => e.RateSeason)
                .HasForeignKey(e => e.IdRate)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.Available)
                .IsFixedLength();

            modelBuilder.Entity<Room>()
                .HasMany(e => e.ReservationRooms)
                .WithRequired(e => e.Room)
                .HasForeignKey(e => e.IdRoom)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Type>()
                .HasMany(e => e.ReservationGuests)
                .WithOptional(e => e.Type)
                .HasForeignKey(e => e.RoomType);

            modelBuilder.Entity<Type>()
                .HasMany(e => e.Rooms)
                .WithRequired(e => e.Type)
                .HasForeignKey(e => e.RoomTypeID)
                .WillCascadeOnDelete(false);
        }
    }
}
