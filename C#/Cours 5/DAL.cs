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
            //La prochaine lignes est l� pour simuler l'ID auto incr�ment� en BDD
            //L'ID est calcul� lors de l'insertion en BDD et consiste en l'ID maximum +1.
            int prochainIDDisponible = GetProchainIDLivreDisponible();

            //Puis on cr�� les donn�es en BDD � partir des donn�es fournies.
            LivreSQL livreEnBDD = new LivreSQL();
            livreEnBDD.ID = prochainIDDisponible;
            livreEnBDD.Titre = livre.Titre;
            livreEnBDD.Auteur =livre.Auteur;
            livreEnBDD.NombreDePages = livre.NombreDePages;
            livreEnBDD.DateEdition = livre.DateEdition;

            //On les rajoute dans la table "Livre"
            _tableLivres.Add(livreEnBDD);

            //Petite subtilit�, j'assigne l'ID calcul� et inject� en BDD � la propri�t� "ID" du livre sp�cifi�
            //Cela facilite grandement la manipulation par la suite.
            livre.ID = prochainIDDisponible;

            //On retourne l'ID que l'on a ins�r�
            return prochainIDDisponible;
        }


        public List<Livre> GetAllLivres()
        {
            //On construit la liste qui contiendra tous les r�sultats
            List<Livre> resultat = new List<Livre>();

            //TODO : � vous d'it�rer sur toutes les lignes en BDD pour reconstruire les livres les uns apr�s les autres et les rajouter dans resultat

            //Puis on retourne la liste
            return resultat;
        }

        public Livre GetLivreByID(int id)
        {
            //TODO : Ici, il s'agit d'it�rer sur toutes les lignes en BDD
            //TODO : Si l'une d'entre elle a l'ID que l'on a sp�cifi�, on construit une instance de Livre � partir des informations en BDD
            //TODO : Si l'on n'a rien trouv�, on retourne "null".

            //Ce code est � supprimer
            throw new NotImplementedException();
        }

        public void UpdateLivre(Livre livre)
        {
            //TODO : Ici, il faut it�rer sur toutes les lignes en BDD
            //TODO : Si l'une d'entre elle a l'ID du livre sp�cifi�, on modifie les valeurs de ses champs pour correspondre au livre sp�cifi�.

            //TODO : Ce code est � supprimer
            throw new NotImplementedException();
        }

        public void DeleteLivre(int id)
        {
            //TODO : Ici, il faut it�rer sur toutes les lignes en BDD
            //TODO : Si l'on trouve un livre correspondant, on l'enl�ve de la table en BDD

            //TODO : Ce code est � supprimer
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