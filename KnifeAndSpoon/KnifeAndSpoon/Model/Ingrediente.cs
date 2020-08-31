namespace KnifeAndSpoon.Model
{
    class Ingrediente
    {
        public string Nome { get; set; }
        public string Qt { get; set; }
        public string Ut { get; set; }

        public Ingrediente(string nome,string qt, string ut)
        {
            Nome = nome;
            Qt = qt;
            Ut = ut;
        }
    }
}
