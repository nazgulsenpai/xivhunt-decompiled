using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFXIV_GameSense.IPC;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x02000095 RID: 149
	internal class Performance
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x00012040 File Offset: 0x00010240
		// (set) Token: 0x060003D1 RID: 977 RVA: 0x00012048 File Offset: 0x00010248
		internal List<Note> Sheet { get; set; }

		// Token: 0x060003D2 RID: 978 RVA: 0x00012054 File Offset: 0x00010254
		public Performance(string i)
		{
			this.Sheet = new List<Note>();
			string[] array = i.Split(',', StringSplitOptions.None);
			for (int j = 0; j < array.Length; j++)
			{
				string s = array[j];
				string st = s.Trim();
				st = Performance.LeewaySharpFlat(st);
				st = Performance.LeewayNote(st);
				uint duration;
				if (Performance.notes.Any((Note x) => x.Name.Equals(st)))
				{
					Note t = Performance.notes.Single((Note x) => x.Name.Equals(st));
					this.Sheet.Add(new Note
					{
						Id = t.Id,
						Name = t.Name,
						Wait = 500u
					});
				}
				else if (st.StartsWith("w", StringComparison.OrdinalIgnoreCase) && uint.TryParse(st.Substring(1), out duration))
				{
					this.Sheet.Last<Note>().Wait = duration;
				}
				else if (st.StartsWith("l", StringComparison.OrdinalIgnoreCase) && uint.TryParse(st.Substring(1), out duration))
				{
					this.Sheet.Last<Note>().Length = duration;
				}
			}
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x000121A5 File Offset: 0x000103A5
		private static string LeewaySharpFlat(string nn)
		{
			nn = nn.Replace('#', '♯');
			if (nn.Length > 1 && nn[1] == 'b')
			{
				nn = nn.ReplaceAt(1, '♭');
			}
			return nn;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000121DC File Offset: 0x000103DC
		private static string LeewayNote(string note)
		{
			if (Performance.notes.Any((Note x) => x.Name == note) || note.Length < 2)
			{
				return note;
			}
			string nn = null;
			if (note[1] == '♭')
			{
				nn = note.Replace('♭', '♯');
				nn = ((note[0] == 'A') ? nn.ReplaceAt(0, 'G') : nn.ReplaceAt(0, nn[0].Decrement()));
			}
			else if (note[1] == '♯')
			{
				nn = note.Replace('♯', '♭');
				nn = ((note[0] == 'G') ? nn.ReplaceAt(0, 'A') : nn.ReplaceAt(0, nn[0].Increment()));
			}
			if (Performance.notes.Any((Note x) => x.Name == nn))
			{
				return nn;
			}
			if (nn != null)
			{
				nn = nn.Remove(1, 1);
				if (Performance.notes.Any((Note x) => x.Name == nn))
				{
					return nn;
				}
			}
			return note;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00012370 File Offset: 0x00010570
		internal async Task Play(int pid, CancellationToken cts)
		{
			PipeMessage noteOff = new PipeMessage(pid, PMCommand.PlayNote, 0);
			foreach (Note i in this.Sheet)
			{
				PersistentNamedPipeServer.SendPipeMessage(new PipeMessage(pid, PMCommand.PlayNote, i.Id));
				await Task.Delay((int)i.Length).ConfigureAwait(false);
				PersistentNamedPipeServer.SendPipeMessage(noteOff);
				TimeSpan untilNextNote = TimeSpan.FromMilliseconds((double)(i.Wait - i.Length));
				if (untilNextNote.TotalMilliseconds > 0.0)
				{
					await Task.Delay(untilNextNote).ConfigureAwait(false);
				}
				if (cts.IsCancellationRequested)
				{
					break;
				}
				i = null;
			}
			List<Note>.Enumerator enumerator = default(List<Note>.Enumerator);
		}

		// Token: 0x04000317 RID: 791
		private static readonly IEnumerable<Note> notes = GameResources.GetPerformanceNotes();
	}
}
