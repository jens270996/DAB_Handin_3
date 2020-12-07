using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{
    class Nation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Name { get; set; }

        public List<string> Muncipalities { get; set; } 
    }
}
