using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class CompteBancaireController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CompteBancaireController(MyDbContext context)
        {
            _context = context;
        }


        // GET: CompteBancaire/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return Ok();
        }


        // POST: CompteBancaire/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Salaire_Mensuel,Montant_Bloqué,RIB,Segmentation,Engagement_Total,Solde_disponible,Devise_Compte,IBAN,Classe_De_Risque")] CompteBancaire compteBancaire)
        {
            return Ok();
        }

        // GET: CompteBancaire/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return Ok();
        }

        // POST: CompteBancaire/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Salaire_Mensuel,Montant_Bloqué,RIB,Segmentation,Engagement_Total,Solde_disponible,Devise_Compte,IBAN,Classe_De_Risque")] CompteBancaire compteBancaire)
        {
            return Ok();
        }

        // GET: CompteBancaire/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok();
        }

        // POST: CompteBancaire/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return Ok();
        }

        private bool CompteBancaireExists(int id)
        {
            return (true);
        }
    }
}
