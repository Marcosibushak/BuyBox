using Ibushak.Productos.Core.DomainModel.Interfaces;
using System;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using Ibushak.Productos.Core.DAL.Repositorios;
using System.Data.Entity.Validation;

namespace Ibushak.Productos.Core.DAL
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo
    {
        private readonly IbushakProductosContext _context;

        public IASINRepositorio ASIN { get; private set; }

        public IUPCRepositorio UPC { get; private set; }

        public IProductoRepositorio Producto { get; private set; }

        public IBuyBoxRepositorio BuyBox { get; private set; }

        public IResumenRepositorio Resumen { get; private set; }

        public ICaracteristicasRepositorio Caracteristicas { get; private set; }

        public IComentariosRepositorio Comentarios { get; private set; }

        public IDimensionesRepositorio Dimensiones { get; private set; }

        public IDimensionesPaqueteRepositorio DimensionesPaquete { get; private set; }

        public ISimilaresRepositorio Similares { get; private set; }

        public IUPCsRepositorio UPCs { get; private set; }

        public IUsuarioRepositorio Usuario { get; private set; }

        public UnidadDeTrabajo(IbushakProductosContext context)
        {
            _context = context;
            ASIN = new ASINRepositorio(_context);
            UPC = new UPCRepositorio(_context);
            Producto = new ProductoRepositorio(_context);
            BuyBox = new BuyBoxRepositorio(_context);
            Resumen = new ResumenRepositorio(_context);
            Caracteristicas = new CaracteristicasRepositorio(_context);
            Comentarios = new ComentariosRespositorio(_context);
            Dimensiones = new DimensionesRepositorio(_context);
            DimensionesPaquete = new DimensionesPaqueteRepositorio(_context);
            Similares = new SimilaresRepositorio(_context);
            UPCs = new UPCsRepositorio(_context);
            Usuario = new UsuarioRepositorio(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void guardarCambios()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        Console.WriteLine(@"Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);
                }
                throw new Exception(dbEx.Message);
            }
        }

        public bool hayCambios()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}