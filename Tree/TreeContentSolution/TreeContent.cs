using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Tree.TreeContentSolution
{
    public class TreeContent
    {
        public static FormMain Form
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Name { get; set; }
        public string TypeValue { get; set; }
        public string Value { get; set; }

        public List<TreeContent> Contents = new List<TreeContent>();
    }
}
