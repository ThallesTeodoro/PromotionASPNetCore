using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Promotion.Models;

namespace Promotion.Data.Seeds
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var contenxt = new PromotionContext(
                serviceProvider.GetRequiredService<DbContextOptions<PromotionContext>>()
            ))
            {
                bool saveChanges = false;

                if (!contenxt.Users.Any())
                {
                    contenxt.Users.AddRange(
                        new User
                        {
                            Name = "Thalles",
                            Email = "thallesmjteodoro@outlook.com",
                            Password = "mjV2OfkRLFHzqQzFZ3WujCpVPeODCowFhfMQ2NLHV5c=", // secret
                            CreatedAt = DateTime.Now
                        }
                    );

                    saveChanges = true;
                }

                if (!contenxt.Participants.Any())
                {
                    contenxt.Participants.AddRange(
                        new Participant
                        {
                            Name = "Thalles Teodoro",
                            Email = "thallesmjteodoro@gmail.com",
                            Password = "mjV2OfkRLFHzqQzFZ3WujCpVPeODCowFhfMQ2NLHV5c=", // secret
                            Birthdate = new DateTime(1998,8,27),
                            Gender = Gender.M,
                            CreatedAt = DateTime.Now
                        }
                    );

                    saveChanges = true;
                }

                if (saveChanges) {
                    contenxt.SaveChanges();
                }
            }
        }
    }
}