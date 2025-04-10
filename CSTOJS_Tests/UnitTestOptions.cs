using CSharpToJavaScript;
using System;
using System.Runtime.InteropServices;
using System.Text;


namespace CSTOJS_Tests
{

	public class UnitTestOptions
    {
		private readonly CSTOJS _CSTOJS = new();

		public UnitTestOptions()
		{
			_CSTOJS = new CSTOJS();
		}

		[Theory]
		[InlineData("true", "var i = 5;")]
		[InlineData("false", "let i = 5;")]
		public void TestUseVarOverLet(string value, string expected) 
		{
			CSTOJSOptions options = new()
			{
				UseVarOverLet = bool.Parse(value)
			};

			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"int i = 5;", options);

			Assert.Equal(expected, sb.ToString());
		}

		[Theory]
		[InlineData("true", @"function TestMethod() {

}")]
		[InlineData("false", @"function TestMethod()
{

}")]
		public void TestKeepBraceOnTheSameLine(string value, string expected)
		{
			CSTOJSOptions options = new()
			{
				KeepBraceOnTheSameLine = bool.Parse(value)
			};

			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"int TestMethod()
{

}", options);

			Assert.Equal(expected, sb.ToString());
		}
		[Theory]
		[InlineData("true", @"function TestMethod()
{
}")]
		[InlineData("false", @"function TestMethod()
{

}")]
		public void TestNormalizeWhitespace(string value, string expected)
		{
			if (bool.Parse(value) == true)
			{
				Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
				Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");
			}
			CSTOJSOptions options = new()
			{
				NormalizeWhitespace = bool.Parse(value)
			};

			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"int TestMethod()
{

}", options);

			Assert.Equal(expected, sb.ToString());
		}

		[Theory]
		[InlineData("true", @"function TestMethod()
{
	if(5 === 10)
	{
			
	}
}")]
		[InlineData("false", @"function TestMethod()
{
	if(5 == 10)
	{
			
	}
}")]
		public void TestUseStrictEquality(string value, string expected)
		{
			CSTOJSOptions options = new()
			{
				UseStrictEquality = bool.Parse(value)
			};

			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"int TestMethod()
{
	if(5 == 10)
	{
			
	}
}", options);

			Assert.Equal(expected, sb.ToString());
		}

		
		[Fact]
		public void TestCustomCSNamesToJS()
		{
			CSTOJSOptions options = new()
			{
				CustomCSNamesToJS = [ new("Asd", "console"), new("Dsa", "log") ]
			};

			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"Asd.Dsa();", options);

			Assert.Equal("console.log();", sb.ToString());
		}
	}
}
