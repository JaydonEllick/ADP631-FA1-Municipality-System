using System;
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
    public class StaffManagementModelsControllerTests
    {
        private async Task<MunicipalityManagementSystemV2Context> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<MunicipalityManagementSystemV2Context>()
                .UseInMemoryDatabase(databaseName: "StaffTestDB_" + Guid.NewGuid()) //new method to start every test in fresh state, too lazy to change others tho.
                .Options;

            var databaseContext = new MunicipalityManagementSystemV2Context(options);
            databaseContext.Database.EnsureCreated();

            //creates new staff only if fake database has none.
            if (!databaseContext.StaffManagementModel.Any())
            {
                databaseContext.StaffManagementModel.Add(new StaffManagementModel
                {
                    StaffID = 1,
                    FullName = "John Dorris",
                    Position = "Manager",
                    Department = "HR",
                    Email = "john.dorris@gmail.com",
                    HiredDate = DateTime.Now
                });

                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        [Fact]
        public async Task Index_ReturnsViewWithStaffList()
        {

            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<StaffManagementModel>>(viewResult.Model);
            Assert.Single(model); // Check if there's one staff record
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithStaffDetails()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var result = await controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StaffManagementModel>(viewResult.Model);
            Assert.Equal(1, model.StaffID);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var result = await controller.Details(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var staff = new StaffManagementModel
            {
                FullName = "John Dorris Updated",
                Position = "Developer",
                Department = "IT",
                Email = "john.dorris.updated@gmail.com",
                HiredDate = DateTime.Now
            };

            var result = await controller.Create(staff);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var addedStaff = dbContext.StaffManagementModel.FirstOrDefault(s => s.Email == staff.Email);
            Assert.NotNull(addedStaff);
        }

        [Fact]
        public async Task Create_ExistingEmail_ReturnsViewWithError()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var staff = new StaffManagementModel
            {
                FullName = "Gregory Dorris",
                Position = "HR",
                Department = "HR",
                Email = "john.dorris@gmail.com", //Same email as John Dorris.
                HiredDate = DateTime.Now
            };

            var result = await controller.Create(staff); //create new staff. 

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Contains("This email is already registered by another staff member.", viewResult.ViewData.ModelState["Email"].Errors.Select(e => e.ErrorMessage).ToList());

        }

        [Fact]
        public async Task Edit_ValidId_ReturnsViewWithStaffDetails()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var staff = await dbContext.StaffManagementModel.FirstOrDefaultAsync(s => s.StaffID == 1);

            staff.FullName = "James Smith";

            var result = await controller.Edit(1, staff);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result); //confirms 
            Assert.Equal("Index", redirectResult.ActionName);

            var updatedStaff = await dbContext.StaffManagementModel.FindAsync(1);
            Assert.Equal("James Smith", updatedStaff.FullName);
        }

        [Fact]
        public async Task Edit_InvalidEmail_ReturnsViewWithError()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            // Ensure the staff with ID exists, if not creates him.
            var staff = await dbContext.StaffManagementModel.FirstOrDefaultAsync(s => s.StaffID == 2);
            if (staff == null)
            {
                // If the staff member doesn't exist, create one with a valid email for testing
                staff = new StaffManagementModel
                {
                    StaffID = 2,
                    FullName = "James Smith",
                    Position = "Developer",
                    Department = "IT",
                    Email = "james.smith@gmail.com",
                    HiredDate = DateTime.Now
                };
                dbContext.StaffManagementModel.Add(staff);
                await dbContext.SaveChangesAsync();
            }

            // Prepare an updated staff with an existing email
            var updatedStaff = new StaffManagementModel
            {
                StaffID = 2, // Same ID as the one being edited
                FullName = "James Smith The Second",
                Position = "Developer",
                Department = "IT",
                Email = "john.dorris@gmail.com", // Duplicate email
                HiredDate = DateTime.Now
            };

            // Detach the existing entity to avoid tracking conflicts
            dbContext.Entry(staff).State = EntityState.Detached;

            var result = await controller.Edit(2, updatedStaff); //ID should be the new dude.


            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Contains("This email is already registered by another staff member.", viewResult.ViewData.ModelState["Email"].Errors.Select(e => e.ErrorMessage).ToList());
        }


        [Fact]
        public async Task Delete_ValidId_RedirectsToIndex()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            //Add new staff to be deleted.
            var staff = new StaffManagementModel
            {
                StaffID = 5,
                FullName = "Lisa Turner",
                Position = "Manager",
                Department = "HR",
                Email = "lisa.turner@gmail.com",
                HiredDate = DateTime.Now
            };

            dbContext.StaffManagementModel.Add(staff);
            await dbContext.SaveChangesAsync();


            var result = await controller.DeleteConfirmed(5); //bye bye number 5

   
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var deletedStaff = await dbContext.StaffManagementModel.FindAsync(5);
            Assert.Null(deletedStaff); // The staff record should have been removed
        }

        [Fact]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new StaffManagementModelsController(dbContext);

            var result = await controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
