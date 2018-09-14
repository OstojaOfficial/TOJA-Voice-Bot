//////////////////////////////////////
//            T.O.J.A.              //
// Talking Objective Jump Assistant //
//////////////////////////////////////
// Created By Ostoja Sredojevic     //
//////////////////////////////////////

#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Xml;
using System.IO;
#endregion

namespace Voice_Bot
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            textBox1.Text = File.ReadAllText(@"C:/Voice Bot/log.txt");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox1.BackColor = System.Drawing.SystemColors.Window;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"C:/Voice Bot/log.txt", String.Empty);
            textBox1.Clear();
        }
    }
}
