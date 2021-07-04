namespace DiverHotel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReservationGuest")]
    public partial class ReservationGuest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Country { get; set; }

        public int Adults { get; set; }

        public int? Children { get; set; }

        public int? RoomType { get; set; }

        public int? MealPlan { get; set; }

        [Column(TypeName = "date")]
        public DateTime CheckIn { get; set; }

        [Column(TypeName = "date")]
        public DateTime Checkout { get; set; }

        public int TotalPrice { get; set; }

        public virtual MealPlan MealPlan1 { get; set; }

        public virtual Type Type { get; set; }
    }
}
