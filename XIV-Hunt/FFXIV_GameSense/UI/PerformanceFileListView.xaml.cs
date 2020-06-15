using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using FFXIV_GameSense.MML;
using FFXIV_GameSense.Properties;
using Splat;
using TextPlayer;
using TextPlayer.MML;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000BA RID: 186
	public partial class PerformanceFileListView : UserControl, IDisposable, IStyleConnector
	{
		// Token: 0x060004CA RID: 1226 RVA: 0x00016850 File Offset: 0x00014A50
		public PerformanceFileListView()
		{
			this.InitializeComponent();
			this.IndexPerformances(null);
			Settings.Default.SettingChanging += this.SettingChanging;
			this.ListView.ItemsSource = this.collection;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x000168A4 File Offset: 0x00014AA4
		private void IndexPerformances(string nd = null)
		{
			this.collection.Clear();
			if (nd == null)
			{
				nd = Settings.Default.PerformDirectory;
			}
			if (Directory.Exists(nd))
			{
				this.DisposePerformDirWatcher();
				new Thread(delegate()
				{
					try
					{
						using (IEnumerator<string> enumerator = (from fn in Directory.EnumerateFiles(nd, "*.*", SearchOption.AllDirectories)
						where PerformanceFileListView.fileTypes.Contains(Path.GetExtension(fn), StringComparer.OrdinalIgnoreCase)
						select fn).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string s = enumerator.Current;
								if (this.HasNotes(s))
								{
									this.Dispatcher.Invoke(delegate()
									{
										this.collection.Add(new PerformanceListViewItem
										{
											RelativePath = s.Substring(nd.Length + 1),
											LastModified = File.GetLastWriteTime(s)
										});
									});
								}
							}
						}
					}
					catch (Exception e)
					{
						LogHost.Default.WarnException("[IndexPerformances] Could not index files/performances.", e);
					}
				}).Start();
				this.MakePerformDirWatcher(nd);
			}
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00016920 File Offset: 0x00014B20
		private void MakePerformDirWatcher(string nd)
		{
			this.performDirWatcher = new FileSystemWatcher(nd)
			{
				IncludeSubdirectories = true,
				EnableRaisingEvents = true
			};
			this.performDirWatcher.Created += this.PerformDirWatcher_Created;
			this.performDirWatcher.Changed += this.PerformDirWatcher_Changed;
			this.performDirWatcher.Deleted += this.PerformDirWatcher_Deleted;
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0001698C File Offset: 0x00014B8C
		private void PerformDirWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			if (PerformanceFileListView.fileTypes.Contains(Path.GetExtension(e.FullPath), StringComparer.OrdinalIgnoreCase))
			{
				string rp = e.FullPath.Substring(Settings.Default.PerformDirectory.Length + 1);
				if (this.collection.Any((PerformanceListViewItem x) => x.RelativePath == rp))
				{
					Func<PerformanceListViewItem, bool> <>9__2;
					base.Dispatcher.Invoke(delegate()
					{
						Collection<PerformanceListViewItem> collection = this.collection;
						IEnumerable<PerformanceListViewItem> source = this.collection;
						Func<PerformanceListViewItem, bool> predicate;
						if ((predicate = <>9__2) == null)
						{
							predicate = (<>9__2 = ((PerformanceListViewItem x) => x.RelativePath == rp));
						}
						collection.Remove(source.Single(predicate));
					});
				}
			}
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00016A14 File Offset: 0x00014C14
		private void PerformDirWatcher_Created(object sender, FileSystemEventArgs e)
		{
			if (PerformanceFileListView.fileTypes.Contains(Path.GetExtension(e.FullPath), StringComparer.OrdinalIgnoreCase) && this.HasNotes(e.FullPath))
			{
				base.Dispatcher.Invoke(delegate()
				{
					this.collection.Add(new PerformanceListViewItem
					{
						RelativePath = e.FullPath.Substring(Settings.Default.PerformDirectory.Length + 1),
						LastModified = File.GetLastWriteTime(e.FullPath)
					});
				});
			}
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x00016A80 File Offset: 0x00014C80
		private void PerformDirWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (PerformanceFileListView.fileTypes.Contains(Path.GetExtension(e.FullPath), StringComparer.OrdinalIgnoreCase))
			{
				this.PerformDirWatcher_Deleted(sender, e);
				this.PerformDirWatcher_Created(sender, e);
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x00016AB0 File Offset: 0x00014CB0
		private void DisposePerformDirWatcher()
		{
			if (this.performDirWatcher != null)
			{
				this.performDirWatcher.Created -= this.PerformDirWatcher_Created;
				this.performDirWatcher.Changed -= this.PerformDirWatcher_Changed;
				this.performDirWatcher.Deleted -= this.PerformDirWatcher_Deleted;
				this.performDirWatcher.Dispose();
				this.performDirWatcher = null;
			}
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00016B1C File Offset: 0x00014D1C
		private bool HasNotes(string s)
		{
			bool result;
			try
			{
				string[] f = File.ReadAllLines(s);
				if (Path.GetExtension(s).Equals(PerformanceFileListView.fileTypes[1], StringComparison.OrdinalIgnoreCase) && new Performance(string.Join(",", f)).Sheet.Count > 0)
				{
					result = true;
				}
				else
				{
					ImplementedPlayer mml = new ImplementedPlayer();
					for (int i = 0; i < f.Length; i++)
					{
						f[i] = f[i].RemoveLineComments();
					}
					string fmml = string.Join(string.Empty, f).RemoveBlockComments();
					mml.Load(fmml);
					result = mml.Tracks.Any((MMLPlayerTrack x) => x.notes.Any<Note>());
				}
			}
			catch (Exception e)
			{
				LogHost.Default.WarnException("HasNotes Could not read file: " + s, e);
				result = false;
			}
			return result;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00016C04 File Offset: 0x00014E04
		private void SettingChanging(object sender, SettingChangingEventArgs e)
		{
			if (e.SettingName == "PerformDirectory")
			{
				this.IndexPerformances((string)e.NewValue);
			}
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x00016C29 File Offset: 0x00014E29
		private bool Filter(object obj)
		{
			return string.IsNullOrWhiteSpace(this.FilterTextBox.Text) || ((PerformanceListViewItem)obj).RelativePath.IndexOf(this.FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x00016C64 File Offset: 0x00014E64
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

		// Token: 0x060004D5 RID: 1237 RVA: 0x00016D44 File Offset: 0x00014F44
		private void Sort(string sortBy, ListSortDirection direction)
		{
			ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ListView.ItemsSource);
			if (dataView != null)
			{
				dataView.SortDescriptions.Clear();
				SortDescription sd = new SortDescription(sortBy, direction);
				dataView.SortDescriptions.Add(sd);
				dataView.Refresh();
			}
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00016D8C File Offset: 0x00014F8C
		private void FilterTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.FilterCoverTextBlock.Visibility = Visibility.Hidden;
			if (!this.filterApplied)
			{
				CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(this.ListView.ItemsSource);
				if (cv != null)
				{
					cv.Filter = new Predicate<object>(this.Filter);
					this.filterApplied = true;
				}
			}
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00016DDF File Offset: 0x00014FDF
		private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
			{
				this.FilterCoverTextBlock.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00016DFF File Offset: 0x00014FFF
		private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(this.ListView.ItemsSource).Refresh();
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00016E16 File Offset: 0x00015016
		private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			((Window1)Window.GetWindow(this)).ProcessChatCommand(this, new CommandEventArgs(Command.Perform, ((PerformanceListViewItem)((ListViewItem)sender).Content).RelativePath));
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x00016E44 File Offset: 0x00015044
		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			((Window1)Window.GetWindow(this)).ProcessChatCommand(this, new CommandEventArgs(Command.PerformStop, string.Empty));
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00016E62 File Offset: 0x00015062
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.performDirWatcher.Dispose();
			}
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00016E72 File Offset: 0x00015072
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00016F8C File Offset: 0x0001518C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.8.1.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 4)
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = Control.MouseDoubleClickEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.ListViewItem_MouseDoubleClick);
				((Style)target).Setters.Add(eventSetter);
			}
		}

		// Token: 0x040003B2 RID: 946
		private GridViewColumnHeader _lastHeaderClicked;

		// Token: 0x040003B3 RID: 947
		private ListSortDirection _lastDirection;

		// Token: 0x040003B4 RID: 948
		private bool filterApplied;

		// Token: 0x040003B5 RID: 949
		private readonly ObservableCollection<PerformanceListViewItem> collection = new ObservableCollection<PerformanceListViewItem>();

		// Token: 0x040003B6 RID: 950
		private FileSystemWatcher performDirWatcher;

		// Token: 0x040003B7 RID: 951
		private static readonly string[] fileTypes = new string[]
		{
			".mml",
			".txt"
		};
	}
}
