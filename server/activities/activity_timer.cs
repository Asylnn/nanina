using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using Nanina.Database;

namespace Nanina.Activities
{
    class ActivityTimer : System.Timers.Timer
    {
        public required ulong activityId;
        public required string userId;

        public ActivityTimer(double interval) : base(interval)
        {
            Elapsed += ActivityFinished;
            Start();
        }

        public static void ActivityFinished(object? sender, ElapsedEventArgs e)
        {
            if (sender is not ActivityTimer timer)
            {
                Console.Error.WriteLine("Activity timer is undefined on Activity Finished??");
                return;
            }
            Console.WriteLine("Activity timer finished");
            var user = DBUtils.Get<UserData.User>(x => x.Id == timer.userId)!;
            user.ActivityTimeout(timer.activityId);
            timer.Dispose();
            Global.activityTimers.Remove(timer.activityId);
        }
    }
}
