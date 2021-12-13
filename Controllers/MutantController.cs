using CerebroXMenAPI.Models;
using System;
using System.Net;
using System.Web.Http;
using CerebroXMenAPI.app_data;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Web.Http.Cors;


namespace CerebroXMenAPI.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers:"*", methods:"*")]
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

        [Route("all")]
        [ResponseType(typeof(List<IStats>))]
        public IHttpActionResult GetAll()
        {
            try
            {
                List<IGenInfo> iGenInfo = _mutanteService.GetAll();

                return Ok(iGenInfo);

            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("mutant/{ID:int}")]
        [ResponseType(typeof(List<IStats>))]
        public IHttpActionResult GetByID(int ID)
        {
            try
            {
                IGenInfo iGenInfo = _mutanteService.GetByID(ID);

                return Ok(iGenInfo);

            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest();
            }
        }
    }
}
