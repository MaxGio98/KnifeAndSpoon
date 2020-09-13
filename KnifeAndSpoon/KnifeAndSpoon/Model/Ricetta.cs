using Plugin.CloudFirestore;
using System.Collections.Generic;
using Plugin.CloudFirestore.Attributes;

namespace KnifeAndSpoon.Model
{
    public class Ricetta
    {
        [Id]
        public string Id { get; set; }

        public string Autore { get; set; }
        public string Categoria { get; set; }
        public string NumeroPersone { get; set; }
        public string TempoPreparazione { get; set; }
        public string Thumbnail { get; set; }
        public string Titolo { get; set; }
        public Timestamp Timestamp { get; set; }
        public bool isApproved { get; set; }
        public List<string> Passaggi { get; set; }
        public List<IDictionary<string, object>> Ingredienti { get; set; }


    }
}