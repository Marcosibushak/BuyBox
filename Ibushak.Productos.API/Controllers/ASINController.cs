using AutoMapper;
using Ibushak.Productos.API.Dtos;
using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Linq;
using System.Web.Http;

namespace Ibushak.Productos.API.Controllers
{
    public class ASINController : ApiController
    {
        // POST /api/asin
        [HttpPost]
        public IHttpActionResult CreateAsin(ASINDto asinDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var asin = Mapper.Map<ASINDto, ASIN>(asinDto);
                using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                {
                    int cont = unidadDeTrabajo.ASIN.buscar(a => a.Id == asin.Id).AsQueryable().Count();
                    if(cont == 0)
                    {
                        unidadDeTrabajo.ASIN.agregar(asin);
                        unidadDeTrabajo.guardarCambios();
                    }
                    asinDto.Id = asin.Id;
                }
                return Created(new Uri($"{Request.RequestUri}/{asin.Id}"), asinDto);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELTE /api/asin/1
        [HttpDelete]
        public IHttpActionResult DeleteAsin(string id)
        {
            try
            {
                using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                {
                    var asin = unidadDeTrabajo.ASIN.Obtener(id);
                    unidadDeTrabajo.ASIN.borrar(asin);
                    unidadDeTrabajo.guardarCambios();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}