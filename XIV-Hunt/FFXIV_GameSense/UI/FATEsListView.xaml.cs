using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using FFXIV_GameSense.Properties;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using XIVDB;

namespace FFXIV_GameSense.UI
{
	public partial class FATEsListView : UserControl, IStyleConnector
	{
		public FATEsListView()
		{
			this.InitializeComponent();
			this.UpdateFATEsSelectedCount();
			this.AddPresets();
			this.PresetCheckComboBox.ItemSelectionChanged += this.PresetCheckComboBox_ItemSelectionChanged;
		}

		private void AddPresets()
		{
			foreach (FATEPresetViewItem p in from x in GameResources.GetRelicNotes()
			select new FATEPresetViewItem(x))
			{
				this.PresetCheckComboBox.Items.Add(p);
				bool select = true;
				foreach (ushort fid in p.FATEIDs)
				{
					if (!Settings.Default.FATEs.Contains(fid))
					{
						select = false;
						break;
					}
				}
				if (select)
				{
					this.PresetCheckComboBox.SelectedItems.Add(p);
				}
			}
		}

		private void PresetCheckComboBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
		{
			IEnumerable<FATEListViewItem> source = this.ListView.ItemsSource.Cast<FATEListViewItem>();
			Func<FATEListViewItem, bool> <>9__0;
			Func<FATEListViewItem, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((FATEListViewItem x) => ((FATEPresetViewItem)e.Item).FATEIDs.Contains(x.ID)));
			}
			foreach (FATEListViewItem fatelistViewItem in source.Where(predicate))
			{
				fatelistViewItem.Announce = e.IsSelected;
			}
			this.CheckBox_Checked(null, null);
		}

		private bool Filter(object obj)
		{
			if (string.IsNullOrWhiteSpace(this.FilterTextBox.Text))
			{
				return true;
			}
			FATEListViewItem item = (FATEListViewItem)obj;
			return item.Name.IndexOf(this.FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.Zones.IndexOf(this.FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.ID.ToString(CultureInfo.CurrentCulture) == this.FilterTextBox.Text;
		}

		private void FATEsListView_GridViewColumnHeaderClick(object sender, RoutedEventArgs e)
		{
			GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
			if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
			{
				ListSortDirection direction;
				if (headerClicked != this._lastHeaderClicked)
				{
					direction = ListSortDirection.Ascending;
				}
				else if (this._lastDirection == ListSortDirection.Ascending)
				{
					direction = ListSortDirection.Descending;
				}
				else
				{
					direction = ListSortDirection.Ascending;
				}
				string header = (string)((GridViewColumnHeader)headerClicked.Column.Header).Tag;
				this.Sort(header, direction);
				if (direction == ListSortDirection.Ascending)
				{
					headerClicked.Column.HeaderTemplate = (base.Resources["HeaderTemplateArrowUp"] as DataTemplate);
				}
				else
				{
					headerClicked.Column.HeaderTemplate = (base.Resources["HeaderTemplateArrowDown"] as DataTemplate);
				}
				if (this._lastHeaderClicked != null && this._lastHeaderClicked != headerClicked)
				{
					this._lastHeaderClicked.Column.HeaderTemplate = null;
				}
				this._lastHeaderClicked = headerClicked;
				this._lastDirection = direction;
			}
		}

		private void Sort(string sortBy, ListSortDirection direction)
		{
			ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.ListView.ItemsSource);
			defaultView.SortDescriptions.Clear();
			SortDescription sd = new SortDescription(sortBy, direction);
			defaultView.SortDescriptions.Add(sd);
			defaultView.Refresh();
		}

		private void FilterTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.FilterCoverTextBlock.Visibility = Visibility.Hidden;
			if (!this.filterApplied)
			{
				((CollectionView)CollectionViewSource.GetDefaultView(this.ListView.ItemsSource)).Filter = new Predicate<object>(this.Filter);
				this.filterApplied = true;
			}
		}

		private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
			{
				this.FilterCoverTextBlock.Visibility = Visibility.Visible;
			}
		}

		private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(this.ListView.ItemsSource).Refresh();
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (e != null)
			{
				this.UpdatePresetCheckComboBox(e);
			}
			this.UpdateFATEsSelectedCount();
			if (Settings.Default.FATEs.Count == 0)
			{
				this.AllFATEsDeselected(null, EventArgs.Empty);
				return;
			}
			this.FATESelected(null, EventArgs.Empty);
		}

		private void UpdatePresetCheckComboBox(RoutedEventArgs e)
		{
			CheckBox cb = (CheckBox)e.Source;
			FATEListViewItem fatelistViewItem = (FATEListViewItem)cb.DataContext;
			List<FATEPresetViewItem> tr = new List<FATEPresetViewItem>();
			bool? isChecked = cb.IsChecked;
			bool flag = false;
			if (isChecked.GetValueOrDefault() == flag & isChecked != null)
			{
				foreach (object obj in this.PresetCheckComboBox.SelectedItems)
				{
					FATEPresetViewItem fpvi = (FATEPresetViewItem)obj;
					if (!tr.Contains(fpvi))
					{
						if (fpvi.FATEIDs.Any((ushort fateid) => !Settings.Default.FATEs.Contains(fateid)))
						{
							tr.Add(fpvi);
						}
					}
				}
				this.PresetCheckComboBox.ItemSelectionChanged -= this.PresetCheckComboBox_ItemSelectionChanged;
				foreach (FATEPresetViewItem r in tr)
				{
					this.PresetCheckComboBox.SelectedItems.Remove(r);
				}
				this.PresetCheckComboBox.ItemSelectionChanged += this.PresetCheckComboBox_ItemSelectionChanged;
				return;
			}
			isChecked = cb.IsChecked;
			flag = true;
			if (isChecked.GetValueOrDefault() == flag & isChecked != null)
			{
				foreach (object obj2 in ((IEnumerable)this.PresetCheckComboBox.Items))
				{
					FATEPresetViewItem fpvi2 = (FATEPresetViewItem)obj2;
					if (!this.PresetCheckComboBox.SelectedItems.Contains(fpvi2))
					{
						if (fpvi2.FATEIDs.All((ushort fateid) => Settings.Default.FATEs.Contains(fateid)))
						{
							tr.Add(fpvi2);
						}
					}
				}
				this.PresetCheckComboBox.ItemSelectionChanged -= this.PresetCheckComboBox_ItemSelectionChanged;
				foreach (FATEPresetViewItem r2 in tr)
				{
					this.PresetCheckComboBox.SelectedItems.Add(r2);
				}
				this.PresetCheckComboBox.ItemSelectionChanged += this.PresetCheckComboBox_ItemSelectionChanged;
			}
		}

		private void UpdateFATEsSelectedCount()
		{
			if (Settings.Default.FATEs.Count == 1)
			{
				this.SelectedFateCountTextBlock.Text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.FormFATESingle, Settings.Default.FATEs.Count);
			}
			else if (Settings.Default.FATEs.Count > 1)
			{
				this.SelectedFateCountTextBlock.Text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.FormFATEPlural, Settings.Default.FATEs.Count);
			}
			if (Settings.Default.FATEs.Count == 0)
			{
				this.SelectedFateCountTextBlock.Text = string.Empty;
			}
		}

		public event EventHandler AllFATEsDeselected;

		public event EventHandler FATESelected;

		private void ResizeFilterBox()
		{
			this.FilterTextBox.BeginAnimation(FrameworkElement.WidthProperty, null);
			this.FilterTextBox.Width = ((Grid)this.FilterTextBox.Parent).ColumnDefinitions[0].ActualWidth - this.SelectedFateCountTextBlock.ActualWidth - (double)((this.SelectedFateCountTextBlock.ActualWidth > 0.0) ? 12 : 5);
		}

		private void SelectedFateCountTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newWidth = ((Grid)this.FilterTextBox.Parent).ColumnDefinitions[0].ActualWidth - e.NewSize.Width - (double)((e.NewSize.Width > 0.0) ? 12 : 5);
			this.FilterTextBox.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation
			{
				From = new double?(this.FilterTextBox.ActualWidth),
				To = new double?(newWidth),
				Duration = TimeSpan.FromMilliseconds(250.0)
			});
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ResizeFilterBox();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.8.1.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 7)
			{
				((CheckBox)target).Checked += this.CheckBox_Checked;
				((CheckBox)target).Unchecked += this.CheckBox_Checked;
			}
		}

		private GridViewColumnHeader _lastHeaderClicked;

		private ListSortDirection _lastDirection;

		private bool filterApplied;
	}
}
