using System.IO;
using JumpList.Automatic;
using JumpList.Custom;

namespace JumpList
{
    public static class JumpList
    {
        static JumpList()
        {
            if (AppIdList == null)
            {
                AppIdList = new AppIDList();
            }
        }

        public static AppIDList AppIdList { get; }


        public static AutomaticDestination LoadAutoJumplist(string autoName)
        {
            var raw = File.ReadAllBytes(autoName);

            return new AutomaticDestination(raw, autoName);
        }

        public static CustomDestination LoadCustomJumplist(string customName)
        {
            var raw = File.ReadAllBytes(customName);

            return new CustomDestination(raw, customName);
        }
    }
}