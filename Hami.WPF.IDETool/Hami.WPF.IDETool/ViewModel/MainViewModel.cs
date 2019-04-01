using GalaSoft.MvvmLight.Command;
using Hami.Common.IDE;
using Hami.Common.IDE.VisualStudio.Models;
using Hami.WPF.IDETool.Common.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;

namespace Hami.WPF.IDETool.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        #region Properties
        private bool clearVS2015RecentProjectsNotPinned = true;

        public bool ClearVS2015RecentProjectsNotPinned
        {
            get { return clearVS2015RecentProjectsNotPinned; }
            set
            {
                if (value != clearVS2015RecentProjectsNotPinned)
                {
                    clearVS2015RecentProjectsNotPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2015RecentProjectsNotPinned);
                }
            }
        }

        private bool clearVS2015RecentProjectsPinned;

        public bool ClearVS2015RecentProjectsPinned
        {
            get { return clearVS2015RecentProjectsPinned; }
            set
            {
                if (value != clearVS2015RecentProjectsPinned)
                {
                    clearVS2015RecentProjectsPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2015RecentProjectsPinned);
                }
            }
        }

        private bool clearVS2015RecentFiles = true;

        public bool ClearVS2015RecentFiles
        {
            get { return clearVS2015RecentFiles; }
            set
            {
                if (value != clearVS2015RecentFiles)
                {
                    clearVS2015RecentFiles = value;
                    RaisePropertyChanged(() => this.ClearVS2015RecentFiles);
                }
            }
        }

        private bool clearVS2015JumpListNotPinned = true;

        public bool ClearVS2015JumpListNotPinned
        {
            get { return clearVS2015JumpListNotPinned; }
            set
            {
                if (value != clearVS2015JumpListNotPinned)
                {
                    clearVS2015JumpListNotPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2015JumpListNotPinned);
                }
            }
        }

        private bool clearVS2015JumListPinned;

        public bool ClearVS2015JumListPinned
        {
            get { return clearVS2015JumListPinned; }
            set
            {
                if (value != clearVS2015JumListPinned)
                {
                    clearVS2015JumListPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2015JumListPinned);
                }
            }
        }

        private bool clearVS2017RecentProjectsNotPinned = true;

        public bool ClearVS2017RecentProjectsNotPinned
        {
            get { return clearVS2017RecentProjectsNotPinned; }
            set
            {
                if (value != clearVS2017RecentProjectsNotPinned)
                {
                    clearVS2017RecentProjectsNotPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2017RecentProjectsNotPinned);
                }
            }
        }

        private bool clearVS2017RecentProjectsPinned;

        public bool ClearVS2017RecentProjectsPinned
        {
            get { return clearVS2017RecentProjectsPinned; }
            set
            {
                if (value != clearVS2017RecentProjectsPinned)
                {
                    clearVS2017RecentProjectsPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2017RecentProjectsPinned);
                }
            }
        }
        
        private bool clearVS2017RecentFiles = false;

        public bool ClearVS2017RecentFiles
        {
            get { return clearVS2017RecentFiles; }
            set
            {
                if (value != clearVS2017RecentFiles)
                {
                    clearVS2017RecentFiles = value;
                    RaisePropertyChanged(() => this.ClearVS2017RecentFiles);
                }
            }
        }

        private bool clearVS2017JumpListNotPinned = true;

        public bool ClearVS2017JumpListNotPinned
        {
            get { return clearVS2017JumpListNotPinned; }
            set
            {
                if (value != clearVS2017JumpListNotPinned)
                {
                    clearVS2017JumpListNotPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2017JumpListNotPinned);
                }
            }
        }

        private bool clearVS2017JumpListPinned;

        public bool ClearVS2017JumpListPinned
        {
            get { return clearVS2017JumpListPinned; }
            set
            {
                if (value != clearVS2017JumpListPinned)
                {
                    clearVS2017JumpListPinned = value;
                    RaisePropertyChanged(() => this.ClearVS2017JumpListPinned);
                }
            }
        }
        #endregion

        #region Commands
        private RelayCommand closeCommand;

        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand
                    ?? (closeCommand = new RelayCommand(
                    () =>
                    {
                        RadWindowManager.Current.CloseAllWindows();
                    }));
            }
        }

        private RelayCommand clearCommand;

        /// <summary>
        /// Gets the ClearCommand.
        /// </summary>
        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand
                    ?? (clearCommand = new RelayCommand(
                    () =>
                    {
                        List<string> msgList = new List<string>();
                        bool checkAny = false;

                        if (ClearVS2015RecentProjectsNotPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentProjects(VSVersion.VS2015, false, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2015RecentProjectsPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentProjects(VSVersion.VS2015, true, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2015RecentFiles)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentFiles(VSVersion.VS2015, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2015JumpListNotPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentJumplist(VSVersion.VS2015, false, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2015JumListPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentJumplist(VSVersion.VS2015, true, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2017RecentProjectsNotPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentProjects(VSVersion.VS2017, false, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2017RecentProjectsPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentProjects(VSVersion.VS2017, true, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2017RecentFiles)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentFiles(VSVersion.VS2017, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2017JumpListNotPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentJumplist(VSVersion.VS2017, false, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (ClearVS2017JumpListPinned)
                        {
                            checkAny = true;
                            VSServiceContext.Instance.ClearRecentJumplist(VSVersion.VS2017, true, out string tempMessage);
                            msgList.Add(tempMessage);
                        }

                        if (checkAny)
                        {
                            string msg = string.Join(Environment.NewLine, msgList);
                            if (!string.IsNullOrWhiteSpace(msg))
                            {
                                MessageBox.Show(msg);
                            }
                            else
                            {
                                MessageBox.Show("清理成功，请重启Visual Studio");
                            }
                        }
                    }));
            }
        }

        private RelayCommand checkAllCommand;

        /// <summary>
        /// Gets the CheckAllCommand.
        /// </summary>
        public RelayCommand CheckAllCommand
        {
            get
            {
                return checkAllCommand
                    ?? (checkAllCommand = new RelayCommand(
                    () =>
                    {
                        ClearVS2015RecentProjectsNotPinned = true;
                        ClearVS2015RecentProjectsPinned = true;
                        ClearVS2015RecentFiles = true;
                        ClearVS2015JumpListNotPinned = true;
                        ClearVS2015JumListPinned = true;

                        ClearVS2017RecentProjectsNotPinned = true;
                        ClearVS2017RecentProjectsPinned = true;
                        //ClearVS2017RecentFiles = true;
                        ClearVS2017JumpListNotPinned = true;
                        ClearVS2017JumpListPinned = true;
                    }));
            }
        }

        private RelayCommand uncheckAllCommand;

        /// <summary>
        /// Gets the UncheckAllCommand.
        /// </summary>
        public RelayCommand UncheckAllCommand
        {
            get
            {
                return uncheckAllCommand
                    ?? (uncheckAllCommand = new RelayCommand(
                    () =>
                    {
                        ClearVS2015RecentProjectsNotPinned =false;
                        ClearVS2015RecentProjectsPinned = false;
                        ClearVS2015RecentFiles = false;
                        ClearVS2015JumpListNotPinned = false;
                        ClearVS2015JumListPinned = false;

                        ClearVS2017RecentProjectsNotPinned = false;
                        ClearVS2017RecentProjectsPinned = false;
                        ClearVS2017RecentFiles = false;
                        ClearVS2017JumpListNotPinned = false;
                        ClearVS2017JumpListPinned = false;
                    }));
            }
        }
        
        #endregion
    }
}