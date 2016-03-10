using System;
using System.Data.Entity;
using EffinghamDataAccess;

namespace DbBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing database! Hold on to your butts!");

            Database.SetInitializer<DataAccessContext>(new DataAccessInitializer());

            using (var database = new DataAccessContext())
            {
                database.Database.Initialize(true);
            }

            Console.WriteLine("Let go of your butts!");
        }
    }
}
