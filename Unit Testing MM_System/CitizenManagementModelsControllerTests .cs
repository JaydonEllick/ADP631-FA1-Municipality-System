using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunicipalityManagementSystemV2.Controllers;
using MunicipalityManagementSystemV2.Data;
using Municipality_Management_System.Models;
using Xunit;

namespace Unit_Testing_MM_System
{
    public class CitizenManagementModelsControllerTests 
    {
        //uses EF to create fake database
        private async Task<MunicipalityManagementSystemV2Context> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<MunicipalityManagementSystemV2Context>()
                .UseInMemoryDatabase(databaseName: "CitizenTestDB_" + Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new MunicipalityManagementSystemV2Context(options);
            databaseContext.Database.EnsureCreated(); // Only uses it if already created

            //create test citizen if the fake database is empty
            if (!databaseContext.CitizenManagementModel.Any())
            {
                databaseContext.CitizenManagementModel.Add(new CitizenManagementModel
                {
                    CitizenID = 1,
                    Fullname = "John Dorris",
                    Address = "123 Main Street",
                    PhoneNumber = "0833902460",
                    Email = "johndoe@gmail.com",
                    DateOfBirth = new System.DateTime(1990, 5, 15),
                    RegistrationDate = System.DateTime.Now
                });

                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        //Makes sure the index view reuturns the list of citizens and not empty.
        [Fact]
        public async Task Index_ReturnsViewWithCitizens() 
        {
            var dbContext = await GetDatabaseContext(); //creates context with fake data.
            var controller = new CitizenManagementModelsController(dbContext); //puts context into controller.

            var result = await controller.Index(); //call the index action

            var viewResult = Assert.IsType<ViewResult>(result); //Makes sure it is returning a view
            var model = Assert.IsAssignableFrom<List<CitizenManagementModel>>(viewResult.Model); //Make sure model is a list of citizens
            Assert.Single(model); //It only expects one record,
        }

        //Tests getting citizen details when valid id is used.
        [Fact]
        public async Task Details_ValidId_ReturnsViewWithCitizen()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new CitizenManagementModelsController(dbContext);

            var result = await controller.Details(1); //Call details with valid ID.

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CitizenManagementModel>(viewResult.Model); //Makes sure it returns a citizen model.
            Assert.Equal(1, model.CitizenID); // Confirm correct citizen was returned
        }

        //Tests for getting details of invalid id
        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new CitizenManagementModelsController(dbContext);

            var result = await controller.Details(999); // Non-existent ID

            Assert.IsType<NotFoundResult>(result);
        }

        // Tests creating a new valid citizen and redirects properly.
        [Fact]
        public async Task Create_ValidCitizen_RedirectsToIndex()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new CitizenManagementModelsController(dbContext);

            var newCitizen = new CitizenManagementModel
            {
                Fullname = "Amy Strawson",
                Address = "456 Elm Street",
                PhoneNumber = "0833902430",
                Email = "amy.strawson@gmail.com",
                DateOfBirth = new System.DateTime(1995, 8, 20),
                RegistrationDate = System.DateTime.Now
            };

            var result = await controller.Create(newCitizen); //calls the create post.

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);  //makes sure redirect happens.

            Assert.Equal(2, dbContext.CitizenManagementModel.Count()); //Confirms there is now 2 citizens. 
        }


        // Tests updating a citizen and saves the changes.
        [Fact]
        public async Task Edit_ValidCitizen_UpdatesDetails()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new CitizenManagementModelsController(dbContext);

            // Retrieve existing citizen
            var existingCitizen = await dbContext.CitizenManagementModel.FindAsync(1);

            if (existingCitizen != null)
            {
                dbContext.Entry(existingCitizen).State = EntityState.Detached; // Detach the existing entity. Fixing the issue of two entities already being tracked.
            }

            var updatedCitizen = new CitizenManagementModel
            {
                CitizenID = 1, // Should be existing record other fail.
                Fullname = "John Dorris The Second",
                Address = "789 Dorris Street",
                PhoneNumber = "0833902461",
                Email = "johndorris_updated@gmail.com",
                DateOfBirth = new System.DateTime(1990, 5, 15),
                RegistrationDate = System.DateTime.Now
            };

            var result = await controller.Edit(1, updatedCitizen); //calls edit post methdo

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); // confirms if redirects after update

            var citizenInDb = await dbContext.CitizenManagementModel.FindAsync(1);
            Assert.Equal("John Dorris The Second", citizenInDb.Fullname); //check if name update
            Assert.Equal("789 Dorris Street", citizenInDb.Address); //check if address updated.
        }

        // Tests deleting a valid citizen
        [Fact]
        public async Task Delete_ValidId_RemovesCitizen()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new CitizenManagementModelsController(dbContext);

            var result = await controller.DeleteConfirmed(1); //call delete confirmed method.

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); //makes sure redirect after delete

            var citizenInDb = await dbContext.CitizenManagementModel.FindAsync(1);
            Assert.Null(citizenInDb); // Ensure citizen was deleted
        }

        [Fact]
        public async Task Create_DuplicateEmail_ReturnsError()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new CitizenManagementModelsController(dbContext);

            // Add a new citizen with the same email as the one added at the start.
            var duplicateCitizen = new CitizenManagementModel
            {
                Fullname = "I don't care",
                Address = "321 IDC Street",
                PhoneNumber = "0833901111",
                Email = "johndoe@gmail.com", // dupe email
                DateOfBirth = new System.DateTime(1985, 4, 10),
                RegistrationDate = System.DateTime.Now
            };

            if (dbContext.CitizenManagementModel.Any(c => c.Email == duplicateCitizen.Email))
            {
                controller.ModelState.AddModelError("Email", "Email already exists");
            }

            var result = await controller.Create(duplicateCitizen);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("Email"));
        }
    }
}

