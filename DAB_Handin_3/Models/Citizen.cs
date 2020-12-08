
/*{ "_id":{
 * "$oid":"5fbcfc2b43ab601c286048c2"},
 * "SSN":"270996-1240",
 * "FirstName":"Jens",
 * "LastName":"Kristensen",
 * "Age":24,
 * "Sex":"Male",
 * "Muni":"Aarhus",
 * "Tests":[{ "Date":"2020-11-24","TC":"KoldingTC","Res":"0","Status":"Fin"},{ "Date":"2020-12-24","TC":"KoldingTC","Res":"1","Status":"Bad"}],
 * "ID":"1"}*/
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DAB_Handin_3.Models
{
    public class Citizen
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string SSN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public int Muni { get; set; }
        public List<Test> Tests { get; set; }
        public int ID { get; set; }
        
        public Citizen(int id, string forNavn, string efterNavn, string SSN1, int alder, string køn, int municipality)
        {
            ID = id;
            FirstName = forNavn;
            LastName = efterNavn;
            SSN = SSN1;
            Age = alder;
            Sex = køn;
            Tests = new List<Test>();

            Muni = municipality;
        }
        public Citizen()
        {
            Tests = new List<Test>();
        }
    }
    
    

    public class Test
    {
        [BsonElement]
        [BsonDateTimeOptions(Kind =System.DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public string TC { get; set; }
        public string Res { get; set; }
        public string Status { get; set; }
    }
}
