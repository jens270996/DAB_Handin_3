using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{
    class Muncipalities
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int Population { get; set; }
        public string Name { get; set; }
        public string Nation{ get; set; }
        public List<string> Citizens { get; set; }
        public List<string>  TCS { get; set; }
        
    }
}
