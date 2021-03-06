﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Plugin.CloudFirestore.Attributes;

namespace KnifeAndSpoon.Model
{
    public class Utente
    {
        [Id]
        public string Id { get; set; }
        public string Immagine { get; set; }
        public string Mail { get; set; }
        public string Nome { get; set; }
        public bool isAdmin { get; set; }
        public List<string> Preferiti { get; set; }
    }
}
