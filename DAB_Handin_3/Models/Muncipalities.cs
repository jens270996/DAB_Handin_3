using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{
    public class Municipality
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int Population { get; set; }
        public string Name { get; set; }
        public string Nation{ get; set; }
        public List<string> Citizens { get; set; }
        public List<string>  TCS { get; set; }

        public int ID { get; set; }

        public Municipality(int ID1, string Name1, int CItz)
        {
            ID = ID1;
            Name = Name1;
            Population = CItz;

        }

    }
}
