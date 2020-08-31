namespace KnifeAndSpoon.Model
{
    class Passaggio
    {
        public int Numero { get; set; }
        public string Testo { get; set; }

        public Passaggio(int num, string text)
        {
            Numero = num;
            Testo = text;
        }
    }
}
