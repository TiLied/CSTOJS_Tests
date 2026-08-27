using CSharpToJavaScript;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace CSTOJS_Tests;


public class UnitTestOptions
{
	public UnitTestOptions()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
	}
	[Theory]
	[InlineData("true", "var i = 5;")]
	[InlineData("false", "let i = 5;")]
	public void TestUseVarOverLet(string value, string expected)
	{
		FileData file = new()
		{
			OptionsForFile = new()
			{
				UseVarOverLet = bool.Parse(value)
			},
			SourceStr = @"int i = 5;"
		};

		FileData[] files = CSTOJS.Translate([file]);

		Assert.Equal(expected, files[0].TranslatedStr);
	}

	[Theory]
	[InlineData("true", @"function TestMethod() {
	return 0;
}")]
	[InlineData("false", @"function TestMethod()
{
	return 0;
}")]
	public void TestKeepBraceOnTheSameLine(string value, string expected)
	{
		FileData file = new()
		{
			OptionsForFile = new()
			{
				KeepBraceOnTheSameLine = bool.Parse(value)
			},
			SourceStr = @"int TestMethod()
{
	return 0;
}"
		};

		FileData[] files = CSTOJS.Translate([file]);

		Assert.Equal(expected, files[0].TranslatedStr);
	}
	[Theory]
	[InlineData("true", @"function TestMethod()
{
    return 0;
}")]
	[InlineData("false", @"function TestMethod()
{
return 0;

}")]
	public void TestNormalizeWhitespace(string value, string expected)
	{
		if (bool.Parse(value) == true)
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");
		}
		FileData file = new()
		{
			OptionsForFile = new()
			{
				NormalizeWhitespace = bool.Parse(value)
			},
			SourceStr = @"int TestMethod()
{
return 0;

}"
		};

		FileData[] files = CSTOJS.Translate([file]);

		Assert.Equal(expected, files[0].TranslatedStr);
	}

	[Fact]
	public void TestCustomCSNamesToJS()
	{
		FileData file = new()
		{
			OptionsForFile = new()
			{
				CustomCSNamesToJS = new()
				{
					["Console"] = "console",
					["Beep"] = "log"
				}
			},
			SourceStr = @"Console.Beep();"
		};

		FileData[] files = CSTOJS.Translate([file]);

		Assert.Equal("console.log();", files[0].TranslatedStr);
	}

	//TODO! more tests!
	//TODO! better formating!
	[Fact]
	public void Test_MakePropertiesEnumerable()
	{
		FileData file = new()
		{
			OptionsForFile = new()
			{
				MakePropertiesEnumerable = true
			},
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;

namespace Test_MakePropertiesEnumerable;

public class TestClass
{
	public int Prop { get; set; } = 5;
	public TestClass()
	{
		Console.WriteLine(Prop);
	}
}"
		};

		FileData[] files = CSTOJS.Translate([file]);

		Assert.Equal(@"
class TestClass
{
	#_Prop_= 5;
	constructor()
	{
Object.defineProperty(this, 'Prop', { enumerable: true, get: function() { return this.#_Prop_; }, set: function (value) { this.#_Prop_ = value; } });
		console.log(this.Prop);
	}
}", files[0].TranslatedStr);
	}

	//TODO! more!
	[Fact]
	public void Test_EnableModules()
	{
		FileData file = new()
		{
			FileName = "f1.js",
			OptionsForFile = new()
			{
				EnableModules = 2
			},
			SourceStr = @"namespace Test_EnableModules;
public class C{}"
		};
		FileData file2 = new()
		{
			FileName = "f2.js",
			OptionsForFile = new()
			{
				EnableModules = 2
			},
			SourceStr = @"namespace Test_EnableModules;
public class C2{ public C2(){ var l = new C(); } }"
		};

		FileData[] files = CSTOJS.Translate([file, file2]);

		Assert.Equal(@"class C{}
export { C };", files[0].TranslatedStr);
		Assert.Equal(@"import { C } from './f1.js';
class C2{ constructor(){ let l = new C(); } }
export { C2 };", files[1].TranslatedStr);
	}
	
	[Theory]
	[InlineData(false, @"let a =""push"";")]
	[InlineData(true, @"let a =""Add"";")]
	public void Test_DisableNameOfProcessing(bool opt, string expected)
	{
		FileData file = new()
		{
			SourceStr = @"var a =nameof(System.Collections.Generic.List<int>.Add);",
			OptionsForFile = new(){ DisableNameOfProcessing = opt }
		};
		
		FileData[] files = CSTOJS.Translate([file]);
		
		Assert.Equal(expected, files[0].TranslatedStr);
	}
	[Theory]
	[InlineData(false, @"console.log("""");")]
	[InlineData(true, @"")]
	public void Test_DisableTranslation(bool opt, string expected)
	{
		FileData file = new()
		{
			SourceStr = @"Console.WriteLine("""");",
			OptionsForFile = new(){ DisableTranslation = opt }
		};
		
		FileData[] files = CSTOJS.Translate([file]);
		
		Assert.Equal(expected, files[0].TranslatedStr);
	}
}

