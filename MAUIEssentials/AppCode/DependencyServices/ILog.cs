namespace MAUIEssentials
{
    public interface ILog
	{
		void Log(LogEnum log, string message);
	}

	public enum LogEnum
	{
		Error,
		Info,
		Warn,
		Debug
	}
	public partial class LogUtils
	{
        public partial void Log(LogEnum log, string message);
    }
}
