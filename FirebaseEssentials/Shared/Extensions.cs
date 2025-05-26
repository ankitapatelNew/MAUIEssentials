using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FirebaseEssentials.Shared
{
    public static class Extensions
	{
		public static void LogException(this Exception ex, [CallerFilePath] string filePath = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			var message = ex.Message;
			var stackTrace = ex.StackTrace;
			var log = $"App Log : FilePath: {filePath} MethodName: {member} Line: {line}\nExReport: {message}\nStackTrace: {stackTrace}";

			CrossFirebaseEssentials.Crashlytics.SetCustomKey("File path", filePath);
			CrossFirebaseEssentials.Crashlytics.SetCustomKey("Method", member);
			CrossFirebaseEssentials.Crashlytics.SetCustomKey("Line number", line);
			CrossFirebaseEssentials.Crashlytics.SetCustomKey("Message", message);
			CrossFirebaseEssentials.Crashlytics.SetCustomKey("Stacktrace", stackTrace);

			CrossFirebaseEssentials.Crashlytics.Log(message);
			CrossFirebaseEssentials.Crashlytics.LogException(ex);

#if DEBUG
			Debug.WriteLine(log);
#else
			Console.WriteLine(log);
#endif
		}
	}
}