using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFichasPorLinea.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FichasXLineaController : Controller
    {
        private readonly ILogger<FichasXLineaController> _logger;
        private ApiOptions _apiOptions;

        public FichasXLineaController(
            ILogger<FichasXLineaController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string lineas, DateTime desde, DateTime hasta)
        {
            //var resultado = new List<ParLineaFicha>();

            // TODO: preguntarle a la DB
            // TODO: poblar "resultado"

            if (string.IsNullOrEmpty(lineas))
            {
                return BadRequest();    
            }

            if (desde == DateTime.MinValue)
            {
                return BadRequest();
            }

            List<int> lstLineas = null;

            try
            {
                lstLineas = lineas
                    .Split (',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sLinea => int.Parse(sLinea))
                    .ToList()
                ;
            }
            catch
            {
                return BadRequest();
            }

            var resultado = DameFichasDeLineas(
                _apiOptions.MainConnectionString,
                lstLineas,
                desde,
                hasta
            );

            return Ok(resultado);
        }

        static List<ParLineaFicha> DameFichasDeLineas(string connectionString, List<int> lineasOrdenadas, DateTime desde, DateTime hasta)
        {
            // conectarme a db-tecnobus...
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // tomar las fichas que necesito
            var lineasSeparadasPorComas = string.Join(",", lineasOrdenadas.ToArray());
            var cmdText = @$"
                SELECT DISTINCT cod_linea, nro_ficha
                FROM log_gpsConCalculoAtraso
                WHERE
	                fechaHora >= '{desde.Day:00}/{desde.Month:00}/{desde.Year:0000}'
	                AND
	                fechaHora <  '{hasta.Day:00}/{hasta.Month:00}/{hasta.Year:0000}'
	                AND
	                cod_linea in ({lineasSeparadasPorComas})
                ORDER BY cod_linea, nro_ficha
            ";
            using SqlCommand command = new SqlCommand(cmdText.Trim(), connection);
            using SqlDataReader reader = command.ExecuteReader();
            var resultado = new List<ParLineaFicha>();
            while (reader.Read())
            {
                var objFicha = reader["nro_ficha"];
                var objLinea = reader["cod_linea"];

                if (objFicha != null && objLinea != null)
                {
                    try
                    {
                        var ficha = Convert.ToInt32(objFicha);
                        var linea = Convert.ToInt32(objLinea);
                        resultado.Add(new ParLineaFicha { 
                            Ficha = ficha, 
                            Linea = linea 
                        });
                    }
                    catch (Exception exx)
                    {
                        Console.WriteLine(exx);
                    }
                }
            }

            // devuelvo del resultado por acá
            return resultado;
        }

        class ParLineaFicha
        {
            public int Linea { get; set; }
            public int Ficha { get; set; }
        }
    }
}
