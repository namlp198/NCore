using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace NpcCore.Wpf.Ex
{
    /// <summary>
    /// 토글형 드롭다운 버튼 컨트롤
    /// </summary>
    public class ToogleDropDownButton : ToggleButton
    {
        /// <summary>
        /// 목록
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ToogleDropDownButton));


        /// <summary>
        /// 아이템 템플릿
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ToogleDropDownButton));


        /// <summary>
        /// 셀렉션
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ToogleDropDownButton), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            ToogleDropDownButton view = dobj as ToogleDropDownButton;
            if (view != null && e.NewValue is object)
            {
                view.SelectedItemChanged();
            }
        }

        private void SelectedItemChanged()
        {
            this.SetValue(ToggleButton.IsCheckedProperty, SelectedItem != null);

            OnItemChecked(this.IsChecked, this.SelectedItem);
        }

        public ToogleDropDownButton()
        {
        }

        protected override void OnClick()
        {
            base.OnClick();
            OnItemChecked(this.IsChecked, this.SelectedItem);
        }

        public event Action<bool?, object> ItemSelected;
        public void OnItemChecked(bool? isChecked, object e)
        {
            if (ItemSelected != null)
            {
                ItemSelected(isChecked, e);
            }
        }

    }
}
