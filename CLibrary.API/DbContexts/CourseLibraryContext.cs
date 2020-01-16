using System;
using CLibrary.API.Entities;
using CLibrary.API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CLibrary.API.DbContexts
{
    public class CourseLibraryContext : DbContext
    {
        public CourseLibraryContext(DbContextOptions options)
           : base(options)
        {}

        public DbSet<Author> Authors { get; set; }
        public DbSet<Course> Courses { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
            builder.AddFilter((category, lvl) => category == DbLoggerCategory.Database.Command.Name
                        && lvl == LogLevel.Information)
                    .AddProvider(new LoggerProvider()));
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed the database with dummy data
            modelBuilder.Entity<Author>().HasData(
                new Author()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    FirstName = "Berry",
                    LastName = "Griffin Beak Eldritch",
                    DateOfBirth = new DateTime(1650, 7, 23),
                    MainCategory = "Ships"
                },
                new Author()
                {
                    Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                    FirstName = "Nancy",
                    LastName = "Swashbuckler Rye",
                    DateOfBirth = new DateTime(1668, 5, 21),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                    FirstName = "Eli",
                    LastName = "Ivory Bones Sweet",
                    DateOfBirth = new DateTime(1701, 12, 16),
                    MainCategory = "Singing"
                },
                new Author()
                {
                    Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                    FirstName = "Arnold",
                    LastName = "The Unseen Stafford",
                    DateOfBirth = new DateTime(1702, 3, 6),
                    MainCategory = "Singing"
                },
                new Author()
                {
                    Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                    FirstName = "Seabury",
                    LastName = "Toxic Reyson",
                    DateOfBirth = new DateTime(1690, 11, 23),
                    MainCategory = "Maps"
                },
                new Author()
                {
                    Id = Guid.Parse("2aadd2df-7caf-45ab-9355-7f6332985a87"),
                    FirstName = "Rutherford",
                    LastName = "Fearless Cloven",
                    DateOfBirth = new DateTime(1723, 4, 5),
                    MainCategory = "General debauchery"
                },
                new Author()
                {
                    Id = Guid.Parse("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"),
                    FirstName = "Atherton",
                    LastName = "Crow Ridley",
                    DateOfBirth = new DateTime(1721, 10, 11),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("9245fe4a-d402-451c-b9ed-9c1a04247482"),
                    FirstName = "Jason",
                    LastName = "Netherthon",
                    DateOfBirth = new DateTime(1971, 10, 11),
                    MainCategory = "Play"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("7be89ge4-fyk6-4f73-7511-3yb25b6ca33f"),
                //     FirstName = "Jari",
                //     LastName = "Maenpaa",
                //     DateOfBirth = new DateTime(1976, 11, 3),
                //     MainCategory = "Singing"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("5ddag2df-7caf-34tb-9355-1q7832985b98"),
                //     FirstName = "Adam",
                //     LastName = "Fearless Cloven",
                //     DateOfBirth = new DateTime(1983, 4, 5),
                //     MainCategory = "General debauchery"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("ne2ft609-t457-4mar-8dca-z4f9vv13po69"),
                //     FirstName = "Andy",
                //     LastName = "Ryezgorhf",
                //     DateOfBirth = new DateTime(1668, 5, 21),
                //     MainCategory = "Rum"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("1236b666-1488-4c70-9815-f6t2d9172450"),
                //     FirstName = "Ivar",
                //     LastName = "The Boneless",
                //     DateOfBirth = new DateTime(1701, 12, 16),
                //     MainCategory = "General Debauchery"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("5idgaf2f-7cat-54yb-2724-1q7454985b23"),
                //     FirstName = "Hvitserk",
                //     LastName = "Lothbrok",
                //     DateOfBirth = new DateTime(1563, 12, 22),
                //     MainCategory = "Boozing"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("ad5tf948-t346-4jan-8fgr-z4f9ol13ss69"),
                //     FirstName = "Mickal",
                //     LastName = "Rzepsewsky",
                //     DateOfBirth = new DateTime(1668, 5, 21),
                //     MainCategory = "Rum"
                // },
                // new Author()
                // {
                //     Id = Guid.Parse("1511b114-3432-7u89-3862-n32d917590y"),
                //     FirstName = "Lauri",
                //     LastName = "Honkalampi",
                //     DateOfBirth = new DateTime(1995, 4, 7),
                //     MainCategory = "Play"
                }
                );

            modelBuilder.Entity<Course>().HasData(
               new Course
               {
                   Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                   AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Commandeering a Ship Without Getting Caught",
                   Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
               },
               new Course
               {
                   Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                   AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Overthrowing Mutiny",
                   Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny."
               },
               new Course
               {
                   Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
                   AuthorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                   Title = "Avoiding Brawls While Drinking as Much Rum as You Desire",
                   Description = "Every good pirate loves rum, but it also has a tendency to get you into trouble.  In this course you'll learn how to avoid that.  This new exclusive edition includes an additional chapter on how to run fast without falling while drunk."
               },
               new Course
               {
                   Id = Guid.Parse("40ff5488-fdab-45b5-bc3a-14302d59869a"),
                   AuthorId = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                   Title = "Singalong Pirate Hits",
                   Description = "In this course you'll learn how to sing all-time favourite pirate songs without sounding like you actually know the words or how to hold a note."
               }
               );

            base.OnModelCreating(modelBuilder);
        }
    }
}
