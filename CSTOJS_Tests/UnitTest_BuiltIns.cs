
using System;
using System.Globalization;
using CSharpToJavaScript;
using Jint;

namespace CSTOJS_Tests;

public class UnitTest_BuiltIns
{
	private readonly Engine _JintEngine = new(cfg => cfg.Culture(CultureInfo.InvariantCulture));

	public UnitTest_BuiltIns()
	{
		_JintEngine.SetValue("log", new Action<object>(Common.ConsoleOutput));
	}
	
	[Theory]
	[InlineData("Console.WriteLine(1);", "console.log(1);", "", "1")]
	[InlineData("using static System.Console;WriteLine(1);", "console.log(1);", "", "1")]
	[InlineData("System.Console.WriteLine(1);", "console.log(1);", "", "1")]
	[InlineData("Console.Write(1);", "console.log(1);", "", "1")]
	public void Test_ConsoleType(string cs, string expectedJS, string executeStr, string expectedResult)
	{
		FileData file = new()
		{
			SourceStr = cs
		};

		file = CSTOJS.Translate(file);

		Assert.Equal(expectedJS, file.TranslatedStr);

		_JintEngine.Execute(Common.PrepareStringForExecution(file.TranslatedStr, executeStr));

		Assert.Equal(expectedResult, Common.ConsoleStr);
	}


	[Theory]
	//sbyte
	[InlineData(@"var a = sbyte.MinValue;", @"let a = Number.parseInt(""-128"");", "a", "-128")]
	[InlineData(@"var a = sbyte.MaxValue;", @"let a = Number.parseInt(""127"");", "a", "127")]
	//byte
	[InlineData(@"var a = byte.MinValue;", @"let a = Number.parseInt(""0"");", "a", "0")]
	[InlineData(@"var a = byte.MaxValue;", @"let a = Number.parseInt(""255"");", "a", "255")]
	//short
	[InlineData(@"var a = short.MinValue;", @"let a = Number.parseInt(""-32768"");", "a", "-32768")]
	[InlineData(@"var a = short.MaxValue;", @"let a = Number.parseInt(""32767"");", "a", "32767")]
	//ushort
	[InlineData(@"var a = ushort.MinValue;", @"let a = Number.parseInt(""0"");", "a", "0")]
	[InlineData(@"var a = ushort.MaxValue;", @"let a = Number.parseInt(""65535"");", "a", "65535")]
	//int
	[InlineData(@"var a = int.MinValue;", @"let a = Number.parseInt(""-2147483648"");", "a", "-2147483648")]
	[InlineData(@"var a = int.MaxValue;", @"let a = Number.parseInt(""2147483647"");", "a", "2147483647")]
	//uint
	[InlineData(@"var a = uint.MinValue;", @"let a = Number.parseInt(""0"");", "a", "0")]
	[InlineData(@"var a = uint.MaxValue;", @"let a = Number.parseInt(""4294967295"");", "a", "4294967295")]
	public void Test_NumberTypes(string cs, string expectedJS, string executeStr, string expectedResult)
	{
		FileData file = new()
		{
			SourceStr = cs
		};

		file = CSTOJS.Translate(file);

		Assert.Equal(expectedJS, file.TranslatedStr);

		_JintEngine.Execute(Common.PrepareStringForExecution(file.TranslatedStr, executeStr));

		Assert.Equal(expectedResult, Common.ConsoleStr);
	}
	[Fact]
	public void Test_StringType()
	{
		FileData file = new()
		{
			SourceStr = @"namespace n;
class C
{
	public static string P {get;set;} = string.Empty;
	public string _F = string.Empty;
	public C()
	{
		var l = string.Empty;
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"class C
{
	static #_P_= String.raw``;
	static get P(){return this.#_P_;}
	static set P(value){this.#_P_ = value;}
	_F = String.raw``;
	constructor()
	{
		let l = String.raw``;
	}
}", file.TranslatedStr);

		_JintEngine.Execute(Common.PrepareStringForExecution(file.TranslatedStr, "new C()"));

		Assert.Equal("System.Dynamic.ExpandoObject", Common.ConsoleStr);
	}
}