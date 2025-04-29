using CSharpToJavaScript;
using CSharpToJavaScript.APIs.JS.Ecma;
using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSTOJS_Tests;
public class UnitTestECMA
{
	private readonly Engine _Engine = new();
	private string _ConsoleStr = string.Empty;
	private readonly CSTOJS _CSTOJS = new();
	private readonly CSTOJSOptions _DefaultUnitOpt = new()
	{
		AddSBAtTheTop = new("let console = { log: log };")
	};

	public UnitTestECMA()
	{
		_Engine.SetValue("log", new Action<object>(ConsoleOutPut));

		_CSTOJS = new CSTOJS();
	}

	[Theory]
	//new
	[InlineData("new Array().ToString()", "")]
	[InlineData("new Boolean().ToString()", "false")]
	[InlineData("new Error().ToString()", "Error")]
	[InlineData("new Function().ToString()", "function anonymous() { [native code] }")]
	[InlineData("new Number().ToString()", "0")]
	[InlineData("new Object().ToString()", "[object Object]")]
	[InlineData("new RegExp().ToString()", "/(?:)/")]
	[InlineData("new String().ToString()", "")]
	//without new
	[InlineData("Math.ToString()", "[object Math]")]
	[InlineData("Date.ToString()", "function Date() { [native code] }")]
	//without new(globalThis.-)
	[InlineData("GlobalThis.BigInt(0).ToString()", "0")]
	[InlineData("GlobalThis.Symbol(0).ToString()", "Symbol(0)")]
	public void TestObjectToString(string value, string expected)
	{
		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS.Ecma;
using Array = CSharpToJavaScript.APIs.JS.Ecma.Array;
using Date = CSharpToJavaScript.APIs.JS.Ecma.Date;
using Math = CSharpToJavaScript.APIs.JS.Ecma.Math;
using Object = CSharpToJavaScript.APIs.JS.Ecma.Object;
using String = CSharpToJavaScript.APIs.JS.Ecma.String;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
Console.WriteLine({value});", _DefaultUnitOpt);
		_Engine.Execute(sb.ToString());
		Assert.Equal(expected, _ConsoleStr);
	}

	private void ConsoleOutPut(object obj)
	{
		_ConsoleStr = obj.ToString() ?? "null";
	}
}

