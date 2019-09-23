using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarDealer.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public decimal Discount { get; set; }

        public int CarId { get; set; }

        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }

        public int CustomerId { get; set; }


        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
    }
}
