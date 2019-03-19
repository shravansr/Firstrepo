using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SampleApi.Models
{
    public class Class1
    {
        [DataContract]
        public class UserDetails
        {
            [DataMember]
            public string UserName { get; set; }
            [DataMember]
            public string Password { get; set; }
            [DataMember]
            public string Country { get; set; }
            [DataMember]
            public string MobileNumber { get; set; }

        }

        public class UserResponse
        {
            [DataMember]
            public string Status { get; set; }
            [DataMember]
            public string StatusMessage { get; set; }            
        }

        public class Products
        {
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public decimal Price { get; set; }
            [DataMember]
            //public string type { get; set; }
           // [DataMember]
            public DateTime created_date { get; set; }

        }


        public class AllProducts
        {
            [DataMember]
            public List<Products> Products { get; set; }
        }

        public class AllProductsDynamic
        {
            [DataMember]
            public List<dynamic> Products { get; set; }
        }


    }
}