using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Handin_3.Models
{
    public class TestCenter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string Name { get; set; }

        public int ID { get; set;}

        public int OpenHour { get; set; }
        public int CloseHour { get; set; }
        public string Muni { get; set; }

        public TestManagement TestManagement { get; set; }
        
    }

    public class TestManagement
    {
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
