using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MigDashboard.Data;
using MigDashboard.Models;

namespace MigDashboard.Controllers
{
    public class QueryConfigsController : Controller
    {
        private readonly QueryContext _context;

        public QueryConfigsController(QueryContext context)
        {
            _context = context;
        }

        // GET: QueryConfigs
        public async Task<IActionResult> Index()
        {
            return View(await _context.QueyConfigs.ToListAsync());
        }

        // GET: QueryConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QueyConfigs == null)
            {
                return NotFound();
            }

            var queryConfig = await _context.QueyConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (queryConfig == null)
            {
                return NotFound();
            }

            return View(queryConfig);
        }

        // GET: QueryConfigs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QueryConfigs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,MysqlDB,OracleTable,MysqlSQL,Memo")] QueryConfig queryConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(queryConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(queryConfig);
        }

        // GET: QueryConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.QueyConfigs == null)
            {
                return NotFound();
            }

            var queryConfig = await _context.QueyConfigs.FindAsync(id);
            if (queryConfig == null)
            {
                return NotFound();
            }
            return View(queryConfig);
        }

        // POST: QueryConfigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,MysqlDB,OracleTable,MysqlSQL,Memo")] QueryConfig queryConfig)
        {
            if (id != queryConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(queryConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QueryConfigExists(queryConfig.Id))
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
            return View(queryConfig);
        }

        // GET: QueryConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.QueyConfigs == null)
            {
                return NotFound();
            }

            var queryConfig = await _context.QueyConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (queryConfig == null)
            {
                return NotFound();
            }

            return View(queryConfig);
        }

        // POST: QueryConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.QueyConfigs == null)
            {
                return Problem("Entity set 'QueryContext.QueyConfigs'  is null.");
            }
            var queryConfig = await _context.QueyConfigs.FindAsync(id);
            if (queryConfig != null)
            {
                _context.QueyConfigs.Remove(queryConfig);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QueryConfigExists(int id)
        {
            return _context.QueyConfigs.Any(e => e.Id == id);
        }
    }
}
