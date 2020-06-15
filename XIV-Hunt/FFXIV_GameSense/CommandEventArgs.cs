using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000060 RID: 96
	internal class CommandEventArgs : EventArgs
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x0000E25C File Offset: 0x0000C45C
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x0000E264 File Offset: 0x0000C464
		public Command Command { get; private set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0000E26D File Offset: 0x0000C46D
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x0000E275 File Offset: 0x0000C475
		public string Parameter { get; private set; }

		// Token: 0x060002C4 RID: 708 RVA: 0x0000E280 File Offset: 0x0000C480
		public CommandEventArgs(Command cmd, string p)
		{
			this.Command = cmd;
			if (p.Equals("/" + cmd.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				this.Parameter = string.Empty;
				return;
			}
			this.Parameter = p;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000E2D0 File Offset: 0x0000C4D0
		public override string ToString()
		{
			return "/" + this.Command.ToString() + " " + this.Parameter;
		}
	}
}
