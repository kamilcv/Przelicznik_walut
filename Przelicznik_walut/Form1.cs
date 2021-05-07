using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Przelicznik_walut
{
    public partial class Form1 : Form
    {
        string sign;
        double num1;
        double num2;
        bool startNewNumber = true;
        public Form1()
        {
            InitializeComponent();
            List<ListaWalut> lista_walut = MySQL.PobierzListeWalut();
            //wypelnienie combo box'ow kodami i nazwami walut
            for (int i = 0; i < lista_walut.Count; i++)
            {
                Console.WriteLine(lista_walut[i].Kod);
                this.comboBox1.Items.Add(lista_walut[i].Kod + " ( " + lista_walut[i].nazwa + " )");
                this.comboBox2.Items.Add(lista_walut[i].Kod + " ( " + lista_walut[i].nazwa + " )");
                this.comboBox4.Items.Add(lista_walut[i].Kod + " ( " + lista_walut[i].nazwa + " )");
                this.comboBoxWykres.Items.Add(lista_walut[i].Kod + " ( " + lista_walut[i].nazwa + " )");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart2.Series["WykresPrognoz"].Points.Clear();
            DateTime aktualna_data = DateTime.Now.Date;
            DateTime przeszla_data = DateTime.Today.AddMonths(-1);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBox4.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBox4.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            for (int i = 0; i < ListaCen.Count; i++)
            {
                chart2.Series["WykresPrognoz"].Points.AddXY(ListaDat[i].ToString("MM-dd"), ListaCen[i]);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        //rysowanie wykresow walut
        /*private void comboBoxWykres_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Series["WykresWaluty"].Points.Clear();
            DateTime aktualna_data = DateTime.Today;
            DateTime przeszla_data = DateTime.Today.AddMonths(-1);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0,3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            for(int i=0;i<ListaCen.Count;i++)
            {
                chart1.Series["WykresWaluty"].Points.AddXY(ListaDat[i].ToString("MM-dd"), ListaCen[i]);
            }
        }*/
        private void comboBoxWykres_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Series["WykresWaluty"].Points.Clear();
            DateTime aktualna_data = DateTime.Today;
            DateTime przeszla_data = DateTime.Today.AddMonths(-1);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            int licznikDni = 15;
            while(ListaCen.Count<15)
            {
                aktualna_data = DateTime.Today.AddDays(-licznikDni);
                przeszla_data = DateTime.Today.AddMonths(-(licznikDni+licznikDni));
                ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                licznikDni = licznikDni + 15;
            }
            for (int i = 0; i < ListaCen.Count; i++)
            {
                chart1.Series["WykresWaluty"].Points.AddXY(ListaDat[i].ToString("yyyy-MM-dd"), ListaCen[i]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            NumberBt_Click("7");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            NumberBt_Click("1");
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        //przycisk 7 dni
        private void buttonDay_Click(object sender, EventArgs e)
        {
            chart1.Series["WykresWaluty"].Points.Clear();
            DateTime aktualna_data = DateTime.Now.Date;
            DateTime przeszla_data = DateTime.Today.AddDays(-7);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            int licznikDni = 7;
            while (ListaCen.Count < 3)
            {
                aktualna_data = DateTime.Today.AddDays(-licznikDni);
                przeszla_data = DateTime.Today.AddMonths(-(licznikDni + licznikDni));
                ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                licznikDni = licznikDni + 7;
            }
            for (int i = 0; i < ListaCen.Count; i++)
            {
                chart1.Series["WykresWaluty"].Points.AddXY(ListaDat[i].ToString("yyyy-MM-dd"), ListaCen[i]);
            }
        }
        //przycisk 1 miesiac
        private void buttonMonth_Click(object sender, EventArgs e)
        {
            chart1.Series["WykresWaluty"].Points.Clear();
            DateTime aktualna_data = DateTime.Now.Date;
            DateTime przeszla_data = DateTime.Today.AddMonths(-1);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            int licznikDni = 15;
            while (ListaCen.Count < 15)
            {
                aktualna_data = DateTime.Today.AddDays(-licznikDni);
                przeszla_data = DateTime.Today.AddMonths(-(licznikDni + licznikDni));
                ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                licznikDni = licznikDni + 15;
            }
            for (int i = 0; i < ListaCen.Count; i++)
            {
                chart1.Series["WykresWaluty"].Points.AddXY(ListaDat[i].ToString("yyyy-MM-dd"), ListaCen[i]);
            }
        }
        //przycisk 1 rok
        private void buttonYear_Click(object sender, EventArgs e)
        {
            chart1.Series["WykresWaluty"].Points.Clear();
            DateTime aktualna_data = DateTime.Now.Date;
            DateTime przeszla_data = DateTime.Today.AddYears(-1);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            int licznikDni = 160;
            while (ListaCen.Count < 30)
            {
                aktualna_data = DateTime.Today.AddDays(-licznikDni);
                przeszla_data = DateTime.Today.AddMonths(-(licznikDni + licznikDni));
                ListaCen = MySQL.PobierzKursyWalutyData(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                ListaDat = MySQL.PobierzListaDat(comboBoxWykres.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
                licznikDni = licznikDni + 160;
            }
            for (int i = 0; i < ListaCen.Count; i++)
            {
                chart1.Series["WykresWaluty"].Points.AddXY(ListaDat[i].ToString("yyyy-MM-dd"), ListaCen[i]);
            }
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.IndexOf(".") == -1)
            {
                textBox1.Text += ".";
                startNewNumber = false;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            NumberBt_Click("0");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                textBox1.Text = "0";
            }
            else
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            NumberBt_Click("2");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            NumberBt_Click("3");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NumberBt_Click("4");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NumberBt_Click("5");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            NumberBt_Click("6");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NumberBt_Click("8");
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            NumberBt_Click("9");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void NumberBt_Click(string number)
        {
            if (startNewNumber == false)
            {
                textBox1.Text += number;
            }
            else
            {
                textBox1.Text = number;
                startNewNumber = false;
            }
        }
    }
}
