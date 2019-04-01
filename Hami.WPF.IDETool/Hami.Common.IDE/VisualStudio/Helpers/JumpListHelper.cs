using Hami.Common.IDE.VisualStudio.Models;
using JumpList.Automatic;
using OpenMcdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hami.Common.IDE.VisualStudio.Helpers
{
    internal class JumpListHelper
    {
        public static bool ClearJumpList(string fileName, bool pinned, out string msg)
        {
            bool result = false;
            msg = string.Empty;

            string fileFullName = GetJumpListFileFullName(fileName);

            if (!File.Exists(fileFullName))
            {
                return true;
            }

            try
            {
                byte[] rawBytes = File.ReadAllBytes(fileFullName);
                CompoundFile comFile = new CompoundFile(fileFullName, CFSUpdateMode.Update, CFSConfiguration.Default);

                try
                {
                    AutomaticDestination des = new AutomaticDestination(rawBytes, fileFullName);
                    int numberDirs = comFile.GetNumDirectories();

                    int sid = -1;
                    for (int i = 0; i < numberDirs; i++)
                    {
                        string dirName = comFile.GetNameDirEntry(i);
                        if (dirName.ToLowerInvariant() == "destlist")
                        {
                            sid = i;
                            break;
                        }
                    }

                    if (sid <= 0)
                    {
                        return true;
                    }

                    byte[] bufferDesList = comFile.GetDataBySID(sid);
                    DestList desList = new DestList(bufferDesList);

                    if (pinned)
                    {
                        desList.Entries.RemoveAll(ele => ele.PinStatus != -1);
                    }
                    else
                    {
                        desList.Entries.RemoveAll(ele => ele.PinStatus == -1);
                    }

                    byte[] desListResultBuffer = desList.ToBuffer();
                    comFile.RootStorage.VisitEntries(cfItem =>
                    {
                        if (cfItem is CFStream cfStream && cfItem.Name.ToLowerInvariant() == "destlist")
                        {

                            cfStream.SetData(desListResultBuffer);
                        }
                    }, true);

                    comFile.Commit();
                    result = true;
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    result = false;
                }
                finally
                {
                    comFile.Close();
                }
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            
            return result;
        }

        //private static AutomaticDestination GetJumpListDestination(string fileFullName)
        //{
        //    AutomaticDestination jumpListModel = null;
        //    try
        //    {
        //        jumpListModel = JumpList.JumpList.LoadAutoJumplist(fileFullName);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return jumpListModel;
        //}

        public static List<JumpListModel> GetJumpListItems(string fileName)
        {
            string fileFullName = GetJumpListFileFullName(fileName);
            AutomaticDestination des = JumpList.JumpList.LoadAutoJumplist(fileFullName);
            if (des == null)
            {
                throw new Exception("can note read jumplist file");
            }

            List<JumpListModel> resultList = new List<JumpListModel>();

            foreach (var item in des.DestListEntries)
            {
                resultList.Add(new JumpListModel()
                {
                    IsPinned = item.Pinned,
                    SourceFileName = item.Path,
                    MRUPosition = item.MRUPosition,
                });
            }

            return resultList;
        }

        public static string GetJumpListFileFullName(string fileName)
        {
            return Path.Combine(GetJumpListFolder(), fileName);
        }

        public static string GetJumpListFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Recent), "AutomaticDestinations");
        }
    }
}
