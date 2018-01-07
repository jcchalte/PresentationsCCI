using System;

namespace UnitTestProject1
{
    public class Livre
    {
        public Livre(string titre, string auteur, int nombreDePages, DateTime dateEdition)
        {
            ID = 0;
            Titre = titre;
            Auteur = auteur;
            NombreDePages = nombreDePages;
            DateEdition = dateEdition;
        }

        public Livre(int id, string titre, string auteur, int nombreDePages, DateTime dateEdition)
        {
            ID = id;
            Titre = titre;
            Auteur = auteur;
            NombreDePages = nombreDePages;
            DateEdition = dateEdition;
        }

        public int ID { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }
        public int NombreDePages { get; set; }
        public DateTime DateEdition { get; set; }
    }
}