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
	// Token: 0x020000A7 RID: 167
	public partial class AlarmButton : UserControl
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000448 RID: 1096 RVA: 0x00014528 File Offset: 0x00012728
		// (remove) Token: 0x06000449 RID: 1097 RVA: 0x00014536 File Offset: 0x00012736
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

		// Token: 0x0600044A RID: 1098 RVA: 0x00014544 File Offset: 0x00012744
		private void RaiseClickEvent()
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(AlarmButton.ClickEvent);
			base.RaiseEvent(newEventArgs);
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00014563 File Offset: 0x00012763
		private void OnClick()
		{
			this.RaiseClickEvent();
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0001456B File Offset: 0x0001276B
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

		// Token: 0x0600044D RID: 1101 RVA: 0x000145A8 File Offset: 0x000127A8
		private void AlarmButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				base.Opacity = 1.0;
				return;
			}
			base.Opacity = 0.25;
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000145D7 File Offset: 0x000127D7
		public void SetOn()
		{
			((AlarmButtonModel)base.DataContext).Icon = "/Resources/Images/sound_on.png";
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x000145EE File Offset: 0x000127EE
		public void SetOff()
		{
			((AlarmButtonModel)base.DataContext).Icon = "/Resources/Images/sound_off.png";
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00014605 File Offset: 0x00012805
		public bool IsOn()
		{
			return ((AlarmButtonModel)base.DataContext).Icon == "/Resources/Images/sound_on.png";
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001466E File Offset: 0x0001286E
		// Note: this type is marked as 'beforefieldinit'.
		static AlarmButton()
		{
			AlarmButton.ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AlarmButton));
		}
	}
}
