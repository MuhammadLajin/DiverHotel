namespace DiverHotel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ReservationMealPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int MealID { get; set; }

        public int StartMonth { get; set; }

        public int EndMonth { get; set; }

        public int Price { get; set; }

        public virtual MealPlan MealPlan { get; set; }
    }
}
