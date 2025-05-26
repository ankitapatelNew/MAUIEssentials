using FirebaseEssentials.Shared;

namespace MAUIEssentials.AppCode.Exceptions
{
    public class ApiRequestMissingLinkException : Exception
    {
        public ApiRequestMissingLinkException()
        {
        }

        public ApiRequestMissingLinkException(string message) : base(message)
        {
            CrossFirebaseEssentials.Crashlytics.Log(message);
            CrossFirebaseEssentials.Crashlytics.LogException(this);
        }

        public ApiRequestMissingLinkException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}