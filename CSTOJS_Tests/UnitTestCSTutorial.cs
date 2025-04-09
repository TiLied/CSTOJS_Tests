using CSharpToJavaScript;
using Jint;
using System;
using System.Text;

namespace CSTOJS_Tests
{
	public class UnitTestCSTutorial
    {
		private readonly Engine _Engine = new();
		private string _ConsoleStr = string.Empty;
		private CSTOJS _CSTOJS = new();
		private readonly CSTOJSOptions _DefaultUnitOpt = new()
		{
			AddSBAtTheTop = new("let console = { log: log };")
		};

		public UnitTestCSTutorial()
		{
			_Engine.SetValue("log", new Action<object>(ConsoleOutPut));

			_CSTOJS = new CSTOJS();
		}

		[Fact]
		public void FirstProgram()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"Console.WriteLine(""Hello, World!"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("Hello, World!", _ConsoleStr);
		}
		[Fact]
		public void DeclareAndUseVariables()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string aFriend = ""Bill"";
Console.WriteLine(aFriend);", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("Bill", _ConsoleStr);
		}
		[Fact]
		public void DeclareAndUseVariables2()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string aFriend = ""Bill"";
aFriend = ""Maira"";
Console.WriteLine(aFriend);", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("Maira", _ConsoleStr);
		}
		[Fact]
		public void DeclareAndUseVariables3()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string aFriend = ""Bill"";
aFriend = ""Maira"";
Console.WriteLine(""Hello "" + aFriend);", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("Hello Maira", _ConsoleStr);
		}
		[Fact]
		public void DeclareAndUseVariables4()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string aFriend = ""Bill"";
aFriend = ""Maira"";
Console.WriteLine($""Hello {aFriend}"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("Hello Maira", _ConsoleStr);
		}
		[Fact]
		public void WorkWithStrings()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string firstFriend = ""Maria"";
string secondFriend = ""Sage"";
Console.WriteLine($""My friends are {firstFriend} and {secondFriend}"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("My friends are Maria and Sage", _ConsoleStr);
		}
		[Fact]
		public void WorkWithStrings2()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string firstFriend = ""Maria"";
string secondFriend = ""Sage"";
Console.WriteLine($""The name {firstFriend} has {firstFriend.Length} letters."");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("The name Maria has 5 letters.", _ConsoleStr);
		}
		[Fact]
		public void WorkWithStrings3()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string firstFriend = ""Maria"";
string secondFriend = ""Sage"";
Console.WriteLine($""The name {secondFriend} has {secondFriend.Length} letters."");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("The name Sage has 4 letters.", _ConsoleStr);
		}
		[Fact]
		public void Trim()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string greeting = ""      Hello World!       "";
Console.WriteLine($""[{greeting}]"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("[      Hello World!       ]", _ConsoleStr);
		}
		[Fact]
		public void Trim2()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string greeting = ""      Hello World!       "";
string trimmedGreeting = greeting.TrimStart();
Console.WriteLine($""[{trimmedGreeting}]"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("[Hello World!       ]", _ConsoleStr);
		}
		[Fact]
		public void Trim3()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string greeting = ""      Hello World!       "";
trimmedGreeting = greeting.TrimEnd();
Console.WriteLine($""[{trimmedGreeting}]"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("[      Hello World!]", _ConsoleStr);
		}
		[Fact]
		public void Trim4()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string greeting = ""      Hello World!       "";
trimmedGreeting = greeting.Trim();
Console.WriteLine($""[{trimmedGreeting}]"");", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("[Hello World!]", _ConsoleStr);
		}
		[Fact]
		public void Replace()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string sayHello = ""Hello World!"";
Console.WriteLine(sayHello);
sayHello = sayHello.Replace(""Hello"", ""Greetings"");
Console.WriteLine(sayHello);", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("Greetings World!", _ConsoleStr);
		}
		[Fact]
		public void Replace2ToUpper()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string sayHello = ""Hello World!"";
Console.WriteLine(sayHello);
sayHello = sayHello.Replace(""Hello"", ""Greetings"");
Console.WriteLine(sayHello);
Console.WriteLine(sayHello.ToUpper());", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("GREETINGS WORLD!", _ConsoleStr);
		}
		[Fact]
		public void Replace3ToLower()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string sayHello = ""Hello World!"";
Console.WriteLine(sayHello);
sayHello = sayHello.Replace(""Hello"", ""Greetings"");
Console.WriteLine(sayHello);
Console.WriteLine(sayHello.ToLower());", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("greetings world!", _ConsoleStr);
		}
		[Fact]
		public void SearchStrings()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"string songLyrics = ""You say goodbye, and I say hello"";
Console.WriteLine(songLyrics.Contains(""goodbye""));
Console.WriteLine(songLyrics.Contains(""greetings""));", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("False", _ConsoleStr);
		}
		private void ConsoleOutPut(object obj)
		{
			_ConsoleStr = obj.ToString() ?? "null";
		}

	}
}
