using CerebroXMenAPI.Models;
using System;
using System.Net;
using System.Web.Http;
using CerebroXMenAPI.app_data;
using System.Web.Http.Description;

namespace CerebroXMenAPI.Controllers
{
    public class MutantController : ApiController
    {
        private readonly MutanteService _mutanteService;

        private readonly ConexionSQLServerAutogestionWeb _cn;

        public MutantController()
        {
            _cn = ConexionSQLServerAutogestionWeb.New();
            _mutanteService = new MutanteService(_cn);
        }


        [Route("stats")]
        [ResponseType(typeof(IStats))]
        public IHttpActionResult GetStats()
        {
            try
            {
                IStats iStats = _mutanteService.RatioMutantes();

                return Ok(iStats);

            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest();
            }
        }

        [Route("mutant")]
        public IHttpActionResult Post([FromBody] IGen IGen)
        {
            try
            {
                bool esMutante = _mutanteService.EsMutante(IGen.Dna);

                if (esMutante)
                { 
                    return Ok();
                }
                else
                {
                    return StatusCode(HttpStatusCode.Forbidden);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest();
            }
        }
    }
}
