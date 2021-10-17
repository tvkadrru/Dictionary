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
    public partial class Form1 : Form
    {
        private string DictionaryPath;

        public Form1()
        {
            InitializeComponent();

            DictionaryPath = Application.StartupPath + "\\Dictionaries\\";

            if(!Directory.Exists(DictionaryPath))
            {
                Directory.CreateDirectory(DictionaryPath);
            }
            
            listBox1.Sorted = true;

            if (!IsDirectoryEmpty(DictionaryPath))
            {
                // Заполнение списка словарей.
                string[] dicts = Directory.GetFiles(DictionaryPath, "*.txt");
                foreach (string dict in dicts)
                {
                    listBox1.Items.Add(Path.GetFileNameWithoutExtension(dict));
                }
            }

            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            textBox1.Enabled = false;
        }

        // Проверка папки со словарями на наличие словарей.
        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        // Добавление нового словаря.
        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "|*.txt";
            saveFileDialog1.InitialDirectory = DictionaryPath;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Title = "Создание нового словаря";
            saveFileDialog1.ShowDialog();

            // Если имя файла - не пустая строка, то открываем её для сохранения.
            if (saveFileDialog1.FileName != "")
            {
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(fs.Name));
                fs.Close();
                
            }
        }

        // Выбор словаря.
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(DictionaryPath);
            if(listBox1.SelectedIndex >= 0)
            {
                FileInfo[] fi = di.GetFiles(listBox1.Items[listBox1.SelectedIndex].ToString() + ".txt");
                button2.Enabled = true;
                if (fi[0].Length > 0)
                {
                    button3.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;
                    textBox1.Enabled = true;
                }
                else
                {
                    button3.Enabled = false;
                    button4.Enabled = false;
                    button5.Enabled = false;
                    textBox1.Enabled = false;
                }
            }
        }

        // Добавление нового слова.
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 word = new Form2(DictionaryPath + listBox1.SelectedItem.ToString() + ".txt");
            word.ShowDialog();
            word.Dispose();

            DirectoryInfo di = new DirectoryInfo(DictionaryPath);
            FileInfo[] fi = di.GetFiles(listBox1.Items[listBox1.SelectedIndex].ToString() + ".txt");
            if (fi[0].Length > 0)
            {
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                textBox1.Enabled = true;
            }
        }

        // Перевод.
        private void Translate()
        {
            bool entry = false;

            if (textBox1.Text != "")
            {
                label3.Text = "";
                textBox2.Text = "";

                using (StreamReader sr = new StreamReader(
                    DictionaryPath + listBox1.SelectedItem.ToString() + ".txt", Encoding.Unicode))
                {
                    while (sr.Peek() != -1)
                    {
                        string line = sr.ReadLine();
                        string left = line.Substring(0, line.IndexOf('='));
                        string right = line.Substring(line.IndexOf('=') + 1);
                        if (textBox1.Text == left)
                        {
                            entry = true;
                            label3.Text = " " + left;
                            textBox2.Text = textBox2.Text + "\u25AA " + right + Environment.NewLine;
                        }
                        if (textBox1.Text == right)
                        {
                            entry = true;
                            label3.Text = " " + right;
                            textBox2.Text = textBox2.Text + "\u25AA " + left + Environment.NewLine;
                        }
                    }
                }

                if (!entry)
                {
                    label3.Text = " " + textBox1.Text;
                    textBox2.Text = " Перевод в словаре \"" + listBox1.SelectedItem.ToString() + "\" отсутствует!";
                }

                textBox1.Text = "";
                textBox1.Select();
            }
        }

        // Кнопка "Перевод"
        private void button5_Click(object sender, EventArgs e)
        {
            Translate();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && button5.Enabled == true)
            {
                Translate();
            }
        }

        // Режим изучения.
        private void button3_Click(object sender, EventArgs e)
        {
            Form3 learn = new Form3(DictionaryPath + listBox1.SelectedItem.ToString() + ".txt",
                                                          listBox1.SelectedItem.ToString());
            learn.ShowDialog();
            learn.Dispose();
        }

        // Режим проверки знаний.
        private void button4_Click(object sender, EventArgs e)
        {
            Form4 exam = new Form4(DictionaryPath + listBox1.SelectedItem.ToString() + ".txt",
                                                        listBox1.SelectedItem.ToString());
            exam.ShowDialog();
            exam.Dispose();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
