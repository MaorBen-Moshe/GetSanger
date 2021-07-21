using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Threading.Tasks;

namespace GetSanger.Utils
{
    public static class ActivitiesConfirmationHelper
    {
        private static readonly IPageService sr_PageService;
        private static readonly IUiPush sr_PushService;

        static ActivitiesConfirmationHelper()
        {
            sr_PageService = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            sr_PushService = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
        }


        public async static void ConfirmActivity(Activity activity, Action action)
        {
            if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client) && activity.Status.Equals(eActivityStatus.Pending))
            {
                await sr_PageService.DisplayAlert("Note", "Are you sure?", "Yes", "No",
                    async (answer) =>
                    {
                        activity.Status = eActivityStatus.Active;
                        await FireStoreHelper.UpdateActivity(activity);
                        await sr_PushService.SendToDevice(activity.SangerID, activity, typeof(Activity).Name, "Activity Confirmed", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} accepted your job offer :)");
                        User sanger = await FireStoreHelper.GetUser(activity.SangerID);
                        sanger.ActivatedMap.Add(activity.ActivityId, false);
                        await FireStoreHelper.UpdateUser(sanger);
                        //  need to check that the list(ActivitiesSource) is updated
                        foreach (Activity current in AppManager.Instance.ConnectedUser.Activities)
                        {
                            if (current.JobDetails.JobId.Equals(activity.JobDetails.JobId))
                            {
                                current.Status = eActivityStatus.Rejected;
                                await sr_PushService.SendToDevice(current.SangerID, activity, typeof(Activity).Name, "Activity Rejected", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :)");
                            }
                        }
                        action?.Invoke();
                    });
            }
        }

        public static async void RejectActivity(Activity activity, Action action)
        {
            if (activity.Status.Equals(eActivityStatus.Pending) || activity.Status.Equals(eActivityStatus.Active))
            {
                switch (AppManager.Instance.CurrentMode)
                {
                    case eAppMode.Client:
                        await doReject(activity, activity.SangerID, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :(");
                        break;
                    case eAppMode.Sanger:
                        await doReject(activity, activity.ClientID, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} decided to cancel the job offer he already accepted. for more information please contact him :(");
                        break;
                }

                action?.Invoke();
            }
        }

        private async static Task doReject(Activity i_Activity, string i_SendToUserId, string i_Message)
        {
            await sr_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No",
                async (answer) =>
                {
                    if (answer)
                    {
                        if (i_Activity.Status.Equals(eActivityStatus.Active))
                        {
                            User user;
                            if (i_Activity.SangerID.Equals(AuthHelper.GetLoggedInUserId()))
                            {
                                user = AppManager.Instance.ConnectedUser;
                            }
                            else
                            {
                                user = await FireStoreHelper.GetUser(i_Activity.SangerID);
                            }

                            user.ActivatedMap.Remove(i_Activity.ActivityId);
                            await FireStoreHelper.UpdateUser(user);
                        }

                        i_Activity.Status = eActivityStatus.Rejected;
                        await FireStoreHelper.UpdateActivity(i_Activity);
                        await sr_PushService.SendToDevice(i_SendToUserId, i_Activity, typeof(Activity).Name, "Activity Rejected", i_Message);
                    }
                });
        }
    }
}
