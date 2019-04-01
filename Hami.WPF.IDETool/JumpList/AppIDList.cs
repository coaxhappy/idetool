using System;
using System.Collections.Generic;
using System.IO;
using JumpList.Properties;

namespace JumpList
{
    public class AppIDList
    {
        private readonly Dictionary<string, string> AppIDs;

        public AppIDList()
        {
            //load included
            string[] stringSeparators = { "\r\n", "\n" };

            var lines = Resources.AppIDs.Split(stringSeparators, StringSplitOptions.None);

            //    var lines = File.ReadAllLines(Resources.AppIDs);

            AppIDs = new Dictionary<string, string>();

            IterateLines(lines);
        }

        public string GetDescriptionFromId(string id)
        {
            var desc = "Unknown AppId";

            var intId = id.ToLowerInvariant();

            if (AppIDs.ContainsKey(intId))
            {
                desc = AppIDs[id];
            }


            return desc;
        }

        private int IterateLines(string[] lines)
        {
            var added = 0;

            foreach (var line in lines)
            {
                var segs = line.Split('|');

                if (segs.Length != 2)
                {
                    continue;
                }

                var id = segs[0].Trim().ToLowerInvariant();
                var desc = segs[1].Trim();

                if (id.Length != 16)
                {
                    continue;
                }

                if (AppIDs.ContainsKey(id) == false)
                {
                    AppIDs.Add(id, desc);
                    added += 1;
                }
                else
                {
                    //key exists, so replace descrption
                    AppIDs[id] = desc;
                }
            }


            return added;
        }

        public int LoadAppListFromFile(string filename)
        {
            //open file, parse, add to dictionary if key isnt already there, replace if it is there

            var lines = File.ReadAllLines(filename);

            return IterateLines(lines);
        }
    }
}