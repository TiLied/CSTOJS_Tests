using System;
using System.Globalization;
using CSharpToJavaScript;
using Jint;

namespace CSTOJS_Tests;

public class UnitTestCSTutorial
{
	private readonly Engine _JintEngine = new(cfg => cfg.Culture(CultureInfo.InvariantCulture));
	
	private readonly CSTOJSOptions _DefaultUnitOpt = new()
	{
		AddSBAtTheTop = new("let console = { log: log };")
	};
	public UnitTestCSTutorial()
	{
		_JintEngine.SetValue("log", new Action<object>(Common.ConsoleOutput));
	}
	[Fact]
	public void FirstProgram()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"Console.WriteLine(""Hello, World!"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("Hello, World!", Common.ConsoleStr);
	}
	[Fact]
	public void DeclareAndUseVariables()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string aFriend = ""Bill"";
Console.WriteLine(aFriend);"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("Bill", Common.ConsoleStr);
	}
	[Fact]
	public void DeclareAndUseVariables2()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string aFriend = ""Bill"";
aFriend = ""Maira"";
Console.WriteLine(aFriend);"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("Maira", Common.ConsoleStr);
	}
	[Fact]
	public void DeclareAndUseVariables3()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string aFriend = ""Bill"";
aFriend = ""Maira"";
Console.WriteLine(""Hello "" + aFriend);"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("Hello Maira", Common.ConsoleStr);
	}
	[Fact]
	public void DeclareAndUseVariables4()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string aFriend = ""Bill"";
aFriend = ""Maira"";
Console.WriteLine($""Hello {aFriend}"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("Hello Maira", Common.ConsoleStr);
	}
	[Fact]
	public void WorkWithStrings()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string firstFriend = ""Maria"";
string secondFriend = ""Sage"";
Console.WriteLine($""My friends are {firstFriend} and {secondFriend}"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("My friends are Maria and Sage", Common.ConsoleStr);
	}
	[Fact]
	public void WorkWithStrings2()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string firstFriend = ""Maria"";
string secondFriend = ""Sage"";
Console.WriteLine($""The name {firstFriend} has {firstFriend.Length} letters."");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("The name Maria has 5 letters.", Common.ConsoleStr);
	}
	[Fact]
	public void WorkWithStrings3()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string firstFriend = ""Maria"";
string secondFriend = ""Sage"";
Console.WriteLine($""The name {secondFriend} has {secondFriend.Length} letters."");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("The name Sage has 4 letters.", Common.ConsoleStr);
	}
	[Fact]
	public void Trim()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string greeting = ""      Hello World!       "";
Console.WriteLine($""[{greeting}]"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("[      Hello World!       ]", Common.ConsoleStr);
	}
	[Fact]
	public void Trim2()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string greeting = ""      Hello World!       "";
string trimmedGreeting = greeting.TrimStart();
Console.WriteLine($""[{trimmedGreeting}]"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("[Hello World!       ]", Common.ConsoleStr);
	}
	[Fact]
	public void Trim3()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string greeting = ""      Hello World!       "";
string trimmedGreeting = greeting.TrimEnd();
Console.WriteLine($""[{trimmedGreeting}]"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("[      Hello World!]", Common.ConsoleStr);
	}
	[Fact]
	public void Trim4()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string greeting = ""      Hello World!       "";
string trimmedGreeting = greeting.Trim();
Console.WriteLine($""[{trimmedGreeting}]"");"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("[Hello World!]", Common.ConsoleStr);
	}
	[Fact]
	public void Replace()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string sayHello = ""Hello World!"";
Console.WriteLine(sayHello);
sayHello = sayHello.Replace(""Hello"", ""Greetings"");
Console.WriteLine(sayHello);"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("Greetings World!", Common.ConsoleStr);
	}
	[Fact]
	public void Replace2ToUpper()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string sayHello = ""Hello World!"";
Console.WriteLine(sayHello);
sayHello = sayHello.Replace(""Hello"", ""Greetings"");
Console.WriteLine(sayHello);
Console.WriteLine(sayHello.ToUpper());"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("GREETINGS WORLD!", Common.ConsoleStr);
	}
	[Fact]
	public void Replace3ToLower()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string sayHello = ""Hello World!"";
Console.WriteLine(sayHello);
sayHello = sayHello.Replace(""Hello"", ""Greetings"");
Console.WriteLine(sayHello);
Console.WriteLine(sayHello.ToLower());"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("greetings world!", Common.ConsoleStr);
	}
	[Fact]
	public void SearchStrings()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"string songLyrics = ""You say goodbye, and I say hello"";
Console.WriteLine(songLyrics.Contains(""goodbye""));
Console.WriteLine(songLyrics.Contains(""greetings""));"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_JintEngine.Execute(files[0].TranslatedStr);
		Assert.Equal("False", Common.ConsoleStr);
	}
}

