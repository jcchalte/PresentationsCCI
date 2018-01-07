using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class TesterDAL
    {
        [TestMethod]
        public void UnLivreInsereAUnID()
        {
            //Contexte
            DAL dal = new DAL();
            Livre livre = new Livre("Merci pour ce moment", "Valérie T.", 3, new DateTime(2014,09,04));

            //Action
            dal.InsertLivre(livre);

            //Assertion
            Assert.AreNotEqual(0, livre.ID, "Un livre inséré en BDD doit avoir un ID");
        }

        [TestMethod]
        public void LaDALRenvoitTousLesLivres()
        {
            //Contexte
            DAL dal = new DAL();
            Livre livre = new Livre("Merci pour ce moment", "Valérie T.", 3, new DateTime(2014, 09, 04));
            int idLivreInséré = dal.InsertLivre(livre);

            //Action
            var tousLesLivresEnBDD = dal.GetAllLivres();


            //Assertion
            bool leLivreExisteDansLesLivresEnBDD = false;
            foreach (var livreEnBDD in tousLesLivresEnBDD)
            {
                if (livreEnBDD.ID == idLivreInséré)
                {
                    leLivreExisteDansLesLivresEnBDD = true;
                    break;
                }
            }
            Assert.IsTrue(leLivreExisteDansLesLivresEnBDD,"Un livre inséré en BDD doit être renvoyé par la méthode GetAllLivres");
        }

        [TestMethod]
        public void LaDALRenvoitLeLivreInsere()
        {
            //Contexte
            DAL dal = new DAL();
            Livre livre = new Livre("Merci pour ce moment", "Valérie T.", 3, new DateTime(2014, 09, 04));
            int idLivreInséré = dal.InsertLivre(livre);

            //Action
            Livre livreLuDepuisBDD = dal.GetLivreByID(idLivreInséré);

            //Assertions
            Assert.AreEqual(livreLuDepuisBDD.ID, livre.ID);
            Assert.AreEqual(livreLuDepuisBDD.Titre, livre.Titre);
            Assert.AreEqual(livreLuDepuisBDD.Auteur, livre.Auteur);
            Assert.AreEqual(livreLuDepuisBDD.DateEdition, livre.DateEdition);
            Assert.AreEqual(livreLuDepuisBDD.NombreDePages, livre.NombreDePages);
        }

        [TestMethod]
        public void MettreAJourUnLivreSauvegardeBienLesInformationsEnBDD()
        {
            //Contexte
            DAL dal = new DAL();
            Livre livre = new Livre("Merci pour ce moment", "Valérie T.", 3, new DateTime(2014, 09, 04));
            int idLivreInséré = dal.InsertLivre(livre);

            //action
            livre.DateEdition = new DateTime(2016,12,24);
            dal.UpdateLivre(livre);
            Livre livreLuDepuisBDD = dal.GetLivreByID(idLivreInséré);

            //Assertions
            Assert.AreEqual(livreLuDepuisBDD.DateEdition, new DateTime(2016, 12, 24));
        }
    }
}