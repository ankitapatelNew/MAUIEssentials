namespace MAUIEssentials
{
    public partial class Logging
    {
        static Logging? _instance;
        public static Logging Instance
        {
            get {
                if(_instance == null) _instance = new Logging();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public partial string GetFile();
        public partial void ClearLog();
        public partial void WriteLog(string title, string log);
        public partial void OpenLog();
        public partial void WriteLogFile();
    }
}
