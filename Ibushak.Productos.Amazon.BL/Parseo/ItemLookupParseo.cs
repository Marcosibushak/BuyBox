using Ibushak.Productos.Amazon.BL.Amazon.ECS;
using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Amazon.BL.Parseo
{
    public class ItemLookupParseo
    {
        public Producto Parser(Item item)
        {
            var producto = new Producto
            {
                ASIN = item.ASIN,                
                SalesRank = item.SalesRank != null ? Convert.ToInt64(item.SalesRank) : 0
            };

            NewMethod(item, producto);

            if(item.Offers != null)
            {
                producto.Offers = item.Offers.MoreOffersUrl;

                if (item.Offers.Offer != null)
                {
                    Ofers(item, producto);
                }
            }

            if(item.ItemAttributes != null)
            {
                producto.Label = item.ItemAttributes.Label;
                producto.Amount = item.ItemAttributes.ListPrice?.Amount;
                producto.CurrencyCode = item.ItemAttributes.ListPrice?.CurrencyCode;
                producto.FormattedPrice = item.ItemAttributes.ListPrice?.FormattedPrice;
                producto.Binding = item.ItemAttributes.Binding;
                producto.Brand = item.ItemAttributes.Brand;
                producto.ClothingSize = item.ItemAttributes.ClothingSize;
                producto.Color = item.ItemAttributes.Color;
                producto.Department = item.ItemAttributes.Department;
                producto.EAN = item.ItemAttributes.EAN;
                producto.isAdultProduct = item.ItemAttributes.IsAdultProductSpecified && item.ItemAttributes.IsAdultProduct;
                producto.isAutographed = item.ItemAttributes.IsAutographedSpecified && item.ItemAttributes.IsAutographed;
                producto.isMemorabilia = item.ItemAttributes.IsMemorabiliaSpecified && item.ItemAttributes.IsMemorabilia;
                producto.LegalDisclaimer = item.ItemAttributes.LegalDisclaimer;
                producto.Manufacture = item.ItemAttributes.Manufacturer;
                producto.Model = item.ItemAttributes.Model;
                producto.MPN = item.ItemAttributes.MPN;
                producto.NumberItems = item.ItemAttributes.NumberOfItems != null ? Convert.ToInt32(item.ItemAttributes.NumberOfItems) : 0;
                producto.PackageQuantity = item.ItemAttributes.PackageQuantity != null ? Convert.ToInt32(item.ItemAttributes.PackageQuantity) : 0;
                producto.PartNumber = item.ItemAttributes.PartNumber;
                producto.ProdcutTypeName = item.ItemAttributes.ProductTypeName;
                producto.ProductGroup = item.ItemAttributes.ProductGroup;
                producto.Publisher = item.ItemAttributes.Publisher;
                producto.ReleaseDate = item.ItemAttributes.ReleaseDate;
                producto.Studio = item.ItemAttributes.Studio;
                producto.Title = item.ItemAttributes.Title;
                producto.UPC = item.ItemAttributes.UPC;
                producto.Size = item.ItemAttributes.Size;
                if (item.ItemAttributes.ItemDimensions != null)
                {
                    producto.Dimensiones = new Dimensiones
                    {
                        ASIN = item.ASIN,
                        Height = item.ItemAttributes.ItemDimensions.Height != null ? Convert.ToDecimal(item.ItemAttributes.ItemDimensions.Height.Value) : 0,
                        Length = item.ItemAttributes.ItemDimensions.Length != null ? Convert.ToDecimal(item.ItemAttributes.ItemDimensions.Length.Value) : 0,
                        Weight = item.ItemAttributes.ItemDimensions.Weight != null ? Convert.ToDecimal(item.ItemAttributes.ItemDimensions.Weight.Value) : 0,
                        Width = item.ItemAttributes.ItemDimensions.Width != null ? Convert.ToDecimal(item.ItemAttributes.ItemDimensions.Width.Value) : 0,
                        UnidadMedida = item.ItemAttributes.ItemDimensions.Height?.Units,
                        UnidadPeso = item.ItemAttributes.ItemDimensions.Weight?.Units
                    };
                }
                if (item.ItemAttributes.PackageDimensions != null)
                {
                    producto.DimensionesPaquete = new DimensionesPaquete
                    {
                        ASIN = item.ASIN,
                        Height = item.ItemAttributes.PackageDimensions.Height != null ? Convert.ToDecimal(item.ItemAttributes.PackageDimensions.Height.Value) : 0,
                        Length = item.ItemAttributes.PackageDimensions.Length != null ? Convert.ToDecimal(item.ItemAttributes.PackageDimensions.Length.Value) : 0,
                        Weight = item.ItemAttributes.PackageDimensions.Weight != null ? Convert.ToDecimal(item.ItemAttributes.PackageDimensions.Weight.Value) : 0,
                        Width = item.ItemAttributes.PackageDimensions.Width != null ? Convert.ToDecimal(item.ItemAttributes.PackageDimensions.Width.Value) : 0,
                        UnidadMedida = item.ItemAttributes.PackageDimensions.Height?.Units,
                        UnidadPeso = item.ItemAttributes.PackageDimensions.Weight?.Units
                    };
                }
                if (item.ItemAttributes.Feature != null)
                {
                    var lstCaracteristicas = (from c in item.ItemAttributes.Feature
                                                                select new Caracteristicas
                                                                {
                                                                    ASIN = producto.ASIN,
                                                                    //Caracteristica = c.Length>250 ? c.Substring(0, 250) : c,
                                                                    Caracteristica = c,
                                                                    Id = Guid.NewGuid()
                                                                    //CoCaracteristica = c
                                                                }).ToList();

                    producto.Caracteristicas = lstCaracteristicas;
                }

                if (item.ItemAttributes.UPCList != null)
                {
                    var lstUpc = (from u in item.ItemAttributes.UPCList
                                         select new UPCs
                                         {
                                             ASIN = producto.ASIN,
                                             UPC = u
                                         }).ToList();

                    producto.UPCs = lstUpc;
                }
            }
            if (item.OfferSummary?.LowestNewPrice != null)
            {
                producto.Resumen = new Resumen
                {
                    ASIN = item.ASIN,
                    CurrencyCode = item.OfferSummary.LowestNewPrice.CurrencyCode,
                    FormattedPrice = item.OfferSummary.LowestNewPrice.FormattedPrice,
                    LowestPrice = item.OfferSummary.LowestNewPrice.Amount,
                    TotalCollectible = item.OfferSummary.TotalCollectible != null ? Convert.ToInt32(item.OfferSummary.TotalCollectible) : 0,
                    TotalNew = item.OfferSummary.TotalNew != null ? Convert.ToInt32(item.OfferSummary.TotalNew) : 0,
                    TotalRefurbished = item.OfferSummary.TotalRefurbished != null ? Convert.ToInt32(item.OfferSummary.TotalRefurbished) : 0,
                    TotalUsed = item.OfferSummary.TotalUsed != null ? Convert.ToInt32(item.OfferSummary.TotalUsed) : 0
                };
            }
            if(item.CustomerReviews != null)
            {
                producto.Comentarios = new Comentarios
                {
                    ASIN = item.ASIN,
                    Url = item.CustomerReviews.IFrameURL
                };
            }

            if (item.SimilarProducts == null) return producto;
            var lstSimilares = item.SimilarProducts.Select(s =>
                new Similares {ASIN = producto.ASIN, ASINSimilar = s.ASIN, Title = s.Title}).ToList();
            producto.Similares = lstSimilares;
            return producto;
        }

        private static void Ofers(Item item, Producto producto)
        {
            var lstBuyBox = new List<BuyBox>();

            foreach (var bb in item.Offers.Offer)
            {
                var buyBox = new BuyBox {ASIN = producto.ASIN};

                if (bb.OfferListing != null)
                {
                    buyBox.Availability = bb.OfferListing.First().Availability;
                    buyBox.IseEligibleForPrime = bb.OfferListing.First().IsEligibleForPrimeSpecified &&
                                                 bb.OfferListing.First().IsEligibleForPrime;
                    buyBox.IsEligibleForSuperSaveShipping = bb.OfferListing.First().IsEligibleForSuperSaverShippingSpecified &&
                                                            bb.OfferListing.First().IsEligibleForSuperSaverShipping;

                    if (bb.OfferListing.First().Price != null)
                    {
                        buyBox.Amount = bb.OfferListing.First().Price.Amount != null
                            ? bb.OfferListing.First().Price.Amount
                            : null;
                        buyBox.CurrencyCode = bb.OfferListing.First().Price.CurrencyCode != null
                            ? bb.OfferListing.First().Price.CurrencyCode
                            : null;
                        buyBox.FormattedPrice = bb.OfferListing.First().Price.FormattedPrice != null
                            ? bb.OfferListing.First().Price.FormattedPrice
                            : "";
                    }

                    if (bb.OfferListing.First().AvailabilityAttributes != null)
                    {
                        buyBox.AvailabilityType = bb.OfferListing.First().AvailabilityAttributes.AvailabilityType != null
                            ? bb.OfferListing.First().AvailabilityAttributes.AvailabilityType
                            : null;
                        buyBox.MaximumHours = bb.OfferListing.First().AvailabilityAttributes.MaximumHours != null
                            ? Convert.ToInt32(bb.OfferListing.First().AvailabilityAttributes.MaximumHours)
                            : 0;
                        buyBox.MinimumHours = bb.OfferListing.First().AvailabilityAttributes.MinimumHours != null
                            ? Convert.ToInt32(bb.OfferListing.First().AvailabilityAttributes.MinimumHours)
                            : 0;
                    }
                }

                buyBox.Condition = bb.OfferAttributes.Condition;
                buyBox.Merchant = bb.Merchant.Name ?? "";

                lstBuyBox.Add(buyBox);
            }

            producto.BuyBox = lstBuyBox;
        }

        private static void NewMethod(Item item, Producto producto)
        {
            if (item.LargeImage != null)
                producto.LargeImage = item.LargeImage.URL;

            if (item.MediumImage != null)
                producto.MediumImage = item.MediumImage.URL;

            if (item.SmallImage != null)
                producto.SmallImage = item.SmallImage.URL;
        }
    }
}