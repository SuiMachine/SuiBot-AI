using System;
using System.Diagnostics;
using System.IO;

namespace SuiBotAI
{
	static class ErrorLogginAI
	{
		const string FILENAME = "SuiBot-AI.log";

		public static void WriteLine(string text)
		{
			Console.WriteLine(text);
			string textToSave = $"{DateTime.Now}: {text}";
#if DEBUG
            Debug.WriteLine(textToSave);
#endif
			File.AppendAllText(FILENAME, textToSave + "\n");
		}
	}
}
