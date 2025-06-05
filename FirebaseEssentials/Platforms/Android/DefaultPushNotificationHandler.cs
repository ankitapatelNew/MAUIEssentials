using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using Java.Net;
using Newtonsoft.Json;
using Application = Android.App.Application;
using Color = Android.Graphics.Color;
using FirebaseEssentials.Shared;

namespace FirebaseEssentials.Platforms.Android
{
    public class DefaultPushNotificationHandler : IPushNotificationHandler
    {
        public const string DomainTag = "DefaultPushNotificationHandler";

        public const string TitleKey = "title";

        public const string TextKey = "text";

        public const string SubtitleKey = "subtitle";

        public const string MessageKey = "message";

        public const string BodyKey = "body";

        public const string AlertKey = "alert";

        public const string IdKey = "id";

        public const string TagKey = "tag";

        public const string ActionKey = "click_action";

        public const string CategoryKey = "category";

        public const string SilentKey = "silent";

        public const string ActionNotificationIdKey = "action_notification_id";

        public const string ActionNotificationTagKey = "action_notification_tag";

        public const string ActionIdentifierKey = "action_identifier";

        public const string ColorKey = "color";

        public const string IconKey = "icon";

        public const string SoundKey = "sound";

        public const string PriorityKey = "priority";

        public const string ChannelIdKey = "android_channel_id";

        public const string BodyLocKey = "body_loc_key";

        public const string BodyLocArgsKey = "body_loc_args";

        public const string TitleLocKey = "title_loc_key";

        public const string TitleLocArgsKey = "title_loc_args";

        public const string ImageKey = "image";

        public const string LinkPathKey = "link_path";

        readonly Context context;
        readonly System.Random random;
        readonly NotificationManagerCompat notificationManager;

        public DefaultPushNotificationHandler()
        {

            random = new System.Random();
            context = global::Android.App.Application.Context;
            notificationManager = NotificationManagerCompat.From(context);

            AppPreferences.SaveNotification(new List<NotificationModel>());
        }

        public void OnOpened(NotificationResponse response)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnOpened");
        }

        public void OnReceived(IDictionary<string, object> parameters)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnReceived");

            if (parameters.TryGetValue(SilentKey, out object silent) && (silent.ToString() == "true" || silent.ToString() == "1"))
                return;

            string title = context.ApplicationInfo.LoadLabel(context.PackageManager);
            var message = string.Empty;
            var tag = string.Empty;
            var image = string.Empty;

            if (!string.IsNullOrEmpty(FirebasePushNotificationManager.NotificationContentTextKey) && parameters.TryGetValue(FirebasePushNotificationManager.NotificationContentTextKey, out object notificationContentText))
                message = notificationContentText.ToString();
            else if (parameters.TryGetValue(AlertKey, out object alert))
                message = $"{alert}";
            else if (parameters.TryGetValue(BodyKey, out object body))
                message = $"{body}";
            else if (parameters.TryGetValue(MessageKey, out object messageContent))
                message = $"{messageContent}";
            else if (parameters.TryGetValue(SubtitleKey, out object subtitle))
                message = $"{subtitle}";
            else if (parameters.TryGetValue(TextKey, out object text))
                message = $"{text}";

            if (!string.IsNullOrEmpty(FirebasePushNotificationManager.NotificationContentTitleKey) && parameters.TryGetValue(FirebasePushNotificationManager.NotificationContentTitleKey, out object notificationContentTitle))
                title = notificationContentTitle.ToString();
            else if (parameters.TryGetValue(TitleKey, out object titleContent))
            {
                if (!string.IsNullOrEmpty(message))
                    title = $"{titleContent}";
                else
                    message = $"{titleContent}";
            }

            // set priority if not available
            if (FirebasePushNotificationManager.DefaultNotificationChannelImportance != NotificationImportance.Default
                && !parameters.ContainsKey(PriorityKey))
            {
                switch (FirebasePushNotificationManager.DefaultNotificationChannelImportance)
                {
                    case NotificationImportance.Min:
                        parameters.Add(PriorityKey, "min");
                        break;
                    case NotificationImportance.High:
                        parameters.Add(PriorityKey, "high");
                        break;
                    case NotificationImportance.Low:
                        parameters.Add(PriorityKey, "low");
                        break;
                    case NotificationImportance.Max:
                        parameters.Add(PriorityKey, "max");
                        break;
                }
            }

