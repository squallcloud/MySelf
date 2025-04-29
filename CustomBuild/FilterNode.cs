using Microsoft.Build.Evaluation;
using System.Collections.Generic;

namespace CustomBuild
{
    class FilterNode
    {
        public string Path {
            get;
            set;
        } = "";

        public string Name {
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

        public List<FilterNode> Filters {
            get;
            private set;
        } = new List<FilterNode>();

        public List<FileNode> Files {
            get;
            private set;
        } = new List<FileNode>();

        public FilterNode(ProjectItem in_item) {
            Item = in_item;
            Path = in_item.EvaluatedInclude;
            Name = System.IO.Path.GetFileName(in_item.EvaluatedInclude);
        }

        public FilterNode() {
            //
        }
    }
}
