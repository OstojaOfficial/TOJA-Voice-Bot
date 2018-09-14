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
    #region Definitions
    public partial class Form1 : Form
    {

        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = false;
        Boolean wake2 = false;

        String logpath = @"C:/Voice Bot/log.txt";
        String path = @"C:/Voice Bot";
        String namefile = @"C:/Voice Bot/name.txt";

        String temp;
        String condition;

        int Fx;

        Choices list = new Choices();
        #endregion
        #region Form1
        public Form1()
        {
            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

            list.Add(new String[] { "hello", "how are you", "what time is it", "what is today", "what's the time", "open google", "sleep", "wake", "open", "close", "restart", "update", "shutdown", "open youtube", "whats the weather like", "whats the temperature", "hey toja", "play", "pause", "whats my name" });

            Grammar gr = new Grammar(new GrammarBuilder(list));

            try
            {
                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_SpeachRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch { return; }

            s.SelectVoiceByHints(VoiceGender.Male);

            InitializeComponent();

        }
        #endregion

        #region GetWeather Yahoo Weather API
        public String GetWeather(String input)
        {
            String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22Belgrade%2C%20rs%22)&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();
            try
            {
                wData.Load(query);
            }
            catch
            {
                MessageBox.Show("No Internet Connection!");
                return "No Internet Connection";
            }

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                temp = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
                condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                if (input == "temp")
                {
                    return temp;
                }

                if (input == "cond")
                {
                    return condition;
                }
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }
        #endregion

        public void restart()
        {
            Process.Start(@"C:\Voice Bot\Voice Bot.exe");
            Environment.Exit(0);
        }
        #region Speak
        public void say(String h)
        {
            //s.Speak(h);
            s.SpeakAsync(h);
            wake2 = false;
            textBox2.AppendText(h + "\n");
        }
        #endregion
        private void rec_SpeachRecognized(object slender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            #region Sleep/Wake

            if (r == "hey toja")
            {
                wake2 = true;
            }

            if (r == "restart" || r == "update")
            {
                restart();
            }

            if (r == "wake")
            {
                say("Sleep mode off.");
                wake = true;
                label3.Text = "State: Awake";
            }
            if (r == "sleep")
            {
                say("Sleep mode on.");
                wake = false;
                label3.Text = "State: Sleep";
            }
            #endregion

            if (wake == true || wake2 == true)
            {
                #region Commands

                //what you say
                if (r == "hello")
                {
                    //what it says
                    say("Hi.");
                }

                if (r == "what time is it" || r == "what's the time")
                {
                    say(DateTime.Now.ToString("h:mm tt"));
                }

                if (r == "what is today")
                {
                    say(DateTime.Now.ToString("M/d/yyyy"));
                }

                if (r == "open google")
                {
                    Process.Start("https://google.rs");
                    say("Opening google.");
                }

                if (r == "open youtube")
                {
                    Process.Start("https://www.youtube.com");
                    say("Opening youtube.");
                }

                if (r == "how are you")
                {
                    say("Great. and you?");
                }

                if (r == "whats the weather like")
                {
                    say("The sky is, " + GetWeather("cond") + ".");
                }

                if (r == "whats the temperature")
                {
                    Int32.TryParse(GetWeather("temp"), out Fx);
                    double Cx = 5.0 / 9.0 * (Fx - 32);
                    say("It is, " + String.Format("{0:0.0}", Cx) + " degreese.");
                }

                if (r == "play" || r == "pause")
                {
                    SendKeys.Send(" ");
                }

                if (r == "whats my name")
                {
                    say(File.ReadAllText(namefile));
                }
            }
            if (r == "shutdown")
            {
                say("Good bye.");
                System.Threading.Thread.Sleep(1500);
                Environment.Exit(0);
            }
            textBox1.AppendText(r + "\n");
        }
        #endregion
        #region Setup
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(path))
            {
                if (File.Exists(logpath))
                {
                    File.AppendAllText(logpath, "Log file exists.\r\n");
                }
            System.Threading.Thread.Sleep(1500);
            File.AppendAllText(logpath, "Directory exists.\r\n");
            }
            else
            {
                Directory.CreateDirectory(path);
                System.Threading.Thread.Sleep(1500);
                File.AppendAllText(logpath, "Directory doesn't exists. \r\n Creating Directory...\r\n");
                File.AppendAllText(logpath, "Log file doesn't exist. \r\n File created.\r\n");
            }

            if (File.Exists(namefile))
            {
                File.AppendAllText(logpath, "Name file exists.\r\n");
            }
            else
            {
                File.AppendAllText(logpath, "Name file doesn't exists. \r\n Creating Name file...\r\n");
            }
            #endregion
            #region labels/textboxes
            textBox2.Enabled = false;
            textBox2.BackColor = System.Drawing.SystemColors.Window;
            textBox1.Enabled = false;
            textBox1.BackColor = System.Drawing.SystemColors.Window;

            textBox3.MaxLength = 15;

            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            label3.Parent = pictureBox1;
            label4.Parent = pictureBox1;
            label5.Parent = pictureBox1;
            label6.Parent = pictureBox1;
            label7.Parent = pictureBox1;
            label8.Parent = pictureBox1;
            label10.Parent = pictureBox1;
            label1.BackColor = System.Drawing.Color.Transparent;
            label2.BackColor = System.Drawing.Color.Transparent;
            label3.BackColor = System.Drawing.Color.Transparent;
            label4.BackColor = System.Drawing.Color.Transparent;
            label5.BackColor = System.Drawing.Color.Transparent;
            label6.BackColor = System.Drawing.Color.Transparent;
            label7.BackColor = System.Drawing.Color.Transparent;
            label8.BackColor = System.Drawing.Color.Transparent;
            label10.BackColor = System.Drawing.Color.Transparent;
            #endregion
        }
        #region button functions
        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText(namefile, textBox3.Text);
            textBox3.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }
        #endregion
    }
}