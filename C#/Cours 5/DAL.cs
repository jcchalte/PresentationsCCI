using System;
using System.Collections.Generic;

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

            //TODO : à vous d'itérer sur toutes les lignes en BDD pour reconstruire les livres les uns après les autres et les rajouter dans resultat

            //Puis on retourne la liste
            return resultat;
        }

        public Livre GetLivreByID(int id)
        {
            //TODO : Ici, il s'agit d'itérer sur toutes les lignes en BDD
            //TODO : Si l'une d'entre elle a l'ID que l'on a spécifié, on construit une instance de Livre à partir des informations en BDD
            //TODO : Si l'on n'a rien trouvé, on retourne "null".

            //Ce code est à supprimer
            throw new NotImplementedException();
        }

        public void UpdateLivre(Livre livre)
        {
            //TODO : Ici, il faut itérer sur toutes les lignes en BDD
            //TODO : Si l'une d'entre elle a l'ID du livre spécifié, on modifie les valeurs de ses champs pour correspondre au livre spécifié.

            //TODO : Ce code est à supprimer
            throw new NotImplementedException();
        }

        public void DeleteLivre(int id)
        {
            //TODO : Ici, il faut itérer sur toutes les lignes en BDD
            //TODO : Si l'on trouve un livre correspondant, on l'enlève de la table en BDD

            //TODO : Ce code est à supprimer
            throw new NotImplementedException();
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