using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{



    public class LocationDate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string LocID { get; set; }
        public BsonDateTime Date { get; set; }
        public Citizen[] Citizens { get; set; }
    }

  

  
}
