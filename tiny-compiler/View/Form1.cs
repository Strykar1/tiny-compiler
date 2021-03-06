﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TinyCompiler.Controller;
using TinyCompiler.Model;
using TinyCompiler.View;

namespace TinyCompiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void OpenFile_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = OFD_Setup();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.OpenFile() != null)
                {
                    string fileText = File.ReadAllText(openFileDialog.FileName);
                    CodeText.Text = fileText;
                }
            }
        }
        private void Edit_button_Click(object sender, EventArgs e)
        {
            CodeText.ReadOnly = false;
            CodePanel.Visible = true;
            CodeText.Focus();
        }
        private void Save_button_Click(object sender, EventArgs e)
        {
            CodeText.ReadOnly = true;
            SaveFileDialog saveFileDialog = SFD_Setup();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.OpenFile()))
                {
                    streamWriter.WriteLine(CodeText.Text);
                }
            }
        }
        private void LA_button_Click(object sender, EventArgs e)
        {
            if(CodeText.Text != "")
            {
                Scanner scanner = new Scanner(CodeText.Text + " ");
                var tokens = scanner.getTokens();
                string tableText = "";
                foreach(Token token in tokens)
                {
                    tableText += token.Lexeme + "\t\t\t\t" + token.Type.ToString() + "\n";
                }
                //tableForm.SetTableText(tableText);
                //tableForm.Show();
                CodePanel.Visible = false;
                TreeText.Visible = true;
                TreeText.Text = tableText;

                string errorString = "";
                foreach(string error in scanner.Errors)
                {
                    errorString += error + "\n";
                }
                if(errorString.Length != 0)
                {
                    ErrorText.Text = errorString;
                }
                else
                {
                    ErrorText.Text = "All Clear!";
                }
            }
        }
        private void TT_button_Click(object sender, EventArgs e)
        {
            if (CodeText.Text != "")
            {
                Scanner scanner = new Scanner(CodeText.Text + " ");
                var tokens = scanner.getTokens();
                Parser parser = new Parser(tokens);

                TreeView tree = new TreeView();
                tree.Nodes.Add(parser.Parse());
                tree.Dock = DockStyle.Fill;

                CodePanel.Visible = false;
                TreeText.Visible = false;
                TreePanel.Visible = true;

                TreePanel.Controls.Add(tree);
                tree.ExpandAll();
                if(parser.ErrorString != "")
                    ErrorText.Text = parser.ErrorString;
                else
                    ErrorText.Text = "All Clear!";
            }
            else
            {
                ErrorText.Text = "No parsable code detected";
            }
        }
        OpenFileDialog OFD_Setup()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|tiny file (*.tny)|*.tny|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            return openFileDialog;
        }
        SaveFileDialog SFD_Setup()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files(*.txt)| *.txt | tiny file(*.tny) | *.tny | All files(*.*) | *.* ";
            saveFileDialog.FilterIndex = 2;
            return saveFileDialog;
        }
        private void Exit_button_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
