using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municipality_Management_System.Models;
using MunicipalityManagementSystemV2.Controllers;
using MunicipalityManagementSystemV2.Data;
using Xunit;

namespace Unit_Testing_MM_System
{
    public class ServiceRequestModelsControllerTests
    {
        // Method to create a fresh controller and context for each test. Used a better(?) way to do it in my citizens and staff, but not gonna change it up now.
        private async Task<MunicipalityManagementSystemV2Context> GetDatabaseContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<MunicipalityManagementSystemV2Context>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var databaseContext = new MunicipalityManagementSystemV2Context(options);
            databaseContext.Database.EnsureCreated();

            if (!databaseContext.ServiceRequestModel.Any())
            {
                databaseContext.ServiceRequestModel.Add(new ServiceRequestModel
                {
                    RequestID = 1,
                    CitizenID = 101,
                    ServiceType = "Water Supply Issue",
                    RequestDate = System.DateTime.Now,
                    Status = "Pending"
                });

                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task Index_ReturnsViewWithServiceRequests()
        {
            var dbContext = await GetDatabaseContext(nameof(Index_ReturnsViewWithServiceRequests));
            var controller = new ServiceRequestModelsController(dbContext);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ServiceRequestModel>>(viewResult.Model);
            Assert.Single(model); // Ensure one request exists
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithServiceRequest()
        {
            var dbContext = await GetDatabaseContext(nameof(Details_ValidId_ReturnsViewWithServiceRequest));
            var controller = new ServiceRequestModelsController(dbContext);

            var result = await controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ServiceRequestModel>(viewResult.Model);
            Assert.Equal(1, model.RequestID); // Check if correct service request is retrieved
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext(nameof(Details_InvalidId_ReturnsNotFound));
            var controller = new ServiceRequestModelsController(dbContext);

            var result = await controller.Details(99);

            Assert.IsType<NotFoundResult>(result); // Check if NotFoundResult is returned
        }

        [Fact]
        public async Task Create_ValidCitizenID_RedirectsToIndex()
        {
            var dbContext = await GetDatabaseContext(nameof(Create_ValidCitizenID_RedirectsToIndex));
            var controller = new ServiceRequestModelsController(dbContext);

            var citizen = new CitizenManagementModel
            {
                CitizenID = 101,
                Fullname = "John Dorris",
                Address = "123 Main St",
                PhoneNumber = "08392450",
                Email = "john.dorris@gmail.com"
            };
            dbContext.CitizenManagementModel.Add(citizen);
            await dbContext.SaveChangesAsync();

            // Ensure no service requests are in the database before adding a new one
            dbContext.ServiceRequestModel.RemoveRange(dbContext.ServiceRequestModel); // Remove any existing requests
            await dbContext.SaveChangesAsync();

            var serviceRequest = new ServiceRequestModel
            {
                CitizenID = 101,
                ServiceType = "Street Repair",
                RequestDate = System.DateTime.Now,
                Status = "Pending"
            };

            var result = await controller.Create(serviceRequest);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); // Should redirect to index
            Assert.Single(dbContext.ServiceRequestModel); // Ensure service request is saved
        }

        [Fact]
        public async Task Create_InvalidCitizenID_ReturnsViewWithError()
        {
            var dbContext = await GetDatabaseContext(nameof(Create_InvalidCitizenID_ReturnsViewWithError));
            var controller = new ServiceRequestModelsController(dbContext);

            var serviceRequest = new ServiceRequestModel
            {
                CitizenID = 9999, // Invalid CitizenID
                ServiceType = "Street Repair",
                RequestDate = System.DateTime.Now,
                Status = "Pending"
            };

            var result = await controller.Create(serviceRequest);

            var viewResult = Assert.IsType<ViewResult>(result);
            var modelState = viewResult.ViewData.ModelState;
            Assert.True(modelState.ContainsKey("CitizenID"));
            Assert.Contains("Invalid Citizen ID.", modelState["CitizenID"].Errors.Select(e => e.ErrorMessage).ToList()); // Ensure correct error message is returned
        }

        [Fact]
        public async Task Delete_ValidId_RedirectsToIndex()
        {
            var dbContext = await GetDatabaseContext(nameof(Delete_ValidId_RedirectsToIndex));
            var controller = new ServiceRequestModelsController(dbContext);

            var serviceRequest = new ServiceRequestModel
            {
                CitizenID = 101,
                ServiceType = "Water Supply Issue",
                RequestDate = DateTime.Now,
                Status = "Pending"
            };
            dbContext.ServiceRequestModel.Add(serviceRequest);
            await dbContext.SaveChangesAsync();

            var result = await controller.DeleteConfirmed(serviceRequest.RequestID);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); // Redirect to index after deletion

            var deletedRequest = await dbContext.ServiceRequestModel
                .FirstOrDefaultAsync(r => r.RequestID == serviceRequest.RequestID);
            Assert.Null(deletedRequest); // Ensure the service request is deleted
        }

        [Fact]
        public async Task UpdateStatus_ValidId_UpdatesStatusAndRedirects()
        {
            var dbContext = await GetDatabaseContext(nameof(UpdateStatus_ValidId_UpdatesStatusAndRedirects));
            var controller = new ServiceRequestModelsController(dbContext);

            // Retrieve the existing service request to update it.
            var serviceRequest = await dbContext.ServiceRequestModel.FindAsync(1);

            //Detach the entities to avoid conflicts with tracking
            var trackedEntity = dbContext.Entry(serviceRequest);
            trackedEntity.State = EntityState.Detached;

            var result = await controller.UpdateStatus(1, "Completed"); //update the service

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); // Redirect to index after status update

            // Ensure the service request's status was updated
            var updatedServiceRequest = await dbContext.ServiceRequestModel.FindAsync(1);
            Assert.NotNull(updatedServiceRequest); // Ensure the request exists
            Assert.Equal("Completed", updatedServiceRequest.Status); // Ensure status was updated
        }
    }
}
