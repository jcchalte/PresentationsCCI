using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace UnitTestProject1
{
    public class DAL
    {
        private static List<LivreSQL> _tableLivres;

        static DAL()
        {
            _tableLivres = new List<LivreSQL>();
        }

        public int InsertLivre(Livre livre)
        {
            //La prochaine lignes est là pour simuler l'ID auto incrémenté en BDD
            //L'ID est calculé lors de l'insertion en BDD et consiste en l'ID maximum +1.
            int prochainIDDisponible = GetProchainIDLivreDisponible();

            //Puis on créé les données en BDD à partir des données fournies.
            LivreSQL livreEnBDD = new LivreSQL();
            livreEnBDD.ID = prochainIDDisponible;
            livreEnBDD.Titre = livre.Titre;
            livreEnBDD.Auteur =livre.Auteur;
            livreEnBDD.NombreDePages = livre.NombreDePages;
            livreEnBDD.DateEdition = livre.DateEdition;

            //On les rajoute dans la table "Livre"
            _tableLivres.Add(livreEnBDD);

            //Petite subtilité, j'assigne l'ID calculé et injecté en BDD à la propriété "ID" du livre spécifié
            //Cela facilite grandement la manipulation par la suite.
            livre.ID = prochainIDDisponible;

            //On retourne l'ID que l'on a inséré
            return prochainIDDisponible;
        }


        public List<Livre> GetAllLivres()
        {
            //On construit la liste qui contiendra tous les résultats
            List<Livre> resultat = new List<Livre>();

            foreach (var livreEnBDD in _tableLivres)
            {
                Livre livre = new Livre(livreEnBDD.ID, livreEnBDD.Titre, livreEnBDD.Auteur, livreEnBDD.NombreDePages, livreEnBDD.DateEdition);
                resultat.Add(livre);
            }

            //Puis on retourne la liste
            return resultat;
        }

        public Livre GetLivreByID(int id)
        {
            LivreSQL livreCorrespondantEnBDD = null;
            foreach (var livreEnBDD in _tableLivres)
            {
                if (livreEnBDD.ID == id)
                {
                    livreCorrespondantEnBDD = livreEnBDD;
                    break;
                }
            }
            if (livreCorrespondantEnBDD != null)
            {
                return new Livre(livreCorrespondantEnBDD.ID, livreCorrespondantEnBDD.Titre,
                    livreCorrespondantEnBDD.Auteur, livreCorrespondantEnBDD.NombreDePages,
                    livreCorrespondantEnBDD.DateEdition);
            }
            else
            {
                return null;
            }
        }

        public void UpdateLivre(Livre livre)
        {
            foreach (var livreEnBDD in _tableLivres)
            {
                if (livreEnBDD.ID == livre.ID)
                {
                    livreEnBDD.Titre = livre.Titre;
                    livreEnBDD.Auteur = livre.Auteur;
                    livreEnBDD.DateEdition = livre.DateEdition;
                    livreEnBDD.NombreDePages = livre.NombreDePages;
                    break;
                }
            }
        }

        public void DeleteLivre(int id)
        {
            foreach (var livreEnBDD in _tableLivres)
            {
                if (livreEnBDD.ID == id)
                {
                    _tableLivres.Remove(livreEnBDD);
                    break;
                }
            }
        }


        #region Helpers
        private int GetProchainIDLivreDisponible()
        {
            var plusGrandIDDejaEnBDD = GetPlusGrandLivbreIDEnBDD();
            int prochainIDDisponible = plusGrandIDDejaEnBDD + 1;
            return prochainIDDisponible;
        }

        private int GetPlusGrandLivbreIDEnBDD()
        {
            int plusGrandIDDejaEnBDD = 0;
            foreach (var livreEnBDD in _tableLivres)
            {
                if (livreEnBDD.ID > plusGrandIDDejaEnBDD)
                    plusGrandIDDejaEnBDD = livreEnBDD.ID;
            }
            return plusGrandIDDejaEnBDD;
        } 
        #endregion
    }
}