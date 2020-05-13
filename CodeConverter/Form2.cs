using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CodeConverter
{
    public partial class Form2 : Form
    {
        bool patterns = false;
        public static string def_pos = "projects";
        public Form2()
        {
            InitializeComponent();
            listBox1.Items.Clear();
            //string PatchProfile = @"../../projects/";
            upd_list();

        }
        public static string pr_name;
        public static string pr_path;
        private void button1_Click(object sender, EventArgs e)
        {
            pr_name = textBox1.Text;
            string path = $@"../../projects/{textBox1.Text}";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
                textBox1.Text = "";
                Form1 newForm = new Form1();
                newForm.Show();
                path = $@"../../projects/{pr_name}/{pr_name}.cs";
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("");
                }
                path = $@"../../projects/{pr_name}/{pr_name}.txt";
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("");
                }
                upd_list();
            }
            else
            {
                MessageBox.Show("Project with this name already exist\nRename your project", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static string[] SearchDirectory(string patch)
        {
            string[] ReultSearch = Directory.GetDirectories(patch);
            return ReultSearch;
        }
        static string[] SearchFile(string patch, string pattern)
        {
            string[] ReulltSearch = Directory.GetFileSystemEntries(patch,pattern);
            return ReulltSearch;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pr_path = listBox1.SelectedItem.ToString();
            pr_name = Path.GetFileNameWithoutExtension(listBox1.SelectedItem.ToString());
            //MessageBox.Show(pr_path+"  "+pr_name);
            Form1 newForm = new Form1();
            newForm.Show();
        }
        void upd_list()
        {
            string PatchProfile = Environment.GetEnvironmentVariable("USERPROFILE");
            string[] S = SearchDirectory($@"..\..\{def_pos}\") ;

            string[] ListPatch = new string[S.Length]; //заголовок для строк

            for (int i = 0; i < S.Length; i++)
            {
                try
                {
                    //пытаемся найти данные в папке 
                    string[] F = SearchFile(S[i], "*.txt");
                    foreach (string FF in F)
                    {
                        //добавляем файл в список 
                        ListPatch[i] = FF;
                    }
                }
                catch
                {
                }
            }

            for (int i = 0; i < S.Length; i++)
            {
                listBox1.Items.Add($"{ListPatch[i]}");
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if(!patterns)
            {
                button2.Text = "Old projects";
                label2.Text = "Patterns";
                def_pos = "Patterns";
                patterns = true;
                upd_list();
            }
            else
            {
                button2.Text = "Patterns";
                label2.Text = "Old projects"; 
                def_pos = "projects";
                patterns = false;
                upd_list();

            }
        }
    }
}
