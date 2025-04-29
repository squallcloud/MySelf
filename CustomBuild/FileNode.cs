using Microsoft.Build.Evaluation;

namespace CustomBuild
{
    class FileNode
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

        public FileNode(ProjectItem in_item) {
            Item = in_item;
            Path = in_item.EvaluatedInclude;
            Name = System.IO.Path.GetFileName(in_item.EvaluatedInclude);
        }

        public FileNode() {
            //
        }
    }
}
