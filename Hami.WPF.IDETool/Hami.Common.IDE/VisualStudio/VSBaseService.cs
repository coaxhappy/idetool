using Hami.Common.IDE.VisualStudio.Helpers;
using Hami.Common.IDE.VisualStudio.Models;
using JumpList.Automatic;
using OpenMcdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using StructuredData.Common;
//using StructuredData.Common.Wrappers;

namespace Hami.Common.IDE.VisualStudio
{
    internal abstract class VSBaseService
    {
        #region public abstract methods
        public abstract bool ClearRecentProjects(bool pinned, out string msg);

        public abstract bool ClearRecentFiles(out string msg);
        #endregion

        #region protect abstract methods
        protected abstract string GetJumpListFileName();
        #endregion

        #region public methods
        /// <summary>
        /// 清空VS TaskBar上右键中的最近打开
        /// </summary>
        public bool ClearRecentJumpList(bool pinned, out string msg)
        {
            string fileName = GetJumpListFileName();

            return JumpListHelper.ClearJumpList(fileName, pinned, out msg);
        }

        public List<JumpListModel> GetJumListItems()
        {
            return JumpListHelper.GetJumpListItems(GetJumpListFileName());
        }
        #endregion

        #region protect methods

        #endregion

        #region private methods
        #endregion
    }
}
