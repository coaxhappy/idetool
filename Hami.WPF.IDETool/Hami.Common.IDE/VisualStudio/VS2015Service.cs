using Hami.Common.IDE.VisualStudio.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hami.Common.IDE.VisualStudio
{
    internal class VS2015Service : VSBaseService
    {
        public override bool ClearRecentProjects(bool pinned, out string msg)
        {
            bool result = false;
            msg = string.Empty;

            //最近的项目HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0\MRUItems\{a9c4a31f-f9cb-47a9-abc0-49ce82d0b3ac}\Items
            //最近的文件HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0\MRUItems\{01235aad-8f1b-429f-9d02-61a0101ea275}\Items
            try
            {
                List<RecentProjectItemVS2015> recentProjectItems = new List<RecentProjectItemVS2015>();
                RegistryKey rootKey = GetVSRootRegKey("{a9c4a31f-f9cb-47a9-abc0-49ce82d0b3ac}");
                if (rootKey == null)
                {
                    return true;
                }
                
                string[] recentProjectRegNames = rootKey.GetValueNames();
                foreach (var item in recentProjectRegNames)
                {
                    object o = rootKey.GetValue(item);
                    if (o is string tempString)
                    {
                        if (tempString.Length > 0)
                        {
                            string[] tempArray = tempString.Split('|');
                            if (tempArray.Length >= 4)
                            {
                                recentProjectItems.Add(new RecentProjectItemVS2015()
                                {
                                    Name = tempArray[3],
                                    NameRegValue = item,
                                    Path = tempArray[0],
                                    IsPinned = tempArray[2] == "True"
                                });
                            }
                        }
                    }
                }

                List<RecentProjectItemVS2015> projectItemsToClear = recentProjectItems.Where(ele => ele.IsPinned == pinned).ToList();

                foreach (var item in projectItemsToClear)
                {
                    rootKey.DeleteValue(item.NameRegValue);
                }

                rootKey.Close();
                result = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return result;
        }

        public override bool ClearRecentFiles(out string msg)
        {
            bool result = false;
            msg = string.Empty;

            //最近的项目HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0\MRUItems\{a9c4a31f-f9cb-47a9-abc0-49ce82d0b3ac}\Items
            //最近的文件HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0\MRUItems\{01235aad-8f1b-429f-9d02-61a0101ea275}\Items
            try
            {
                List<RecentProjectItemVS2015> recentProjectItems = new List<RecentProjectItemVS2015>();

                RegistryKey rootKey = GetVSRootRegKey("{01235aad-8f1b-429f-9d02-61a0101ea275}");
                if (rootKey == null)
                {
                    return true;
                }

                string[] recentFileRegNames = rootKey.GetValueNames();
                foreach (var item in recentFileRegNames)
                {
                    rootKey.DeleteValue(item);
                }

                rootKey.Close();
                result = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return result;
        }

        protected override string GetJumpListFileName()
        {
            //vs enterprise 2015 version 14.0 update 3
            return "b08971c77377bde3.automaticDestinations-ms";
        }

        private RegistryKey GetVSRootRegKey(string keyId)
        {
            RegistryKey rootKey = null;
            try
            {
                rootKey = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\Microsoft\VisualStudio\14.0\MRUItems\{0}\Items", keyId), true);
            }
            catch (Exception)
            {
                rootKey = null;
            }

            return rootKey;
        }
    }
}
