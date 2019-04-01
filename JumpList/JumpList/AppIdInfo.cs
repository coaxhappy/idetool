namespace JumpList
{
    public class AppIdInfo
    {
        public AppIdInfo(string appId)
        {
            AppId = appId;
        }

        public string AppId { get; }

        public string Description =>
            JumpList.AppIdList.GetDescriptionFromId(AppId);

        public override string ToString()
        {
            return $"{AppId} ==> {Description}";
        }
    }
}