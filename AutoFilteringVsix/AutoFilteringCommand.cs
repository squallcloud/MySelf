﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using Task = System.Threading.Tasks.Task;

namespace AutoFiltering
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AutoFilteringCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a0ae8a40-b4b0-4257-98c0-5edbf75733ec");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFilteringCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AutoFilteringCommand(AsyncPackage package, OleMenuCommandService commandService) {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AutoFilteringCommand Instance {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider {
            get {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package) {
            // Switch to the main thread - the call to AddCommand in AutoFilteringCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AutoFilteringCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Package.GetGlobalService(typeof(DTE)) is DTE dte) {

                var prjList = new List<string>();

                foreach (SelectedItem selItem in dte.SelectedItems) {
                    var prj = selItem.Project;
                    if (prj == null || prj.Object == null) {
                        continue;
                    }
                    var vcprj = prj.Object as VCProject;
                    var fullPath = vcprj.ProjectFile;
                    prjList.Add(fullPath);
                }

                foreach (var prjPath in prjList) {
                    var prjFilterPath = prjPath + ".filters";
                    if (System.IO.File.Exists(prjFilterPath)) {
                        using (var prjCtrl = new ProjectControl(prjFilterPath)) {
                            prjCtrl.Execute();
                        }
                    }
                }

                dte.ExecuteCommand("Project.UnloadProject");
                dte.ExecuteCommand("Project.ReloadProject");
            }
        }
    }
}