            int notifyId = random.Next(1, 999);
            if (parameters.TryGetValue(IdKey, out object id))
            {
                try
                {
                    notifyId = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    // Keep the default value of zero for the notify_id, but log the conversion problem.
                    System.Diagnostics.Debug.WriteLine($"Failed to convert {id} to an integer {ex}");
                }
            }

            if (parameters.TryGetValue(TagKey, out object tagContent))
            {
                tag = tagContent.ToString();
            }

            if (parameters.TryGetValue(ImageKey, out object imageContent))
            {
                image = imageContent.ToString();
            }

            try
            {
                if (parameters.TryGetValue(IconKey, out object icon) && icon != null)
                {
                    try
                    {
                        FirebasePushNotificationManager.IconResource = context.Resources.GetIdentifier(icon.ToString(), "drawable", global::Android.App.Application.Context.PackageName);
                    }
                    catch (Resources.NotFoundException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                if (FirebasePushNotificationManager.IconResource == 0)
                    FirebasePushNotificationManager.IconResource = context.ApplicationInfo.Icon;
                else
                {
                    string name = context.Resources.GetResourceName(FirebasePushNotificationManager.IconResource);
                    if (name == null)
                        FirebasePushNotificationManager.IconResource = context.ApplicationInfo.Icon;
                }
            }
            catch (Resources.NotFoundException ex)
            {
                FirebasePushNotificationManager.IconResource = context.ApplicationInfo.Icon;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            if (parameters.TryGetValue(ColorKey, out object color) && color != null)
            {
                try
                {
                    FirebasePushNotificationManager.Color = Color.ParseColor(color.ToString());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{DomainTag} - Failed to parse color {ex}");
                }
            }

            Intent resultIntent = typeof(Activity).IsAssignableFrom(FirebasePushNotificationManager.NotificationActivityType) ? new Intent(global::Android.App.Application.Context, FirebasePushNotificationManager.NotificationActivityType) : (FirebasePushNotificationManager.DefaultNotificationActivityType == null ? context.PackageManager.GetLaunchIntentForPackage(context.PackageName) : new Intent(global::Android.App.Application.Context, FirebasePushNotificationManager.DefaultNotificationActivityType));

            Bundle extras = new Bundle();
            foreach (var p in parameters)
                extras.PutString(p.Key, p.Value.ToString());

            if (extras != null)
            {
                extras.PutInt(ActionNotificationIdKey, notifyId);
                extras.PutString(ActionNotificationTagKey, tag);
                resultIntent.PutExtras(extras);
            }

            if (FirebasePushNotificationManager.NotificationActivityFlags != null)
            {
                resultIntent.SetFlags(FirebasePushNotificationManager.NotificationActivityFlags.Value);
            }
            int requestCode = new Java.Util.Random().NextInt();

            var notificationStr = AppPreferences.GetNotifications();
            List<NotificationModel> notificationList = JsonConvert.DeserializeObject<List<NotificationModel>>(notificationStr);

            notificationList.Add(new NotificationModel
            {
                NotifiyId = notifyId,
                Title = title,
                Message = message
            });
            AppPreferences.SaveNotification(notificationList);

            var pendingIntent = PendingIntent.GetActivity(context, requestCode, resultIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            var deleteIntent = new Intent(context, typeof(PushNotificationDeletedReceiver));
            deleteIntent.PutExtras(extras);

            var pendingDeleteIntent = PendingIntent.GetBroadcast(context, requestCode, deleteIntent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);

            var chanId = FirebasePushNotificationManager.DefaultNotificationChannelId;
            if (parameters.TryGetValue(ChannelIdKey, out object channelId) && channelId != null)
            {
                chanId = $"{channelId}";
            }

            // Image Notification
            if (!string.IsNullOrEmpty(image))
            {
                URL url = new URL(image);
                Bitmap bitmap = BitmapFactory.DecodeStream(url.OpenConnection().InputStream);

                NotificationCompat.BigPictureStyle notifyImageStyle = new NotificationCompat.BigPictureStyle();
                notifyImageStyle.BigPicture(bitmap);
                notifyImageStyle.BigLargeIcon(bitmap);

                var imgNotification = new NotificationCompat.Builder(context, chanId)
                 .SetSmallIcon(FirebasePushNotificationManager.IconResource)
                 .SetContentTitle(title)
                 .SetContentText(message)
                 .SetStyle(notifyImageStyle)
                 .SetAutoCancel(true)
                 .SetDefaults((int)NotificationDefaults.Lights)
                 .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                 .SetContentIntent(pendingIntent)
                 .SetDeleteIntent(pendingDeleteIntent);

                SetNotificationPriority(imgNotification, parameters);
                ResolveLocalizedParameters(imgNotification, parameters);
                SetNotificationAction(imgNotification, parameters, extras);
                OnBuildNotification(imgNotification, parameters);

                if (CrossFirebaseEssentials.Notifications.CanShowNotification)
                {
                    notificationManager.Notify(104, imgNotification.Build());
                }
                return;
            }

            // Group Notification
            string groupKey = $"{context.ApplicationInfo.LoadLabel(context.PackageManager)}Notification";

            var notification = new NotificationCompat.Builder(context, chanId)
                 .SetSmallIcon(FirebasePushNotificationManager.IconResource)
                 .SetContentTitle(title)
                 .SetContentText(message)
                 .SetAutoCancel(true)
                 .SetGroup(groupKey)
                 .SetDefaults((int)NotificationDefaults.Lights)
                 .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                 .SetContentIntent(pendingIntent)
                 .SetDeleteIntent(pendingDeleteIntent)
                 .SetStyle(new NotificationCompat.BigTextStyle()
                    .BigText(message));

            SetNotificationPriority(notification, parameters);
            ResolveLocalizedParameters(notification, parameters);
            SetNotificationAction(notification, parameters, extras);
            OnBuildNotification(notification, parameters);

            // summary inbox style
            var summaryInboxStyle = new NotificationCompat.InboxStyle()
                .SetBigContentTitle(context.ApplicationInfo.LoadLabel(context.PackageManager));

            foreach (var item in notificationList)
            {
                summaryInboxStyle.AddLine($"{item.Title} {item.Message}");
            }

            // summary notification
            var summaryNotification = new NotificationCompat.Builder(context, chanId)
                 .SetSmallIcon(FirebasePushNotificationManager.IconResource)
                 .SetAutoCancel(true)
                 .SetStyle(summaryInboxStyle)
                 .SetGroup(groupKey)
                 .SetGroupAlertBehavior(NotificationCompat.GroupAlertChildren)
                 .SetGroupSummary(true)
                 .SetDefaults((int)NotificationDefaults.Lights)
                 .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                 .SetContentIntent(pendingIntent)
                 .SetDeleteIntent(pendingDeleteIntent);

            SetNotificationPriority(summaryNotification, parameters);
            ResolveLocalizedParameters(summaryNotification, parameters);
            SetNotificationAction(summaryNotification, parameters, extras);
            OnBuildNotification(summaryNotification, parameters);

            if (CrossFirebaseEssentials.Notifications.CanShowNotification)
            {
                notificationManager.Notify(notifyId, notification.Build());
                notificationManager.Notify(AppPreferences.GetSummaryNotifyId(), summaryNotification.Build());
            }
        }

        private void SetNotificationAction(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters, Bundle extras)
        {
            string category = string.Empty;
            if (parameters.TryGetValue(CategoryKey, out object categoryContent))
                category = categoryContent.ToString();

            if (parameters.TryGetValue(ActionKey, out object actionContent))
                category = actionContent.ToString();

            if (FirebasePushNotificationManager.Color != null)
            {
                notificationBuilder.SetColor(FirebasePushNotificationManager.Color.Value);
            }

            var notificationCategories = CrossFirebaseEssentials.Notifications?.GetUserNotificationCategories();
            if (notificationCategories != null && notificationCategories.Length > 0)
            {
                foreach (var userCat in notificationCategories)
                {

                    if (userCat != null && userCat.Actions != null && userCat.Actions.Count > 0)
                    {
                        foreach (var action in userCat.Actions)
                        {
                            var aRequestCode = Guid.NewGuid().GetHashCode();

                            if (userCat.Category.Equals(category, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Intent actionIntent;
                                PendingIntent pendingActionIntent;

                                if (action.Type == NotificationActionType.Foreground)
                                {
                                    actionIntent = typeof(Activity).IsAssignableFrom(FirebasePushNotificationManager.NotificationActivityType) ? new Intent(global::Android.App.Application.Context, FirebasePushNotificationManager.NotificationActivityType) : (FirebasePushNotificationManager.DefaultNotificationActivityType == null ? context.PackageManager.GetLaunchIntentForPackage(context.PackageName) : new Intent(global::Android.App.Application.Context, FirebasePushNotificationManager.DefaultNotificationActivityType));

                                    if (FirebasePushNotificationManager.NotificationActivityFlags != null)
                                    {
                                        actionIntent.SetFlags(FirebasePushNotificationManager.NotificationActivityFlags.Value);
                                    }

                                    extras.PutString(ActionIdentifierKey, action.Id);
                                    actionIntent.PutExtras(extras);
                                    pendingActionIntent = PendingIntent.GetActivity(context, aRequestCode, actionIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                                }
                                else
                                {
                                    actionIntent = new Intent(context, typeof(PushNotificationActionReceiver));
                                    extras.PutString(ActionIdentifierKey, action.Id);
                                    actionIntent.PutExtras(extras);
                                    pendingActionIntent = PendingIntent.GetBroadcast(context, aRequestCode, actionIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                                }

                                notificationBuilder.AddAction(new NotificationCompat.Action.Builder(context.Resources.GetIdentifier(action.Icon, "drawable", global::Android.App.Application.Context.PackageName), action.Title, pendingActionIntent).Build());
                            }
                        }
                    }
                }
            }
        }

        private void SetNotificationPriority(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                if (parameters.TryGetValue(PriorityKey, out object priority) && priority != null)
                {
                    var priorityValue = $"{priority}";

                    if (!string.IsNullOrEmpty(priorityValue))
                    {
                        switch (priorityValue.ToLower())
                        {
                            case "max":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityMax);
                                break;
                            case "high":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityHigh);
                                break;
                            case "default":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityDefault);
                                break;
                            case "low":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityLow);
                                break;
                            case "min":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityMin);
                                break;
                            default:
                                notificationBuilder.SetPriority(NotificationCompat.PriorityDefault);
                                break;
                        }
                    }
                }
            }
        }

