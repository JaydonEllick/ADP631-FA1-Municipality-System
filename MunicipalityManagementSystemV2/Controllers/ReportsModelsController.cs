using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MunicipalityManagementSystemV2.Data;
using Municipality_Management_System.Models;

namespace MunicipalityManagementSystemV2.Controllers
{
    public class ReportsModelsController : Controller
    {
        private readonly MunicipalityManagementSystemV2Context _context;

        public ReportsModelsController(MunicipalityManagementSystemV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ReportsModel.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportsModel = await _context.ReportsModel
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (reportsModel == null)
            {
                return NotFound();
            }

            return View(reportsModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportID,CitizenID,ReportType,Details,SubmissionDate,Status")] ReportsModel reportsModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the provided CitizenID exists
                bool citizenExists = await _context.CitizenManagementModel.AnyAsync(c => c.CitizenID == reportsModel.CitizenID);

                if (!citizenExists)
                {
                    ModelState.AddModelError("CitizenID", "Invalid Citizen ID. Please enter a valid Citizen.");
                    return View(reportsModel); // Return view if invalid

                }

                _context.Add(reportsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(reportsModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportsModel = await _context.ReportsModel
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (reportsModel == null)
            {
                return NotFound();
            }

            return View(reportsModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reportsModel = await _context.ReportsModel.FindAsync(id);
            if (reportsModel != null)
            {
                _context.ReportsModel.Remove(reportsModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReviewReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportsModel = await _context.ReportsModel
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (reportsModel == null)
            {
                return NotFound();
            }

            return View(reportsModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewReport(int id, string status)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // Fetch the existing ReportsModel from the database
            var reportsModel = await _context.ReportsModel.FindAsync(id);
            if (reportsModel == null)
            {
                return NotFound();
            }

            // Update only the Status field
            reportsModel.Status = status;

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the model in the database
                    _context.Update(reportsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportsModelExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ReportsModelExists(int id)
        {
            return _context.ReportsModel.Any(e => e.ReportID == id);
        }
    }
}
