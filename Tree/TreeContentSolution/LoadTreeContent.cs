using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Tree.TreeContentSolution
{
    class LoadTreeContent
    {
        public TreeView SetList(ref List<TreeContent> contents)
        {
            TreeView localTreeView = new TreeView();
            contents = new List<TreeContent>();
            string path = Directory.GetCurrentDirectory();
            string pathXmlTree = Path.Combine(path, @"XML\XMLTree.xml");
            localTreeView.Nodes.Clear();


            string pathXmlTreeType = Path.Combine(path, @"XML\XMLTreeType.xml");
            XmlDocument xmlTreeType = new XmlDocument();
            xmlTreeType.Load(pathXmlTreeType);
            XmlElement xRootType = xmlTreeType.DocumentElement;

            XmlDocument xmlTree = new XmlDocument();
            xmlTree.Load(pathXmlTree);
            XmlElement xRoot = xmlTree.DocumentElement;

            if (xRoot != null)
                foreach (XmlNode xNode in xRoot)
                {
                    contents.Add(new TreeContent { Name = xNode.Name });
                    localTreeView.Nodes.Add(xNode.Name);
                    localTreeView.Nodes[localTreeView.Nodes.Count - 1].Tag = contents[contents.Count - 1];
                    var tNode = localTreeView.Nodes[localTreeView.Nodes.Count - 1];
                    addTreeNode(xNode, ref tNode, contents[contents.Count - 1]);
                }
            AddTypeNode(contents, xRootType);
            return localTreeView;
        }

        private void addTreeNode(XmlNode xmlNode, ref TreeNode treeNode, TreeContent treeContent)
        {
            if (xmlNode.HasChildNodes) //The current node has children
            {
                var xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x < xNodeList.Count; x++)
                {
                    var xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    treeContent.Contents.Add(new TreeContent
                    {
                        Name = xNode.Name,
                        Value = xNode.InnerText
                    });
                    treeNode.Nodes[treeNode.Nodes.Count - 1].Tag = treeContent.Contents[treeContent.Contents.Count - 1];
                    var tNode = treeNode.Nodes[x];
                    var treeContentContent = treeContent.Contents[x];
                    addTreeNode(xNode, ref tNode, treeContentContent);
                }
            }
            else //No children, so add the outer xml (trimming off whitespace)
            {
                treeNode.Remove();
            }
        }

        private void AddTypeNode(List<TreeContent> contents, XmlNode xRootType)
        {
            int lastPoint = 0;
            int countPoint = xRootType.ChildNodes.Count;
            foreach (TreeContent t in contents)
            {
                for (int i = lastPoint; i < countPoint; ++i)
                {
                    if (t.Name == xRootType.ChildNodes[i].Name)
                    {
                        lastPoint = i + 1;
                        if (!xRootType.ChildNodes[i].HasChildNodes)
                        {
                            t.TypeValue = xRootType.ChildNodes[i].InnerText;
                        }
                        else
                        {
                            AddTypeNode(t.Contents, xRootType.ChildNodes[i]);
                        }
                        break;
                    }
                }
            }
        }
    }
}
