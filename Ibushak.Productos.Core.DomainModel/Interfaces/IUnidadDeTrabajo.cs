using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DomainModel.Interfaces
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IASINRepositorio ASIN { get; }

        IUPCRepositorio UPC { get; }

        IProductoRepositorio Producto { get; }

        IBuyBoxRepositorio BuyBox { get; }

        IResumenRepositorio Resumen { get; }

        ICaracteristicasRepositorio Caracteristicas { get; }

        IComentariosRepositorio Comentarios { get; }

        IDimensionesRepositorio Dimensiones { get; }

        IDimensionesPaqueteRepositorio DimensionesPaquete { get; }

        ISimilaresRepositorio Similares { get; }

        IUPCsRepositorio UPCs { get; }

        IUsuarioRepositorio Usuario { get; }
        
        void guardarCambios();

        bool hayCambios();
    }
}
