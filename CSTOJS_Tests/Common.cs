using System.Globalization;
using System.Threading;

internal class Common
{
	public static string ConsoleStr { get; set; } = string.Empty;

	static Common()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
	}

	public static string PrepareStringForExecution(string translatedStr, string executeStr)
	{
		string str = "let console = { log: log };\n";
		str += translatedStr;
		if (executeStr != string.Empty)
			str += $"\nconsole.log({executeStr});";

		return str;
	}
	public static void ConsoleOutput(object? obj)
	{
		ConsoleStr = obj?.ToString() ?? "null";
	}
}