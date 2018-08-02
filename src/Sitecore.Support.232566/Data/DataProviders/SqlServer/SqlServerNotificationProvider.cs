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

namespace Sitecore.Support.Data.DataProviders.SqlServer
{
  public class SqlServerNotificationProvider : Sitecore.Data.DataProviders.SqlServer.SqlServerNotificationProvider
  {
    protected SqlServerNotificationProvider(SqlDataApi api, string databaseName)
 : base(api, databaseName)
    {
    }

    public SqlServerNotificationProvider(string connectionStringName, string databaseName)
      : base(connectionStringName, databaseName)
    {
    }

    public override void AddNotification(Notification notification)
    {
      if (this.ShouldAutomaticallyAcceptChanges(notification))
      {
        Item item = ItemManager.GetItem(notification.Uri.ItemID, notification.Uri.Language, notification.Uri.Version, Database.GetDatabase(notification.Uri.DatabaseName));
        if (notification is FieldChangedNotification)
        {
          FieldChangedNotification f = (FieldChangedNotification)notification;
          item.Editing.BeginEdit();
          item.Fields[f.FieldID].Reset();
          item.Editing.EndEdit();
        }
        else
        {
          notification.Accept(item);
        }
      }
      else
      {
        base.AddNotification(notification);
      }
    }

    private bool ShouldAutomaticallyAcceptChanges(Notification notification)
    {
      return Settings.GetBoolSetting("ItemCloning.ForceUpdate", false);
    }

  }
}