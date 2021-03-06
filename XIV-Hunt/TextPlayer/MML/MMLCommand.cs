﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextPlayer.MML
{
	public struct MMLCommand
	{
		public static MMLCommand Parse(string token)
		{
			MMLCommand cmd = default(MMLCommand);
			List<string> args = new List<string>();
			char c = token.ToLowerInvariant()[0];
			MMLCommandType t;
			if (c <= '<')
			{
				if (c == '&')
				{
					t = MMLCommandType.Tie;
					goto IL_17E;
				}
				if (c == '<')
				{
					t = MMLCommandType.OctaveDown;
					goto IL_17E;
				}
			}
			else
			{
				if (c == '>')
				{
					t = MMLCommandType.OctaveUp;
					goto IL_17E;
				}
				switch (c)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'g':
					t = MMLCommandType.Note;
					MMLCommand.AddPart(args, token, "[a-gA-G]");
					MMLCommand.AddPart(args, token, "(\\+|#|-)");
					MMLCommand.AddPart(args, token, MMLCommand.lengths);
					MMLCommand.AddPart(args, token, "\\.");
					goto IL_17E;
				case 'l':
					t = MMLCommandType.Length;
					MMLCommand.AddPart(args, token, MMLCommand.lengths);
					MMLCommand.AddPart(args, token, "\\.");
					goto IL_17E;
				case 'n':
					t = MMLCommandType.NoteNumber;
					MMLCommand.AddPart(args, token, "\\d+");
					MMLCommand.AddPart(args, token, "\\.");
					goto IL_17E;
				case 'o':
					t = MMLCommandType.Octave;
					MMLCommand.AddPart(args, token, "\\d");
					goto IL_17E;
				case 'r':
					t = MMLCommandType.Rest;
					MMLCommand.AddPart(args, token, MMLCommand.lengths);
					MMLCommand.AddPart(args, token, "\\.");
					goto IL_17E;
				case 't':
					t = MMLCommandType.Tempo;
					MMLCommand.AddPart(args, token, "\\d{1,3}");
					goto IL_17E;
				case 'v':
					t = MMLCommandType.Volume;
					MMLCommand.AddPart(args, token, "\\d+");
					goto IL_17E;
				}
			}
			t = MMLCommandType.Unknown;
			args.Add(token);
			IL_17E:
			cmd.Type = t;
			cmd.Args = args;
			return cmd;
		}

		private static void AddPart(List<string> args, string token, string pattern)
		{
			string s = Regex.Match(token, pattern).Value;
			args.Add(s);
		}

		public MMLCommandType Type;

		public List<string> Args;

		internal static readonly string lengths = "(" + string.Join<int>("|", Enumerable.Range(1, 32).Concat(new int[]
		{
			64
		}).Reverse<int>().ToArray<int>()) + ")";
	}
}
