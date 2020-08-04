using Plugin.CloudFirestore;
using System.Collections.Generic;

namespace KnifeAndSpoon.Model
{
    public class Ricetta
    {
        public string Autore { get; set; }
        public string Categoria { get; set; }
        public string NumeroPersone { get; set; }
        public string TempoPreparazione { get; set; }
        public string Thumbnail { get; set; }
        public string Titolo { get; set; }
        public Timestamp TimeStamp { get; set; }
        public bool isApproved { get; set; }
        public List<string> Passaggi { get; set; }
        public List<IDictionary<string, object>> Ingredienti { get; set; }


    }
}