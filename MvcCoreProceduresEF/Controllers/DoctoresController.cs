using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcCoreProceduresEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RepositoryDoctores repo;
        
        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }
        
        public async Task<IActionResult> Index()
        {
            List<string> especialidades = await repo.GetEspecialidadesAsync();
            ViewData["Especialidades"] = especialidades;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string especialidad, int incremento)
        {
            if (accion.ToLower == "incrementar")
            {
                await this.repo.IncrementarSalarioEspecialidad(especialidad,incremento);
            }
            if (accion.ToLower == "incrementar")
            {
                await this.repo.UpdateDoctoresEspecialidadEFAsync(especialidad, incremento);
            }
            List<string> especialidades = await repo.GetEspecialidadesAsync();
            ViewData["Especialidades"] = especialidades;
            List<Doctor> doctores = await this.repo.GetDoctoresEspecialidadAsync(especialidad);
            return View(doctores);
        }

        
    }
}
