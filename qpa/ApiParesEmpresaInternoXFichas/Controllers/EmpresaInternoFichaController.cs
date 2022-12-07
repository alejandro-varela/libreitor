using ComunSUBE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiParesEmpresaInternoXFichas.Controllers
{
    // curl -v   http://vm-coches:5008/EmpresaInternoFicha
    // curl -vk https://vm-coches:5009/EmpresaInternoFicha
    // https://localhost:44347/EmpresaInternoFicha

    [Route("[controller]")]
    [ApiController]
    public class EmpresaInternoFichaController : Controller
    {
        private readonly ILogger<EmpresaInternoFichaController> _logger;
        private readonly ApiOptions _apiOptions;

        public EmpresaInternoFichaController(
            ILogger<EmpresaInternoFichaController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(DateTime desde, DateTime hasta)
        {
            if (hasta <= desde)
            {
                hasta = desde.AddDays(1);
            }

            DatosEmpIntFicha datosEmpIntFicha = new DatosEmpIntFicha(new DatosEmpIntFicha.Configuration
            {
                ConnectionString = _apiOptions.ConnectionStringDbIdentificadores,
            });

            var ret = new List<EmpresaInternoFicha>();

            foreach (var kvp in datosEmpIntFicha.Get())
            {
                var (empresa, interno) = kvp.Key;
                var ficha = kvp.Value;

                var eif = new EmpresaInternoFicha
                {
                    Empresa = empresa,
                    Interno = interno,
                    Ficha = ficha,
                };

                ret.Add(eif);
            }

            return Ok(ret);
        }

        public class EmpresaInternoFicha
        {
            public int Empresa { get; set; }
            public int Interno { get; set; }
            public int Ficha { get; set; }
        }
    }
}
