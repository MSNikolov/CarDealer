﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class SaleDto
    {
        public decimal Discount { get; set; }

        public int CarId { get; set; }

        public int CustomerId { get; set; }
    }
}
