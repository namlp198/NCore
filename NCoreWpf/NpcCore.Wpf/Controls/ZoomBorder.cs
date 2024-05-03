using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class ZoomBorder : Border
    {
        private UIElement child = null;
        private Point origin;
        private Point start;
        private ScaleTransform m_st = new ScaleTransform();
        private TranslateTransform m_tt = new TranslateTransform();
        private TransformGroup m_tg = new TransformGroup();

        //public static ZoomBorder _instance;
        //public static ZoomBorder Instance()
        //{
        //    if (_instance == null)
        //        _instance = new ZoomBorder();
        //    return _instance;
        //}
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public ScaleTransform ST
        {
            get => m_st;
            set => m_st = value;
        }
        public TranslateTransform TT
        {
            get => m_tt;
            set => m_tt = value;
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                m_tg.Children.Add(m_st);
                m_tg.Children.Add(m_tt);
                child.RenderTransform = m_tg;
                child.RenderTransformOrigin = new Point(0.0, 0.0);

                //event border
                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                  child_PreviewMouseRightButtonDown);

            }
        }
        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                m_st = GetScaleTransform(child);
                m_st.ScaleX = 1.0;
                m_st.ScaleY = 1.0;

                // reset pan
                m_tt = GetTranslateTransform(child);
                m_tt.X = 0.0;
                m_tt.Y = 0.0;
            }
        }

        #region Child Events


        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                m_st = GetScaleTransform(child);
                m_tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (m_st.ScaleX < .4 || m_st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * m_st.ScaleX + m_tt.X;
                absoluteY = relative.Y * m_st.ScaleY + m_tt.Y;

                m_st.ScaleX += zoom;
                m_st.ScaleY += zoom;

                m_tt.X = absoluteX - relative.X * m_st.ScaleX;
                m_tt.Y = absoluteY - relative.Y * m_st.ScaleY;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (child != null)
            {
                m_tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new Point(m_tt.X, m_tt.Y);
                this.Cursor = Cursors.ScrollAll;
                child.CaptureMouse();
            }


        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }

        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    m_tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    m_tt.X = origin.X - v.X;
                    m_tt.Y = origin.Y - v.Y;
                }
            }


        }
        #endregion
    }
}
