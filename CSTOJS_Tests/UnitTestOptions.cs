using CSharpToJavaScript;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace CSTOJS_Tests
{

	public class UnitTestOptions
    {
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
			
			FileData[] files = CSTOJS.Translate([ file ]);

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

			FileData[] files = CSTOJS.Translate([ file ]);

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

			FileData[] files = CSTOJS.Translate([ file ]);

			Assert.Equal(expected, files[0].TranslatedStr);
		}

		[Theory]
		[InlineData("true", @"function TestMethod()
{
	if(5 === 10)
	{
			
	}
	return 0;
}")]
		[InlineData("false", @"function TestMethod()
{
	if(5 == 10)
	{
			
	}
	return 0;
}")]
		public void TestUseStrictEquality(string value, string expected)
		{
			FileData file = new()
			{
				OptionsForFile = new()
				{
					UseStrictEquality = bool.Parse(value)
				},
				SourceStr = @"int TestMethod()
{
	if(5 == 10)
	{
			
	}
	return 0;
}"
			};

			FileData[] files = CSTOJS.Translate([ file ]);

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

			FileData[] files = CSTOJS.Translate([ file ]);

			Assert.Equal("console.log();", files[0].TranslatedStr);
		}
	}
}
