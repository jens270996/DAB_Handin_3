using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{
    class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Addresse { get; set; }
        public Muncipalities muncipalities { get; set; }

        public LocationDate locationDate { get; set; }
    }
}
