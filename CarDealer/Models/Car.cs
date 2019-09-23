using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CarDealer.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int TravelledDistance { get; set; }

        public HashSet<PartCar> Parts { get; set; } = new HashSet<PartCar>();

        public HashSet<Sale> Sales { get; set; } = new HashSet<Sale>();

        public decimal Price => this.Parts.Sum(p => p.Part.Price);
    }
}
