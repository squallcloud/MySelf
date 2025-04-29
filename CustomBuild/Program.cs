using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBuild
{
    class Program
    {
        static int Main(string[] args) {
            string vcxprjPath = args[0];
            Project project = new Project(vcxprjPath);
            var projectControl = new ProjectControl(project);
            projectControl.Execute();

            return 0;
        }
    }
}
