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
    public class CitizenManagementModelsController : Controller
    {
        private readonly MunicipalityManagementSystemV2Context _context;

        public CitizenManagementModelsController(MunicipalityManagementSystemV2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.CitizenManagementModel.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizenManagementModel = await _context.CitizenManagementModel
                .FirstOrDefaultAsync(m => m.CitizenID == id);
            if (citizenManagementModel == null)
            {
                return NotFound();
            }

            return View(citizenManagementModel);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitizenID,Fullname,Address,PhoneNumber,Email,DateOfBirth,RegistrationDate")] CitizenManagementModel citizenManagementModel)
        {
            if (ModelState.IsValid)
            {
                bool emailExists = _context.CitizenManagementModel.Any(c => c.Email == citizenManagementModel.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already registered by another citizen.");
                    return View(citizenManagementModel);
                }

                _context.Add(citizenManagementModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(citizenManagementModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizenManagementModel = await _context.CitizenManagementModel.FindAsync(id);
            if (citizenManagementModel == null)
            {
                return NotFound();
            }
            return View(citizenManagementModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CitizenID,Fullname,Address,PhoneNumber,Email,DateOfBirth,RegistrationDate")] CitizenManagementModel citizenManagementModel)
        {
            if (id != citizenManagementModel.CitizenID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool emailExists = _context.CitizenManagementModel.Any(c => c.Email == citizenManagementModel.Email && c.CitizenID != id);

                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "This email is already registered by another citizen.");
                        return View(citizenManagementModel);
                    }

                    _context.Update(citizenManagementModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizenManagementModelExists(citizenManagementModel.CitizenID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(citizenManagementModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizenManagementModel = await _context.CitizenManagementModel
                .FirstOrDefaultAsync(m => m.CitizenID == id);
            if (citizenManagementModel == null)
            {
                return NotFound();
            }

            return View(citizenManagementModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var citizenManagementModel = await _context.CitizenManagementModel.FindAsync(id);
            if (citizenManagementModel != null)
            {
                _context.CitizenManagementModel.Remove(citizenManagementModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitizenManagementModelExists(int id)
        {
            return _context.CitizenManagementModel.Any(e => e.CitizenID == id);
        }
    }
}
