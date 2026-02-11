using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;
using System.Threading.Tasks;

namespace MvcCoreProceduresEF.Controllers
{
    public class EnfermosController : Controller
    {
        private RepositoryEnfermos repo;
        public EnfermosController(RepositoryEnfermos repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Enfermo> enfermos = await this.repo.GetEnfermosAsync();
            return View(enfermos);
        }
        public async Task<IActionResult> Details(string inscripcion)
        {
            Enfermo enfermo = await this.repo.FindEnfermoAsync(inscripcion);
            return View(enfermo);
        }
        public async Task<IActionResult> Delete(string inscripcion)
        {
            //await this.repo.DelteEnfermoAsync(inscripcion);
            await this.repo.DeleteENfermoRawAsync(inscripcion);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Create()
        {
          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enfermo enfermo)
        {
            await this.repo.InsertEnfermoAsynx(enfermo.Apellido,enfermo.Direccion,enfermo.FechaNacimiento,enfermo.Genero,enfermo.Nss);
            return RedirectToAction("Index");
        }
    }
}
