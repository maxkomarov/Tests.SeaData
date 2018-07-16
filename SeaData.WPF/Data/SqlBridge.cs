using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace SeaData.WPF.Data
{
    public class SqlBridge
    {
        private static SqlBridge sqlBridge = new SqlBridge();
        private string connectionString;

        private SqlBridge()
        {
            connectionString = Properties.Settings.Default.ConnectionString;
        }

        /// <summary>
        /// Возвращает единственный экземляр бриджа
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlBridge GetInstance()
        {
            return sqlBridge;
        }

        #region Методы для Customers
        /// <summary>
        /// Инициализирует экземпляр клиента записью из базы
        /// У экземпляра должно быть предустановлено значение Customer.Id
        /// </summary>
        /// <param name="account">Инициализируемый экземпляр клиента</param>
        /// <returns></returns>
        public Models.Customer Get(Models.Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "sp_CustomersGet";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter()
                    {
                        Value = customer.Id,
                        ParameterName = "@id",
                        Direction = System.Data.ParameterDirection.Input
                    };
                    cmd.Parameters.Add(param);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customer.Name = reader.GetSqlString(1).ToString();
                            customer.Inn = reader.GetSqlString(2).ToString();
                            customer.Address = !reader.IsDBNull(3) ? reader.GetSqlString(3).ToString() : string.Empty;
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            return customer;
        }

        /// <summary>
        /// Возвращает список экземпляров клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        public List<Models.Customer> GetCustomers()
        {
            List<Models.Customer> customers = new List<Models.Customer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "sp_CustomersGet";

                    SqlParameter param = new SqlParameter()
                    {
                        Value = 0,
                        ParameterName = "@id",
                        Direction = System.Data.ParameterDirection.Input
                    };
                    cmd.Parameters.Add(param);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Models.Customer customer = new Models.Customer();

                            customer.Id = (int)reader.GetSqlInt32(0);
                            customer.Name = reader.GetSqlString(1).ToString();
                            customer.Inn = reader.GetSqlString(2).ToString();
                            customer.Address = !reader.IsDBNull(3) ? reader.GetSqlString(3).ToString() : string.Empty;

                            customers.Add(customer);
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            return customers;
        }

        /// <summary>
        /// Обновляет или добавляет в базу запись о клиенте
        /// </summary>
        /// <param name="customer">Экземпляр обновляемого/добавляемого клиента</param>
        public void Update(Models.Customer customer)
        {
            int rc = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "sp_CustomersUpdate";
                        cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
                        cmd.Parameters.Add(new SqlParameter("@inn", customer.Inn));
                        cmd.Parameters.Add(new SqlParameter("@name", customer.Name));
                        cmd.Parameters.Add(new SqlParameter("@address", customer.Address));

                        rc = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("IX_Customers_Inn"))
                        MessageBox.Show("Доваление или изменение невозможно - контрагент с таким ИНН уже существует.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    else
                        MessageBox.Show($"Ошибка при работе с базой данных: {e.Message}");
                    
                }
                finally { conn.Close(); }
            }
        }

        /// <summary>
        /// Удаляет из базы запись о клиенте
        /// </summary>
        /// <param name="customer">Экземпляр удаляемого клиента</param>
        public void Delete(Models.Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "sp_CustomersDelete";
                        cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("FK_AccountsOfCustomers"))
                        MessageBox.Show("Удаление невозможно - присутствуют связанные с контрагентом счета.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    else
                        MessageBox.Show($"Ошибка при работе с базой данных: {e.Message}");
                }
                finally { conn.Close(); }
            }

        }
        #endregion

        #region Методы для Accounts

        /// <summary>
        /// Инициализирует экземпляр счета записью из базы
        /// У экземпляра должно быть предустановлено значение Account.Id
        /// </summary>
        /// <param name="account">Инициализируемый экземпляр счета</param>
        /// <returns></returns>
        public Models.Account Get(Models.Account account)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "sp_AccountGet";
                    cmd.Parameters.Add(new SqlParameter("@id", account.Id));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            account.Owner = Get(new Models.Customer((int)reader.GetSqlInt32(1)));
                            account.Number = reader.GetSqlString(2).ToString();
                            account.BIC = reader.GetSqlString(3).ToString();
                            account.Saldo = (decimal)reader.GetSqlDecimal(4);
                            account.Name = !reader.IsDBNull(5) ? reader.GetSqlString(5).ToString() : string.Empty;
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            return account;
        }

        /// <summary>
        /// Возвращает список экземпляров счетов
        /// </summary>
        /// <param name="customer">Клиент, счета которого должны быть возвращены. Если равно null или отсутствует, будут возвращены все счета</param>
        /// <returns>Список счетов</returns>
        public List<Models.Account> GetAccounts(Models.Customer customer = null)
        {
            List<Models.Account> accounts = new List<Models.Account>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "sp_AccountsGet";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter()
                        {
                            Value = customer == null ? 0 : customer.Id,
                            ParameterName = "@customerId",
                            Direction = System.Data.ParameterDirection.Input
                        };
                        cmd.Parameters.Add(param);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Models.Account account = new Models.Account();
                                account.Id = (int)reader.GetSqlInt32(0);
                                account.Owner = Get(new Models.Customer((int)reader.GetSqlInt32(1)));
                                account.Number = reader.GetSqlString(2).ToString();
                                account.BIC = reader.GetSqlString(3).ToString();
                                account.Saldo = (decimal)reader.GetSqlDecimal(4);
                                account.Name = !reader.IsDBNull(5) ? reader.GetSqlString(5).ToString() : string.Empty;

                                accounts.Add(account);
                            }
                            reader.Close();
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            { }
            return accounts;
        }

        /// <summary>
        /// Добавляет или обновляет запись о счете
        /// </summary>
        /// <param name="account">Экземпляр обновляемого счета</param>
        public void Update(Models.Account account)
        {
            int rc = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "sp_AccountsUpdate";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", account.Id));
                        cmd.Parameters.Add(new SqlParameter("@customerId", account.Owner.Id));
                        cmd.Parameters.Add(new SqlParameter("@number", account.Number));
                        cmd.Parameters.Add(new SqlParameter("@name", account.Name));
                        cmd.Parameters.Add(new SqlParameter("@bic", account.BIC));
                        cmd.Parameters.Add(new SqlParameter("@saldo", account.Saldo));

                        rc = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("IX_Accounts_Unique"))
                        MessageBox.Show("Доваление или изменение невозможно - счет с таким номером для данного БИК уже существует.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    else
                        MessageBox.Show($"Ошибка при работе с базой данных: {e.Message}");

                }
                finally { conn.Close(); }
            }
        }

        /// <summary>
        /// Удаляет из базы запись о счете
        /// </summary>
        /// <param name="customer">Экземпляр удаляемого счета</param>
        public void Delete(Models.Account account)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "sp_AccountsDelete";
                    cmd.Parameters.Add(new SqlParameter("@id", account.Id));
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        #endregion
    }
}
