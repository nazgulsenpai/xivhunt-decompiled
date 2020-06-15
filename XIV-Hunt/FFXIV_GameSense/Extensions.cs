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
	internal static class Extensions
	{
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

		public static string Replace(this string input, IEnumerable<string> replaceThese, string replaceWith)
		{
			foreach (string s in replaceThese)
			{
				input = input.Replace(s, replaceWith);
			}
			return input;
		}

		public static string Remove(this string input, IEnumerable<string> removeThese)
		{
			return input.Replace(removeThese, string.Empty);
		}

		public static char Increment(this char c)
		{
			return Convert.ToChar(Convert.ToInt32(c) + 1);
		}

		public static char Decrement(this char c)
		{
			return Convert.ToChar(Convert.ToInt32(c) - 1);
		}

		public static bool ContainsNonANSICharacters(this string input)
		{
			return input.Any((char c) => c > 'ÿ');
		}

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

		public static uint ToEpoch(this DateTime d)
		{
			return Convert.ToUInt32(TimeZoneInfo.ConvertTimeToUtc(d).ToUnixTimeSeconds());
		}

		public static bool IsWithin(this float val, int min, int max)
		{
			return val >= (float)min && val <= (float)max;
		}

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

		public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
		{
			bool allAdded = true;
			foreach (T item in items)
			{
				allAdded &= @this.Add(item);
			}
			return allAdded;
		}

		public static bool RemoveRange<T>(this HashSet<T> @this, IEnumerable<T> items)
		{
			bool allRemoved = true;
			foreach (T item in items)
			{
				allRemoved &= @this.Remove(item);
			}
			return allRemoved;
		}

		public static void MakeWindowUntransparent(this Window wnd)
		{
			if (!wnd.IsInitialized)
			{
				throw new Exception("The extension method MakeWindowUntransparent can not be called prior to the window being initialized.");
			}
			IntPtr handle = new WindowInteropHelper(wnd).Handle;
			NativeMethods.SetWindowLongPtr(handle, -20, (long)((ulong)Convert.ToUInt32(NativeMethods.GetWindowLongPtr3264(handle, -20).ToInt32() & -524289 & -33)));
		}

		public static void HideFromAltTab(this Window wnd)
		{
			if (!wnd.IsInitialized)
			{
				throw new Exception("The extension method HideFromAltTab can not be called prior to the window being initialized.");
			}
			IntPtr handle = new WindowInteropHelper(wnd).Handle;
			NativeMethods.SetWindowLongPtr(handle, -20, (long)((ulong)Convert.ToUInt32(NativeMethods.GetWindowLongPtr3264(handle, -20).ToInt32() | 128)));
		}

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

		public enum ExtendedWindowStyles : long
		{
			WS_EX_TOOLWINDOW = 128L,
			WS_EX_LAYERED = 524288L,
			WS_EX_TRANSPARENT = 32L
		}

		public enum GetWindowLongFields
		{
			GWL_EXSTYLE = -20
		}
	}
}
