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
    public class StaffManagementModelsController : Controller
    {
        private readonly MunicipalityManagementSystemV2Context _context;

        public StaffManagementModelsController(MunicipalityManagementSystemV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.StaffManagementModel.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffManagementModel = await _context.StaffManagementModel
                .FirstOrDefaultAsync(m => m.StaffID == id);
            if (staffManagementModel == null)
            {
                return NotFound();
            }

            return View(staffManagementModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffID,FullName,Position,Department,Email,HiredDate")] StaffManagementModel staffManagementModel)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                bool emailExists = _context.StaffManagementModel.Any(s => s.Email == staffManagementModel.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already registered by another staff member.");
                    return View(staffManagementModel);
                }

                _context.Add(staffManagementModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staffManagementModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffManagementModel = await _context.StaffManagementModel.FindAsync(id);
            if (staffManagementModel == null)
            {
                return NotFound();
            }
            return View(staffManagementModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffID,FullName,Position,Department,Email,HiredDate")] StaffManagementModel staffManagementModel)
        {
            if (id != staffManagementModel.StaffID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    bool emailExists = _context.StaffManagementModel.Any(s => s.Email == staffManagementModel.Email && s.StaffID != id);

                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "This email is already registered by another staff member.");
                        return View(staffManagementModel);
                    }

                    _context.Update(staffManagementModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffManagementModelExists(staffManagementModel.StaffID))
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
            return View(staffManagementModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffManagementModel = await _context.StaffManagementModel
                .FirstOrDefaultAsync(m => m.StaffID == id);
            if (staffManagementModel == null)
            {
                return NotFound();
            }

            return View(staffManagementModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staffManagementModel = await _context.StaffManagementModel.FindAsync(id);
            if (staffManagementModel != null)
            {
                _context.StaffManagementModel.Remove(staffManagementModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffManagementModelExists(int id)
        {
            return _context.StaffManagementModel.Any(e => e.StaffID == id);
        }
    }
}
