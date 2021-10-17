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
    public partial class Form2 : Form
    {
        private string path;

        public Form2(string path)
        {
            InitializeComponent();

            this.path = path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Неполный ввод.");
                return;
            }
            else
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Unicode))
                {
                    while (sr.Peek() != -1)
                    {
                        if (sr.ReadLine() == textBox1.Text + "=" + textBox2.Text)
                        {
                            MessageBox.Show("Такое слово уже имеется в словаре.", "Ввод дубликата.");
                            textBox1.Text = "";
                            textBox2.Text = "";
                            return;
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(path, true, Encoding.Unicode))
            {
                sw.WriteLine(textBox1.Text + "=" + textBox2.Text);
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
