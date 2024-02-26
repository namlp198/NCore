using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NpcCore.Wpf.Panels
{
    public class ViewStackGrid : Grid
    {
        // todo : 최상단만 enable주기, 나머지 딤처리, 호스트 포함

        public ViewStackGrid()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        public UserControl Push(UserControl view)
        {
            this.Children.Add(view);

            Update();
            return view;
        }

        private void Update()
        {
            this.Visibility = this.Children.Count == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public UserControl Pop()
        {
            var lastView = this.Children.Cast<UserControl>().LastOrDefault();
            if (lastView != null)
            {
                this.Children.Remove(lastView);
            }

            Update();
            return lastView;
        }

        public void Clear()
        {
            this.Children.Clear();
            Update();
        }
    }
}
