using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NpcCore.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for CalendarControl.xaml
    /// </summary>
    public partial class CalendarControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyRaised(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public CalendarControl()
        {
            InitializeComponent();
            InitData();
            DateTime currentDate = DateTime.Now;
            string currentDateStr = currentDate.ToString("yyyy-MM-dd"); // yyyy-MM-dd | dd/MM/yyyy | dd-MM-yyyy
            tbShowDate.Text = currentDateStr;
            tbShowDate2.Text = currentDateStr;

            InitTime();
        }

        public void InitData()
        {
            for (int i = 1; i < 13; i++)
            {
                CurrentMonth.Items.Add(i);
                PreMonth.Items.Add(i);
            }
            for (int j = 1990; j < 2050; j++)
            {
                CurrentYear.Items.Add(j);
                PreYear.Items.Add(j);
            }
            DateTime localDate = DateTime.Now;
            CurrentMonth.SelectedItem = localDate.Month;
            CurrentYear.SelectedItem = localDate.Year;
            if (localDate.Month == 1)
            {
                PreYear.SelectedItem = localDate.Year - 1;
                PreMonth.SelectedItem = 12;
            }
            else
            {
                PreYear.SelectedItem = localDate.Year;
                PreMonth.SelectedItem = localDate.Month - 1;
            }
            var lastDayOfMonth1 = DateTime.DaysInMonth((int)PreYear.SelectedItem, (int)PreMonth.SelectedItem);
            DateTime displayDate1 = DateTime.Parse((int)PreMonth.SelectedItem + "/" + "1/" + (int)PreYear.SelectedItem);
            DateTime displayDateEnd1 = DateTime.Parse((int)PreMonth.SelectedItem + "/" + lastDayOfMonth1 + "/" + (int)PreYear.SelectedItem);
            Calendar1.DisplayDate = displayDate1;
            Calendar1.DisplayDateEnd = displayDateEnd1;
            Calendar1.SelectedDate = DateTime.Now;

            var lastDayOfMonth2 = DateTime.DaysInMonth((int)CurrentYear.SelectedItem, (int)CurrentMonth.SelectedItem);
            DateTime displayDate2 = DateTime.Parse((int)CurrentMonth.SelectedItem + "/" + "1/" + (int)CurrentYear.SelectedItem);
            DateTime displayDateEnd2 = DateTime.Parse((int)CurrentMonth.SelectedItem + "/" + lastDayOfMonth2 + "/" + (int)CurrentYear.SelectedItem);
            Calendar2.DisplayDate = displayDate2;
            Calendar2.DisplayDateEnd = displayDateEnd2;
            Calendar2.SelectedDate = DateTime.Now;
            
        }

        public void InitTime()
        {
            TimeStartBorder.Visibility = TimeRange.IsChecked ?? true ? Visibility.Visible : Visibility.Collapsed;
            TimeEndBorder.Visibility = TimeRange.IsChecked ?? true ? Visibility.Visible : Visibility.Collapsed;
            for (int i = 0; i < 24; i++)
            {
                StartHour.Items.Add(i);
            }
            StartHour.SelectedItem = 0;
            for (int i = 0; i < 60; i++)
            {
                StartMinute.Items.Add(i);
            }
            StartMinute.SelectedItem = 0;

            for (int i = 0; i < 24; i++)
            {
                EndHour.Items.Add(i);
            }
            EndHour.SelectedItem = 23;
            for (int i = 0; i < 60; i++)
            {
                EndMinute.Items.Add(i);
            }
            EndMinute.SelectedItem = 59;

        }

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            popupCalendar.IsOpen = true;
            Button btn = sender as Button;
            if (btn.Name.Equals("ButtonArrowDown"))
            {
                isStartDate = true;
                BorderStartDate.BorderBrush = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                isStartDate = false;
                BorderEndDate.BorderBrush = new SolidColorBrush(Colors.Aqua);
            }
            if(startDate != null && endDate != null)
            {
                if (((DateTime)startDate).CompareTo((DateTime)endDate) < 1)
                {
                    isAddRange = true;
                    Calendar1.SelectedDates.Clear();
                    Calendar2.SelectedDates.Clear();
                    Calendar1.SelectedDates.AddRange((DateTime)startDate, (DateTime)endDate);
                    Calendar2.SelectedDates.AddRange((DateTime)startDate, (DateTime)endDate);
                    isAddRange = false;
                }
                if (isStartDate)
                    TemSelectedDate = startDate;
                else
                    TemSelectedDate = endDate;
            }
        }

        private bool isAddRange = false;
        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isAddRange)
            {
                popupCalendar.IsOpen = false;
                Calendar tem = sender as Calendar;
                GetSelectedDate(tem.Name);
            }
        }

        private void TimeVisibleButtonClick(object sender, RoutedEventArgs e)
        {
            var timeCheckbox = sender as CheckBox;
            if (timeCheckbox != null)
            {
                TimeStartBorder.Visibility = timeCheckbox.IsChecked ?? true ? Visibility.Visible : Visibility.Collapsed;
                TimeEndBorder.Visibility = timeCheckbox.IsChecked ?? true ? Visibility.Visible : Visibility.Collapsed;
            }
            StartHour.SelectedItem = 0;
            StartMinute.SelectedItem = 0;
            EndHour.SelectedItem = 23;
            EndMinute.SelectedItem = 59;
            {
                DateTime myDate = DateTime.Parse(tbShowDate.Text);
                TimeSpan ts = new TimeSpan((int)StartHour.SelectedItem, (int)StartMinute.SelectedItem, 0);
                myDate = myDate.Date + ts;
                Calendar1.SelectedDate = myDate;
                CalendarStartDate.SelectedDate = myDate;
            }
            {
                DateTime myDate = DateTime.Parse(tbShowDate2.Text);
                TimeSpan ts = new TimeSpan((int)EndHour.SelectedItem, (int)EndMinute.SelectedItem, 0);
                myDate = myDate.Date + ts;
                Calendar2.SelectedDate = myDate;
                CalendarEndDate.SelectedDate = myDate;
            }


        }

        private void PreMonth_Click(object sender, RoutedEventArgs e)
        {
            int newValueMonth = 1;
            int newValueYear1 = (int)PreYear.SelectedItem;
            if ((int)PreMonth.SelectedItem > 1)
            {
                newValueMonth = (int)PreMonth.SelectedItem - 1;
            }
            else
            {
                newValueMonth = 12;
                newValueYear1 = (int)PreYear.SelectedItem - 1;
            }
            PreMonth.SelectedItem = newValueMonth;
            PreYear.SelectedItem = newValueYear1;
            var lastDayOfMonth1 = DateTime.DaysInMonth(newValueYear1, newValueMonth);
            DateTime displayDate1 = DateTime.Parse(newValueMonth + "/" + "1/" + newValueYear1);
            DateTime displayDateEnd1 = DateTime.Parse(newValueMonth + "/" + lastDayOfMonth1 + "/" + newValueYear1);
            Calendar1.DisplayDate = displayDate1;
            Calendar1.DisplayDateEnd = displayDateEnd1;
           
            int newValueMonth2 = 1;
            int newValueYear2 = (int)CurrentYear.SelectedItem;
            if ((int)CurrentMonth.SelectedItem > 1)
            {
                newValueMonth2 = (int)CurrentMonth.SelectedItem - 1;
            }
            else
            {
                newValueMonth2 = 12;
                newValueYear2 = (int)CurrentYear.SelectedItem - 1;
            }
            CurrentYear.SelectedItem = newValueYear2;
            CurrentMonth.SelectedItem = newValueMonth2;
            var lastDayOfMonth2 = DateTime.DaysInMonth(newValueYear2, newValueMonth2);
            DateTime displayDate2 = DateTime.Parse(newValueMonth2 + "/" + "1/" + newValueYear2);
            DateTime displayDateEnd2 = DateTime.Parse(newValueMonth2 + "/" + lastDayOfMonth2 + "/" + newValueYear2);
            Calendar2.DisplayDate = displayDate2;
            Calendar2.DisplayDateEnd = displayDateEnd2;
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            int newValueMonth = 1;
            int newValueYear1 = (int)PreYear.SelectedItem;
            if ((int)PreMonth.SelectedItem < 12)
            {
                newValueMonth = (int)PreMonth.SelectedItem + 1;
            }
            else
            {
                newValueMonth = 1;
                newValueYear1 = (int)PreYear.SelectedItem + 1;
            }
            PreMonth.SelectedItem = newValueMonth;
            PreYear.SelectedItem = newValueYear1;
            var lastDayOfMonth1 = DateTime.DaysInMonth(newValueYear1, newValueMonth);
            DateTime displayDate1 = DateTime.Parse(newValueMonth + "/" + "1/" + newValueYear1);
            DateTime displayDateEnd1 = DateTime.Parse(newValueMonth + "/" + lastDayOfMonth1 + "/" + newValueYear1);
            Calendar1.DisplayDate = displayDate1;
            Calendar1.DisplayDateEnd = displayDateEnd1;

            int newValueMonth2 = 1;
            int newValueYear2 = (int)CurrentYear.SelectedItem;
            if ((int)CurrentMonth.SelectedItem < 12)
            {
                newValueMonth2 = (int)CurrentMonth.SelectedItem + 1;
            }
            else
            {
                newValueMonth2 = 1;
                newValueYear2 = (int)CurrentYear.SelectedItem + 1;
            }
            CurrentYear.SelectedItem = newValueYear2;
            CurrentMonth.SelectedItem = newValueMonth2;
            var lastDayOfMonth2 = DateTime.DaysInMonth(newValueYear2, newValueMonth2);
            DateTime displayDate2 = DateTime.Parse(newValueMonth2 + "/" + "1/" + newValueYear2);
            DateTime displayDateEnd2 = DateTime.Parse(newValueMonth2 + "/" + lastDayOfMonth2 + "/" + newValueYear2);
            Calendar2.DisplayDate = displayDate2;
            Calendar2.DisplayDateEnd = displayDateEnd2;
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            if (isStartDate)
            {
                DateTime myDate = DateTime.Parse(tbShowDate.Text);
                TimeSpan ts = new TimeSpan((int)StartHour.SelectedItem,(int)StartMinute.SelectedItem, 0);
                myDate = myDate.Date + ts;
                Calendar1.SelectedDate = myDate;
                CalendarStartDate.SelectedDate = myDate;
            }
            else
            {
                DateTime myDate = DateTime.Parse(tbShowDate2.Text);
                TimeSpan ts = new TimeSpan((int)EndHour.SelectedItem, (int)EndMinute.SelectedItem, 0);
                myDate = myDate.Date + ts;
                Calendar2.SelectedDate = myDate;
                CalendarEndDate.SelectedDate = myDate;
            }

            BorderStartDate.BorderBrush = new SolidColorBrush(Colors.Transparent);
            BorderEndDate.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        public void SetCalendarDate(object sender, RoutedEventArgs e)
        {
            if(PreMonth.SelectedItem != null && PreYear.SelectedItem != null)
            {
                int month1 = (int)PreMonth.SelectedItem;
                int year1 = (int)PreYear.SelectedItem;

                var lastDayOfMonth1 = DateTime.DaysInMonth(year1, month1);
                DateTime displayDate1 = DateTime.Parse(month1 + "/" + "1/" + year1);
                DateTime displayDateEnd1 = DateTime.Parse(month1 + "/" + lastDayOfMonth1 + "/" + year1);
                Calendar1.DisplayDate = displayDate1;
                Calendar1.DisplayDateEnd = displayDateEnd1;
            }

            if (CurrentMonth.SelectedItem != null && CurrentYear.SelectedItem != null)
            {
                int month2 = (int)CurrentMonth.SelectedItem;
                int year2 = (int)CurrentYear.SelectedItem;
                var lastDayOfMonth2 = DateTime.DaysInMonth(year2, month2);
                DateTime displayDate2 = DateTime.Parse(month2 + "/" + "1/" + year2);
                DateTime displayDateEnd2 = DateTime.Parse(month2 + "/" + lastDayOfMonth2 + "/" + year2);
                Calendar2.DisplayDate = displayDate2;
                Calendar2.DisplayDateEnd = displayDateEnd2;
            }
        }

        private DateTime? startDate = DateTime.Now;
        private DateTime? endDate = DateTime.Now;
        private bool isStartDate = false;
        private void GetSelectedDate(string nameCalendar)
        {
            if (nameCalendar.Equals("Calendar1") && Calendar1.SelectedDate != null)
            {
                DateTime? selectDate = Calendar1.SelectedDate.Value;
                string rs = selectDate?.ToString("yyyy-MM-dd");
                Calendar2.SelectedDate = null;
                if (isStartDate)
                {
                    startDate = selectDate;
                    tbShowDate.Text = rs;
                }
                else
                {
                    endDate = selectDate;
                    tbShowDate2.Text = rs;
                }
                SelectedDate = selectDate;
            }
            else if(nameCalendar.Equals("Calendar2") && Calendar2.SelectedDate != null)
            {
                DateTime? selectDate2 = Calendar2.SelectedDate.Value;
                string rs = selectDate2?.ToString("yyyy-MM-dd");
                Calendar1.SelectedDate = null;
                if (isStartDate)
                {
                    startDate = selectDate2;
                    tbShowDate.Text = rs;
                }
                else
                {
                    endDate = selectDate2;
                    tbShowDate2.Text = rs;
                }
                SelectedDate = selectDate2;
            }
        }

        public DateTime? TemSelectedDate;
        public DateTime? SelectedDate
        {
            get 
            { 
                return (DateTime?)GetValue(SelectedDateProperty); 
            }
            set 
            {
                SetValue(SelectedDateProperty, value);
                OnPropertyRaised(nameof(SelectedDate));
            }
        }

        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(CalendarControl), new FrameworkPropertyMetadata(null));

    
    }
}
