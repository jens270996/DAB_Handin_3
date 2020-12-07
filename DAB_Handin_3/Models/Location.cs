﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int _id { get; set; }
        
        public int id { get; set; }
        public string AddressName { get; set; }
        public string Muni { get; set; }
        public string City { get; set; }
        public int PostNr { get; set; }
    }
}
