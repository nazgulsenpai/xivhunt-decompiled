using System;

namespace FFXIV_GameSense
{
	internal class CommandEventArgs : EventArgs
	{
		public Command Command { get; private set; }

		public string Parameter { get; private set; }

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

		public override string ToString()
		{
			return "/" + this.Command.ToString() + " " + this.Parameter;
		}
	}
}
