using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Clones;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.DataProviders.SqlServer;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Data.DataProviders.Sql
{
  public class CustomSqlServerNotificationProvider : SqlServerNotificationProvider
  {
    protected CustomSqlServerNotificationProvider(SqlDataApi api, string databaseName)
 : base(api, databaseName)
    {
    }

    public CustomSqlServerNotificationProvider(string connectionStringName, string databaseName)
      : base(connectionStringName, databaseName)
    {
    }

    public override void AddNotification(Notification notification)
    {
      if (this.ShouldAutomaticallyAcceptChanges(notification))
      {
        Item item = ItemManager.GetItem(notification.Uri.ItemID, notification.Uri.Language, notification.Uri.Version, Database.GetDatabase(notification.Uri.DatabaseName));
        try
        {
          FieldChangedNotification f = (FieldChangedNotification)notification;
          item.Editing.BeginEdit();
          item.Fields[f.FieldID].Reset();
          item.Editing.EndEdit();
        }
        catch
        {
          notification.Accept(item);
        }

        // Sitecore.Web.UI.Sheer.ClientPage c= Context.ClientPage;
        //   ((VersionAddedNotification)notification).ForceAccept = true;
        //  notification.Accept(i);

        //  Context.ClientPage = c;
      }
      else
      {
        base.AddNotification(notification);
      }
    }

    private bool ShouldAutomaticallyAcceptChanges(Notification notification)
    {
      //Accepting notifications based on the setting value
      return Settings.GetBoolSetting("ItemCloning.ForceUpdate", false);

      //Acceptiing notifications of the specific type
      //return notification is VersionAddedNotification;
    }

  }
}