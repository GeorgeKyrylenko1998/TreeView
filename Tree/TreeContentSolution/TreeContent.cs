using System.Collections.Generic;

namespace Tree.TreeContentSolution
{
    public class TreeContent
    {
        public static FormMain Form { get; }
        public string Name { get; set; }
        public string TypeValue { get; set; }
        public string Value { get; set; }

        public List<TreeContent> Contents = new List<TreeContent>();
    }
}
