using Hami.Common.IDE.VisualStudio.Helpers;
using Hami.Common.IDE.VisualStudio.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Hami.Common.IDE.VisualStudio
{
    internal class VS2017Service : VSBaseService
    {
        public override bool ClearRecentProjects(bool pinned, out string msg)
        {
            bool result = false;
            msg = string.Empty;

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string subPath = @"Microsoft\VisualStudio";
            string dirPath = Path.Combine(folderPath, subPath);

            if (!Directory.Exists(dirPath))
            {
                return true;
            }

            dirPath = Directory.GetDirectories(dirPath, "15.0_*", SearchOption.TopDirectoryOnly).ToList().FirstOrDefault();
            if (string.IsNullOrWhiteSpace(dirPath))
            {
                return true;
            }
            else if (!Directory.Exists(dirPath))
            {
                return true;
            }

            string filePath = Path.Combine(dirPath, "ApplicationPrivateSettings.xml");
            if (!File.Exists(filePath))
            {
                return true;
            }

            try
            {
                XDocument xDoc = XDocument.Load(filePath);
                XElement xElement = xDoc.Descendants("collection").FirstOrDefault(ele => ele.Attribute("name") != null && ele.Attribute("name").Value == "CodeContainers.Offline");
                if (xElement == null)
                {
                    return true;
                }

                XElement valueElement = xElement.Element("value");
                string jsonRecentProjectList = valueElement.Value;
                List<RecentProjectItemVS2017> recentProjectList = JsonConvert.DeserializeObject<List<RecentProjectItemVS2017>>(jsonRecentProjectList);

                recentProjectList.RemoveAll(ele => ele.Value != null && ele.Value.IsFavorite == pinned);
                jsonRecentProjectList = JsonConvert.SerializeObject(recentProjectList);
                valueElement.Value = jsonRecentProjectList;
                xDoc.Save(filePath);

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
            throw new NotImplementedException();
        }

        protected override string GetJumpListFileName()
        {
            //vs enterprise 2017 version 15.9.10
            return "c31b3d36438b5e2c.automaticDestinations-ms";
        }
        #region Private Methods
        #endregion
    }
}
