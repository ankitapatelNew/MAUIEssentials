using System;
using CoreGraphics;
using Foundation;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.Renderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        bool _disposed;
        NSIndexPath[]? paths;
        UIRefreshControl? refreshControl;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                if (refreshControl != null)
                {
                    refreshControl.ValueChanged -= RefreshControl_ValueChanged;
                }

                refreshControl?.Dispose();
                paths = null;

                if (Element is CustomListView listView)
                {
                    listView.EventScrollToTop -= View_EventScrollToTop;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            try
            {
                base.OnElementChanged(e);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            if (e.OldElement != null || e.NewElement == null)
            {
                Dispose(true);
                return;
            }

            if (e.OldElement != null)
            {
                if (refreshControl != null)
                {
                    refreshControl.ValueChanged -= RefreshControl_ValueChanged;
                }

                refreshControl?.Dispose();
                (e.OldElement as CustomListView).EventScrollToTop -= View_EventScrollToTop;
                (e.OldElement as CustomListView).NativeFirstVisbileIndex = null;
            }

            if (e.NewElement != null && !_disposed)
            {
                var element = e.NewElement as CustomListView;

                Control.TableFooterView = new UIView(CGRect.Empty);
                Control.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;

                element.EventScrollToTop += View_EventScrollToTop;
                element.NativeFirstVisbileIndex = new Func<int>(GetFirstVisbileIndex);

            }
        }

        private int GetFirstVisbileIndex()
        {
            if (Control != null && Control.IndexPathsForVisibleRows != null && Control.IndexPathsForVisibleRows.Count() > 0)
            {
                var indexPath = Control.IndexPathsForVisibleRows[0];
                return indexPath.Row;
            }
            return -1;
        }

        private void Control_Scrolled(object sender, EventArgs e)
        {
            var indexPath = Control.IndexPathsForVisibleRows[0];
            var firstVisibleItemIndex = indexPath.Row;
            ((CustomListView)Element).NotifyFirstVisibleItemIndexChanged(firstVisibleItemIndex);
        }


        void View_EventScrollToTop(object sender, EventArgs e)
        {
            var tableView = this.Control as UITableView;
            var indexPath = NSIndexPath.FromItemSection(0, 0);
            Control.ScrollToRow(indexPath, UITableViewScrollPosition.Top, true);
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            if (refreshControl != null && refreshControl.Refreshing)
            {
                refreshControl.EndRefreshing();
            }
        }

        private void AnimationAction()
        {
            Control.ReloadRows(paths, UITableViewRowAnimation.None);
        }
    }
}
