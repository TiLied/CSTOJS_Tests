using CSharpToJavaScript;
using Jint;
using System;

namespace CSTOJS_Tests.ECMA;
public class UnitTest_ObjectPrototype
{
	private readonly Engine _Engine = new();
	private string _ConsoleStr = string.Empty;
	private readonly CSTOJSOptions _DefaultUnitOpt = new()
	{
		AddSBAtTheTop = new("let console = { log: log };")
	};

	public static TheoryDataRow<TestData>[] TestData_ToString =
		[
			new(new(" new Array().ToString()", "")),
			new(new(" GlobalThis.Array().ToString()", "")),

			new(new( "new ArrayBuffer(0).ToString()", "[object ArrayBuffer]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/ArrayBuffer/ArrayBuffer#syntax

			new(new( "GlobalThis.BigInt(0).ToString()", "0")),

			new(new("new Boolean(true).ToString()", "true")),
			//Boolean without new returns bool/value
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Boolean/Boolean#return_value

			new(new( "new DataView(new ArrayBuffer(0)).ToString()", "[object DataView]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/DataView/DataView#syntax

			new(new( "new Date(\"0\").ToString()", "Invalid Date")),
			//Date without new returns string
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/Date#return_value

			new(new( "new Error(\"\").ToString()", "Error")),
			new(new("GlobalThis.Error(\"\").ToString()", "Error")),

			new(new("new FinalizationRegistry(() => { }).ToString()", "[object FinalizationRegistry]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/FinalizationRegistry/FinalizationRegistry#syntax

			new(new( "new Function(\"\",\"\").ToString()", "function anonymous() { [native code] }" )),
			new(new( "GlobalThis.Function(\"\",\"\").ToString()", "function anonymous() { [native code] }")),

			new(new("new Map().ToString()", "[object Map]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Map/Map#syntax

			//Math?

			new(new("new Number(0).ToString()", "0")),
			//Number without new returns value
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/Number#return_value

			new(new("new Object().ToString()", "[object Object]")),
			new(new("GlobalThis.Object().ToString()", "[object Object]")),

			new(new( "new Promise(() => { }).ToString()", "[object Promise]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise/Promise#syntax

			new(new( "new Proxy(new {}, new {}).ToString()", "[object Object]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Proxy/Proxy#syntax

			new(new("new RegExp(\"\").ToString()", "/(?:)/")),
			//RegExp without new returns pattern
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/RegExp/RegExp#return_value

			new(new("new Set().ToString()", "[object Set]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Set/Set#syntax

			new(new("new SharedArrayBuffer(0).ToString()", "[object SharedArrayBuffer]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/SharedArrayBuffer/SharedArrayBuffer#syntax

			new(new("new String().ToString()", "")),
			//String without new returns string/value
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/String#return_value

			new(new("GlobalThis.Symbol().ToString()", "Symbol()" )),

			//No need to test others. I think...
			//Can be called only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Int8Array/Int8Array#syntax
			new(new( "new Int8Array().ToString()", "" )),

			new(new( "new WeakMap().ToString()", "[object WeakMap]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/WeakMap/WeakMap#syntax
			
			new(new( "new WeakRef(new Object()).ToString()", "[object WeakRef]" )),
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/WeakRef/WeakRef#syntax

			new(new( "new WeakSet().ToString()", "[object WeakSet]" ))
			//Only with new:
			//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/WeakSet/WeakSet#syntax
			
		];
	public UnitTest_ObjectPrototype()
	{
		_Engine.SetValue("log", new Action<object>(ConsoleOutPut));
	}

	[Theory]
	[MemberData(nameof(TestData_ToString))]
	public void Test_ToString(TestData data)
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS.Ecma;
using Array = CSharpToJavaScript.APIs.JS.Ecma.Array;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
using Date = CSharpToJavaScript.APIs.JS.Ecma.Date;
using Object = CSharpToJavaScript.APIs.JS.Ecma.Object;
using String = CSharpToJavaScript.APIs.JS.Ecma.String;
Console.WriteLine({data.CSValue});"
		};
		FileData[] files = CSTOJS.Translate([ file ]);

		_Engine.Execute(files[0].TranslatedStr);
		Assert.Equal(data.Expected, _ConsoleStr);
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

