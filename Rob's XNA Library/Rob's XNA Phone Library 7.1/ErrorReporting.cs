/*
Copy and paste this in to XNA app:

using System.Windows;

public Constructor(){
	Application.Current.UnhandledException+=(s,e) => {
		if (!System.Diagnostics.Debugger.IsAttached)   <- optional
			try {
				ErrorReporting.SendException(e.e);
			} catch {
				// We do not want to throw exceptions in our exception handler 
			}
	};
}

Don't forget to add a reference to System.Windows.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO.IsolatedStorage;
using System.IO;

namespace RobsXNALibrary {
	public static class ErrorReporting {
		private const string REPORT_URL="http://robware.co.uk/errorreport.php";

		public static string AppName="";
		public static string Version="";

		private static string _filename="ErrorReport";
		private static string _extension=".xml";
		private static IsolatedStorageFile _store=IsolatedStorageFile.GetUserStoreForApplication();

		private static string GetFileName() {
			if (!_store.FileExists(_filename+_extension))
				return _filename+_extension;
			int i=0;
			while (_store.FileExists(_filename+i+_extension)) {
				i++;
			}
			return _filename+i+_extension;
		}

		public static void SendException(Exception ex) {
			string xml="<error>\n";
			xml+="	<application>"+AppName+"</application>\n";
			xml+="	<version>"+Version+"</version>\n";
			xml+="	<timestamp>"+DateTime.Now.ToString()+"</timestamp>\n";
			xml+="	<message>"+ex.Message+"</message>\n";
			xml+="	<stacktrace>"+ex.StackTrace+"</stacktrace>\n";
			xml+="</error>";
			string filename=GetFileName();
			TextWriter tw=new StreamWriter(_store.CreateFile(filename));
			tw.WriteLine(xml);
			tw.Flush();
			tw.Close();
			/*using (StreamWriter sw=new StreamWriter(new IsolatedStorageFileStream(filename,FileMode.Create,FileAccess.Write,_store))) {
				sw.WriteLine(xml);
				sw.Close();
			}*/
		}
	}
}
