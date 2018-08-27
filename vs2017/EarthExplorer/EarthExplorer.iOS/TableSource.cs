using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EarthExplorer.Core;
using Foundation;
using UIKit;

namespace EarthExplorer.iOS
{
    public class TableSource : UITableViewSource
    {
        List<PointOfInterest> TableItems;
        string CellIdentifier = "TableCell";
        public event Action<PointOfInterest> OnClick;

        public TableSource(List<PointOfInterest> items)
        {
            TableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            OnClick?.Invoke(TableItems[indexPath.Row]);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            PointOfInterest item = TableItems[indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            }

            cell.TextLabel.Text = item.Name;
            return cell;
        }
    }
}