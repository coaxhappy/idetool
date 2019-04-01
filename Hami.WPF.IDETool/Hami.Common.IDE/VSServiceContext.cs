using Hami.Common.IDE.VisualStudio;
using Hami.Common.IDE.VisualStudio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hami.Common.IDE
{
    public class VSServiceContext
    {
        /// <summary>
        /// The object to lock for singleton instance
        /// </summary>
        private static object asyncObj = new object();

        private VSServiceContext()
        {

        }

        private static VSServiceContext instance;

        /// <summary>
        /// Singleton instance property
        /// </summary>
        public static VSServiceContext Instance
        {
            get
            {
                lock (asyncObj)
                {
                    if (instance == null)
                    {
                        instance = new VSServiceContext();
                    }

                    return instance;
                }
            }
        }

        public bool ClearRecentProjects(VSVersion version, bool pinned, out string message)
        {
            bool result = false;
            message = string.Empty;
            try
            {
                VSBaseService service = GetVSService(version);
                result = service.ClearRecentProjects(pinned, out message);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return result;
        }

        public bool ClearRecentFiles(VSVersion version, out string message)
        {
            bool result = false;
            message = string.Empty;
            try
            {
                VSBaseService service = GetVSService(version);
                result = service.ClearRecentFiles(out message);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return result;
        }

        public bool ClearRecentJumplist(VSVersion version, bool pinned, out string message)
        {
            bool result = false;
            message = string.Empty;
            try
            {
                VSBaseService service = GetVSService(version);
                result = service.ClearRecentJumpList(pinned, out message);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return result;
        }

        #region Private Methods
        private VSBaseService GetVSService(VSVersion version)
        {
            VSBaseService service = null;
            switch (version)
            {
                case VSVersion.VS2015:
                    service = new VS2015Service();
                    break;
                case VSVersion.VS2017:
                    service = new VS2017Service();
                    break;
                default:
                    break;
            }

            return service;
        }
        #endregion
    }
}
