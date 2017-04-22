﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Tree.TreeContentSolution;

namespace Tree
{
    using System.IO;
    public partial class FormMain : Form
    {
        public delegate void AddNodeDelegats(ref TreeView treeView);
        public delegate void AddElementDelegats(Control element);

        public delegate string GetText(int n);
        public AddNodeDelegats AddNodeDelegate;
        public GetText GetTextDelegate;
        public AddElementDelegats AddElement;
        public Action ClearAction;
        public List<TreeContent> Content = new List<TreeContent>();
        private List<object> _controls = new List<object>();
        public TreeView TreeView;
        private object _selectNode;
        public FormMain()
        {
            InitializeComponent();
            AddNodeDelegate = CreateTreeView;
            AddElement = CreateElement;
            ClearAction = Clear;
            GetTextDelegate = GetTextMethod;
        }

        public string GetTextMethod(int n)
        {
            Control c = (Control) _controls[n];
            return c.Text;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(LoadView);
            thread.Start();
        }
        private void LoadView()
        {
            TreeView = new LoadTreeContent().SetList(ref Content);
            TreeView.Height = Height;
            TreeView.Width = Width / 3;
            TreeView.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top);
            Invoke(AddNodeDelegate, TreeView);
        }
        private void CreateTreeView(ref TreeView treeView)
        {
            Controls.Add(treeView);
            treeView.AfterSelect += SelectNode;
        }

        private void Clear()
        {
            foreach (var t in _controls)
            {
                Controls.Remove((Control)t);
            }
            _controls = new List<object>();
        }
        public void CreateElement(object element)
        {

            var el = (Control)element;
            int x = Width - el.Width - 20;
            int y = 0;
            if (_controls.Count > 0)
            {
                var last = (Control)_controls[_controls.Count - 1];
                y = last.Location.Y + last.Height;
            }
            el.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            el.Location = new Point(x, y);

            _controls.Add(element);
            Controls.Add(el);
        }

        public void SelectNode(object sender, TreeViewEventArgs e)
        {
            _selectNode = e.Node;
            TreeNode tNode = e.Node;
            TreeContent tContent = (TreeContent)tNode.Tag;
            Thread generateForm = new Thread(GenerateForm);
            generateForm.Start(tContent);
        }

        public void GenerateForm(object tContent)
        {
            TreeContent currentContent = (TreeContent)tContent;
            Invoke(ClearAction);
            GetType(currentContent);
        }

        public void GetType(TreeContent tContent)
        {
            if (tContent.Contents.Count == 0)
            {
                switch (tContent.TypeValue)
                {
                    case "TextBox":
                        {
                            var tBox = new TextBox { Text = tContent.Value, Tag = tContent.Value };
                            Invoke(AddElement, tBox);
                            break;
                        }
                    case "Lable":
                        {
                            var label = new Label { Text = tContent.Value, Tag = tContent.Value };
                            Invoke(AddElement, label);
                            break;
                        }
                    case "RichTextBox":
                        {
                            var richTextBox = new RichTextBox { Text = tContent.Value, Tag = tContent.Value };
                            Invoke(AddElement, richTextBox);
                            break;
                        }
                }
            }
            else
            {
                foreach (TreeContent t in tContent.Contents)
                {
                    GetType(t);
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            TreeView.ExpandAll();
            Controls.Remove(TreeView);
            Thread saveChange = new Thread(SaveChange);
            saveChange.Start();
        }
        // 21.05.23 06.04.2017 - - - xdfgxdfgxdgdf
        private void SaveChange()
        {
            string path = Directory.GetCurrentDirectory();
            string pathXmlTree = Path.Combine(path, @"XML\XMLTree.xml");
            TreeNode selectTreeNode = (TreeNode)_selectNode;

            XmlDocument xmlTree = new XmlDocument();
            try
            {
                xmlTree.Load(pathXmlTree);
            }
            catch (Exception e)
            {
                Logs.Logs.WriteException(e.Message);
            }
            try
            {
                XmlElement xRoot = xmlTree.DocumentElement;
                if (xRoot == null) throw new ArgumentNullException(nameof(xRoot));
                int num = 0;
                if (xRoot.HasChildNodes)
                {
                    foreach (XmlNode xNode in xRoot)
                    {
                        ChangeTree(ref num, xNode, selectTreeNode);
                    }
                }
            }
            catch (Exception e)
            {
                Logs.Logs.WriteException(e.Message);
            }
           
            try
            {

                xmlTree.Save(pathXmlTree);
            }
            catch (Exception e)
            {
                Logs.Logs.WriteException(e.Message);
            }
            LoadView();
        }

        private void ChangeTree(ref int num, XmlNode xNode, TreeNode selectNode)
        {
            if (xNode.Name == selectNode.Text)
            {
                if (selectNode.Nodes.Count==0)
                {
                    XmlNode xn= xNode.ChildNodes[0];
                    xn.InnerText= (string) Invoke(GetTextDelegate,num);
                    ++num;
                }
                else
                {
                    for (int i = 0; i < xNode.ChildNodes.Count; ++i)
                    {
                        ChangeTree(ref num,xNode.ChildNodes[i],selectNode.Nodes[i]);
                    }
                }
            }
            
            else
            {
                foreach (XmlNode xmlNode in xNode)
                {
                    ChangeTree(ref num, xmlNode, selectNode);
                }
            }
        }
    }
}