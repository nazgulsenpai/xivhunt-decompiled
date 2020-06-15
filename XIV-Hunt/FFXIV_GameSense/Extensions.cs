using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using FFXIV_GameSense.Properties;

namespace FFXIV_GameSense
{
	// Token: 0x0200002D RID: 45
	internal static class Extensions
	{
		// Token: 0x0600015D RID: 349 RVA: 0x000063DD File Offset: 0x000045DD
		public static IEnumerable<string> SplitAndKeep(this string s, string delim, StringComparison stringComparison)
		{
			int start = 0;
			int index;
			while ((index = s.IndexOf(delim, start, stringComparison)) != -1)
			{
				if (index - start > 0)
				{
					int num = start;
					int length = index - num;
					yield return s.Substring(num, length);
				}
				yield return s.Substring(index, delim.Length);
				start = index + delim.Length;
			}
			if (start < s.Length)
			{
				yield return s.Substring(start);
			}
			yield break;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000063FC File Offset: 0x000045FC
		public static string RemoveLineComments(this string i)
		{
			string lineComments = "//";
			int p = i.IndexOf(lineComments, StringComparison.Ordinal);
			if (p > -1)
			{
				return i.Substring(0, p);
			}
			return i;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00006428 File Offset: 0x00004628
		public static string RemoveBlockComments(this string i)
		{
			string blockComments = "/\\*(.*?)\\*/";
			return Regex.Replace(i, blockComments, delegate(Match me)
			{
				if (!me.Value.StartsWith("/*", StringComparison.Ordinal) && !me.Value.StartsWith("//", StringComparison.Ordinal))
				{
					return me.Value;
				}
				if (!me.Value.StartsWith("//", StringComparison.Ordinal))
				{
					return "";
				}
				return Environment.NewLine;
			}, RegexOptions.Singleline);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00006464 File Offset: 0x00004664
		public static int NthIndexOf(this string input, string value, int startIndex, int nth = 1)
		{
			if (nth < 1)
			{
				throw new NotSupportedException("Param 'nth' must be greater than 0!");
			}
			if (nth == 1)
			{
				return input.IndexOf(value, startIndex);
			}
			int idx = input.IndexOf(value, startIndex);
			if (idx == -1)
			{
				return -1;
			}
			return input.NthIndexOf(value, idx + 1, --nth);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000064AC File Offset: 0x000046AC
		public static int NthLastIndexOf(this string input, string value, int nth = 1)
		{
			if (nth < 1)
			{
				throw new NotSupportedException("Param 'nth' must be greater than 0!");
			}
			if (nth == 1)
			{
				return input.LastIndexOf(value);
			}
			int idx = input.LastIndexOf(value);
			if (idx == -1)
			{
				return -1;
			}
			return input.Substring(0, idx).NthLastIndexOf(value, --nth);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000064F6 File Offset: 0x000046F6
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00006510 File Offset: 0x00004710
		public static byte[] ReplaceSequence(this byte[] input, byte[] toRemove, byte[] replaceWith)
		{
			if (toRemove.Length == 0)
			{
				return input;
			}
			List<byte> result = new List<byte>();
			int i;
			for (i = 0; i <= input.Length - toRemove.Length; i++)
			{
				bool foundMatch = true;
				for (int j = 0; j < toRemove.Length; j++)
				{
					if (input[i + j] != toRemove[j])
					{
						foundMatch = false;
						break;
					}
				}
				if (foundMatch)
				{
					result.AddRange(replaceWith);
					i += toRemove.Length - 1;
				}
				else
				{
					result.Add(input[i]);
				}
			}
			while (i < input.Length)
			{
				result.Add(input[i]);
				i++;
			}
			return result.ToArray();
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00006591 File Offset: 0x00004791
		public static string ReplaceAt(this string input, int index, char newChar)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			StringBuilder stringBuilder = new StringBuilder(input);
			stringBuilder[index] = newChar;
			return stringBuilder.ToString();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000065B4 File Offset: 0x000047B4
		public static string Replace(this string input, IEnumerable<string> replaceThese, string replaceWith)
		{
			foreach (string s in replaceThese)
			{
				input = input.Replace(s, replaceWith);
			}
			return input;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00006600 File Offset: 0x00004800
		public static string Remove(this string input, IEnumerable<string> removeThese)
		{
			return input.Replace(removeThese, string.Empty);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000660E File Offset: 0x0000480E
		public static char Increment(this char c)
		{
			return Convert.ToChar(Convert.ToInt32(c) + 1);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000661D File Offset: 0x0000481D
		public static char Decrement(this char c)
		{
			return Convert.ToChar(Convert.ToInt32(c) - 1);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000662C File Offset: 0x0000482C
		public static bool ContainsNonANSICharacters(this string input)
		{
			return input.Any((char c) => c > 'ÿ');
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00006654 File Offset: 0x00004854
		public static string FirstLetterToUpperCase(this string s)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				return s;
			}
			char[] a = s.ToCharArray();
			a[0] = char.ToUpper(a[0], CultureInfo.CreateSpecificCulture(Settings.Default.LanguageCI));
			return new string(a);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00006694 File Offset: 0x00004894
		public static uint ToEpoch(this DateTime d)
		{
			return Convert.ToUInt32(TimeZoneInfo.ConvertTimeToUtc(d).ToUnixTimeSeconds());
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000066B9 File Offset: 0x000048B9
		public static bool IsWithin(this float val, int min, int max)
		{
			return val >= (float)min && val <= (float)max;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x000066CC File Offset: 0x000048CC
		public static int IndexOf<T>(this IEnumerable<T> collection, IEnumerable<T> sequence)
		{
			int ccount = collection.Count<T>();
			int scount = sequence.Count<T>();
			if (scount > ccount)
			{
				return -1;
			}
			if (collection.Take(scount).SequenceEqual(sequence))
			{
				return 0;
			}
			int index = Enumerable.Range(1, ccount - scount + 1).FirstOrDefault((int i) => collection.Skip(i).Take(scount).SequenceEqual(sequence));
			if (index == 0)
			{
				return -1;
			}
			return index;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00006760 File Offset: 0x00004960
		public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
		{
			bool allAdded = true;
			foreach (T item in items)
			{
				allAdded &= @this.Add(item);
			}
			return allAdded;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x000067B0 File Offset: 0x000049B0
		public static bool RemoveRange<T>(this HashSet<T> @this, IEnumerable<T> items)
		{
			bool allRemoved = true;
			foreach (T item in items)
			{
				allRemoved &= @this.Remove(item);
			}
			return allRemoved;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00006800 File Offset: 0x00004A00
		public static void MakeWindowUntransparent(this Window wnd)
		{
			if (!wnd.IsInitialized)
			{
				throw new Exception("The extension method MakeWindowUntransparent can not be called prior to the window being initialized.");
			}
			IntPtr handle = new WindowInteropHelper(wnd).Handle;
			NativeMethods.SetWindowLongPtr(handle, -20, (long)((ulong)Convert.ToUInt32(NativeMethods.GetWindowLongPtr3264(handle, -20).ToInt32() & -524289 & -33)));
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00006854 File Offset: 0x00004A54
		public static void HideFromAltTab(this Window wnd)
		{
			if (!wnd.IsInitialized)
			{
				throw new Exception("The extension method HideFromAltTab can not be called prior to the window being initialized.");
			}
			IntPtr handle = new WindowInteropHelper(wnd).Handle;
			NativeMethods.SetWindowLongPtr(handle, -20, (long)((ulong)Convert.ToUInt32(NativeMethods.GetWindowLongPtr3264(handle, -20).ToInt32() | 128)));
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000068A4 File Offset: 0x00004AA4
		public static byte[] ToByteArray<T>(this T structure) where T : struct
		{
			int bufferSize = Marshal.SizeOf<T>(structure);
			byte[] byteArray = new byte[bufferSize];
			IntPtr handle = Marshal.AllocHGlobal(bufferSize);
			try
			{
				Marshal.StructureToPtr<T>(structure, handle, false);
				Marshal.Copy(handle, byteArray, 0, bufferSize);
			}
			finally
			{
				Marshal.FreeHGlobal(handle);
			}
			return byteArray;
		}

		// Token: 0x0200002E RID: 46
		public enum ExtendedWindowStyles : long
		{
			// Token: 0x040000DC RID: 220
			WS_EX_TOOLWINDOW = 128L,
			// Token: 0x040000DD RID: 221
			WS_EX_LAYERED = 524288L,
			// Token: 0x040000DE RID: 222
			WS_EX_TRANSPARENT = 32L
		}

		// Token: 0x0200002F RID: 47
		public enum GetWindowLongFields
		{
			// Token: 0x040000E0 RID: 224
			GWL_EXSTYLE = -20
		}
	}
}
