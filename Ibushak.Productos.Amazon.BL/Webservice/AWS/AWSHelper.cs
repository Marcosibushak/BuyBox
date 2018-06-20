using Ibushak.Productos.Amazon.BL.Amazon.ECS;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Ibushak.Productos.Amazon.BL.Webservice.AWS
{ 
    public class AWSHelper
    {
        AWSECommerceServicePortTypeClient client;
        string AssociateTag;
        string AWSAccessKeyId;

        public AWSHelper(string accessKeyId, string secretKey, string AssociateTag, string AWSAccessKeyId)
        {
            this.AssociateTag = AssociateTag;
            this.AWSAccessKeyId = AWSAccessKeyId;

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.MaxBufferSize = int.MaxValue;
            basicHttpBinding.MaxReceivedMessageSize = 2147483647;
            client = new AWSECommerceServicePortTypeClient(basicHttpBinding,
            new EndpointAddress("https://webservices.amazon.com.mx/onca/soap?Service=AWSECommerceService"));
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(accessKeyId, secretKey));
        }

        public ItemLookupResponse ItemLookUp(List<string> items, ItemLookupRequestIdType id)
        {
            try
            {
                var request = new ItemLookupRequest
                {
                    ItemId = items.ToArray(),
                    IdType = id,
                    IdTypeSpecified = true,
                    ResponseGroup = new string[]
                        {"Large", "OfferFull", "OfferListings", "Offers", "OfferSummary", "Images"}
                };
                //new string[] { "132018245828" };

                if(id != ItemLookupRequestIdType.ASIN)
                    request.SearchIndex = "All";

                var amazon = new ItemLookup
                {
                    AssociateTag = AssociateTag,
                    AWSAccessKeyId = AWSAccessKeyId,
                    Request = new ItemLookupRequest[] {request}
                };
                var resultado = client.ItemLookup(amazon);

                return resultado;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}