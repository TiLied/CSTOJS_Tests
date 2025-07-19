
using CSharpToJavaScript;
using CSharpToJavaScript.APIs.JS.Ecma;
using Jint;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CSTOJS_Tests;

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

	public static TheoryDataRow<TestData>[] TestData_Data =
	[
		new (new("bool", "true", "True")),
			new (new("bool", "false", "False" )),

			new (new("char", "'c'", "c")),

			new (new("object", "new()", "System.Dynamic.ExpandoObject" )
			{
				SkipMethods = new()
				{
					[ nameof(TestDefaultParameterInMethod)] = "error CS1736: Default parameter value for 'value' must be a compile-time constant"
				}
			}),
			new (new("object", "new object()", "System.Dynamic.ExpandoObject" )
			{
				SkipMethods = new()
				{
					[ nameof(TestDefaultParameterInMethod)] = "error CS1736: Default parameter value for 'value' must be a compile-time constant"
				}
			}),

			new (new("string", "\"str\"", "str" )),
			new (new("string?", "null", "null" )),

			new (new("dynamic", "0", string.Empty )
			{
				SkipMethods = new()
				{
					[ nameof(TestDefaultParameterInMethod)] = "error CS1763: 'value' is of type 'dynamic'. A default parameter value of a reference type other than string can only be initialized with null"
				}
			}),

			new (new("CustomClass", "new()", "System.Dynamic.ExpandoObject")
			{
				SkipMethods = new()
				{
					[ nameof(TestDefaultParameterInMethod)] = "error CS1736: Default parameter value for 'value' must be a compile-time constant"
				}
			}),
			new (new("CustomClass", "new CustomClass()", "System.Dynamic.ExpandoObject" )
			{
				SkipMethods = new()
				{
					[ nameof(TestDefaultParameterInMethod)] = "error CS1736: Default parameter value for 'value' must be a compile-time constant"
				}
			}),
			new (new("CustomClass?", "null", "null" )),

			new (new("Boolean", "new Boolean(true)", "True" )
			{
				SkipMethods = new()
				{
					[ nameof(TestDefaultParameterInMethod)] = "error CS1736: Default parameter value for 'value' must be a compile-time constant"
				}
			})
	];
	public static TheoryDataRow<TestData>[] TestData_Numbers =
	[
		new (new("byte", byte.MinValue.ToString(), string.Empty )),
			new (new("sbyte", sbyte.MinValue.ToString() , string.Empty)),
			new (new("decimal", decimal.MinValue.ToString(), Number.MIN_SAFE_INTEGER.ToString())
			{
				SkipMethods = new()
				{ 
					//Todo?
					[ nameof(TestFieldsDefaultValue) ] = "error CS1021: Integral constant is too large",
					[ nameof(TestDefaultParameterInMethod) ] = "error CS1021: Integral constant is too large",
					[ nameof(TestLocalValue) ] = "error CS1021: Integral constant is too large",
					[ nameof(TestPassValueToMethod) ] = "error CS1021: Integral constant is too large",
					[ nameof(TestPropertiesDefaultValue) ] = "error CS1021: Integral constant is too large"
				}
			}),
			new (new("double", "-1.7976931348623157E+308", Number.MIN_SAFE_INTEGER.ToString())),
			new (new("float", "-3.4028235E+38f", Number.MIN_SAFE_INTEGER.ToString())),
			new (new("int", int.MinValue.ToString(), string.Empty)),
			new (new("uint", uint.MinValue.ToString(), string.Empty)),
			new (new("nint", nint.MinValue.ToString(), Number.MIN_SAFE_INTEGER.ToString() )
			{
				SkipMethods = new()
				{ 
					//Todo?
					[ nameof(TestFieldsDefaultValue) ] = "error CS0266: Cannot implicitly convert type 'long' to 'nint'. An explicit conversion exists (are you missing a cast?)",
					[ nameof(TestDefaultParameterInMethod)] = "error CS1750: A value of type 'long' cannot be used as a default parameter because there are no standard conversions to type 'nint'",
					[ nameof(TestLocalValue) ] = "error CS0266: Cannot implicitly convert type 'long' to 'nint'. An explicit conversion exists (are you missing a cast?)",
					[ nameof(TestPassValueToMethod) ] = "error CS1503: Argument 1: cannot convert from 'long' to 'nint'",
					[ nameof(TestPropertiesDefaultValue) ] = "error CS0266: Cannot implicitly convert type 'long' to 'nint'. An explicit conversion exists (are you missing a cast?)",

				}
			}),
			new (new("nuint", nuint.MinValue.ToString(), string.Empty)),
			new (new("long", long.MinValue.ToString(), Number.MIN_SAFE_INTEGER.ToString())),
			new (new("ulong", ulong.MinValue.ToString(), string.Empty)),
			new (new("short", short.MinValue.ToString() , string.Empty)),
			new (new("ushort", ushort.MinValue.ToString(), string.Empty )),

			new (new("byte", byte.MaxValue.ToString() , string.Empty)),
			new (new("sbyte", sbyte.MaxValue.ToString(), string.Empty)),
			new (new("decimal", decimal.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString())
			{
				SkipMethods = new()
				{
					//Todo?
					[ nameof(TestFieldsDefaultValue)] = "error CS1021: Integral constant is too large",
					[ nameof(TestDefaultParameterInMethod)] = "error CS1021: Integral constant is too large",
					[ nameof(TestLocalValue) ] = "error CS1021: Integral constant is too large",
					[ nameof(TestPassValueToMethod) ] = "error CS1021: Integral constant is too large",
					[ nameof(TestPropertiesDefaultValue) ] = "error CS1021: Integral constant is too large"
				}
			}),
			new (new("double", "1.7976931348623157E+308", Number.MAX_SAFE_INTEGER.ToString())),
			new (new("float", "3.4028235E+38f", Number.MAX_SAFE_INTEGER.ToString() )),
			new (new("int", int.MaxValue.ToString(), string.Empty)),
			new (new("uint", uint.MaxValue.ToString() , string.Empty)),
			new (new("nint", nint.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString())
			{
				SkipMethods = new()
				{
					//Todo?
					[ nameof(TestFieldsDefaultValue)] = "error CS0266: Cannot implicitly convert type 'long' to 'nint'. An explicit conversion exists (are you missing a cast?)",
					[ nameof(TestDefaultParameterInMethod)] = "error CS1750: A value of type 'long' cannot be used as a default parameter because there are no standard conversions to type 'nint'",
					[ nameof(TestLocalValue) ] = "error CS0266: Cannot implicitly convert type 'long' to 'nint'. An explicit conversion exists (are you missing a cast?)",
					[ nameof(TestPassValueToMethod) ] = "error CS1503: Argument 1: cannot convert from 'long' to 'nint'",
					[ nameof(TestPropertiesDefaultValue) ] = "error CS0266: Cannot implicitly convert type 'long' to 'nint'. An explicit conversion exists (are you missing a cast?)",
				}
			}),
			new (new("nuint", nuint.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString())
			{
				SkipMethods = new()
				{
					//Todo? Expected fail? see TestDefaultParameterInMethod
					[ nameof(TestFieldsDefaultValue)] = "error CS0266: Cannot implicitly convert type 'ulong' to 'nuint'. An explicit conversion exists (are you missing a cast?)",
					[ nameof(TestDefaultParameterInMethod)] = "error CS1750: A value of type 'ulong' cannot be used as a default parameter because there are no standard conversions to type 'nuint'",
					[ nameof(TestLocalValue)] = "error CS0266: Cannot implicitly convert type 'ulong' to 'nuint'. An explicit conversion exists (are you missing a cast?)",
					[ nameof(TestPassValueToMethod) ] = "error CS1503: Argument 1: cannot convert from 'ulong' to 'nuint'",
					[ nameof(TestPropertiesDefaultValue)] = "error CS0266: Cannot implicitly convert type 'ulong' to 'nuint'. An explicit conversion exists (are you missing a cast?)",
				}
			}),
			new (new("long", long.MaxValue.ToString(), Number.MAX_SAFE_INTEGER.ToString())),
			new (new("ulong", ulong.MaxValue.ToString() , Number.MAX_SAFE_INTEGER.ToString())),
			new (new("short", short.MaxValue.ToString() , string.Empty)),
			new (new("ushort", ushort.MaxValue.ToString() , string.Empty))
	];
	public static TheoryDataRow<TestData>[] TestData_VariousNumbers =
	[
		new(new("decimal", "3_000.5m", "3000.5")),
			new(new("decimal", "400.75M", "400.75")),

			new(new("double", "3D", "3")),
			new(new("double", "4d", "4")),
			new(new("double", "3.934_001", "3.934001")),
			new(new("double", "3.141592653589793", "3.141592653589793")),

			new(new("float", "3_000.5F", "3000.5")),
			new(new("float", "5.4f", "5.4"))
	];

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
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;

