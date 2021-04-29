﻿using System;
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
                this.comboBox5.Items.Add(lista_walut[i].Kod + " ( " + lista_walut[i].nazwa + " )");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        //wersja alpha rysowania wykresow
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Series["WykresWaluty"].Points.Clear();
            /*List<ListaWalut> lista_walut = MySQL.PobierzListeWalut();       //chyba niepotrzebne
            for (int i = 0; i < lista_walut.Count; i++)
            {
                Console.WriteLine(lista_walut[i].Kod);
                this.comboBox1.Items.Add(lista_walut[i].Kod);
                this.comboBox2.Items.Add(lista_walut[i].Kod);
                this.comboBox4.Items.Add(lista_walut[i].Kod);
            }*/
            DateTime aktualna_data = DateTime.Now.Date;
            DateTime przeszla_data = DateTime.Today.AddMonths(-1);
            List<double> ListaCen = MySQL.PobierzKursyWalutyData(comboBox5.Text.Substring(0,3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBox5.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            int a = 0;
            for(int i=0;i<ListaCen.Count;i++)
            {
                Console.WriteLine(ListaDat[i]);
                chart1.Series["WykresWaluty"].Points.AddXY(ListaDat[i].ToString("MM-dd"), ListaCen[i]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            DateTime aktualna_data = DateTime.Now.Date;
            DateTime przeszla_data = DateTime.Today.AddMonths(-1);
            List<DateTime> ListaDat = MySQL.PobierzListaDat(comboBox5.Text.Substring(0, 3), przeszla_data.ToString("yyyy-MM-dd"), aktualna_data.ToString("yyyy-MM-dd"));
            foreach (DateTime dzien in ListaDat)
            {
                Console.WriteLine(dzien);
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
