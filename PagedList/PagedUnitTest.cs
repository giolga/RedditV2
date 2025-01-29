using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Models;
using Reddit.Repositories;
using System;

namespace PagedList
{
    public class PagedUnitTest
    {
        public async Task<PagedList<T>> Pagination<T>(IQueryable<T> items, int pageNumber, int pageSize)
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return null;
            return await PagedList<T>.CreateAsync(items, pageNumber, pageSize);
        }

        //Main test case!
        [Fact]
        public async Task User_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            List<User> users = Users();

            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync(); // Await the SaveChangesAsync call

            var queryableUsers = dbContext.Users.AsQueryable();

            int pageNumber = 2;
            int pageSize = 2;

            // Act: Call the Pagination method
            var pagedList = await Pagination(queryableUsers, pageNumber, pageSize);

            Assert.Equal("Israel", pagedList.Items[0].Name);
            Assert.Equal("Alex", pagedList.Items[1].Name);
            Assert.True(pagedList.HasPreviousPage);
            Assert.True(pagedList.HasNextPage);
            Assert.Equal(2, pagedList.Items.Count);


            int pageNumber2 = 1;
            int pageSize2 = 4;

            var pagedList2 = await Pagination(queryableUsers, pageNumber2, pageSize2);


            Assert.Equal("Ilia", pagedList2.Items[0].Name);
            Assert.False(pagedList2.HasPreviousPage);
            Assert.True(pagedList2.HasNextPage);
            Assert.Equal("Kamaru", pagedList2.Items[1].Name);
            Assert.NotEmpty(pagedList2.Items); //List not empty! test case passed

            pagedList2.Items = new List<User>();

            Assert.Empty(pagedList2.Items); // List empty! test case passed

            int pageNumber3 = 2;
            int pageSize3 = 5;

            var pagedList3 = await Pagination(queryableUsers, pageNumber3, pageSize3);

            Assert.True(pageSize3 > pagedList3.Items.Count); //PageSize is larger then the total number of items on the second page (4 users)
            Assert.Equal(4, pagedList3.Items.Count);
            Assert.True(pagedList3.TotalCount > pageSize3); // total count is more than pageSize

            int pageNumber4 = 4;
            int pageSize4 = 0;

            var pagedList4 = await Pagination(queryableUsers, pageNumber4, pageSize4);
            Assert.True(pagedList4 is null); //page size or number is 0 or less

            //var ex = Assert.Throws<ArgumentNullException>(() => foo.Bar(null));
            //Assert.That(ex.ParamName, Is.EqualTo("bar"));

            int pageNumber5 = 4;
            int pageSize5 = -2;

            var pagedList5 = await Pagination(queryableUsers, pageNumber5, pageSize5);
            Assert.True(pagedList5 is null);

        }


        [Fact]
        public async Task Community_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);

            List<Community> communities = Communities();

            dbContext.Communities.AddRange(communities);
            dbContext.Users.AddRange(Users());
            await dbContext.SaveChangesAsync();

            var queryableCommunities = dbContext.Communities.AsQueryable();

            int pageNumber = 1;
            int pageSize = 3;

            // Act: Call the Pagination method
            var pagedList = await Pagination(queryableCommunities, pageNumber, pageSize);

            Assert.Equal(pageNumber, pagedList.PageNumber);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(communities.Count, pagedList.TotalCount);
            Assert.Equal(pageSize, pagedList.Items.Count);
            Assert.True(pagedList.HasNextPage);
            Assert.False(pagedList.HasPreviousPage);

            Assert.Equal(5, pagedList.Items[2].OwnerId); //owner id of the owner in the first page should be 5 (el cucuy)

            var person = await dbContext.Users.FirstAsync(u => u.Id == pagedList.Items[0].OwnerId);
            Assert.Equal(1, person.Id);

            int pageNumber2 = 5;
            int pageSize2 = 2;

            var pagedList2 = await Pagination(queryableCommunities, pageNumber2, pageSize2); //Page should have 1 article
            Assert.Single(pagedList2.Items);
            Assert.False(pagedList2.HasNextPage);
            Assert.True(pagedList2.HasPreviousPage);
            Assert.Equal("Networking Community", pagedList2.Items[0].Name);
            Assert.NotEqual(communities.Count, pagedList2.Items.Count);

        }

        private List<User> Users()
        {
            return new List<User>
                {
                    new User { Name = "Ilia", Email = "kumi@gmail.com" },
                    new User { Name = "Kamaru", Email = "nightmare@gmail.com" },
                    new User { Name = "Israel", Email = "the_last_stylebender@gmail.com" },
                    new User { Name = "Alex", Email = "poatan@gmail.com" },
                    new User { Name = "Tony", Email = "el_cucuy@gmail.com" },
                    new User { Name = "Charles", Email = "do_bronx@gmail.com" },
                    new User { Name = "Alex", Email = "the_great@gmail.com" },
                    new User { Name = "Anderson", Email = "spider@gmail.com" },
                    new User { Name = "Jon", Email = "goat@gmail.com" }
                };
        } // 9 users

        private List<Community> Communities()
        {
            return new List<Community>
            {
                new Community
                {
                    Name = "MMA Community",
                    Description = "Merab THE GOAT",
                    OwnerId = 1 //Giorgi
                },
                new Community
                {
                    Name = "Alex Pereira Training Sessions",
                    Description = "CHAMA",
                    OwnerId = 4 //Poatan owner
                },
                new Community
                {
                    Name = "C# Masterclass",
                    Description = "This is the best C# community to learn this prog language",
                    OwnerId = 5 // Tony                   
                },
                new Community
                {
                    Name = "Culinary community",
                    Description = "Interested in cooking? Let's cook with Volk!",
                    OwnerId = 7 //the great
                },
                new Community
                {
                    Name = "Striking masterclass",
                    Description = "Learn Israel's question mark kicks within 5 minutes!",
                    OwnerId = 3 //izzy
                },
                new Community
                {
                    Name = "GOAT community",
                    Description = "Jon Jones teaches us how to become the best!",
                    OwnerId = 9
                },
                new Community
                {
                    Name = "Head movement community",
                    Description = "Learn how to become Neo with Spider",
                    OwnerId = 8
                },
                new Community
                {
                    Name = "Muay Thai",
                    Description = "OOOooooooOoooooooowwweeeeeeeeeeeee",
                    OwnerId = 1
                },
                new Community
                {
                    Name = "Networking Community",
                    Description = "Cisco",
                    OwnerId = 1
                }
            };
        }


    }
}
