using System;
using System.Reflection;

static class AssemblyFiles{
	static Assembly assembly = Assembly.GetExecutingAssembly();
	
	const string assemblyName = "unisim";
	
	public static byte[] get(string name){
		byte[] b;
		
		string resourceName = assembly.GetManifestResourceNames().Single(str => str == assemblyName + "." + name);
		
		using (Stream stream = assembly.GetManifestResourceStream(resourceName))
		using (MemoryStream memoryStream = new MemoryStream()){
			stream.CopyTo(memoryStream);  // Copy the stream to memory
			b = memoryStream.ToArray();
		}
		
		return b;
	}
	
	public static string getText(string name){
		string t;
		
		string resourceName = assembly.GetManifestResourceNames().Single(str => str == assemblyName + "." + name);
		
		using (Stream stream = assembly.GetManifestResourceStream(resourceName))
		using (StreamReader reader = new StreamReader(stream)){
			t = reader.ReadToEnd();
		}
		
		return t;
	}
	
	public static bool exists(string name){
		return assembly.GetManifestResourceNames().Any(str => str == assemblyName + "." + name);
	}
}