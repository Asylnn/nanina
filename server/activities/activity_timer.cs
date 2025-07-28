using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using Nanina.Database;

namespace Nanina.Activities
{
    class ActivityTimer : System.Timers.Timer
    {
        public ulong activityId;
        public string userId;

        public ActivityTimer(double interval) : base(interval)
        {
            Elapsed += ActivityFinished;
            Start();
        }

        public static void ActivityFinished(object sender, ElapsedEventArgs e)
        {
            var timer = sender as ActivityTimer;
            Console.WriteLine("Activity timer finished");
            var user = DBUtils.Get<UserData.User>(x => x.Id == timer.userId);
            user.ActivityTimeout(timer.activityId);
            timer.Dispose();
        }
    }
}
