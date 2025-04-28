using Microsoft.Build.BuildEngine;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBuild
{
    class FilterNode
    {
        public string NodeType {
            get;
            set;
        } = "";

        public string Path {
            get;
            set;
        } = "";

        public ProjectItem Item {
            get;
            set;
        } = null;

        public FilterNode Parent {
            get;
            set;
        } = null;

        public List<FilterNode> Children {
            get;
            private set;
        } = new List<FilterNode>();

        public FilterNode(ProjectItem in_item) {
            NodeType = in_item.ItemType;
        }
    }
}
