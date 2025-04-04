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
    public class ServiceRequestModelsController : Controller
    {
        private readonly MunicipalityManagementSystemV2Context _context;

        public ServiceRequestModelsController(MunicipalityManagementSystemV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceRequestModel.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequestModel = await _context.ServiceRequestModel
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (serviceRequestModel == null)
            {
                return NotFound();
            }

            return View(serviceRequestModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestID,CitizenID,ServiceType,RequestDate,Status")] ServiceRequestModel serviceRequestModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the provided CitizenID exists
                bool citizenExists = await _context.CitizenManagementModel.AnyAsync(c => c.CitizenID == serviceRequestModel.CitizenID);

                if (!citizenExists)
                {
                    ModelState.AddModelError("CitizenID", "Invalid Citizen ID.");
                    return View(serviceRequestModel); // Return view if invalid

                }
                _context.Add(serviceRequestModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceRequestModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequestModel = await _context.ServiceRequestModel
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (serviceRequestModel == null)
            {
                return NotFound();
            }

            return View(serviceRequestModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequestModel = await _context.ServiceRequestModel.FindAsync(id);
            if (serviceRequestModel != null)
            {
                _context.ServiceRequestModel.Remove(serviceRequestModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequestModel = await _context.ServiceRequestModel.FindAsync(id);
            if (serviceRequestModel == null)
            {
                return NotFound();
            }

            return View(serviceRequestModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // Fetch the existing ServiceRequestModel from the database
            var serviceRequestModel = await _context.ServiceRequestModel.FindAsync(id);
            if (serviceRequestModel == null)
            {
                return NotFound();
            }

            // Update only the Status field
            serviceRequestModel.Status = status;

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the model in the database
                    _context.Update(serviceRequestModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestModelExists(serviceRequestModel.RequestID))
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




        private bool ServiceRequestModelExists(int id)
        {
            return _context.ServiceRequestModel.Any(e => e.RequestID == id);
        }
    }
}
