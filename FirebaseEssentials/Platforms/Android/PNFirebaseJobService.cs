using System;
using Android.App.Job;

namespace FirebaseEssentials.Platforms.Android
{
    public class PNFirebaseJobService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            return false;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return false;
        }
    }
}
