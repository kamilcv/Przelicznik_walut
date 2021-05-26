using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Przelicznik_walut
{
    public class Rate
    {
        public string Currency { get; set; }
        public string Code { get; set; }
        public double Mid { get; set; }
    }
    public class Product
    {
        public string table { get; set; }
        public string No { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<Rate> rates { get; set; }

    }

    public class ListaWalut
    {
        public string Kod { get; set; }
        public string nazwa { get; set; }
    }

    public class OstatniKursWalut
    {
        public int ID { get; set; }
        public string Kod { get; set; }
        public DateTime Data { get; set; }
        public double Cena { get; set; }
    }

    class API
    {
        static HttpClient client = new HttpClient();

        static void ShowProduct(List<Product> product)
        {
            for (int i = 0; i < product.Count; i++)
            {
                Console.WriteLine($"Date: {product[i].EffectiveDate}");

                for (int j = 0; j < product[i].rates.Count; j++)
                {
                    Console.WriteLine($"Currency: {product[i].rates[j].Currency}\tCode: {product[i].rates[j].Code}\tPrice: {product[i].rates[j].Mid}");
                }

                Console.WriteLine("\n\n\n\n");
            }
        }

        public static void AktualizujBazeDanych()
        {
            string connStr = "server=localhost;user=root;database=waluty;port=3306";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                // Pobierz ostatnią date i ostatni ID

                string sql = "SELECT MAX(Data), MAX(ID) FROM kursy;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MySqlDataReader rdr = cmd.ExecuteReader();

                DateTime data = new DateTime();
                int ostatni_ID = 1;

                while (rdr.Read())
                {
                    //rozmiar = Convert.ToInt32(rdr[0].ToString());
                    data = Convert.ToDateTime(rdr[0]);
                    data = data.AddDays(1);
                    ostatni_ID = Convert.ToInt32(rdr[1].ToString());
                    ostatni_ID++;
                }

                //Console.WriteLine(data.ToString());

                rdr.Close();

                // Pobranie kodow z bazy danych

                sql = "SELECT Kod FROM Waluty;";

                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();

                //string [] tablica_kodow = new string[rozmiar];

                List<string> tablica_kodow = new List<string>();

                //int licznik = 0;
                while (rdr.Read())
                {
                    tablica_kodow.Add(rdr[0].ToString());
                }
                rdr.Close();

                List<Product> product = RunAsync(data).GetAwaiter().GetResult();

                sql = "INSERT INTO waluty(kod, nazwa) VALUES ";

                bool znaleziono = false;
                int ilu_walut_brakuje = 0;

                for (int i = 0; i < product.Count; i++)
                {
                    for (int j = 0; j < product[i].rates.Count; j++)
                    {
                        for (int o = 0; o < tablica_kodow.Count; o++)
                        {
                            if (tablica_kodow[o] == product[i].rates[j].Code)
                            {
                                znaleziono = true;
                            }
                        }

                        if (znaleziono == false)
                        {
                            sql += "( '" + product[i].rates[j].Code + "', '" + product[i].rates[j].Currency + "'),";

                            ilu_walut_brakuje++;

                            tablica_kodow.Add(product[i].rates[j].Code);
                        }
                        znaleziono = false;
                    }
                }

                if (ilu_walut_brakuje > 0)
                {
                    sql = sql.Remove(sql.Length - 1, 1).Insert(sql.Length - 1, ";");

                    Console.WriteLine(sql);

                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Dodano brakujace waluty, ilosc: " + ilu_walut_brakuje);

                //string sql = "SELECT Kod, Nazwa FROM Waluty";
                sql = "INSERT INTO kursy(ID,Data,Kod,Cena) VALUES ";

                Console.WriteLine("Uzyskano rekordow: " + product.Count);

                for (int i = 0; i < product.Count; i++)
                {
                    sql = "INSERT INTO kursy(ID,Data,Kod,Cena) VALUES ";
                    for (int j = 0; j < product[i].rates.Count; j++)
                    {
                        sql += "( '" + ostatni_ID + "', '" + product[i].EffectiveDate.ToString("yyyy-MM-dd") + "', '" + product[i].rates[j].Code + "', '" + product[i].rates[j].Mid.ToString(CultureInfo.InvariantCulture) + "' ),";
                        ostatni_ID++;
                    }
                    sql = sql.Remove(sql.Length - 1, 1).Insert(sql.Length - 1, ";");
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    //Console.Write(i + " ");
                }

                //Console.WriteLine(sql);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        static async Task<List<Product>> GetProductAsync(string path)
        {
            List<Product> product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<List<Product>>();
                //product.AddRange(product);
            }
            else
            {
                Console.WriteLine("\n\nNie mozna polaczyc sie z serwerem lub brak dzisiejszych danych o kursie walut\n\n");
            }
            return product;
        }

        static async Task<List<Product>> RunAsync(DateTime data)
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://api.nbp.pl/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            DateTime aktualna_data = DateTime.Now.Date;

            string path = "";

            List<Product> product = new List<Product>();

            List<Product> pojedynczy;

            for (; ; )
            {
                if (data.AddDays(90) <= aktualna_data)
                {
                    path = "exchangerates/tables/a/" + data.ToString("yyyy-MM-dd") + "/" + data.AddDays(90).ToString("yyyy-MM-dd");
                    data = data.AddDays(91);

                    //Console.WriteLine(path);

                    pojedynczy = await GetProductAsync(path);
                    product.AddRange(pojedynczy);
                }
                else
                {
                    if (data < aktualna_data)
                    {
                        path = "exchangerates/tables/a/" + data.ToString("yyyy-MM-dd") + "/" + aktualna_data.ToString("yyyy-MM-dd");

                        //Console.WriteLine(path);

                        pojedynczy = await GetProductAsync(path);
                        product.AddRange(pojedynczy);
                    }
                    else if (data == aktualna_data)
                    {
                        path = "exchangerates/tables/a/" + aktualna_data.ToString("yyyy-MM-dd");

                        //Console.WriteLine(path);

                        pojedynczy = await GetProductAsync(path);
                        product.AddRange(pojedynczy);
                    }
                    break;
                }
            }

            return product;
        }
    }

    class MySQL
    {
        public static List<ListaWalut> PobierzListeWalut()
        {
            List<ListaWalut> Lista = new List<ListaWalut>();
            string connStr = "server=localhost;user=root;database=waluty;port=3306";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "SELECT * FROM waluty;";
                Console.WriteLine(sql);

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Lista.Add(new ListaWalut()
                    {
                        Kod = rdr.GetString(rdr.GetOrdinal("kod")),
                        nazwa = rdr.GetString(rdr.GetOrdinal("Nazwa"))
                    });
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");

            return Lista;
        }

        public static double PobierzOstatniKursWaluty(string kod)
        {
            //List<OstatniKursWalut> Lista = new List<OstatniKursWalut>();
            double kurs = 0.0;
            string connStr = "server=localhost;user=root;database=waluty;port=3306";

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "select cena from kursy where kod = '" + kod + "' order by data desc limit 1;";
                Console.WriteLine(sql);

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    kurs = Convert.ToDouble(rdr[0]);
                }

                /*while (rdr.Read())
                {
                    Lista.Add(new OstatniKursWalut()
                    {
                        ID = rdr.GetInt32(rdr.GetOrdinal("ID")),
                        Kod = rdr.GetString(rdr.GetOrdinal("Kod")),
                        Data = rdr.GetDateTime(rdr.GetOrdinal("Data")),
                        Cena = rdr.GetDouble(rdr.GetOrdinal("Cena"))
                    });
                }*/
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");

            return kurs;
        }

        public static List<double> PobierzKursyWalutyData(string kod, string data_od, string data_do)
        {
            //List<OstatniKursWalut> Lista = new List<OstatniKursWalut>();
            List<double> kurs = new List<double>();
            string connStr = "server=localhost;user=root;database=waluty;port=3306";

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "select cena from kursy where kod = '" + kod + "' and data between '" + data_od + "' AND '" + data_do + "';";
                Console.WriteLine(sql);

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    kurs.Add(Convert.ToDouble(rdr[0]));
                    //Console.WriteLine(rdr[0]);
                }

                /*while (rdr.Read())
                {
                    Lista.Add(new OstatniKursWalut()
                    {
                        ID = rdr.GetInt32(rdr.GetOrdinal("ID")),
                        Kod = rdr.GetString(rdr.GetOrdinal("Kod")),
                        Data = rdr.GetDateTime(rdr.GetOrdinal("Data")),
                        Cena = rdr.GetDouble(rdr.GetOrdinal("Cena"))
                    });
                }*/
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");

            return kurs;
        }
        public static List<DateTime> PobierzListaDat(string kod, string data_od, string data_do)
        {
            List<DateTime> daty = new List<DateTime>();
            string connStr = "server=localhost;user=root;database=waluty;port=3306";

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "select data from kursy where kod = '" + kod + "' and data between '" + data_od + "' AND '" + data_do + "';";
                Console.WriteLine(sql);

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    daty.Add(Convert.ToDateTime(rdr[0]));
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");

            return daty;
        }
        public static List<double> Prognozuj(List<double> kurs)
        {
            var rand = new Random();
            //pobierane zostaja 3 ostatnie ceny z listy
            double kurs1 = kurs[kurs.Count - 3];
            double kurs2 = kurs[kurs.Count - 2];
            double kurs3 = kurs[kurs.Count - 1];
            double[] ilorazKurs = new double[2];
            //okreslam wartosci o jakie roznia sie nasze ceny
            ilorazKurs[0] = 1-(kurs1 / kurs2);
            ilorazKurs[1] = 1-(kurs2 / kurs3);
            double prognozowanaWartosc = kurs3 + (kurs3 * ilorazKurs[rand.Next(0, 2)]);
            if((kurs1>=kurs2 && kurs2>=kurs3)||(kurs1 <= kurs2 && kurs2 <= kurs3))
            {
                if(rand.Next(0, 2)==1)
                {
                    prognozowanaWartosc = kurs3 + (kurs3 * ilorazKurs[rand.Next(0, 2)]);
                }
                else
                {
                    prognozowanaWartosc = kurs3 - (kurs3 * ilorazKurs[rand.Next(0, 2)]);
                }
            }
            kurs.Add(prognozowanaWartosc);
            return kurs;
        }
    }

    static class Program
    {

        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            API.AktualizujBazeDanych();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
