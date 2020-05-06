using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        string[] splitvar = { };
        string code = "";

        List<string> datatype = new List<string> { "int", "string", "float", "double", "char", "bool", "DateTime" };
        List<string> variables = new List<string> { };
        List<string> dtpvariables = new List<string> { };
        List<char> ariphm = new List<char> { '+', '-', '*', '/', '=','(',')'}; 

        char[] splitariphm = { '+', '-', '*', '/', '=', '(', ')' };
        int linelength = 0;
        //main proj
        public Form1()
        {
            InitializeComponent();
            richTextBox1.Text = "create int c\ncreate bool k\ncreate int a\ncreate char t\ncreate int b\nc=a+b";
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            validation(richTextBox1.Text);
            dead = false;
            code = "";
            variables.Clear();
            dtpvariables.Clear();
            label1.Text = "";
            string[] arr = new string[richTextBox1.Lines.Count()];

            mainProg();
            for (int i = 0; i < richTextBox1.Lines.Count(); i++)
            {
                richTextBox1.Text = richTextBox1.Text.Trim();
                arr[i] = richTextBox1.Lines[i];
                splitvar = arr[i].Split(' ');
                
                switch (splitvar[0])
                {
                    case "create":
                        if (variables.Contains(splitvar[splitvar.Length - 1]) == false)
                        {
                            if (datatype.Contains(splitvar[1]))
                            {
                                code += Client.ClientCode(new CreateClass(), splitvar) + $";{Environment.NewLine}";
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
                        if (variables.Contains(splitvar[splitvar.Length - 1]) == false)
                        {
                            if (datatype.Contains(splitvar[1]))
                            {
                                code += Client.ClientCode(new InputClass(), splitvar) + $";{Environment.NewLine}";
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
                    case "output":
                        code += Client.ClientCode(new OutputClass(), splitvar) + $";{Environment.NewLine}";
                        existedvar();
                        break;
                    case "if":
                        code += Client.ClientCode(new IfClass(), splitvar) + $"{Environment.NewLine}";
                        code += "{" + $"{Environment.NewLine}";
                        existedvar();
                        endif = false;
                        break;
                    case "endif":
                        if (!endif)
                        {
                            code += "}" + $"{Environment.NewLine}";
                            endif = true;
                        }
                        else
                        {
                            dead = true;
                            MessageBox.Show($"Excess endif{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case string s when variables.Contains(splitvar[0]):
                        code += Client.ClientCode(new VarClass(), splitvar) + $";{Environment.NewLine}";
                        existedvar();
                        break;
                    default:
                        undrline(i);
                        MessageBox.Show($"Syntax Error{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        prcrash();
                        break;
                }
                linelength += richTextBox1.Lines[i].Length;

            }

            code += "}";
            if(!endif)
            {
                dead = true;
                MessageBox.Show($"endif doesnt exist{Environment.NewLine}Close window", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!dead)
                label1.Text = code;
        }
        void mainProg()
        {
            code += "static void main()\n";
            code += "{\n";
        }
        void existedvar()
        {
            for (int j = 1; j < splitvar.Length; j++)
            {
                if (variables.Contains(splitvar[j].TrimStart(splitariphm)))
                {
                    //MessageBox.Show(dtpvariables[variables.IndexOf(splitvar[j].TrimStart(splitariphm))].ToString(), dtpvariables[variables.IndexOf(splitvar[j - 1].TrimStart(splitariphm))].ToString(), MessageBoxButtons.OK);
                    if (splitvar[j - 1] == "if"||dtpvariables[variables.IndexOf(splitvar[j].TrimStart(splitariphm))] == dtpvariables[variables.IndexOf(splitvar[j - 1].TrimStart(splitariphm))])
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
                if (stroka[i] != ' ' && ariphm.Contains(stroka[i + 1]))
                {
                    result.Append(' ');
                }
            }

            if (stroka.Length > 0)
                result.Append(stroka[stroka.Length - 1]);
            richTextBox1.Clear();
            MessageBox.Show(result.ToString(), "", MessageBoxButtons.OK);
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
    }
    abstract class AbstractClass
    {
        // Шаблонный метод определяет скелет алгоритма.
        public string TemplateMethod(string[] var)
        {
            return (this.RequiredOperations1(var) + "\n" + this.RequiredOperation2(var));
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
            if (variable[1] == "int")
            {
                s = "32";
            }
            else
                s = "";
            return ($"{variable[1]} {variable[variable.Length - 1]} = Convert.To{ti.ToTitleCase(variable[1])}{s}(Console.ReadLine())");
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
            return ($"Console.WriteLine({variable[variable.Length - 1]})");
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
                StringBuilder str = new StringBuilder() ;
                for (int i = 3; i < variable.Length; i++)
                {
                    str.Append(variable[i]);
                }
                return ($"{variable[1]} {variable[2]} {str}");
            }
        }
    }
    class VarClass:AbstractClass
    {
        protected override string RequiredOperations1(string[] variable)
        {

            return ($"execute variable {variable[0]}");

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

            return ($"execute structure {variable[0]}");

        }

        protected override string RequiredOperation2(string[] variable)
        {

            StringBuilder str = new StringBuilder();
            for (int i = 1; i < variable.Length; i++)
            {
                str.Append(variable[i]);
            }
            return ($"{variable[0]}({str})");

        }
    }
    class Client
    {
        public static string ClientCode(AbstractClass abstractClass,string[] variable)
        {
            // ...
            return(abstractClass.TemplateMethod(variable));
            // ...
        }
    }

    
}
