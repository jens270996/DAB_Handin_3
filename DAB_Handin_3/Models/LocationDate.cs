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
        public int LocID { get; set; }

        [BsonElement]
        [BsonDateTimeOptions(Kind = System.DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public List<Citizen> Citizens { get; set; }
    }

  

  
}
