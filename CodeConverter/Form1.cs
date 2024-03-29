﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeConverter
{
    public partial class Form1 : Form
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        //global variables
        StringBuilder sb = new StringBuilder();
        bool dead = false;
        bool endif = true;
        bool endelse = true;
        bool endfor = true;

        string projectName = "Defaultname";
        string path = "";
        string project_path = Form2.pr_path;
        string def_pos = Form2.def_pos;

        //string projectName =format.tex;
        string startex = null;
        string[] splitvar = { };
        string code = "";
        string extab = "";

        List<string> datatype = new List<string> { "int", "string", "float", "double", "char", "bool", "DateTime", "int[]", "string[]", "float[]", "double[]", "char[]" };
        List<string> variables = new List<string> { };
        List<string> dtpvariables = new List<string> { };
        List<char> ariphm = new List<char> { '+', '-', '*', '/', '=', '(', ')', '<', '>', ';' };

        char[] splitariphm = { '+', '-', '*', '/', '=', '(', ')', '<', '>', ';' };
        int linelength = 0;
        int k;
        //main proj
        public Form1()
        {
            InitializeComponent();
            if (project_path != null)
            {
                using (StreamReader sr = new StreamReader(project_path))
                {
                    richTextBox1.Text = sr.ReadToEnd();
                }
            }
            else
            {
                richTextBox1.Text = "create int c\ncreate int i\ncreate int a\narray char t 4\ncreate int b\nc=9*5+b\nfor i=0; i<b; i++\nc=a+b\nendfor\nif a=b\nc=b-a\nendif\nelse\nc=b*a\nendelse";
            }
            projectName = Form2.pr_name;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (project_path != null)
            {
                path = project_path;
            }
            else
            {
                path = $@"../../{def_pos}/{projectName}/{projectName}.txt";
            }
            validation(richTextBox1.Text);
            dead = false;
            code = "";
            variables.Clear();
            dtpvariables.Clear();
            string[] arr = new string[richTextBox1.Lines.Count()];

            mainProg();
            for (int i = 0; i < richTextBox1.Lines.Count(); i++)
            {
                richTextBox1.Text = richTextBox1.Text.Trim();
                arr[i] = richTextBox1.Lines[i];
                splitvar = arr[i].Split(' ');
                if (!endelse || !endif || !endfor)
                {
                    extab = "\t";
                }
                else
                {
                    extab = "";
                }
                switch (splitvar[0])
                {
                    case "create":
                        if (variables.Contains(splitvar[splitvar.Length - 1]) == false)
                        {
                            if (datatype.Contains(splitvar[1]))
                            {
                                code += Client.ClientCode(new CreateClass(), splitvar, extab) + $";{Environment.NewLine}";
                                variables.Add(splitvar[splitvar.Length - 1]);
                                dtpvariables.Add(splitvar[splitvar.Length - 2]);
                            }
                            else
                            {
                                undrline(i);
                                MessageBox.Show($"Data type do not declared\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                prcrash();
                            }
                        }
                        else
                        {
                            undrline(i);
                            MessageBox.Show("Variable already exist\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            prcrash();
                        }
                        break;
                    case "input":
                        if (splitvar[splitvar.Length - 1].Contains("[")==false&&  variables.Contains(splitvar[splitvar.Length - 1]) == false)
                        {
                            //MessageBox.Show(splitvar[splitvar.Length - 1].ToString() + "  " + splitvar[splitvar.Length - 1].Contains("[").ToString());
                            if (datatype.Contains(splitvar[1]))
                            {
                                code += Client.ClientCode(new InputClass(), splitvar, extab) + $";{Environment.NewLine}";
                                variables.Add(splitvar[splitvar.Length - 1]);
                                dtpvariables.Add(splitvar[splitvar.Length - 2]);
                            }
                            else
                            {
                                undrline(i);
                                MessageBox.Show($"Data type do not declared\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                prcrash();
                            }
                        }
                        else
                        {
                            if (splitvar[splitvar.Length - 1].Contains("[") == true||datatype.Contains(splitvar[1]) && splitvar[1] == dtpvariables[variables.IndexOf(splitvar[2])])
                            {
                                code += Client.ClientCode(new InputDTClass(), splitvar, extab) + $";{Environment.NewLine}";
                                variables.Add(splitvar[splitvar.Length - 1]);
                                dtpvariables.Add(splitvar[splitvar.Length - 2]);
                            }
                            else
                            {
                                undrline(i);
                                MessageBox.Show($"Data type do not equal\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                prcrash();
                            }
                        }
                        break;
                    case "output":
                        code += Client.ClientCode(new OutputClass(), splitvar, extab) + $";{Environment.NewLine}";
                        startex = splitvar[1];
                        existedvar();
                        break;
                    case "if":
                        code += Client.ClientCode(new IfClass(), splitvar, extab) + $"{Environment.NewLine}";
                        code += extab + "\t{" + $"{Environment.NewLine}";
                        startex = splitvar[1];
                        existedvar();
                        endif = false;
                        break;
                    case "endif":
                        if (!endif)
                        {
                            endif = true;
                            code += extab + "\t}" + $"{Environment.NewLine}";
                        }
                        else
                        {
                            dead = true;
                            MessageBox.Show($"endif is excess{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "else":
                        code += "\telse" + $"{Environment.NewLine}";
                        code += extab + "\t{" + $"{Environment.NewLine}";
                        endelse = false;
                        break;
                    case "endelse":
                        if (!endelse)
                        {
                            endelse = true;
                            code += extab + "\t}" + $"{Environment.NewLine}";
                        }
                        else
                        {
                            dead = true;
                            MessageBox.Show($"endelse is excess{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "for":
                        if (splitvar.Contains(";"))
                        {
                            startex = splitvar[1];
                            code += Client.ClientCode(new ForClass(), splitvar, extab) + $"{Environment.NewLine}";
                            code += extab + "\t{" + $"{Environment.NewLine}";
                            existedvar();
                        }
                        else
                        {
                            dead = true;
                            MessageBox.Show($"; in for menu is excess{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        endfor = false;
                        break;
                    case "endfor":
                        if (!endfor)
                        {
                            endfor = true;
                            code += "\t}" + $"{Environment.NewLine}";
                        }
                        else
                        {
                            dead = true;
                            MessageBox.Show($"endfor is excess{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "array":
                        if (int.TryParse(splitvar[splitvar.Length - 1], out k))
                        {
                            if (variables.Contains(splitvar[splitvar.Length - 2]) == false)
                            {
                                if (datatype.Contains(splitvar[1]))
                                {
                                    code += Client.ClientCode(new ArrayClass(), splitvar, extab) + $";{Environment.NewLine}";
                                    variables.Add(splitvar[splitvar.Length - 2]);
                                    dtpvariables.Add(splitvar[splitvar.Length - 3]);
                                }
                                else
                                {
                                    undrline(i);
                                    MessageBox.Show($"Data type do not declared\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    prcrash();
                                }
                            }
                            else
                            {
                                undrline(i);
                                MessageBox.Show("Variable already exist\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                prcrash();
                            }
                        }
                        else
                        {
                            undrline(i);
                            MessageBox.Show("Size of array must be numerable\nClose window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            prcrash();
                        }
                        break;
                    case "":
                        break;
                    case string s when variables.Contains(splitvar[0]):
                        code += Client.ClientCode(new VarClass(), splitvar, extab) + $";{Environment.NewLine}";
                        startex = splitvar[0];
                        existedvar();
                        break;

                    default:
                        undrline(i);
                        undrline(i);
                        MessageBox.Show($"Syntax Error{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        prcrash();
                        break;
                }
                linelength += richTextBox1.Lines[i].Length;

            }

            code += "}";
            if (!endif || !endfor || !endelse)
            {
                dead = true;
                MessageBox.Show($"end structure doesnt exist{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!dead)
            {
                path = $@"../../{def_pos}/{projectName}/{projectName}.cs";
                richTextBox2.Text = code;
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(code);
                }
                path = $@"../../{def_pos}/{projectName}/{projectName}.txt";
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(richTextBox1.Text);
                }
            }
            MessageBox.Show("Code convert succesfuly", "Congrats", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void mainProg()
        {
            code += "static void main()\n";
            code += "{\n";
        }
        void existedvar()
        {
            Regex rgx = new Regex(@"[0-9]+|[\[-\]]$");
            for (int j = 1; j < splitvar.Length; j++)
            {
                if (splitvar[j].Contains("\"") || rgx.IsMatch(splitvar[j]) || splitvar[j].TrimStart(splitariphm) == "" || variables.Contains(splitvar[j].TrimStart(splitariphm)))
                {
                    //MessageBox.Show(splitvar[j]);
                    //MessageBox.Show(dtpvariables[variables.IndexOf(splitvar[j].TrimStart(splitariphm))].ToString(), dtpvariables[variables.IndexOf(splitvar[j - 1].TrimStart(splitariphm))].ToString(), MessageBoxButtons.OK);
                    if (splitvar[j - 1] == "if" || splitvar[j - 1] == "output" || rgx.IsMatch(splitvar[j]) || splitvar[j - 1] == "for" || splitvar[j].TrimStart(splitariphm) == "" || splitvar[j].Contains("\"") || dtpvariables[variables.IndexOf(splitvar[j].TrimStart(splitariphm))] == dtpvariables[variables.IndexOf(startex.TrimStart(splitariphm))])
                    {

                    }
                    else
                    {
                        MessageBox.Show($"Datatype error{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dead = true;
                        break;
                    }
                }
                else
                {
                    MessageBox.Show($"variable {splitvar[j].TrimStart(splitariphm)} doesnt exist{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dead = true;
                    break;
                }
            }
        }
        void undrline(int i)
        {
            this.richTextBox1.Text += " ";
            this.richTextBox1.SelectionStart = linelength;
            this.richTextBox1.SelectionLength = richTextBox1.Lines[i].Length;
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, FontStyle.Underline);
        }

        void validation(string stroka)
        {
            var result = new StringBuilder();
            result.Append(stroka[0]);
            for (int i = 1; i < stroka.Length - 1; ++i)
            {
                result.Append(stroka[i]);
                if (stroka[i] == stroka[i + 1] && ariphm.Contains(stroka[i + 1]))
                {
                    continue;
                }
                else
                if (stroka[i] != ' ' && ariphm.Contains(stroka[i + 1]))
                {
                    result.Append(' ');
                }
            }

            if (stroka.Length > 0)
                result.Append(stroka[stroka.Length - 1]);
            richTextBox1.Clear();
            //MessageBox.Show(result.ToString(), "", MessageBoxButtons.OK);
            richTextBox1.Text = result.ToString();
        }

        void prcrash()
        {
            dead = true;
        }
        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //validation(richTextBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            path = $@"../../{def_pos}/{projectName}/{projectName}.txt";
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(richTextBox1.Text);
            }
            path = $@"../../{def_pos}/{projectName}/{projectName}.cs";
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(richTextBox2.Text);
            }
            this.Close();
        }
    }
    abstract class AbstractClass
    {
        // Шаблонный метод определяет скелет алгоритма.
        public string TemplateMethod(string[] var, string tab)
        {
            return (tab + "\t" + this.RequiredOperations1(var) + "\n\t" + tab + this.RequiredOperation2(var));
        }


        // А эти операции должны быть реализованы в подклассах.
        protected abstract string RequiredOperations1(string[] variable);

        protected abstract string RequiredOperation2(string[] variable);

    }

    class InputClass : AbstractClass
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        protected override string RequiredOperations1(string[] variable)
        {
            return ($"//Entering {variable[1]} variable {variable[variable.Length - 1]}");
        }

        protected override string RequiredOperation2(string[] variable)
        {
            string s = "";
            if (variable[1] == "int" || variable[1] == "int[]")
            {
                s = "32";
            }
            else
                s = "";
            return ($"{variable[1]} {variable[variable.Length - 1]} = Convert.To{ti.ToTitleCase(variable[1]).Replace("[]","")}{s}(Console.ReadLine())");
        }
    }
    class InputDTClass : AbstractClass
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        protected override string RequiredOperations1(string[] variable)
        {
            return ($"//Entering {variable[1]} variable {variable[variable.Length - 1]}");
        }

        protected override string RequiredOperation2(string[] variable)
        {
            string s = "";
            if (variable[1] == "int" || variable[1] == "int[]")
            {
                s = "32";
            }
            else
                s = "";
            return ($"{variable[variable.Length - 1]} = Convert.To{ti.ToTitleCase(variable[1]).Replace("[]", "")}{s}(Console.ReadLine())");
        }
    }
    class OutputClass : AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {
            return ($"//outputing variable {variable[variable.Length - 1]}");
        }

        protected override string RequiredOperation2(string[] variable)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 2; i < variable.Length; i++)
            {
                str.Append("+" + variable[i]);
            }
            return ($"Console.WriteLine({variable[1] + str})");
        }

    }
    class CreateClass : AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {
            return ($"//creating new {variable[1]} variable {variable[2]}");

        }

        protected override string RequiredOperation2(string[] variable)
        {
            if (variable[2] == variable[variable.Length - 1])
            {
                return ($"{variable[1]} {variable[2]}");
            }
            else
            {
                StringBuilder str = new StringBuilder();
                for (int i = 3; i < variable.Length; i++)
                {
                    str.Append(variable[i]);
                }
                return ($"{variable[1]} {variable[2]} {str}");
            }
        }
    }
    class VarClass : AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {

            return ($"//execute variable {variable[0]}");

        }

        protected override string RequiredOperation2(string[] variable)
        {

            StringBuilder str = new StringBuilder();
            for (int i = 1; i < variable.Length; i++)
            {
                str.Append(variable[i]);
            }
            return ($"{variable[0]} {str}");

        }
    }
    class IfClass : AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {

            return ($"//execute structure {variable[0]}");

        }

        protected override string RequiredOperation2(string[] variable)
        {

            StringBuilder str = new StringBuilder();
            for (int i = 1; i < variable.Length; i++)
            {
                if (variable[i].Contains("="))
                {
                    str.Append("=" + variable[i]);
                }
                else
                    str.Append(variable[i]);
            }
            return ($"{variable[0]}({str})");

        }
    }
    class ForClass : AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {

            return ($"//execute structure {variable[0]}");

        }

        protected override string RequiredOperation2(string[] variable)
        {

            StringBuilder str = new StringBuilder();
            for (int i = 1; i < variable.Length; i++)
            {
                //MessageBox.Show(variable[i]);
                str.Append(' ' + variable[i]);
                //MessageBox.Show(str.ToString(), "", MessageBoxButtons.OK);
            }
            return ($"{variable[0]}({str})");

        }
    }
    class ArrayClass : AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {
            return ($"//creating new {variable[1]} array {variable[2]}");

        }

        protected override string RequiredOperation2(string[] variable)
        {
            if (variable[2] == variable[variable.Length - 1])
            {
                return ($"{variable[1]}[] {variable[2]} = new {variable[1]}[{variable[3]}]");
            }
            else
            {
                StringBuilder str = new StringBuilder();
                for (int i = 3; i < variable.Length-1; i++)
                {
                    str.Append(variable[i]);
                }
                return ($"{variable[1]}[] {variable[2]} {str} = new {variable[1]}[{variable[3]}]");
            }
        }
    }

    class Client
    {
        public static string ClientCode(AbstractClass abstractClass, string[] variable, string tab)
        {
            // ...
            return (abstractClass.TemplateMethod(variable, tab));
            // ...
        }
    }


}
