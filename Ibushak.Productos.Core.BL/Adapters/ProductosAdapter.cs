using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class ProductosAdapter
    {
        public static void DeleteTable()
        {
            using (var udt = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                udt.Producto.DeleteAll();
            }
        }

        public static Producto ObtenerProducto(string asin)
        {
            Producto oProducto;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                oProducto = unidadDeTrabajo.Producto.Obtener(asin);
            }
            return oProducto;
        }

        public static IEnumerable<Producto> ObtenerProductosActualizados()
        {
            IEnumerable<Producto> productos;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                productos = unidadDeTrabajo.Producto.obtenerProductosActualizados().ToList();
            }
            return productos;
        }

        public static IEnumerable<Producto> ObtenerProductosTodos()
        {
            IEnumerable<Producto> productos;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                productos = unidadDeTrabajo.Producto.obtenerTodos().ToList();
            return productos;
        }

        public static (IEnumerable<UPC> upcs, IEnumerable<ASIN> asins) ObtenerProductos()
        {
            IEnumerable<UPC> lstUpc;
            IEnumerable<ASIN> lstAsin;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                lstUpc = unidadDeTrabajo.UPC.obtenerTodos().ToList();
                lstAsin = unidadDeTrabajo.ASIN.obtenerTodos().ToList();
            }
            return (lstUpc, lstAsin);
        }

        public static bool Existe (string asin)
        {
            bool existe;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                var cont = unidadDeTrabajo.Producto.buscar(p => p.ASIN == asin).AsQueryable().Count();
                existe = cont > 0;
            }
            return existe;
        }

        public static void Insertar(Producto producto)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                producto.Actualizacion = true;
                unidadDeTrabajo.Producto.agregar(producto);
                unidadDeTrabajo.guardarCambios();
            }
        }

        public static void Actualizar(Producto producto)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                var productoOriginal = unidadDeTrabajo.Producto.Obtener(producto.ASIN);
                productoOriginal.Amount = producto.Amount;
                productoOriginal.Binding = producto.Binding;
                productoOriginal.Brand = producto.Brand;
                productoOriginal.ClothingSize = producto.ClothingSize;
                productoOriginal.Color = producto.Color;
                productoOriginal.CurrencyCode = producto.CurrencyCode;
                productoOriginal.Department = producto.Department;
                productoOriginal.EAN = producto.EAN;
                productoOriginal.FormattedPrice = producto.FormattedPrice;
                productoOriginal.isAdultProduct = producto.isAdultProduct;
                productoOriginal.isAutographed = producto.isAutographed;
                productoOriginal.isMemorabilia = producto.isMemorabilia;
                productoOriginal.Label = producto.Label;
                productoOriginal.LargeImage = producto.LargeImage;
                productoOriginal.LegalDisclaimer = producto.LegalDisclaimer;
                productoOriginal.Manufacture = producto.Manufacture;
                productoOriginal.MediumImage = producto.MediumImage;
                productoOriginal.Model = producto.Model;
                productoOriginal.MPN = producto.MPN;
                productoOriginal.NumberItems = producto.NumberItems;
                productoOriginal.Offers = producto.Offers;
                productoOriginal.PackageQuantity = producto.PackageQuantity;
                productoOriginal.PartNumber = producto.PartNumber;
                productoOriginal.ProdcutTypeName = producto.ProdcutTypeName;
                productoOriginal.ProductGroup = producto.ProductGroup;
                productoOriginal.Publisher = producto.Publisher;
                productoOriginal.ReleaseDate = producto.ReleaseDate;
                productoOriginal.SalesRank = producto.SalesRank;
                productoOriginal.Size = producto.Size;
                productoOriginal.SmallImage = producto.SmallImage;
                productoOriginal.Studio = producto.Studio;
                productoOriginal.Title = producto.Title;
                productoOriginal.UPC = producto.UPC;
                productoOriginal.Actualizacion = true;
                productoOriginal.Netsuite = producto.Netsuite;
                productoOriginal.IsUpdated = producto.IsUpdated;
                unidadDeTrabajo.Producto.actualizar(productoOriginal);
                unidadDeTrabajo.guardarCambios();
            }
        }

        public static void ActualizarBandera(string asin, bool actualizar)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                var productoOriginal = unidadDeTrabajo.Producto.Obtener(asin);
                productoOriginal.Actualizacion = actualizar;
                unidadDeTrabajo.Producto.actualizar(productoOriginal);
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}