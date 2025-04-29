using Microsoft.Build.Evaluation;

namespace AutoFilterginCui
{
    class ProjectControl
    {
        public Project TargetProject {
            get;
            private set;
        } = null;

        public FilterNode RootFilter {
            get;
            private set;
        } = new FilterNode();

        public ProjectControl(Project in_targetProject) {
            TargetProject = in_targetProject;
        }

        public void Execute() {
            CreateFilterNode();
            ParseFromFiles();
            ParseFilters();
            CreateFilters();

            TargetProject.Save();
        }

        void CreateFilterNode() {
            RootFilter = new FilterNode();
            RootFilter.Path = "";
            RootFilter.Name = "";
        }

        void ParseFromFiles() {

            var items = TargetProject.Items.ToList();
            var files = items.Where(x => x.ItemType != "Filter").ToList();

            foreach (ProjectItem file in files) {
                var dir = Path.GetDirectoryName(file.EvaluatedInclude);
                var dirs = dir.Split('\\');
                var curDir = "";
                var curNode = RootFilter;
                foreach (var d in dirs) {
                    curDir = Path.Combine(curDir, d);
                    var node = curNode.Filters.Find(x => x.Name == d);
                    if (node == null) {
                        //
                        var newNode = new FilterNode();
                        newNode.Name = d;
                        newNode.Path = curDir;
                        newNode.Parent = curNode;
                        curNode.Filters.Add(newNode);
                        curNode = newNode;
                    } else {
                        //
                        curNode = node;
                    }
                }

                if (curNode != null) {
                    var node = curNode.Files.Find(x => x.Path == file.EvaluatedInclude);
                    if (node == null) {
                        var newFileNode = new FileNode(file);
                        newFileNode.Path = file.EvaluatedInclude;
                        newFileNode.Parent = curNode;
                        newFileNode.Name = Path.GetFileName(file.EvaluatedInclude);
                        curNode.Files.Add(newFileNode);
                    } else {
                        //
                        if (node.Item == null) {
                            node.Item = file;
                        }
                    }
                }
            }
        }

        void ParseFilters() {
            var filters = TargetProject.Items.Where(x => x.ItemType == "Filter").ToList();
            foreach (var filter in filters) {
                var filterNode = GetFilter(filter.EvaluatedInclude);
                if (filterNode == null) {
                    TargetProject.RemoveItem(filter);
                } else {
                    filterNode.Item = filter;
                }
            }
        }

        void CreateFilters() {
            foreach (var filter in RootFilter.Filters) {
                CreateRecursiveFilter(filter);
            }
        }

        FilterNode GetFilter(string in_path) {

            var curNode = RootFilter;

            var dirs = in_path.Split('\\');
            foreach (var d in dirs) {
                var node = curNode.Filters.Find(x => x.Name == d);
                if (node == null) {
                    return null;
                } else {
                    curNode = node;
                }
            }

            return curNode;
        }

        void CreateRecursiveFilter(FilterNode in_parentFilterNode) {

            if (in_parentFilterNode.Item == null) {
                var newItems = TargetProject.AddItem("Filter", in_parentFilterNode.Path);
                var newItem = newItems.First();
                var newGuid = "{" + Guid.NewGuid().ToString() + "}";
                newItem.SetMetadataValue("UniqueIdentifier", newGuid);
                in_parentFilterNode.Item = newItem;
            }

            foreach (var fileNode in in_parentFilterNode.Files) {
                var fileItem = fileNode.Item;
                if (fileItem == null) {
                    continue;
                }
                fileItem.SetMetadataValue("Filter", in_parentFilterNode.Path);
            }

            foreach (var filter in in_parentFilterNode.Filters) {
                CreateRecursiveFilter(filter);
            }
        }
    }
}
