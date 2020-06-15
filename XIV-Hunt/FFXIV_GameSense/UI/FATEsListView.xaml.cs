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
	// Token: 0x020000B0 RID: 176
	public partial class FATEsListView : UserControl, IStyleConnector
	{
		// Token: 0x06000480 RID: 1152 RVA: 0x00014B20 File Offset: 0x00012D20
		public FATEsListView()
		{
			this.InitializeComponent();
			this.UpdateFATEsSelectedCount();
			this.AddPresets();
			this.PresetCheckComboBox.ItemSelectionChanged += this.PresetCheckComboBox_ItemSelectionChanged;
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00014B54 File Offset: 0x00012D54
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

		// Token: 0x06000482 RID: 1154 RVA: 0x00014C34 File Offset: 0x00012E34
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

		// Token: 0x06000483 RID: 1155 RVA: 0x00014CCC File Offset: 0x00012ECC
		private bool Filter(object obj)
		{
			if (string.IsNullOrWhiteSpace(this.FilterTextBox.Text))
			{
				return true;
			}
			FATEListViewItem item = (FATEListViewItem)obj;
			return item.Name.IndexOf(this.FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.Zones.IndexOf(this.FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.ID.ToString(CultureInfo.CurrentCulture) == this.FilterTextBox.Text;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00014D50 File Offset: 0x00012F50
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

		// Token: 0x06000485 RID: 1157 RVA: 0x00014E30 File Offset: 0x00013030
		private void Sort(string sortBy, ListSortDirection direction)
		{
			ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.ListView.ItemsSource);
			defaultView.SortDescriptions.Clear();
			SortDescription sd = new SortDescription(sortBy, direction);
			defaultView.SortDescriptions.Add(sd);
			defaultView.Refresh();
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00014E74 File Offset: 0x00013074
		private void FilterTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.FilterCoverTextBlock.Visibility = Visibility.Hidden;
			if (!this.filterApplied)
			{
				((CollectionView)CollectionViewSource.GetDefaultView(this.ListView.ItemsSource)).Filter = new Predicate<object>(this.Filter);
				this.filterApplied = true;
			}
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00014EC2 File Offset: 0x000130C2
		private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
			{
				this.FilterCoverTextBlock.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00014EE2 File Offset: 0x000130E2
		private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(this.ListView.ItemsSource).Refresh();
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00014EFC File Offset: 0x000130FC
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

		// Token: 0x0600048A RID: 1162 RVA: 0x00014F50 File Offset: 0x00013150
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

		// Token: 0x0600048B RID: 1163 RVA: 0x000151D8 File Offset: 0x000133D8
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

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600048C RID: 1164 RVA: 0x00015288 File Offset: 0x00013488
		// (remove) Token: 0x0600048D RID: 1165 RVA: 0x000152C0 File Offset: 0x000134C0
		public event EventHandler AllFATEsDeselected;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600048E RID: 1166 RVA: 0x000152F8 File Offset: 0x000134F8
		// (remove) Token: 0x0600048F RID: 1167 RVA: 0x00015330 File Offset: 0x00013530
		public event EventHandler FATESelected;

		// Token: 0x06000490 RID: 1168 RVA: 0x00015368 File Offset: 0x00013568
		private void ResizeFilterBox()
		{
			this.FilterTextBox.BeginAnimation(FrameworkElement.WidthProperty, null);
			this.FilterTextBox.Width = ((Grid)this.FilterTextBox.Parent).ColumnDefinitions[0].ActualWidth - this.SelectedFateCountTextBlock.ActualWidth - (double)((this.SelectedFateCountTextBlock.ActualWidth > 0.0) ? 12 : 5);
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x000153DC File Offset: 0x000135DC
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

		// Token: 0x06000492 RID: 1170 RVA: 0x00015489 File Offset: 0x00013689
		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ResizeFilterBox();
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x000155CE File Offset: 0x000137CE
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

		// Token: 0x04000375 RID: 885
		private GridViewColumnHeader _lastHeaderClicked;

		// Token: 0x04000376 RID: 886
		private ListSortDirection _lastDirection;

		// Token: 0x04000377 RID: 887
		private bool filterApplied;
	}
}
