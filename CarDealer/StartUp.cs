using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new CarDealerContext();

            Mapper.Initialize(cfg => cfg.AddProfile(new CarDealerProfile()));

            var supplierReader = new StreamReader(@"C:\Users\Miroslav\source\repos\CarDealer\CarDealer\Datasets\suppliers.json");

            var suppliers = supplierReader.ReadToEnd();

            var reader = new StreamReader(@"C:\Users\Miroslav\source\repos\CarDealer\CarDealer\Datasets\parts.json");

            var inputJson = reader.ReadToEnd();

            var carReader = new StreamReader(@"C:\Users\Miroslav\source\repos\CarDealer\CarDealer\Datasets\cars.json");

            var cars = carReader.ReadToEnd();

            var customerReader = new StreamReader(@"C:\Users\Miroslav\source\repos\CarDealer\CarDealer\Datasets\customers.json");

            var customers = customerReader.ReadToEnd();

            var salesReader = new StreamReader(@"C:\Users\Miroslav\source\repos\CarDealer\CarDealer\Datasets\sales.json");

            var sales = salesReader.ReadToEnd();

            //Console.WriteLine(ImportSuppliers(context, suppliers)); 

            //Console.WriteLine(ImportParts(context, inputJson));

            //Console.WriteLine(ImportCars(context, cars));

            //Console.WriteLine(ImportCustomers(context, customers));

            //Console.WriteLine(ImportSales(context, sales));

            //Console.WriteLine(GetOrderedCustomers(context));

            //Console.WriteLine(GetCarsFromMakeToyota(context));

            //Console.WriteLine(GetLocalSuppliers(context));

            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //Console.WriteLine(GetTotalSalesByCustomer(context));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersDto = JsonConvert.DeserializeObject<List<SupplierDto>>(inputJson);            

            var suppliers = Mapper.Map<List<Supplier>>(suppliersDto);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var partsDto = JsonConvert.DeserializeObject<List<PartDto>>(inputJson);

            var parts = Mapper.Map<List<Part>>(partsDto);

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDto = JsonConvert.DeserializeObject<List<CarDto>>(inputJson);

            var cars = Mapper.Map<List<Car>>(carsDto);

            context.Cars.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customersDto = JsonConvert.DeserializeObject<List<CustomerDto>>(inputJson);

            var customers = Mapper.Map<List<Customer>>(customersDto);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var salesDto = JsonConvert.DeserializeObject<List<SaleDto>>(inputJson);

            var sales = Mapper.Map<List<Sale>>(salesDto);

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .ToList();

            var customersJson = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return customersJson;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotas = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToList();

            var toyotasJson = JsonConvert.SerializeObject(toyotas, Formatting.Indented);

            return toyotasJson;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var lokalSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var json = JsonConvert.SerializeObject(lokalSuppliers, Formatting.Indented);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .Include(c => c.Parts)
                .Select(c => new
                {
                    c.Make,
                    c.Model,
                    c.TravelledDistance,
                    parts = c.Parts.Select(p => new
                    {
                        p.Part.Name,
                        Price = Math.Round(p.Part.Price, 2)
                    })
                })
                .ToList();

            var json = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Include(c => c.Sales)
                .Select(c => new
                {
                    c.Name,
                    Cars = c.Sales.Count(),
                    SpentMoney = c.Sales.Sum(s => s.Car.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.Cars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Where(s => s.Discount != 0)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount,
                    price = s.Car.Price,
                    priceWithDiscount = s.Car.Price * (1 - s.Discount / 100)
                })
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }
    }
}
