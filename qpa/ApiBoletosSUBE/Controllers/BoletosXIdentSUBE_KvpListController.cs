using Comun;
using ComunSUBE;
using LibQPA.ProveedoresVentas.DbSUBE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBoletosSUBE.Controllers
{
    // curl -v  http://vm-coches:5006/BoletosXIdentSUBE_KvpList?desde=2022-12-02
    // curl -vk https://vm-coches:5007/BoletosXIdentSUBE_KvpList?desde=2022-12-02
    // https://localhost:44364/BoletosXIdentSUBE_KvpList?desde=2022-12-02

    [Route("[controller]")]
    [ApiController]
    public class BoletosXIdentSUBE_KvpListController : Controller
    {
        private readonly ILogger<BoletosXIdentSUBE_KvpListController> _logger;
        private ApiOptions _apiOptions;

        public BoletosXIdentSUBE_KvpListController(
            ILogger<BoletosXIdentSUBE_KvpListController> logger,
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

            try
            {
                Dictionary<ParEmpresaInterno, List<BoletoComun>> dic = HelperBoletosSUBE.GetBoletosFromDbSUBE(
                    _apiOptions.ConnectionStringDbSUBE,
                    _apiOptions.CommandTimeoutDbSUBE,
                    desde,
                    hasta
                );

                if (dic == null)
                {
                    throw new Exception("No se pudieron obtener los boletos SUBE");
                }

                var retVal = dic.Select(kvp => kvp).ToList();

                return Ok(retVal);
            }
            catch (Exception exx)
            {
                _logger.LogError(exx, "Error al tratar de recuperar boletos de la Db");

                throw;
            }
        }
    }
}
