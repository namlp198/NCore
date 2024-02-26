using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ValidationToolkit
{
    public class ViewBase : UserControl
    {
        public virtual void OnLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            ErrorContainer = (IValidationErrorContainer)DataContext;
            AddHandler(System.Windows.Controls.Validation.ErrorEvent, new RoutedEventHandler(Handler), true);
        }

        public virtual void OnUnload(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveHandler(System.Windows.Controls.Validation.ErrorEvent, new RoutedEventHandler(Handler));
        }

        internal IValidationErrorContainer ErrorContainer = null;
        public void Handler(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ValidationErrorEventArgs args = e as System.Windows.Controls.ValidationErrorEventArgs;

            // [NCS-2695] CID 171208 Unchecked dynamic_cast
            //if (args.Error.RuleInError is System.Windows.Controls.ValidationRule)
            if (args != null && args.Error != null && args.Error.RuleInError is System.Windows.Controls.ValidationRule)
            {
                if (ErrorContainer != null)
                {
                    Tracer.LogValidation("ViewBase.Handler called for ValidationRule exception.");
                    BindingExpression bindingExpression = args.Error.BindingInError as System.Windows.Data.BindingExpression;
                    Debug.Assert(bindingExpression != null);

                    // [NCS-4005] : Coverity : CID 181998 (#1 of 1): Dereference null return (stat) (NULL_RETURNS)
                    //string propertyName = bindingExpression.ParentBinding.Path.Path;
                    string propertyName = bindingExpression.ParentBinding?.Path.Path ?? "";
                    DependencyObject OriginalSource = args.OriginalSource as DependencyObject;
                    string errorMessage = "";
                    ReadOnlyObservableCollection<System.Windows.Controls.ValidationError> errors = System.Windows.Controls.Validation.GetErrors(OriginalSource);
                    if (errors.Count > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(propertyName).Append(":");
                        System.Windows.Controls.ValidationError error = errors[errors.Count - 1];
                        {
                            if (error.Exception == null || error.Exception.InnerException == null)
                                builder.Append(error.ErrorContent.ToString());
                            else
                                builder.Append(error.Exception.InnerException.Message);
                        }
                        errorMessage = builder.ToString();
                    }
                    Debug.Assert(args.Action == ValidationErrorEventAction.Added || args.Action == ValidationErrorEventAction.Removed);
                    StringBuilder errorID = new StringBuilder();
                    errorID.Append(args.Error.RuleInError.ToString());
                    if (args.Action == ValidationErrorEventAction.Added)
                    {
                        ErrorContainer.AddError(new ValidationToolkit.ValidationError(propertyName, errorID.ToString(), errorMessage));
                    }
                    else if (args.Action == ValidationErrorEventAction.Removed)
                    {
                        ErrorContainer.RemoveError(propertyName, errorID.ToString());
                    }
                }
            }
        }
    }
}
