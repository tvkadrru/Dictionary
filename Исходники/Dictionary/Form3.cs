using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dictionary
{
    public partial class Form3 : Form
    {
        private string path;
        private Label[] labels;
        private List<String> entry_left;
        private List<String> entry_right;
        private Random rnd;
        private int left_selected;
        private int right_selected;
        private int correct;
        private int[] tags;
        private int words;
        private Color[] colors;
        private Point[,] points;
        private Point start;
        private Point end;
        private int check;

        public Form3(string path, string DictionaryName)
        {
            InitializeComponent();

            this.path = path;
            this.Text = "Изучение: \'" + DictionaryName + "\'.";
            
            label1.Text = "Выберите слово и его перевод.";

            labels = new Label[]{label2, label3, label4, label5, label6, label7, label8,
                   label9, label10, label11, label13, label14, label15, label16, label17};

            entry_left = new List<string>();
            entry_right = new List<string>();
            rnd = new Random();
            tags = new int[5];
            colors = new Color[] {Color.DarkSlateBlue, Color.DarkSalmon, Color.DarkSeaGreen,
                                                       Color.DarkOrchid, Color.DarkSlateGray};
            points = new Point[5, 2];

            for (int i = 0; i < 5; i++)
            {
                labels[i].Click += new EventHandler(this.left_Click);
                labels[i + 5].Click += new EventHandler(this.right_Click);
            }

            LearnSession();
        }

        // Слева располагаются кнопки со словами на иностранном языке.
        private void left_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                labels[i].BackColor = SystemColors.Control;
            }

            Label label = sender as Label;
            label.BackColor = Color.CornflowerBlue;
            left_selected = Convert.ToInt32(label.Tag);

            Check();
        }

        // Справа располагаются кнопки со словами на родном языке.
        private void right_Click(object sender, EventArgs e)
        {
            for (int i = 5; i < 10; i++)
            {
                labels[i].BackColor = SystemColors.Control;
            }

            Label label = sender as Label;
            label.BackColor = Color.DeepPink;
            right_selected = Convert.ToInt32(label.Tag);

            Check();
        }

        // Заполнение словами и их переводом.
        private void SetLabels(int number)
        {
            int count = entry_left.Count;
            int entry_index;
            int random_index;
            List<int> label_index = new List<int>();
            words = number;

            for (int i = 0; i < number; i++)
            {
                labels[i].Enabled = true;
                labels[i].Visible = true;
                labels[i + 5].Enabled = true;
                labels[i + 5].Visible = true;

                entry_index = rnd.Next(count);
                labels[i].Enabled = true;
                labels[i].Text = entry_left.ElementAt(entry_index);
                labels[i].Tag = i;

                do
                {
                    random_index = rnd.Next(number);
                    if (!(label_index.Contains(random_index)))
                    {
                        break;
                    }

                } while (label_index.Contains(random_index));

                label_index.Add(random_index);

                labels[label_index.ElementAt(i) + 5].Enabled = true;
                labels[label_index.ElementAt(i) + 5].Text = entry_right.ElementAt(entry_index);
                labels[label_index.ElementAt(i) + 5].Tag = i;
                tags[i] = label_index.ElementAt(i);

                entry_left.RemoveAt(entry_index);
                entry_right.RemoveAt(entry_index);
                count--;
            }
        }

        // Проверка верности слова и его перевода.
        private void Check()
        {
            if (left_selected == right_selected)
            {
                start = new Point(labels[left_selected].Location.X + labels[left_selected].Width,
                    labels[left_selected].Location.Y + labels[left_selected].Height / 2);

                end = new Point(labels[tags[right_selected] + 5].Location.X - 1,
                labels[tags[right_selected] + 5].Location.Y + labels[tags[right_selected] + 5].Height / 2);

                points[check, 0] = start;
                points[check++, 1] = end;

                this.Refresh();
                
                labels[left_selected].Enabled = false;
                labels[tags[right_selected] + 5].Enabled = false;
                labels[left_selected].BackColor = Color.LightGreen;
                labels[tags[right_selected] + 5].BackColor = Color.LightGreen;
                label12.Text = labels[left_selected].Text + " = " + labels[tags[right_selected] + 5].Text;
                labels[correct++].Text = label12.Text;
                
                if (correct == 10 + words)
                {
                    correct = 10;
                    check = 0;
                    this.Refresh();

                    for (int i = 0; i < 5; i++)
                    {
                        labels[i].Visible = false;
                        labels[i].BackColor = SystemColors.Control;
                        labels[i + 5].Visible = false;
                        labels[i + 5].BackColor = SystemColors.Control;
                        labels[labels.Length - 1 - i].Text = "";
                    }

                    if (entry_left.Count == 0)
                    {
                        if (MessageBox.Show("Хотите начать сначала?", "Урок изучения завершён.",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            LearnSession();
                            return;
                        }
                        else
                        {
                            this.Close();
                        }
                    }

                    if (entry_left.Count > 4)
                    {
                        SetLabels(5);
                    }
                    else
                    {
                        SetLabels(entry_left.Count);
                    }
                }
            }
        }

        // Начало нового изучения.
        private void LearnSession()
        {
            label12.Text = "";
            for (int i = 0; i < 5; i++)
            {
                labels[labels.Length - 1 - i].Text = "";
            }

            left_selected = -1;
            right_selected = -2;
            correct = 10;
            check = 0;

            entry_left.Clear();
            entry_right.Clear();

            using (StreamReader sr = new StreamReader(path, Encoding.Unicode))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    string left = line.Substring(0, line.IndexOf('='));
                    string right = line.Substring(line.IndexOf('=') + 1);

                    entry_left.Add(left);
                    entry_right.Add(right);
                }
            }
            
            if (entry_left.Count > 4)
            {
                SetLabels(5);
            }
            else
            {
                SetLabels(entry_left.Count);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (check > 0)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                for (int i = 0; i < check; i++)
                {
                    using (Pen pen = new Pen(colors[i], 1))
                    {
                        e.Graphics.DrawLine(pen, points[i, 0], points[i, 1]);
                    }
                }
            }
        }
    }
}
