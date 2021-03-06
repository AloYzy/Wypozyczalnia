﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wypozyczalnia.Models
{
    /// <summary>
    /// Class that represents single client.
    /// </summary>
    public class Client
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string LicenseNumber { get; set; }

        /// <summary>
        /// Saves new client to database.
        /// </summary>
        /// <param name="clientName">String that represent client's name.</param>
        /// <param name="clientSurname">String that represent client's surname.</param>
        /// <param name="licenseNumber">String that represent client's license number.</param>
        /// <returns>Client object that represents newly added client.</returns>
        public static Client AddNewClient(string clientName, string clientSurname, string licenseNumber)
        {
            Client client = new Client();
            bool exists = Client.CheckIfClientExists(licenseNumber);

            if (!exists)
            {
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = Helpers.connectionString;
                    sqlConnection.Open();

                    SqlCommand command = new SqlCommand("INSERT INTO klienci (imie, nazwisko, [numer prawa jazdy]) " +
                            $"VALUES(@name, @surname, @licenseNumber)", sqlConnection);

                    command.Parameters.Add(new SqlParameter("name", clientName));
                    command.Parameters.Add(new SqlParameter("surname", clientSurname));
                    command.Parameters.Add(new SqlParameter("licenseNumber", licenseNumber));

                    command.ExecuteNonQuery();
                }
            }

            client = Client.GetClientByLicenceNumber(licenseNumber);

            return client;
        }

        /// <summary>
        /// Reads database and seraches for client id based on given license number.
        /// </summary>
        /// <param name="licenseNumber">String that represent client's license number.</param>
        /// <returns>Integer that represent client record in database.</returns>
        public int GetClientIdByLicenseNumber(string licenseNumber)
        {
            int id = -1;

            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Helpers.connectionString;
                sqlConnection.Open();

                DataTable dataTable = new DataTable();
                var sqlDataAdaper = new SqlDataAdapter($"SELECT * FROM klienci WHERE [Numer prawa jazdy] = '{licenseNumber}'", sqlConnection);
                sqlDataAdaper.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    id = dataTable.Rows[0].Field<int>("Id");
                }
            }

            return id;
        }

        /// <summary>
        /// Reads database and seraches for client based on given license number.
        /// </summary>
        /// <param name="licenseNumber">String that represent client's license number.</param>
        /// <returns>Client object.</returns>
        private static Client GetClientByLicenceNumber(string licenseNumber)
        {
            Client client = new Client();

            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Helpers.connectionString;
                sqlConnection.Open();

                DataTable dataTable = new DataTable();
                var sqlDataAdaper = new SqlDataAdapter($"SELECT * FROM klienci WHERE [Numer prawa jazdy] = '{licenseNumber}'", sqlConnection);
                sqlDataAdaper.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    client.Name = dataTable.Rows[0].Field<string>("Imie");
                    client.Surname = dataTable.Rows[0].Field<string>("Nazwisko");
                    client.LicenseNumber = dataTable.Rows[0].Field<string>("Numer prawa jazdy");
                }
            }

            return client;
        }

        /// <summary>
        /// Reads database and seraches for client based on given id.
        /// </summary>
        /// <param name="id">Integer that represent client record in database.</param>
        /// <returns>Client object.</returns>
        public static Client GetClientById(int id)
        {
            Client client = new Client();

            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Helpers.connectionString;
                sqlConnection.Open();

                DataTable dataTable = new DataTable();
                var sqlDataAdaper = new SqlDataAdapter($"SELECT * FROM klienci WHERE [id] = '{id}'", sqlConnection);
                sqlDataAdaper.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    client.Name = dataTable.Rows[0].Field<string>("Imie");
                    client.Surname = dataTable.Rows[0].Field<string>("Nazwisko");
                    client.LicenseNumber = dataTable.Rows[0].Field<string>("Numer prawa jazdy");
                }
            }

            return client;
        }

        /// <summary>
        /// Reads database and validates if client with given license number is already present.
        /// </summary>
        /// <param name="licenseNumber">String that represent client's license number.</param>
        /// <returns>True if client with given license number is already present in database, false if client with given license number is not present in database.</returns>
        private static bool CheckIfClientExists(string licenseNumber)
        {
            bool exists = false;

            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Helpers.connectionString;
                sqlConnection.Open();

                SqlCommand query = sqlConnection.CreateCommand();
                query.CommandText = $"SELECT * FROM klienci WHERE [Numer prawa jazdy] = '{licenseNumber}'";
                SqlDataReader dataReader = query.ExecuteReader();
                if (dataReader.HasRows)
                {
                    exists = true;
                }
            }

            return exists;
        }
    }
}
