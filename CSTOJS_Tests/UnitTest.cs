
using CSharpToJavaScript;
using CSharpToJavaScript.APIs.JS.Ecma;
using Jint;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CSTOJS_Tests
{
	public class UnitTest
	{
		private readonly Engine _Engine = new(cfg => cfg.Culture(CultureInfo.InvariantCulture));
		private string _ConsoleStr = string.Empty;
		private CSTOJS _CSTOJS = new();
		private readonly CSTOJSOptions _DefaultUnitOpt = new()
		{
			AddSBAtTheTop = new("let console = { log: log };"),
			AddSBAtTheBottom = new("let unitTest = new UnitTest();")
		};

		//0=CS Type Name
		//1=CS Value
		//2=JS(Jint) Value (expected str)
		public static TheoryData<string[]> TestData = new()
		{
			new string[] { "bool", "true", "True" },
			new string[] { "bool", "false", "False" },

			new string[] { "char", "'c'", "c"},

			new string[] { "object", "new()", "System.Dynamic.ExpandoObject" },
			new string[] { "object", "new object()", "System.Dynamic.ExpandoObject" },

			new string[] { "string", "\"str\"", "str" },

			new string[] { "dynamic", "0", string.Empty },

			new string[] { "CustomClass", "new()", "System.Dynamic.ExpandoObject" },
			new string[] { "CustomClass", "new CustomClass()", "System.Dynamic.ExpandoObject" },
		};
		
		public static TheoryData<string[]> TestVariousNumbersData = new()
		{
			new string[] { "decimal", "3_000.5m", "3000.5" },
			new string[] { "decimal", "400.75M", "400.75" },

			new string[] { "double", "3D", "3" },
			new string[] { "double", "4d", "4" },
			new string[] { "double", "3.934_001", "3.934001" },
			new string[] { "double", "3.141592653589793", "3.141592653589793"},

			new string[] { "float", "3_000.5F", "3000.5" },
			new string[] { "float", "5.4f", "5.4" }
		};
		public static TheoryData<string[]> TestNumbersData = new()
		{
			new string[] { "byte", byte.MinValue.ToString(), string.Empty },
			new string[] { "sbyte", sbyte.MinValue.ToString() , string.Empty},
			new string[] { "decimal", decimal.MinValue.ToString(), Number.MIN_SAFE_INTEGER.ToString() },
			new string[] { "double", "-1.7976931348623157E+308", Number.MIN_SAFE_INTEGER.ToString() },
			new string[] { "float", "-3.4028235E+38", Number.MIN_SAFE_INTEGER.ToString() },
			new string[] { "int", int.MinValue.ToString(), string.Empty},
			new string[] { "uint", uint.MinValue.ToString(), string.Empty },
			new string[] { "nint", nint.MinValue.ToString(), Number.MIN_SAFE_INTEGER.ToString() },
			new string[] { "nuint", nuint.MinValue.ToString(), string.Empty },
			new string[] { "long", long.MinValue.ToString(), Number.MIN_SAFE_INTEGER.ToString() },
			new string[] { "ulong", ulong.MinValue.ToString(), string.Empty },
			new string[] { "short", short.MinValue.ToString() , string.Empty},
			new string[] { "ushort", ushort.MinValue.ToString(), string.Empty },

			new string[] { "byte", byte.MaxValue.ToString() , string.Empty},
			new string[] { "sbyte", sbyte.MaxValue.ToString(), string.Empty },
			new string[] { "decimal", decimal.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString()},
			new string[] { "double", "1.7976931348623157E+308", Number.MAX_SAFE_INTEGER.ToString() },
			new string[] { "float", "3.4028235E+38", Number.MAX_SAFE_INTEGER.ToString() },
			new string[] { "int", int.MaxValue.ToString(), string.Empty},
			new string[] { "uint", uint.MaxValue.ToString() , string.Empty},
			new string[] { "nint", nint.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString()},
			new string[] { "nuint", nuint.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString()},
			new string[] { "long", long.MaxValue.ToString(), Number.MAX_SAFE_INTEGER.ToString() },
			new string[] { "ulong", ulong.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString()},
			new string[] { "short", short.MaxValue.ToString() , string.Empty},
			new string[] { "ushort", ushort.MaxValue.ToString() , string.Empty},
		};

		public UnitTest() 
		{
			//https://stackoverflow.com/a/45117890
			CultureInfo info = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = info;
			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

			_Engine.SetValue("log", new Action<object>(ConsoleOutPut));

			_CSTOJS = new CSTOJS();
		}

		[Fact]
		public void Fibonacci()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{
	public class UnitTest
	{
	public UnitTest()
      {  
         int n1=0,n2=1,n3,i,number;    
         //Console.Write(""Enter the number of elements: "");    
         number = 15;  
         //Console.Write(n1+"" ""+n2+"" ""); //printing 0 and 1    
         for(i=2;i<number;++i) //loop starts from 2 because 0 and 1 are already printed    
         {    
          n3=n1+n2;    
          //Console.Write(n3+"" "");    
          n1=n2;    
          n2=n3;    
         }
Console.WriteLine($""{n3}"");
}
}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("377", _ConsoleStr);
		}

		[Fact]
		public void HelloWorld()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{
	public class UnitTest
	{
		public UnitTest()
		{
			Console.WriteLine(""HelloWorld!"");
		}
	}
}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("HelloWorld!", _ConsoleStr);
		}

		[Fact]
		public void HelloWorldField()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{
	public class UnitTest
	{
		private string _HW = ""HelloWorldField!"";
		public UnitTest()
		{
			Console.WriteLine(_HW);
		}
	}
}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("HelloWorldField!", _ConsoleStr);
		}

		[Fact]
		public void GlobalThisDate()
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString(@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{
	public class UnitTest
	{
		public UnitTest()
		{
			var i = globalThis.Date();
			Console.WriteLine(""GlobalThisDate"");
		}
	}
}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());
			Assert.Equal("GlobalThisDate", _ConsoleStr);
		}




		//
		//
		//Old tests! Should probably be deleted?
		[Fact]
		public void AnkiWebQuiz()
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");

			List<StringBuilder> lSB = _CSTOJS.GenerateOne("..\\..\\..\\CSharp\\AnkiWebQuiz.cs");

			_Engine.Execute(lSB[0].ToString());
			Assert.Equal("", _ConsoleStr);
		}
		[Fact]
		public void Test4()
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");

			List<StringBuilder> lSB = _CSTOJS.GenerateOne("..\\..\\..\\CSharp\\test4.cs");

			_Engine.Execute(lSB[0].ToString());
			Assert.Equal("", _ConsoleStr);
		}
		[Fact]
		public void Test6()
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");

			List<StringBuilder> lSB = _CSTOJS.GenerateOne("..\\..\\..\\CSharp\\test6.cs");

			_Engine.Execute(lSB[0].ToString());
			Assert.Equal("", _ConsoleStr);
		}

		[Fact]
		public void TestNBody()
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");

			List<StringBuilder> lSB = _CSTOJS.GenerateOne("..\\..\\..\\CSharp\\NBody.cs");

			_Engine.Execute(lSB[0].ToString());
			Assert.Equal("", _ConsoleStr);
		}

		[Fact]
		public void Test7()
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");

			List<StringBuilder> lSB = _CSTOJS.GenerateOne("..\\..\\..\\CSharp\\Test7.cs", _DefaultUnitOpt);

			_Engine.Execute(lSB[0].ToString());
			Assert.Equal("", _ConsoleStr);
		}

		[Fact]
		public void Test8()
		{
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "TODO!");
			Assert.SkipWhen(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), "TODO!");

			List<StringBuilder> lSB = _CSTOJS.GenerateOne("..\\..\\..\\CSharp\\Test8.cs", _DefaultUnitOpt);


			_Engine.Execute(lSB[0].ToString());
			Assert.Equal("Done!", _ConsoleStr);
		}
		//
		//
		//

		[Theory]
		[MemberData(nameof(TestData))]
		[MemberData(nameof(TestNumbersData))]
		[MemberData(nameof(TestVariousNumbersData))]
		public void TestFieldsDefaultValue(string[] data) 
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		private {data[0]} _TestField = {data[1]};
		public UnitTest()
		{{
			Console.WriteLine(_TestField);
		}}
	}}
	public class CustomClass
	{{
		public CustomClass()
		{{
		}}
	}}
}}", _DefaultUnitOpt);
			
			_Engine.Execute(sb.ToString());

			string strExpected = data[2] == string.Empty ? data[1] : data[2];
			Assert.Equal(strExpected, _ConsoleStr);
		}

		[Theory]
		[MemberData(nameof(TestData))]
		[MemberData(nameof(TestNumbersData))]
		[MemberData(nameof(TestVariousNumbersData))]
		public void TestPropertiesDefaultValue(string[] data)
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public {data[0]} TestProperty {{get; set;}} = {data[1]};
		public UnitTest()
		{{
			Console.WriteLine(TestProperty);
		}}
	}}
	public class CustomClass
	{{
		public CustomClass()
		{{
		}}
	}}
}}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());

			string strExpected = data[2] == string.Empty ? data[1] : data[2];
			Assert.Equal(strExpected, _ConsoleStr);
		}

		[Theory]
		[MemberData(nameof(TestData))]
		[MemberData(nameof(TestNumbersData))]
		[MemberData(nameof(TestVariousNumbersData))]
		public void TestLocalValue(string[] data)
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			{data[0]} local = {data[1]};
			Console.WriteLine(local);
		}}
	}}
	public class CustomClass
	{{
		public CustomClass()
		{{
		}}
	}}
}}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());

			string strExpected = data[2] == string.Empty ? data[1] : data[2];
			Assert.Equal(strExpected, _ConsoleStr);
		}
		[Theory]
		[MemberData(nameof(TestData))]
		[MemberData(nameof(TestNumbersData))]
		[MemberData(nameof(TestVariousNumbersData))]
		public void TestPassValueToMethod(string[] data)
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			UnitMethod({data[1]});
		}}
		public void UnitMethod({data[0]} value)
		{{
			Console.WriteLine(value);
		}}
	}}
	public class CustomClass
	{{
		public CustomClass()
		{{
		}}
	}}
}}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());

			string strExpected = data[2] == string.Empty ? data[1] : data[2];
			Assert.Equal(strExpected, _ConsoleStr);
		}
		[Theory]
		[MemberData(nameof(TestData))]
		[MemberData(nameof(TestNumbersData))]
		[MemberData(nameof(TestVariousNumbersData))]
		public void TestDefaultParameterInMethod(string[] data)
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			UnitMethod();
		}}
		public void UnitMethod({data[0]} value = {data[1]})
		{{
			Console.WriteLine(value);
		}}
	}}
	public class CustomClass
	{{
		public CustomClass()
		{{
		}}
	}}
}}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());

			string strExpected = data[2] == string.Empty ? data[1] : data[2];
			Assert.Equal(strExpected, _ConsoleStr);
		}



		//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Expressions_and_operators
		[Theory]
		//Assignment operators
		//See method body.
		[InlineData("a + 1", "6")]
		[InlineData("(a += 3)", "8")]
		[InlineData("(a -= 3)", "2")]
		[InlineData("(a *= 3)", "15")]
		[InlineData("(a /= 3)", "1.6666666666666667")]
		[InlineData("(a %= 3)", "2")]
		//c# does not have '**' ?
		[InlineData("(a <<= 3)", "40")]
		[InlineData("(a >>= 3)", "0")]
		[InlineData("(a >>>= 3)", "0")]
		[InlineData("(a &= 3)", "1")]
		[InlineData("(a ^= 3)", "6")]
		[InlineData("(a |= 3)", "7")]
		[InlineData("(a &&= 3)", "3")]
		[InlineData("(a ||= 3)", "5")]
		[InlineData("(a ??= 3)", "5")]

		//Comparison operators 
		[InlineData("a == a","True")]
		[InlineData("a != a", "False")]
		//'===' and '!==' Done in "UnitTestOptions".
		[InlineData("a > a", "False")]
		[InlineData("a >= a", "True")]
		[InlineData("a < a", "False")]
		[InlineData("a <= a", "True")]

		//Arithmetic operators
		[InlineData("12 % a", "2")]
		[InlineData("a++", "5")]
		[InlineData("++a", "6")]
		[InlineData("a--", "5")]
		[InlineData("--a", "4")]
		[InlineData("-a", "-5")]
		[InlineData("+a", "5")]
		//c# does not have '**' ?

		//Bitwise operators
		[InlineData("a & b", "1")]
		[InlineData("a | b", "7")]
		[InlineData("a ^ b", "6")]
		[InlineData("~a", "-6")]
		[InlineData("a << b", "40")]
		[InlineData("a >> b", "0")]
		[InlineData("a >>> b", "0")]

		//Logical operators
		[InlineData("a > 0 && b > 0", "True")]
		[InlineData("a > 0 || b > 0", "True")]
		[InlineData("null ?? \"default string\"", "default string")]
		[InlineData("!(a > 0 || b > 0)", "False")]

		//String operators
		[InlineData("\"my \" + \"string\"", "my string")]

		//Conditional (ternary) operator
		[InlineData("a >= 18 ? \"adult\" : \"minor\"", "minor")]

		//Grouping operator
		[InlineData("a + b * c", "11")]
		[InlineData("a + (b * c)", "11")]
		[InlineData("(a + b) * c", "16")]
		[InlineData("a * c + b * c", "16")]
		public void TestExpressionsAndOperators(string expression, string expectedResult)
		{
			StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			int a = 5;
			int b = 3;
			int c = 2;
			var x = {expression};
			Console.WriteLine(x);
		}}
	}}
}}", _DefaultUnitOpt);

			_Engine.Execute(sb.ToString());

			Assert.Equal(expectedResult, _ConsoleStr);
		}


		private void ConsoleOutPut(object obj)
		{
			_ConsoleStr = obj.ToString() ?? "null";
		}

	}
}