using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;

namespace CSTOJS_Test.CSharp
{
	public class UnitTest
	{
		public UnitTest()
		{  
			int n1=0;
			int n2=1;
			int n3=0;
			int i=0;
			int number=15;

			for(i=2;i<number;++i) //loop starts from 2 because 0 and 1 are already printed    
			{    
				n3=n1+n2;    
				n1=n2;    
				n2=n3;    
			}
			Console.WriteLine($""{n3}"");
		}
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
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;

using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;

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
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
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
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{
	public class UnitTest
	{
		public UnitTest()
		{
			var i = GlobalThis.Date();
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
	[MemberData(nameof(TestData_Data))]
	[MemberData(nameof(TestData_Numbers))]
	[MemberData(nameof(TestData_VariousNumbers))]
	public void TestFieldsDefaultValue(TestData data)
	{
		string methodName = nameof(TestFieldsDefaultValue);

		if (data.SkipMethods.TryGetValue(methodName, out string? reason))
			Assert.SkipWhen(true, reason);

		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		private {data.CSType} _TestField = {data.CSValue};
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

		string strExpected = data.Expected == string.Empty ? data.CSValue : data.Expected;
		Assert.Equal(strExpected, _ConsoleStr);
	}

	[Theory]
	[MemberData(nameof(TestData_Data))]
	[MemberData(nameof(TestData_Numbers))]
	[MemberData(nameof(TestData_VariousNumbers))]
	public void TestPropertiesDefaultValue(TestData data)
	{
		string methodName = nameof(TestPropertiesDefaultValue);

		if (data.SkipMethods.TryGetValue(methodName, out string? reason))
			Assert.SkipWhen(true, reason);

		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public {data.CSType} TestProperty {{get; set;}} = {data.CSValue};
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

		string strExpected = data.Expected == string.Empty ? data.CSValue : data.Expected;
		Assert.Equal(strExpected, _ConsoleStr);
	}

	[Theory]
	[MemberData(nameof(TestData_Data))]
	[MemberData(nameof(TestData_Numbers))]
	[MemberData(nameof(TestData_VariousNumbers))]
	public void TestLocalValue(TestData data)
	{
		string methodName = nameof(TestLocalValue);

		if (data.SkipMethods.TryGetValue(methodName, out string? reason))
			Assert.SkipWhen(true, reason);

		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			{data.CSType} local = {data.CSValue};
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

		string strExpected = data.Expected == string.Empty ? data.CSValue : data.Expected;
		Assert.Equal(strExpected, _ConsoleStr);
	}

	[Theory]
	[MemberData(nameof(TestData_Data))]
	[MemberData(nameof(TestData_Numbers))]
	[MemberData(nameof(TestData_VariousNumbers))]
	public void TestPassValueToMethod(TestData data)
	{
		string methodName = nameof(TestPassValueToMethod);

		if (data.SkipMethods.TryGetValue(methodName, out string? reason))
			Assert.SkipWhen(true, reason);

		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			UnitMethod({data.CSValue});
		}}
		public void UnitMethod({data.CSType} value)
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

		string strExpected = data.Expected == string.Empty ? data.CSValue : data.Expected;
		Assert.Equal(strExpected, _ConsoleStr);
	}

	[Theory]
	[MemberData(nameof(TestData_Data))]
	[MemberData(nameof(TestData_Numbers))]
	[MemberData(nameof(TestData_VariousNumbers))]
	public void TestDefaultParameterInMethod(TestData data)
	{
		string methodName = nameof(TestDefaultParameterInMethod);

		if (data.SkipMethods.TryGetValue(methodName, out string? reason))
			Assert.SkipWhen(true, reason);

		StringBuilder sb = _CSTOJS.GenerateOneFromString($@"
using CSharpToJavaScript;
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			UnitMethod();
		}}
		public void UnitMethod({data.CSType} value = {data.CSValue})
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

		string strExpected = data.Expected == string.Empty ? data.CSValue : data.Expected;
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
	//Does not exists in c#
	//[InlineData("(a &&= 3)", "3")]
	//[InlineData("(a ||= 3)", "5")]
	[InlineData("(a ??= 3)", "5")]

	//Comparison operators 
	[InlineData("a == a", "True")]
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
using Microsoft.CodeAnalysis;

using System;
using System.Threading.Tasks;
using Jint;

using System.Text;
using Boolean = CSharpToJavaScript.APIs.JS.Ecma.Boolean;
namespace CSTOJS_Test.CSharp
{{
	public class UnitTest
	{{
		public UnitTest()
		{{
			int? a = 5;
			int b = 3;
			int c = 2;
			Console.WriteLine({expression});
		}}
	}}
}}", _DefaultUnitOpt);

		_Engine.Execute(sb.ToString());

		Assert.Equal(expectedResult, _ConsoleStr);
	}


	private void ConsoleOutPut(object? obj)
	{
		_ConsoleStr = obj?.ToString() ?? "null";
	}

}

