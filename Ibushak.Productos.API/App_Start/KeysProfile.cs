using AutoMapper;
using Ibushak.Productos.API.Dtos;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibushak.Productos.API.App_Start
{
    public class KeysProfile : Profile
    {
        public KeysProfile()
        {
            CreateMap<ASIN, ASINDto>();
            CreateMap<ASINDto, ASIN>();
            CreateMap<UPC, UPCDto>();
            CreateMap<UPCDto, UPC>();
        }
    }
}