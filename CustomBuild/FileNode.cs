using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBuild
{
    class FileNode
    {
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

        public FileNode(ProjectItem in_item) {
            Item = in_item;
        }
    }
}
