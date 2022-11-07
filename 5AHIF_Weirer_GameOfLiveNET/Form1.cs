using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _5AHIF_Weirer_GameOfLiveNET
{
    public partial class Form1 : Form
    {
        #region properties

        public delegate void UpdateTilesCallback(bool[] lebend);

        public delegate int GetTrackBarCallback();

        public delegate List<Color> GetpListBackColorCallback();

        public List<Panel> pList; // Spielfeld, wird in button1_click erstellt

        Label leben = new Label(); //Label zum anzeigen der anzahl der Zellen, welche am leben sind

        int _lebende; // speicher für anzahl der lebendden Zellen
        public int lebende
        {
            get { return _lebende; }
            set
            {
                if (_lebende != value)
                {
                    _lebende = value;
                    leben.Text = _lebende.ToString();
                }
            }
        } // update leben Label, wenn anzahl lebende Zellen änderung

        Thread th = new Thread(() => { }); // thread für simulation

        #endregion

        #region const

        public readonly Color totcolor = Color.Black;
        public readonly Color lebencolor = Color.DarkRed;

        #endregion

        #region ctor

        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region events

        private void button1_MouseDown(object sender, MouseEventArgs e) => button1.BackColor = Color.ForestGreen;

        private void button1_MouseUp(object sender, MouseEventArgs e) => button1.BackColor = Color.Tomato;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            numericUpDown1.Minimum = (int)(trackBar1.Value * trackBar2.Value * 0.25);
            numericUpDown1.Maximum = (int)(trackBar1.Value * trackBar2.Value * 0.75);
            numericUpDown1.Value = (numericUpDown1.Minimum + numericUpDown1.Maximum) / 2;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar2.Value.ToString();
            numericUpDown1.Minimum = (int)(trackBar1.Value * trackBar2.Value * 0.25);
            numericUpDown1.Maximum = (int)(trackBar1.Value * trackBar2.Value * 0.75);
            numericUpDown1.Value = (numericUpDown1.Minimum + numericUpDown1.Maximum) / 2;
        }

        private void trackBar3_Scroll(object sender, EventArgs e) => label8.Text = trackBar3.Value.ToString();

        private void checkBox1_CheckedChanged(object sender, EventArgs e) => numericUpDown1.Visible = !checkBox1.Checked;

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Start")
            {
                th = new Thread(new ThreadStart(simulation));
                th.IsBackground = true;
                th.Start();
                button2.Text = "Stop";
            }
            else
            {
                th.Abort();
                button2.Text = "Start";
            }
        } // Start und Stop der Simulation

        private void button3_Click(object sender, EventArgs e)
        {
            if (!th.IsAlive)
            {
                foreach (var item in pList)
                {
                    item.BackColor = totcolor;
                }
                lebende = 0;
            }
        } // tötet alle Zellen

        private void feldclick(object sender, EventArgs e)
        {
            if (!th.IsAlive)
            {
                if ((sender as Panel).BackColor != lebencolor)
                {
                    (sender as Panel).BackColor = lebencolor;
                    lebende++;
                }
                else
                {
                    (sender as Panel).BackColor = totcolor;
                    lebende--;
                }
            }
        } // wenn Zelle geklickt wird => beleben oder töten

        private void button1_Click(object sender, EventArgs e)
        {
            // aufräumen des Fensters (entfernen aller objekte die nicht mehr benötigt werden)

            Controls.Remove(trackBar1);
            Controls.Remove(trackBar2);
            Controls.Remove(trackBar3);
            Controls.Remove(numericUpDown1);
            Controls.Remove(checkBox1);
            Controls.Remove(button1);
            Controls.Remove(label1);
            Controls.Remove(label2);
            Controls.Remove(label3);
            Controls.Remove(label4);
            Controls.Remove(label5);
            Controls.Remove(label6);
            Controls.Remove(label7);
            Controls.Remove(label8);

            pList = new List<Panel>();
            for (int i = 0; i < trackBar1.Value * trackBar2.Value; i++)
            {
                pList.Add(new Panel() {Width = 25, Height = 25, Location = new Point(12 + (i % trackBar1.Value) * 25, 35 + 25 * (i / trackBar1.Value))/*, BorderStyle = BorderStyle.FixedSingle*/, BackColor = totcolor });
                pList[i].Click += new System.EventHandler(feldclick);
                Controls.Add(pList[i]);
            } // erstellung des Spielfeldes
            if (!checkBox1.Checked)
            {
                Random rnd = new Random();
                for (int i = 0; i < numericUpDown1.Value; i++)
                {
                    int rng = rnd.Next(trackBar1.Value * trackBar2.Value);
                    if (pList[rng].BackColor != lebencolor)
                        pList[rng].BackColor = lebencolor;
                    else
                        i--;
                }
                lebende = (int)numericUpDown1.Value;
            } // Random generierung für lebende Felder, falls User es wollte

            // anpassung des Fensters

            leben.Location = new Point(135, 5);
            leben.Text = lebende.ToString();
            Controls.Add(new Label() { Location = new Point(12, 5), Text = "Aktuell lebende Zellen: ", Width = 120 });
            Controls.Add(leben);
            button2.Location = new Point(trackBar1.Value * 25 - 50, 5);
            button2.Visible = true;
            button3.Location = new Point(trackBar1.Value * 25 - 80 + button3.Width - button2.Width, 5);
            button3.Visible = true;
            this.Width = trackBar1.Value * 25 + 40;
            this.Height = trackBar2.Value * 25 + 85;
        } // Erstellung des Spielfeldes und Anpassung des Fensters

        #endregion

        #region hilfsevents

        private void UpdateTiles(bool[] lebendig)
        {
            int z = 0;
            for (int i = 0; i < lebendig.Length; i++)
            {
                if (lebendig[i])
                {
                    pList[i].BackColor = lebencolor;
                    z++;
                }
                else
                    pList[i].BackColor = totcolor;
            }
            lebende = z;
        } // updated alle Zellen ob sie leben sollen oder nicht

        private int GetTrackBar1() => trackBar1.Value;

        private int GetTrackBar2() => trackBar2.Value;

        private int GetTrackBar3() => trackBar3.Value;

        private List<Color> GetpListBackColor()
        {
            List<Color> cList = new List<Color>();
            foreach (var item in pList)
            {
                cList.Add(item.BackColor);
            }
            return cList;
        } // Gibt alle Farben der Zellen in richtiger Reihenfolge per Liste zurück

        #endregion

        #region methods

        private void simulation()
        {
            int t1 = (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)), t2 = (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)), speed = (int)trackBar3.Invoke(new GetTrackBarCallback(this.GetTrackBar3)); // Werte des einzelnen TrackBars abspeichern
            while (true)
            {
                List<Color> cList = pList[0].Invoke(new GetpListBackColorCallback(this.GetpListBackColor)) as List<Color>; // Liste der Farben aller Felder
                bool[] lebendezellen = new bool[t1 * t2]; // Ergebnisliste der lebenden Zellen
                for (int i = 0, nachbarn = 0; i < lebendezellen.Length; i++, nachbarn = 0)
                {
                    if (i / t1 > 0) // falls Zelle eine Zeile über sich hat
                    {
                        if (i / (t1 * (t2 - 1)) < 1) // falls Zelle eine Zeile unter sich hat
                        {
                            if (i % t1 != 0) // falls Zelle eine Spalte links neben sich hat
                            {
                                if (cList[i - 1] == lebencolor)
                                    nachbarn++;
                                if (cList[i + t1 - 1] == lebencolor)
                                    nachbarn++;
                                if (cList[i - t1 - 1] == lebencolor)
                                    nachbarn++;
                            }
                            if (i % t1 != t1 - 1) // falls Zelle eine Spalte rechts neben sich hat
                            {
                                if (cList[i + 1] == lebencolor)
                                    nachbarn++;
                                if (cList[i + t1 + 1] == lebencolor)
                                    nachbarn++;
                                if (cList[i - t1 + 1] == lebencolor)
                                    nachbarn++;
                            }
                            if (cList[i - t1] == lebencolor)
                                nachbarn++;
                            if (cList[i + t1] == lebencolor)
                                nachbarn++;
                        }
                        else
                        {
                            if (i % t1 != 0) // falls Zelle eine Spalte links neben sich hat
                            {
                                if (cList[i - 1] == lebencolor)
                                    nachbarn++;
                                if (cList[i - t1 - 1] == lebencolor)
                                    nachbarn++;
                            }
                            if (i % t1 != t1 - 1) // falls Zelle eine Spalte rechts neben sich hat
                            {
                                if (cList[i + 1] == lebencolor)
                                    nachbarn++;
                                if (cList[i - t1 + 1] == lebencolor)
                                    nachbarn++;
                            }
                            if (cList[i - t1] == lebencolor)
                                nachbarn++;
                        }
                    }
                    else
                    {
                        if (i % t1 != 0) // falls Zelle eine Spalte links neben sich hat
                        {
                            if (cList[i-1] == lebencolor)
                                nachbarn++;
                            if (cList[i + t1 - 1] == lebencolor)
                                nachbarn++;
                        }
                        if (i % t1 != t1 - 1) // falls Zelle eine Spalte rechts neben sich hat
                        {
                            if (cList[i + 1] == lebencolor)
                                nachbarn++;
                            if (cList[i + t1 + 1] == lebencolor)
                                nachbarn++;
                        }
                        if (cList[i + t1] == lebencolor)
                            nachbarn++;
                    }

                    //überprüfung ob Zelle leben soll

                    if (nachbarn == 3)
                        lebendezellen[i] = true;
                    if (nachbarn == 2 && cList[i] == lebencolor)
                        lebendezellen[i] = true;
                } // für alle Zellen überprüfung ob sie leben oder nicht
                
                Thread.Sleep((int)(5.0 / speed * 100)); // Wartezeit (abhängig von input am anfang)
                object x = pList[0].Invoke(new UpdateTilesCallback(this.UpdateTiles), new object[] { lebendezellen }); // Warten bis Spielfeld aktualisiert wurde
            }
        } // gesamte simulation

        #endregion  
    }
}
