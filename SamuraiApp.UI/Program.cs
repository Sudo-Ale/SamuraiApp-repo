using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        private static void Main(string[] args)
        {
            //AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Neri");
            //AddSamuraisByConsole();
            //AddVariousTypes();

            //ReadOnly
            GetSamurais();
            GetBattles();
            QueryFilters();
            QueryAggregates();
            QueryAndUpdateBattles_Disconnected();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
        private static void AddSamuraisByName(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name });
            }
            _context.SaveChanges();
        }
        private static void AddSamuraisByConsole() //fuori corso
        {
            Console.WriteLine("Add a samurai by name");
            string samuraiName = Console.ReadLine();
            _context.Samurais.Add(new Samurai { Name = samuraiName });
            Console.WriteLine($"Adding samurai {samuraiName} to the database..");
            _context.SaveChanges();
        }
        private static void AddVariousTypes()   //Add multiple types
        {
            _context.AddRange(
                new Samurai { Name = "Giura" },
                new Samurai { Name = "Nanii" },
                new Battle { Name = "Battle of qwerty" },
                new Battle { Name = "Battle of kilo" }
                );
            _context.SaveChanges();
        }
        private static void GetSamurais() //Retrieve samurais count and names on console
        {
            var samurais = _context.Samurais
                .TagWith("ConsoleApp.Program.GetSamurais method")
                .ToList();

            Console.WriteLine($"Samurai count is {samurais.Count()}");
            foreach(var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
        private static void GetBattles() //Retrieve battles count and names on console
        {
            var battles = _context.Battles
                .TagWith("ConsoleApp.Program.GetBattles method")
                .ToList();

            Console.WriteLine($"Battle count is {battles.Count()}");
            foreach(var battle in battles)
            {
                Console.WriteLine(battle.Name);
            }
        }
        private static void QueryFilters() //example filter n°1
        {
            var samurais = _context.Samurais.TagWith("ConsoleApp.Program.QueryFilters method")
                                            .Where(s => EF.Functions.Like(s.Name, "J%"))
                                            .ToList();
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
        private static void QueryAggregates() //example filter n°2
        {
            var name = "Sampson";
            var samurai = _context.Samurais.TagWith("ConsoleApp.Program.QueryAggregates method")
                                            .FirstOrDefault(s => s.Name == name);
        }
        private static void RetrieveAndUpdateSamurai() //example for update a samurai name
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }
        private static void RetrieveAndUpdateMultipleSamurais() //batch commands
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }
        private static void MultipleDatabaseOperations() 
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Shino" });
            _context.SaveChanges();
        }
        private static void RetrieveAndDeleteSamurai() 
        {
            var samurai = _context.Samurais.Find(10);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }
        private static void QueryAndUpdateBattles_Disconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = _context.Battles.ToList();
            }   //context1 is disposed now
            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 01, 01);
            });
            using (var context2 = new SamuraiContext()) 
            {
                context2.UpdateRange(disconnectedBattles);
                context2.SaveChanges();
            }
        }
    }
}