using servicios = Ibushak.Productos.Amazon.BL.Servicios;

namespace Ibushak.Productos.Amazon.Task
{
    class Program
    {
        static void Main(string[] args)
        {
            var oProductosServicios = new servicios.Productos();
            oProductosServicios.ProcesoProductos();

            //Pruebas
            //BL.Webservice.SuiteTalk.STHelper helper = new BL.Webservice.SuiteTalk.STHelper();
            //helper.obtenerId("3606802566828");

            //servicios.Archivos archivos = new servicios.Archivos();
            //archivos.generarProductosActualizados();
        }
    }
}