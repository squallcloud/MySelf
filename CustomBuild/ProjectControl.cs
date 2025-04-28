using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBuild
{
    class ProjectControl
    {
        public Project TargetProject {
            get;
            private set;
        } = null;

        public ProjectControl(Project in_targetProject) {
            TargetProject = in_targetProject;
        }
    }
}
