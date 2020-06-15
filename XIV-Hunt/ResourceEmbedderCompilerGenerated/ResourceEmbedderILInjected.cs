using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ResourceEmbedderCompilerGenerated
{
	[CompilerGenerated]
	public static class ResourceEmbedderILInjected
	{
		private static Assembly FindMainAssembly(AssemblyName requestedAssemblyName)
		{
			if (requestedAssemblyName == null)
			{
				throw new ArgumentNullException("requestedAssemblyName");
			}
			if (!requestedAssemblyName.Name.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException("Not a resource assembly");
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			string b = requestedAssemblyName.Name.Substring(0, requestedAssemblyName.Name.Length - ".resources".Length);
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.GetName().Name == b)
				{
					return assembly;
				}
			}
			return null;
		}

		private static Assembly LoadFromResource(AssemblyName requestedAssemblyName, Assembly requestingAssembly)
		{
			if (requestedAssemblyName == null || requestedAssemblyName.CultureInfo == null)
			{
				return null;
			}
			for (;;)
			{
				string arg = requestedAssemblyName.Name.Substring(0, requestedAssemblyName.Name.Length - ".resources".Length);
				string name = string.Format("{0}.{1}.resources.dll", arg, requestedAssemblyName.CultureInfo.Name);
				Assembly assembly = requestingAssembly ?? ResourceEmbedderILInjected.FindMainAssembly(requestedAssemblyName);
				if (assembly == null)
				{
					break;
				}
				using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
				{
					if (manifestResourceStream != null)
					{
						byte[] array = new byte[manifestResourceStream.Length];
						manifestResourceStream.Read(array, 0, array.Length);
						return Assembly.Load(array);
					}
				}
				string name2 = requestedAssemblyName.CultureInfo.Parent.Name;
				if (string.IsNullOrEmpty(name2))
				{
					goto Block_5;
				}
				requestedAssemblyName = new AssemblyName(requestedAssemblyName.FullName.Replace(string.Format("Culture={0}", requestedAssemblyName.CultureInfo.Name), string.Format("Culture={0}", name2)));
			}
			return null;
			Block_5:
			return null;
		}

		private static bool IsLocalizedAssembly(AssemblyName requestedAssemblyName)
		{
			return requestedAssemblyName.Name.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase);
		}

		public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyName requestedAssemblyName;
			try
			{
				requestedAssemblyName = new AssemblyName(args.Name);
			}
			catch (Exception ex) when (ex is ArgumentException || ex is FileLoadException)
			{
				return null;
			}
			if (!ResourceEmbedderILInjected.IsLocalizedAssembly(requestedAssemblyName))
			{
				return null;
			}
			return ResourceEmbedderILInjected.LoadFromResource(requestedAssemblyName, args.RequestingAssembly);
		}

		public static void Attach()
		{
			AppDomain.CurrentDomain.AssemblyResolve += ResourceEmbedderILInjected.AssemblyResolve;
		}
	}
}
