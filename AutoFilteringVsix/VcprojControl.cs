using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFiltering
{
    internal class VcprojControl
    {
        class UpdateInfo
        {
            public bool IsDeletedFilter { get; set; } = false;
            public bool IsAddFilter { get; set; } = false;
            public bool IsMoveFile { get; set; } = false;

            public bool IsAnyUpdate => IsDeletedFilter || IsAddFilter || IsMoveFile;

            public void Clear() {
                IsDeletedFilter = false;
                IsAddFilter = false;
                IsMoveFile = false;
            }

        }

        VCProject m_vcproj = null;
        Dictionary<string, VCFilter> m_filterMap = new Dictionary<string, VCFilter>();
        UpdateInfo m_updateInfo = new UpdateInfo();

        public VcprojControl(VCProject in_vcproj) {
            this.m_vcproj = in_vcproj;
        }

        public void ExecuteFiltering() {
            m_filterMap.Clear();
            m_updateInfo.Clear();

            Prepare();

            CreateFilter();
            MoveFile();
            RemoveEmptyFilter();

            if (m_updateInfo.IsAnyUpdate) {
                m_vcproj.Save();
            }

        }

        void Prepare() {

            IVCCollection files = m_vcproj.Files as IVCCollection;
            IVCCollection filters = m_vcproj.Filters as IVCCollection;

            //プロジェクトに含まれているファイルからフィルタ候補を登録
            foreach (VCFile file in files) {
                var dir = Path.GetDirectoryName(file.RelativePath);
                if (string.IsNullOrEmpty(dir)) {
                    continue;
                }
                if (m_filterMap.ContainsKey(dir)) {
                    //
                } else {
                    m_filterMap.Add(dir, null);
                }
            }

            //既存のフィルタを登録
            foreach (VCFilter filter in filters) {
                var filterName = filter.CanonicalName;
                if (m_filterMap.ContainsKey(filterName)) {
                    m_filterMap[filterName] = filter;
                }
            }
        }

        void CreateFilter() {

            var newFilterMap = new Dictionary<string, VCFilter>(m_filterMap);

            VCFilter filter = null;
            foreach (var pair in m_filterMap) {
                var dirs = pair.Key.Split('\\');
                string newKey = "";
                foreach (var dir in dirs) {
                    newKey = Path.Combine(newKey, dir);
                    if (pair.Value == null) {
                        if (filter == null) {
                            if (m_vcproj.CanAddFilter(dir)) {
                                filter = m_vcproj.AddFilter(dir) as VCFilter;
                                m_updateInfo.IsAddFilter = true;
                                newFilterMap[pair.Key] = filter;
                            } else {
                                filter = newFilterMap[newKey];
                            }
                        } else {
                            filter = filter.AddFilter(dir) as VCFilter;
                            m_updateInfo.IsAddFilter = true;
                            newFilterMap[pair.Key] = filter;
                        }
                    } else {
                        filter = newFilterMap[pair.Key];
                    }
                }

                filter = null;
            }

            foreach (var newPair in newFilterMap) {
                m_filterMap[newPair.Key] = newPair.Value;
            }
        }

        void MoveFile() {

            IVCCollection files = m_vcproj.Files as IVCCollection;

            foreach (VCFile file in files) {
                var dir = Path.GetDirectoryName(file.RelativePath);
                if (m_filterMap.ContainsKey(dir)) {
                    var filter = m_filterMap[dir];
                    if (file.CanMove(filter)) {
                        var filesInNextFilter = filter.Files as IVCCollection;
                        bool isAlreadyMove = false;
                        foreach (VCFile fileInNextFilter in filesInNextFilter) {
                            if (fileInNextFilter == file) {
                                isAlreadyMove = true;
                            }
                        }

                        if (!isAlreadyMove) {
                            file.Move(filter);
                            m_updateInfo.IsMoveFile = true;
                        }
                    }
                }
            }
        }

        void RemoveEmptyFilter() {
            var items = m_vcproj.Items as IVCCollection;

            var delList = new List<VCFilter>();

            foreach (VCProjectItem item in items) {
                var filter = item as VCFilter;
                if (filter != null) {
                    RemoveEmptyFilter(filter);

                    IVCCollection filters = filter.Filters as IVCCollection;
                    IVCCollection files = filter.Files as IVCCollection;

                    bool isExistFiles = files.Count > 0;
                    bool isExistFilters = filters.Count > 0;
                    if (!isExistFiles && !isExistFilters) {
                        delList.Add(filter);
                    }
                }
            }

            foreach (var filter in delList) {
                filter.Remove();
                m_updateInfo.IsDeletedFilter = true;
            }
        }

        void RemoveEmptyFilter(VCFilter in_filter) {
            var delList = new List<VCFilter>();
            IVCCollection filters = in_filter.Filters as IVCCollection;
            foreach (VCFilter filter in filters) {
                RemoveEmptyFilter(filter);

                IVCCollection childFilters = filter.Filters as IVCCollection;
                IVCCollection files = filter.Files as IVCCollection;

                bool isExistFiles = files.Count > 0;
                bool isExistFilters = childFilters.Count > 0;
                if (!isExistFiles && !isExistFilters) {
                    delList.Add(filter);
                }
            }

            foreach (var filter in delList) {
                filter.Remove();
                m_updateInfo.IsDeletedFilter = true;
            }
        }
    }
}
