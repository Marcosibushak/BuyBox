using System.Collections.Generic;

namespace Ibushak.Productos.Amazon.BL.Model
{
    public class ProductoActualizado
    {
        public string ASIN { get; set; }
        public string UPC { get; set; }
        public List<CampoActualizado> Campos { get; set; }
        public List<BuyBoxActualizado> BuyBox { get; set; }
    }
}