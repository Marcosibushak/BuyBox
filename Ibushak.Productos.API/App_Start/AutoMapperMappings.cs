using AutoMapper;
using Ibushak.Productos.API.Dtos;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibushak.Productos.API.App_Start
{
    public class AutoMapperMappings
    {
        public static void Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ASIN, ASINDto>();
                cfg.CreateMap<ASINDto, ASIN>();
                cfg.CreateMap<UPC, UPCDto>();
                cfg.CreateMap<UPCDto, UPC>();
            });

            var mapper = config.CreateMapper();

            // register your mapper here.
        }
    }
}