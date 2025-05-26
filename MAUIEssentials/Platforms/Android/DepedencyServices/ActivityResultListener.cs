using Android.App;
using Android.Content;

namespace MAUIEssentials.Platforms.Android.DepedencyServices
{
    public class ActivityResultListener
    {
        readonly TaskCompletionSource<bool> Complete = new TaskCompletionSource<bool>();

        public Task<bool> Task => Complete.Task;

        public ActivityResultListener(MainActivity activity)
        {
            try
            {
                activity.ActivityResult += OnActivityResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                var activity = (MainActivity)Platform.CurrentActivity;
                activity.ActivityResult -= OnActivityResult;

                Complete.TrySetResult(resultCode == Result.Ok);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
