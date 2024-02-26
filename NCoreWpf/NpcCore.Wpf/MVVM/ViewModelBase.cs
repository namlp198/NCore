using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NpcCore.Toolkit;

namespace NpcCore.Wpf.MVVM
{
    public abstract class ViewModelBase<TView> : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Data members

        //private static bool? _isInDesignMode;

        protected ViewConnector<TView> m_viewConnector;

        protected ThreadHelpers m_threadHelpers;

        #endregion       

        /// <summary>
        /// Returns a reference to the view's IView interface
        /// </summary>
        public IView View
        {
            get
            {
                if (m_viewConnector != null)
                    return m_viewConnector.View;
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns a reference to the exact instance of the view
        /// </summary>
        protected TView ViewInstance
        {
            get
            {
                return m_viewConnector.GetViewInstance();
            }
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public ViewModelBase()
        {
            m_threadHelpers = new ThreadHelpers();

            CreateCommands();
        }

        public void CreateView(string language)
        {
            m_viewConnector = new ViewConnector<TView>();
            m_viewConnector.CreateView();

            View.SetDataContext(this);
            View.SetLanguage(language);
        }

        public TForeignView CreateForeignView<TForeignView>()
        {
            ViewConnector<TForeignView> viewConnector = new ViewConnector<TForeignView>();
            viewConnector.CreateView();


            viewConnector.View.SetDataContext(this);
            return viewConnector.GetViewInstance();
        }

        public virtual void Initialize(string language)
        {
            CreateView(language);
        }

        protected abstract void CreateCommands();

        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        //   onChanged:
        //     Action that is called after the property value has been changed.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }

        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support System.Runtime.CompilerServices.CallerMemberNameAttribute.
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #region [ Dispose ]
        protected bool disposed = false;

        ~ViewModelBase()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose Managed Resource
                }

                // Dispose UnManaged Resource

                this.disposed = true;
            }
        }
        #endregion // [ Dispose ]
    }
}
