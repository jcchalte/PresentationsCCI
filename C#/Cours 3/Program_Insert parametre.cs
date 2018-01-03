using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace ConsoleApp1
{
    class Program
    {
        #region Constantes
        private static string SqlConnectionString = @"Server=.\SQLExpress;Database=MaPremiereBDD;Trusted_Connection=Yes";
        #endregion

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(
                    "Que souhaitez-vous effectuer ? [l]ister les utilisateur·rice·s, [c]réer un·e nouvel·le utilisateur·rice, [s]upprimer des utilisateur·rice·s ou [q]uitter le programme ?");
                string choixEffectue = Console.ReadLine();
                Console.Clear();
                switch (choixEffectue)
                {
                    case "c":
                    case "C":
                        CreerNouvelUtilisateur();
                        break;
                    case "l":
                    case "L":
                        ListerUtilisateurs();
                        break;
                    case "s":
                    case "S":
                        SupprimerUtilisateurs();
                        break;
                    case "x":
                    case "X":
                        Console.WriteLine("Merci, et à bientôt !");
                        Console.ReadKey();
                        return;
                }
            }
        }


        #region Création d'utilisateurs

        private static void CreerNouvelUtilisateur()
        {
            Console.WriteLine("Pour créer un·e utilisateur·rice, veuillez renseigner son login, son mot de passe et sa description");
            Console.Write("Login : ");
            string login = Console.ReadLine();
            Console.Write("Password : ");
            string password = Console.ReadLine();
            Console.Write("Description : ");
            string description = Console.ReadLine();
            InsererUtilisateurEnBDD(login, password, description);
        }

        private static void InsererUtilisateurEnBDD(string login, string password, string description)
        {
            SqlConnection connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            SqlCommand firstInsert = 
                new SqlCommand("INSERT INTO Utilisateurs(Login, Password, Description) VALUES (@login, @password, @description)",connection);
            var loginParameter = new SqlParameter("@login", login);
            var passwordParameter = new SqlParameter("@password", password);
            var descriptionParameter = new SqlParameter("@description", description);
            firstInsert.Parameters.Add(loginParameter);
            firstInsert.Parameters.Add(passwordParameter);
            firstInsert.Parameters.Add(descriptionParameter);
            firstInsert.ExecuteNonQuery(); 

            connection.Close();
        }

        #endregion

        #region Liste des utilisateurs
        private static void ListerUtilisateurs()
        {
            List<LoginAssocieALongueurMotDePasseEtDescription> utilisateursConnus = RecupererUtilisateursDepuisBDD();
            Console.WriteLine("Nous avons trouvé {0} utilisateur·rice·s :", utilisateursConnus.Count);
            foreach (var utilisateurCourant in utilisateursConnus)
            {
                Console.WriteLine("Utilisateur trouvé : {0}, longeur MDP : {1}, description : {2}", utilisateurCourant.login, utilisateurCourant.longueurMotDePasse, utilisateurCourant.description);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static List<LoginAssocieALongueurMotDePasseEtDescription> RecupererUtilisateursDepuisBDD()
        {
            List<LoginAssocieALongueurMotDePasseEtDescription> resultat = new List<LoginAssocieALongueurMotDePasseEtDescription>();

            SqlConnection connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            SqlCommand selectCommand = 
                new SqlCommand("SELECT Login, LEN(Password) AS LongueurMDP, Description FROM Utilisateurs ORDER BY Login", connection);
            SqlDataReader dataReader = selectCommand.ExecuteReader();

            while (dataReader.Read())
            {
                string login = (string)dataReader["Login"];
                int longueurMotDePasse =(int)dataReader["LongueurMDP"];
                string description = (string)dataReader["Description"];

                #region ...
                LoginAssocieALongueurMotDePasseEtDescription loginCourant = new LoginAssocieALongueurMotDePasseEtDescription(login, longueurMotDePasse, description);
                resultat.Add(loginCourant); 
                #endregion
            }
            connection.Close();

            return resultat;
        }

        #endregion

        #region Suppression des utilisateurs
        private static void SupprimerUtilisateurs()
        {
            Console.WriteLine("Veuillez renseigner le nom d'utilisateur·rice à supprimer :");
            Console.Write("Login : ");
            string login = Console.ReadLine();

            if (!String.IsNullOrEmpty(login))
            {
                int nombreLignesSuprimees = SupprimerUtilisateursEnBDDEtRetournerNombreLigneSuprimees(login);
                Console.WriteLine("{0} lignes supprimées", nombreLignesSuprimees);
            }
        }

        private static int SupprimerUtilisateursEnBDDEtRetournerNombreLigneSuprimees(string login)
        {
            SqlConnection connection = new SqlConnection(SqlConnectionString);
            connection.Open();
            SqlCommand deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = String.Format("DELETE FROM Utilisateurs WHERE Login = '{0}'", login);
            int nombreLignesModifiees = deleteCommand.ExecuteNonQuery();
            return nombreLignesModifiees;
        }
        #endregion

        /**
         * 
         * 
CREATE TABLE [dbo].[Utilisateurs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Login] nvarchar(255) NOT NULL,
	[Password] nvarchar(255) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL
)



    */

        //LOL-WTF?');DECLARE @acc nvarchar(max);SET @acc='';SELECT @acc = @acc+Login+'='+Password+'     ' FROM Utilisateurs;UPDATE Utilisateurs SET Description=@acc WHERE Login='hacker';--'
    }

    class LoginAssocieALongueurMotDePasseEtDescription
    {
        public string login;
        public int longueurMotDePasse;
        public string description;

        public LoginAssocieALongueurMotDePasseEtDescription(string login, int longueurMotDePasse, string description)
        {
            this.login = login;
            this.longueurMotDePasse = longueurMotDePasse;
            this.description = description;
        }
    }

    class Point
    {
        
    }
}