        private void ResolveLocalizedParameters(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            string getLocalizedString(string name, params string[] arguments)
            {
                var resources = context.Resources;
                var identifier = resources.GetIdentifier(name, "string", context.PackageName);
                var sanitizedArgs = arguments?.Where(it => it != null).Select(it => new Java.Lang.String(it)).Cast<Java.Lang.Object>().ToArray();

                try { return resources.GetString(identifier, sanitizedArgs ?? new Java.Lang.Object[] { }); }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{DomainTag}.ResolveLocalizedParameters - Incorrect string arguments {ex}");
                    return null;
                }
            }

            // Resolve title localization
            if (parameters.TryGetValue("title_loc_key", out object titleKey))
            {
                parameters.TryGetValue("title_loc_args", out object titleArgs);

                var localizedTitle = getLocalizedString(titleKey.ToString(), titleArgs as string[]);
                if (localizedTitle != null)
                    notificationBuilder.SetContentTitle(localizedTitle);
            }

            // Resolve body localization
            if (parameters.TryGetValue("body_loc_key", out object bodyKey))
            {
                parameters.TryGetValue("body_loc_args", out object bodyArgs);

                var localizedBody = getLocalizedString(bodyKey.ToString(), bodyArgs as string[]);
                if (localizedBody != null)
                    notificationBuilder.SetContentText(localizedBody);
            }
        }

        public void OnError(string error)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnError - {error}");
        }

        public virtual void OnBuildNotification(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters) { }
    }
}
