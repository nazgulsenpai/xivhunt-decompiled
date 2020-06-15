using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FFXIV_GameSense.Overlay;
using FFXIV_GameSense.Properties;
using Overlay.NET.Common;
using Overlay.NET.Wpf;
using Process.NET.Windows;

namespace FFXIV_GameSense
{
	internal class RadarOverlay : WpfOverlayPlugin
	{
		public RadarOverlay(CancellationToken _ct)
		{
			this.ct = _ct;
		}

		public override void Enable()
		{
			this._tickEngine.IsTicking = true;
			base.Enable();
		}

		internal void SetNewFrameRate()
		{
			this.dispatcher.Interval = (this._tickEngine.Interval = (1000 / (int)Settings.Default.RadarMaxFrameRate).Milliseconds());
		}

		internal void SetBackgroundOpacity()
		{
			base.OverlayWindow.Dispatcher.Invoke<System.Windows.Media.Brush>(() => base.OverlayWindow.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Convert.ToByte((float)Settings.Default.RadarBGOpacity / 100f * 255f), 0, 0, 0)));
		}

		public override void Disable()
		{
			this._tickEngine.IsTicking = false;
			base.Disable();
		}

		public override void Initialize(IWindow targetWindow)
		{
			base.Initialize(targetWindow);
			this._targetWindow = targetWindow;
			base.OverlayWindow = new OverlayWindow(targetWindow)
			{
				Title = base.GetType().Name,
				ShowInTaskbar = false
			};
			base.OverlayWindow.Loaded += this.OverlayWindow_Loaded;
			base.OverlayWindow.MouseLeftButtonDown += this.OverlayWindow_MouseLeftButtonDown;
			base.OverlayWindow.MouseLeftButtonUp += this.OverlayWindow_MouseLeftButtonUp;
			base.OverlayWindow.MouseMove += this.OverlayWindow_MouseMove;
			base.OverlayWindow.SizeChanged += this.OverlayWindow_SizeChanged;
			this._tickEngine.Interval = (1000 / (int)Settings.Default.RadarMaxFrameRate).Milliseconds();
			this._tickEngine.PreTick += this.OnPreTick;
			this._tickEngine.Tick += this.OnTick;
			this.dispatcher = new DispatcherTimer();
			this.dispatcher.Tick += this.Dispatcher_Tick;
			this.dispatcher.Interval = this._tickEngine.Interval;
			this.dispatcher.Start();
		}

		private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
		{
			base.OverlayWindow.HideFromAltTab();
		}

		private void OverlayWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Settings.Default.RadarWindowSize = new System.Drawing.Size((int)e.NewSize.Width, (int)e.NewSize.Height);
			Settings.Default.Save();
		}

		private void Dispatcher_Tick(object sender, EventArgs e)
		{
			if (this.ct.IsCancellationRequested)
			{
				((DispatcherTimer)sender).Stop();
				Dispatcher.CurrentDispatcher.InvokeShutdown();
			}
			try
			{
				this.Update();
			}
			catch (Exception)
			{
			}
		}

		private void OverlayWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.MouseDown)
			{
				System.Windows.Point p = e.GetPosition(base.OverlayWindow);
				base.OverlayWindow.Left += p.X - this.DragStart.X;
				base.OverlayWindow.Top += p.Y - this.DragStart.Y;
			}
		}

		private void OverlayWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			NativeMethods.RECT trect;
			if (NativeMethods.GetWindowRect(this._targetWindow.Handle, out trect))
			{
				Settings.Default.RadarWindowOffset = new System.Drawing.Point((int)(base.OverlayWindow.Left - (double)trect.Left), (int)(base.OverlayWindow.Top - (double)trect.Top));
				Settings.Default.Save();
			}
			this.MouseDown = false;
			base.OverlayWindow.ReleaseMouseCapture();
		}

		private void OverlayWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.MouseDown = true;
			this.DragStart = e.GetPosition(base.OverlayWindow);
			base.OverlayWindow.CaptureMouse();
		}

		private void OnTick(object sender, EventArgs eventArgs)
		{
		}

		private void OnPreTick(object sender, EventArgs eventArgs)
		{
			if (!this._isSetup)
			{
				this.SetUp();
				this._isSetup = true;
			}
			if ((base.TargetWindow.IsActivated && !base.OverlayWindow.IsVisible) || RadarOverlay.ApplicationIsActivated())
			{
				base.OverlayWindow.Show();
				return;
			}
			if (!base.TargetWindow.IsActivated && base.OverlayWindow.IsVisible)
			{
				base.OverlayWindow.Hide();
			}
		}

		public static bool ApplicationIsActivated()
		{
			IntPtr activatedHandle = NativeMethods.GetForegroundWindow();
			if (activatedHandle == IntPtr.Zero)
			{
				return false;
			}
			int procId = Process.GetCurrentProcess().Id;
			int activeProcId;
			NativeMethods.GetWindowThreadProcessId(activatedHandle, out activeProcId);
			return activeProcId == procId;
		}

		internal void EnableResizeMode()
		{
			base.OverlayWindow.Dispatcher.Invoke<ResizeMode>(() => base.OverlayWindow.ResizeMode = ResizeMode.CanResizeWithGrip);
		}

		internal void DisableResizeMode()
		{
			base.OverlayWindow.Dispatcher.Invoke<ResizeMode>(() => base.OverlayWindow.ResizeMode = ResizeMode.NoResize);
		}

		internal void MakeClickable()
		{
			base.OverlayWindow.Dispatcher.Invoke(delegate()
			{
				OverlayWindow overlayWindow = base.OverlayWindow;
				if (overlayWindow == null)
				{
					return;
				}
				overlayWindow.MakeWindowUntransparent();
			});
		}

		internal void MakeClickthru()
		{
			base.OverlayWindow.Dispatcher.Invoke(delegate()
			{
				OverlayWindow overlayWindow = base.OverlayWindow;
				if (overlayWindow == null)
				{
					return;
				}
				overlayWindow.MakeWindowTransparent();
			});
		}

		public override void Update()
		{
			RadarOverlay.<>c__DisplayClass30_0 CS$<>8__locals1 = new RadarOverlay.<>c__DisplayClass30_0();
			this._tickEngine.Pulse();
			if (base.OverlayWindow == null || !base.OverlayWindow.IsVisible)
			{
				return;
			}
			if (!this.MouseDown)
			{
				this.FollowTargetWindow();
			}
			RadarOverlay.<>c__DisplayClass30_0 CS$<>8__locals2 = CS$<>8__locals1;
			FFXIVMemory mem = Program.mem;
			CS$<>8__locals2.self = ((mem != null) ? mem.GetSelfCombatant() : null);
			FFXIVMemory mem2 = Program.mem;
			List<Entity> clist = (mem2 != null) ? mem2.GetCombatantList() : null;
			if (CS$<>8__locals1.self != null && clist != null)
			{
				clist.RemoveAll((Entity c) => c.OwnerID == CS$<>8__locals1.self.ID);
				using (List<uint>.Enumerator enumerator = (from x in clist.OfType<PC>()
				select x.ID).ToList<uint>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						uint ID = enumerator.Current;
						clist.RemoveAll((Entity c) => c.OwnerID == ID);
					}
				}
				this.RemoveUnvantedCombatants(CS$<>8__locals1.self, clist);
				double centerY = base.OverlayWindow.Height / 2.0;
				double centerX = base.OverlayWindow.Width / 2.0;
				foreach (Entity c2 in clist)
				{
					if (c2.ID == 3758096384u && !this.miscDrawMap.ContainsKey(c2.PosX + c2.PosY))
					{
						this.miscDrawMap.Add(c2.PosX + c2.PosY, new EntityOverlayControl(c2, false));
						base.OverlayWindow.Add(this.miscDrawMap[c2.PosX + c2.PosY]);
					}
					else if (!this.drawMap.ContainsKey(c2.ID))
					{
						this.drawMap.Add(c2.ID, new EntityOverlayControl(c2, c2.ID == CS$<>8__locals1.self.ID));
						base.OverlayWindow.Add(this.drawMap[c2.ID]);
					}
					double Top = (double)(c2.PosY - CS$<>8__locals1.self.PosY);
					double Left = (double)(c2.PosX - CS$<>8__locals1.self.PosX);
					Top += centerY + Top * base.OverlayWindow.Height * 0.003 * (double)Settings.Default.RadarZoom;
					Left += centerX + Left * base.OverlayWindow.Width * 0.003 * (double)Settings.Default.RadarZoom;
					if (this.drawMap.ContainsKey(c2.ID))
					{
						this.drawMap[c2.ID].Update(c2);
						Top -= this.drawMap[c2.ID].ActualHeight / 2.0;
						Left -= this.drawMap[c2.ID].ActualWidth / 2.0;
						if (Top < 0.0)
						{
							Canvas.SetTop(this.drawMap[c2.ID], 0.0);
						}
						else if (Top > base.OverlayWindow.Height - this.drawMap[c2.ID].ActualHeight)
						{
							Canvas.SetTop(this.drawMap[c2.ID], base.OverlayWindow.Height - this.drawMap[c2.ID].ActualHeight);
						}
						else
						{
							Canvas.SetTop(this.drawMap[c2.ID], Top);
						}
						if (Left < 0.0)
						{
							Canvas.SetLeft(this.drawMap[c2.ID], 0.0);
						}
						else if (Left > base.OverlayWindow.Width - this.drawMap[c2.ID].ActualWidth)
						{
							Canvas.SetLeft(this.drawMap[c2.ID], base.OverlayWindow.Width - this.drawMap[c2.ID].ActualWidth);
						}
						else
						{
							Canvas.SetLeft(this.drawMap[c2.ID], Left);
						}
					}
					else if (this.miscDrawMap.ContainsKey(c2.PosX + c2.PosY))
					{
						this.miscDrawMap[c2.PosX + c2.PosY].Update(c2);
						Top -= this.miscDrawMap[c2.ID].ActualHeight / 2.0;
						Left -= this.miscDrawMap[c2.ID].ActualWidth / 2.0;
						Canvas.SetTop(this.miscDrawMap[c2.PosX + c2.PosY], Top);
						Canvas.SetLeft(this.miscDrawMap[c2.PosX + c2.PosY], Left);
					}
				}
			}
			KeyValuePair<uint, EntityOverlayControl>[] array = this.drawMap.ToArray<KeyValuePair<uint, EntityOverlayControl>>();
			int i = 0;
			while (i < array.Length)
			{
				KeyValuePair<uint, EntityOverlayControl> entry = array[i];
				if (!entry.Value.GetEntityName().Equals("Hoard!", StringComparison.Ordinal) || clist == null)
				{
					goto IL_626;
				}
				if (!clist.OfType<EObject>().Any((EObject c) => c.SubType == EObjType.Banded))
				{
					goto IL_626;
				}
				entry.Value.Visibility = Visibility.Collapsed;
				if (!this.hoardsDiscovered.Contains(entry.Key))
				{
					this.hoardsDiscovered.Add(entry.Key);
				}
				IL_668:
				i++;
				continue;
				IL_626:
				if (clist == null || !clist.Exists((Entity c) => c.ID == entry.Key))
				{
					entry.Value.Visibility = Visibility.Collapsed;
					this.drawMap.Remove(entry.Key);
					goto IL_668;
				}
				goto IL_668;
			}
			KeyValuePair<float, EntityOverlayControl>[] array2 = this.miscDrawMap.ToArray<KeyValuePair<float, EntityOverlayControl>>();
			for (i = 0; i < array2.Length; i++)
			{
				KeyValuePair<float, EntityOverlayControl> entry = array2[i];
				if (clist == null || !clist.Exists((Entity c) => c.PosX + c.PosY == entry.Key))
				{
					entry.Value.Visibility = Visibility.Collapsed;
					this.miscDrawMap.Remove(entry.Key);
				}
			}
		}

		private void RemoveUnvantedCombatants(Entity self, List<Entity> clist)
		{
			clist.RemoveAll((Entity c) => c is Monster && (((Monster)c).BNpcNameID == 5042 || ((Monster)c).BNpcNameID == 7395));
			if (!Settings.Default.displaySelf)
			{
				clist.RemoveAll((Entity c) => c.ID == self.ID);
			}
			if (!Settings.Default.displayMonsters)
			{
				clist.RemoveAll((Entity c) => c is Monster);
			}
			if (!Settings.Default.displayTreasureCoffers)
			{
				clist.RemoveAll((Entity c) => c is Treasure);
			}
			if (!Settings.Default.displayCairns)
			{
				clist.RemoveAll((Entity c) => c is EObject && ((EObject)c).SubType != EObjType.Silver && ((EObject)c).SubType != EObjType.Gold);
			}
			if (!Settings.Default.displayOtherPCs)
			{
				clist.RemoveAll((Entity c) => c is PC && c.ID != self.ID);
			}
			if (!Settings.Default.displaySilverTreasureCoffers)
			{
				clist.RemoveAll((Entity c) => c is EObject && ((EObject)c).SubType == EObjType.Silver);
			}
			if (!Settings.Default.displayGoldTreasureCoffers)
			{
				clist.RemoveAll((Entity c) => c is EObject && ((EObject)c).SubType == EObjType.Gold);
			}
			if (clist.Any((Entity c) => c is EObject && ((EObject)c).SubType == EObjType.Hoard && this.hoardsDiscovered.Contains(c.ID)))
			{
				clist.RemoveAll((Entity c) => c.ID == this.hoardsDiscovered.Last<uint>());
			}
		}

		private void FollowTargetWindow()
		{
			NativeMethods.RECT trect;
			if (NativeMethods.GetWindowRect(this._targetWindow.Handle, out trect))
			{
				base.OverlayWindow.Left = (double)(trect.Left + Settings.Default.RadarWindowOffset.X);
				base.OverlayWindow.Top = (double)(trect.Top + Settings.Default.RadarWindowOffset.Y);
				if (Settings.Default.RadarWindowSize.IsEmpty)
				{
					base.OverlayWindow.Width = (double)(trect.Right - trect.Left);
					base.OverlayWindow.Height = (double)(trect.Bottom - trect.Top);
					return;
				}
				base.OverlayWindow.Width = (double)Settings.Default.RadarWindowSize.Width;
				base.OverlayWindow.Height = (double)Settings.Default.RadarWindowSize.Height;
			}
		}

		public sealed override void Dispose()
		{
			if (this._isDisposed)
			{
				return;
			}
			if (base.IsEnabled)
			{
				this.Disable();
			}
			base.OverlayWindow = null;
			this._tickEngine.Stop();
			base.Dispose();
			this._isDisposed = true;
			GC.SuppressFinalize(this);
		}

		~RadarOverlay()
		{
			this.Dispose();
		}

		private void SetUp()
		{
			if (!Settings.Default.RadarDisableResize)
			{
				base.OverlayWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
			}
			if (!Settings.Default.RadarWindowSize.IsEmpty)
			{
				base.OverlayWindow.Width = (double)Settings.Default.RadarWindowSize.Width;
				base.OverlayWindow.Height = (double)Settings.Default.RadarWindowSize.Height;
			}
			this.SetBackgroundOpacity();
		}

		private readonly TickEngine _tickEngine = new TickEngine();

		private DispatcherTimer dispatcher;

		private readonly CancellationToken ct;

		private IWindow _targetWindow;

		private bool _isDisposed;

		private bool _isSetup;

		private System.Windows.Point DragStart;

		private bool MouseDown;

		private readonly Dictionary<uint, EntityOverlayControl> drawMap = new Dictionary<uint, EntityOverlayControl>();

		private readonly Dictionary<float, EntityOverlayControl> miscDrawMap = new Dictionary<float, EntityOverlayControl>();

		private readonly List<uint> hoardsDiscovered = new List<uint>();
	}
}
