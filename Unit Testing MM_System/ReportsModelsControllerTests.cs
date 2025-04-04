using System;
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
    public class ReportsModelsControllerTests
    {
        //Method to create a fresh controller and context for each test. Fixes the issue that running the test individually would work but not the entire file.
        private ReportsModelsController GetControllerWithFreshContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<MunicipalityManagementSystemV2Context>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new MunicipalityManagementSystemV2Context(options);

            // New citizen. Whatup
            context.CitizenManagementModel.Add(new CitizenManagementModel
            {
                CitizenID = 1,
                Fullname = "John Dean",
                Address = "12 Main St",
                PhoneNumber = "08338902405",
                Email = "JohnDean@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                RegistrationDate = DateTime.Now
            });

            // Creates new report
            context.ReportsModel.Add(new ReportsModel
            {
                ReportID = 2,
                CitizenID = 1,
                ReportType = "Incident",
                Details = "Details about the incident.",
                SubmissionDate = DateTime.Now,
                Status = "Under Review"
            });

            context.SaveChanges();
            return new ReportsModelsController(context);
        }

        //Checks if returns view of list of reports.
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfReports()
        {
            var controller = GetControllerWithFreshContext(nameof(Index_ReturnsAViewResult_WithAListOfReports));

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ReportsModel>>(viewResult.ViewData.Model);
            Assert.NotEmpty(model);
        }

        //Checks if report with valid id returns valid details for it.
        [Fact]
        public async Task Details_ValidId_ReturnsAViewResult_WithAReport()
        {
            var controller = GetControllerWithFreshContext(nameof(Details_ValidId_ReturnsAViewResult_WithAReport));

            var result = await controller.Details(2);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ReportsModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.ReportID);
        }

        [Fact]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            var controller = GetControllerWithFreshContext(nameof(Create_ValidModel_ReturnsRedirectToActionResult));

            var report = new ReportsModel
            {
                CitizenID = 1,
                ReportType = "Incident",
                Details = "Ahhh no",
                SubmissionDate = DateTime.Now,
                Status = "Under Review"
            };

            var result = await controller.Create(report);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_InvalidCitizenID_ReturnsViewWithModelError()
        {
            var controller = GetControllerWithFreshContext(nameof(Create_InvalidCitizenID_ReturnsViewWithModelError));

            var report = new ReportsModel
            {
                CitizenID = 9999,
                ReportType = "Incident",
                Details = "Save me.",
                SubmissionDate = DateTime.Now,
                Status = "Under Review"
            };

            var result = await controller.Create(report);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("CitizenID"));
        }

        //See if goes to delete confirmed page and returns the correct details.
        [Fact]
        public async Task Delete_ValidId_ReturnsAViewResult_WithAReport()
        {
            var controller = GetControllerWithFreshContext(nameof(Delete_ValidId_ReturnsAViewResult_WithAReport));

            var result = await controller.Delete(2);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ReportsModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.ReportID);
        }

        //See if goes back to index view after confrim deletion
        [Fact]
        public async Task DeleteConfirmed_ValidId_ReturnsRedirectToActionResult()
        {
            var controller = GetControllerWithFreshContext(nameof(DeleteConfirmed_ValidId_ReturnsRedirectToActionResult));

            var result = await controller.DeleteConfirmed(2);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task ReviewReport_ValidId_ReturnsAViewResult_WithAReport()
        {
            var controller = GetControllerWithFreshContext(nameof(ReviewReport_ValidId_ReturnsAViewResult_WithAReport));

            var result = await controller.ReviewReport(2);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ReportsModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.ReportID);
        }
    }
}
