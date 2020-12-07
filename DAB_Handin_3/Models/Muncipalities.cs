﻿using System;
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
        public int _id { get; set; }
        public int Population { get; set; }
        public string nation { get; set; }
        public string Nation_Name { get; set; }
        public List<string> citizens { get; set; }
        public List<string>  TestCenter { get; set; }
        public List<string> Location { get; set; }

    }
}
