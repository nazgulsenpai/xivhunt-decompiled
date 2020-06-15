using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace FFXIV_GameSense.UI
{
	public partial class AlarmButton : UserControl
	{
		public event RoutedEventHandler Click
		{
			add
			{
				base.AddHandler(AlarmButton.ClickEvent, value);
			}
			remove
			{
				base.RemoveHandler(AlarmButton.ClickEvent, value);
			}
		}

		private void RaiseClickEvent()
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(AlarmButton.ClickEvent);
			base.RaiseEvent(newEventArgs);
		}

		private void OnClick()
		{
			this.RaiseClickEvent();
		}

		public AlarmButton()
		{
			this.InitializeComponent();
			base.PreviewMouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs args)
			{
				this.OnClick();
			};
			base.IsEnabledChanged += this.AlarmButton_IsEnabledChanged;
			base.DataContext = new AlarmButtonModel();
		}

		private void AlarmButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				base.Opacity = 1.0;
				return;
			}
			base.Opacity = 0.25;
		}

		public void SetOn()
		{
			((AlarmButtonModel)base.DataContext).Icon = "/Resources/Images/sound_on.png";
		}

		public void SetOff()
		{
			((AlarmButtonModel)base.DataContext).Icon = "/Resources/Images/sound_off.png";
		}

		public bool IsOn()
		{
			return ((AlarmButtonModel)base.DataContext).Icon == "/Resources/Images/sound_on.png";
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AlarmButton()
		{
			AlarmButton.ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AlarmButton));
		}
	}
}
