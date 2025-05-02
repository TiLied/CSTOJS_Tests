using CSharpToJavaScript;
using CSharpToJavaScript.APIs.JS.Ecma;
using Jint;
using System;
using System.Text;

using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;

using Array = CSharpToJavaScript.APIs.JS.Ecma.Array;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
using Date = CSharpToJavaScript.APIs.JS.Ecma.Date;
using Object = CSharpToJavaScript.APIs.JS.Ecma.Object;
using String = CSharpToJavaScript.APIs.JS.Ecma.String;

namespace CSTOJS_Tests.ECMA;
public class UnitTest_ObjectPrototype
{
	private readonly Engine _Engine = new();
	private string _ConsoleStr = string.Empty;
	private readonly CSTOJS _CSTOJS = new();
	private readonly CSTOJSOptions _DefaultUnitOpt = new()
	{
		AddSBAtTheTop = new("let console = { log: log };")
	};

	//0=cs expression
	//1=js expected result
	public static TheoryData<string[]> TestToStringData = new()
		{
			new string[] {"new Array().ToString()", "" },
			new string[] {"GlobalThis.Array().ToString()", ""},

			new string[] { "new ArrayBuffer(0).ToString()", "[object ArrayBuffer]" },
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/ArrayBuffer/ArrayBuffer#syntax

			new string[] { "GlobalThis.BigInt(0).ToString()", "0"},

			new string[] {"new Boolean(true).ToString()", "true"},
			//Boolean without new returns bool/value
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Boolean/Boolean#return_value

			new string[] { "new DataView(new ArrayBuffer(0)).ToString()", "[object DataView]" },
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/DataView/DataView#syntax

			new string[] {"new Date(\"0\").ToString()", "Invalid Date"},
			//Date without new returns string
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/Date#return_value

			new string[] {"new Error(\"\").ToString()", "Error"},
			new string[] {"GlobalThis.Error(\"\").ToString()", "Error"},

			new string[] {"new Function(\"\",\"\").ToString()", "function anonymous() { [native code] }" },
			new string[] {"GlobalThis.Function(\"\",\"\").ToString()", "function anonymous() { [native code] }" },

			new string[] {"new Map().ToString()", "[object Map]" },
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Map/Map#syntax

			//Math?

			new string[] {"new Number(0).ToString()", "0"},
			//Number without new returns value
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/Number#return_value

			new string[] {"new Object().ToString()", "[object Object]"},
			new string[] {"GlobalThis.Object().ToString()", "[object Object]"},

			new string[] {"new RegExp(\"\").ToString()", "/(?:)/"},
			//RegExp without new returns pattern
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/RegExp/RegExp#return_value

			new string[] {"new Set().ToString()", "[object Set]" },
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Set/Set#syntax

			new string[]{ "new SharedArrayBuffer(0).ToString()", "[object SharedArrayBuffer]" },
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/SharedArrayBuffer/SharedArrayBuffer#syntax

			new string[] {"new String().ToString()", ""},
			//String without new returns string/value
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/String#return_value

			new string[] {"GlobalThis.Symbol().ToString()", "Symbol()" },

			//No need to test others. I think...
			//Can be called only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Int8Array/Int8Array#syntax
			new string[] { "new Int8Array().ToString()", "" },

			new string[] { "new WeakMap().ToString()", "[object WeakMap]" },
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/WeakMap/WeakMap#syntax
			
			new string[] { "new WeakSet().ToString()", "[object WeakSet]" }
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/WeakSet/WeakSet#syntax
			
	};
	public UnitTest_ObjectPrototype()
	{
		_Engine.SetValue("log", new Action<object>(ConsoleOutPut));

		_CSTOJS = new CSTOJS();
	}
	private static void CSTest() 
	{
		new Array().ToString();
		GlobalThis.Array().ToString();

		new ArrayBuffer(0).ToString();

		GlobalThis.BigInt(0).ToString();

		new Boolean(true).ToString();

		new DataView(new ArrayBuffer(0)).ToString();

		new Date("0").ToString();

		new Error("").ToString();
		GlobalThis.Error("").ToString();

		new Function("","").ToString();
		GlobalThis.Function("","").ToString();

		new Map().ToString();

		new Number(0).ToString();

		new Object().ToString();
		GlobalThis.Object().ToString();

		new RegExp("").ToString();

		new Set().ToString();

		new SharedArrayBuffer(0).ToString();

		new String().ToString();

		GlobalThis.Symbol().ToString();

		new Int8Array().ToString();

		new WeakMap().ToString();

		new WeakSet().ToString();
	}


	[Theory]
	[MemberData(nameof(TestToStringData))]
	public void Test_ToString(string[] data)
	{
		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS.Ecma;
using Array = CSharpToJavaScript.APIs.JS.Ecma.Array;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
using Date = CSharpToJavaScript.APIs.JS.Ecma.Date;
using Object = CSharpToJavaScript.APIs.JS.Ecma.Object;
using String = CSharpToJavaScript.APIs.JS.Ecma.String;
Console.WriteLine({data[0]});", _DefaultUnitOpt);

		_Engine.Execute(sb.ToString());

		Assert.Equal(data[1], _ConsoleStr);
	}
	/*
	public void Test_HasOwnProperty()
	{
		throw new System.NotImplementedException();
	}

	public void Test_IsPrototypeOf()
	{
		throw new System.NotImplementedException();
	}

	public void Test_PropertyIsEnumerable()
	{
		throw new System.NotImplementedException();
	}

	public void Test_ToLocaleString()
	{
		throw new System.NotImplementedException();
	}

	public void Test_ValueOf()
	{
		throw new System.NotImplementedException();
	}
	*/
	private void ConsoleOutPut(object? obj)
	{
		_ConsoleStr = obj?.ToString() ?? "null";
	}
}

