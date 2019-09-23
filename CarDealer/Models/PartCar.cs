using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarDealer.Models
{
    public class PartCar
    {
        public int PartId { get; set; }

        [ForeignKey(nameof(PartId))]
        public Part Part { get; set; }

        public int CarId { get; set; }

        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }
    }
}
