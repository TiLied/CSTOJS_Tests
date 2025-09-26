using CSharpToJavaScript;
using Jint;
using System;

namespace CSTOJS_Tests
{
	public class UnitTestCSTutorial
    {
		private readonly Engine _Engine = new();
		private string _ConsoleStr = string.Empty;
		private readonly CSTOJSOptions _DefaultUnitOpt = new()
		{
			AddSBAtTheTop = new("let console = { log: log };")
		};

		public UnitTestCSTutorial()
		{
			_Engine.SetValue("log", new Action<object>(ConsoleOutPut));
		}

		[Fact]
		public void FirstProgram()
		{
			FileData file = new()
			{
				OptionsForFile = _DefaultUnitOpt,
				SourceStr = @"Console.WriteLine(""Hello, World!"");"
			};
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("Hello, World!", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("Bill", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("Maira", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("Hello Maira", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("Hello Maira", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("My friends are Maria and Sage", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("The name Maria has 5 letters.", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("The name Sage has 4 letters.", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("[      Hello World!       ]", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("[Hello World!       ]", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("[      Hello World!]", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("[Hello World!]", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("Greetings World!", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("GREETINGS WORLD!", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("greetings world!", _ConsoleStr);
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
			FileData[] files = CSTOJS.Translate([ file ]);

			_Engine.Execute(files[0].TranslatedStr);
			Assert.Equal("False", _ConsoleStr);
		}
		private void ConsoleOutPut(object obj)
		{
			_ConsoleStr = obj.ToString() ?? "null";
		}

	}
}
