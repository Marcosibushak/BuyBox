using AutoMapper;
using Ibushak.Productos.API.Dtos;
using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ibushak.Productos.API.Controllers
{
    public class UPCController : ApiController
    {
        // POST /api/upc
        [HttpPost]
        public IHttpActionResult CreateUpc(UPCDto upcDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var upc = Mapper.Map<UPCDto, UPC>(upcDto);

                using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                {
                    int cont = unidadDeTrabajo.UPC.buscar(u => u.Id == upc.Id).AsQueryable().Count();

                    if(cont == 0)
                    {
                        unidadDeTrabajo.UPC.agregar(upc);
                        unidadDeTrabajo.guardarCambios();
                    }

                    upcDto.Id = upc.Id;
                }

                return Created(new Uri($"{Request.RequestUri}/{upc.Id}"), upcDto);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
            
        }

        // DELTE /api/upc/1
        [HttpDelete]
        public IHttpActionResult DeleteUpc(string id)
        {
            try
            {
                using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                {
                    var upc = unidadDeTrabajo.UPC.Obtener(id);
                    unidadDeTrabajo.UPC.borrar(upc);
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
