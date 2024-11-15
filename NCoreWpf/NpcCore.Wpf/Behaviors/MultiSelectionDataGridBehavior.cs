﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NpcCore.Wpf.Controls.Behaviors
{
    /// <summary>
    /// Behavior to support multi-select in a traditional WPF TreeView control.
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///   <DataGrid ...>
    ///      <i:Interaction.Behaviors>
    ///         <b:MultiSelectionDataGridBehavior SelectedItems="{Binding SelectedItems}" />
    ///      </i:Interaction.Behaviors>
    ///   </DataGrid>
    /// ]]>
    /// </example>
    public class MultiSelectionDataGridBehavior : Behavior<DataGrid>
    {
        #region Static

        /// <summary>
        /// SelectedItems dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList),
                typeof(MultiSelectionDataGridBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged));

        /// <summary>
        /// SelectedValues dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.Register(nameof(SelectedValues), typeof(IList),
                typeof(MultiSelectionDataGridBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedValuesChanged));

        /// <summary>
        /// Called when selected items property is changed from binding source
        /// </summary>
        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (MultiSelectionDataGridBehavior)sender;
            if (behavior._modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior._modelHandled = true;
            behavior.SelectedItemsToValues();
            behavior.SelectItems();
            behavior._modelHandled = false;
        }

        /// <summary>
        /// Called when selected items property is changed from binding source
        /// </summary>
        private static void OnSelectedValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (MultiSelectionDataGridBehavior)sender;
            if (behavior._modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior._modelHandled = true;
            behavior.SelectedValuesToItems();
            behavior.SelectItems();
            behavior._modelHandled = false;
        }

        private static object GetDeepPropertyValue(object obj, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return obj;
            while (true)
            {
                if (path.Contains('.'))
                {
                    var split = path.Split('.');
                    var remainingProperty = path.Substring(path.IndexOf('.') + 1);
                    obj = obj?.GetType().GetProperty(split[0])?.GetValue(obj, null);
                    path = remainingProperty;
                    continue;
                }
                return obj?.GetType().GetProperty(path)?.GetValue(obj, null);
            }
        }

        #endregion

        private bool _viewHandled;
        private bool _modelHandled;

        /// <summary>
        /// Bindable selected items array
        /// </summary>
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Bindable selected values array
        /// </summary>
        public IList SelectedValues
        {
            get => (IList)GetValue(SelectedValuesProperty);
            set => SetValue(SelectedValuesProperty, value);
        }

        /// <summary>
        /// Selects items in DataGrid, based on the SelectedItems property
        /// </summary>
        private void SelectItems()
        {
            _viewHandled = true;
            AssociatedObject.SelectedItems.Clear();
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                    AssociatedObject.SelectedItems.Add(item);
            }
            _viewHandled = false;
        }

        /// <summary>
        /// Updates SelectedItems based on SelectedValues
        /// </summary>
        private void SelectedValuesToItems()
        {
            if (SelectedValues == null)
                SelectedItems = null;
            else
                SelectedItems =
                    AssociatedObject.Items.Cast<object>()
                        .Where(i => SelectedValues.Contains(GetDeepPropertyValue(i, AssociatedObject.SelectedValuePath)))
                        .ToArray();
        }

        /// <summary>
        /// Updates SelectedValues based on SelectedItems
        /// </summary>
        private void SelectedItemsToValues()
        {
            if (SelectedItems == null)
                SelectedValues = null;
            else
                SelectedValues =
                    SelectedItems.Cast<object>()
                        .Select(i => GetDeepPropertyValue(i, AssociatedObject.SelectedValuePath))
                        .ToArray();
        }

        /// <summary>
        /// Called when DataGrid selection changes from view
        /// </summary>
        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (_viewHandled) return;
            if (AssociatedObject.Items.SourceCollection == null) return;
            SelectedItems = AssociatedObject.SelectedItems.Cast<object>().ToObservableCollection();
        }

        /// <summary>
        /// Called when DataGrid items change
        /// </summary>
        private void OnDataGridItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewHandled) return;
            if (AssociatedObject.Items.SourceCollection == null) return;
            SelectItems();
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnDataGridSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnDataGridItemsChanged;

            _modelHandled = true;
            SelectedValuesToItems();
            SelectItems();
            _modelHandled = false;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged -= OnDataGridSelectionChanged;
                ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnDataGridItemsChanged;
            }
        }
    }
}
