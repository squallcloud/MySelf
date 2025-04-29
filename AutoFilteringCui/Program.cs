using AutoFilterginCui;
using Microsoft.Build.Evaluation;

string vcxprjPath = args[0];
Project project = new Project(vcxprjPath);
var projectControl = new ProjectControl(project);
projectControl.Execute();

return 0;
