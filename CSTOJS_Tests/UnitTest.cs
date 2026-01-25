using CSharpToJavaScript;
using CSharpToJavaScript.APIs.JS.Ecma;
using Jint;
using System;
using System.Globalization;
using System.Threading;

namespace CSTOJS_Tests;

//dotnet test --filter "Category!=test262&Category!=test262-parser"
public class UnitTest
{
	private readonly Engine _Engine = new(cfg => cfg.Culture(CultureInfo.InvariantCulture));

	private string _ConsoleStr = string.Empty;

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
					[ nameof(TestDefaultParameterInMethod)] = "error CS1763: 'value' is of type 'dynamic'. A default parameter value of a reference type other than string can only be initialized with null",
					//well..
					//Todo?
					[ nameof(TestPassValueToMethod)] = "error CS1980: Cannot define a class or member that utilizes 'dynamic' because the compiler required type 'System.Runtime.CompilerServices.DynamicAttribute' cannot be found. Are you missing a reference?",
					[ nameof(TestPropertiesDefaultValue)] = "error CS1980: Cannot define a class or member that utilizes 'dynamic' because the compiler required type 'System.Runtime.CompilerServices.DynamicAttribute' cannot be found. Are you missing a reference?",
					[ nameof(TestFieldsDefaultValue)] = "error CS1980: Cannot define a class or member that utilizes 'dynamic' because the compiler required type 'System.Runtime.CompilerServices.DynamicAttribute' cannot be found. Are you missing a reference?"
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
	}

	[Fact]
	public void Fibonacci()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"
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
}"
		};

		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);
		Assert.Equal("377", _ConsoleStr);
	}

	[Fact]
	public void HelloWorld()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"
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
}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);
		Assert.Equal("HelloWorld!", _ConsoleStr);
	}

	[Fact]
	public void HelloWorldField()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"
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
}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);
		Assert.Equal("HelloWorldField!", _ConsoleStr);
	}

	[Fact]
	public void GlobalThisDate()
	{
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = @"
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
}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);
		Assert.Equal("GlobalThisDate", _ConsoleStr);
	}

	[Theory]
	[MemberData(nameof(TestData_Data))]
	[MemberData(nameof(TestData_Numbers))]
	[MemberData(nameof(TestData_VariousNumbers))]
	public void TestFieldsDefaultValue(TestData data)
	{
		string methodName = nameof(TestFieldsDefaultValue);

		if (data.SkipMethods.TryGetValue(methodName, out string? reason))
			Assert.SkipWhen(true, reason);

		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"
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
}}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);

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

		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"
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
}}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);

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


		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"
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
}}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);

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

		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"
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
}}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);

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

		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"
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
}}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);

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
		FileData file = new()
		{
			OptionsForFile = _DefaultUnitOpt,
			SourceStr = $@"
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
}}"
		};
		FileData[] files = CSTOJS.Translate([file]);

		_Engine.Execute(files[0].TranslatedStr);
		Assert.Equal(expectedResult, _ConsoleStr);
	}
	[Fact]
	public void Test_ImplicitOperatorTo()
	{
		FileData file = new()
		{
			SourceStr = $@"using CSharpToJavaScript.Utils;
Unsupported unsupported = ""str"";"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal($@"let unsupported = ""str"";", file.TranslatedStr);
	}
	[Fact]
	public void Test_ImplicitOperatorFrom()
	{
		FileData file = new()
		{
			SourceStr = $@"using CSharpToJavaScript.Utils;
string str = new Unsupported();"
		};
		file = CSTOJS.Translate(file);

		//Test is only c# code!
		//there should not be errors!
		Assert.Equal($@"let str = new Unsupported();", file.TranslatedStr);
	}
	
	[Fact]
	public void Test_GlobalStatementFormating()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
GlobalThis.Window.Document.AddEventListener(""DOMContentLoaded"", (Event e) =>
{
	HTMLElement body = GlobalThis.Window.Document.Body;

	Element paragraph = GlobalThis.Window.Document.CreateElement(""p"");
	Text helloWorld = GlobalThis.Window.Document.CreateTextNode(""Hello, World!"");

	paragraph.AppendChild(helloWorld);

	(body as ParentNode).Append(paragraph);
	
	DeleteEntry(paragraph);
}, true);

void DeleteEntry(Element li)
{
	(li as ChildNode).Remove();
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"globalThis.window.document.addEventListener(""DOMContentLoaded"", (e) =>
{
	let body = globalThis.window.document.body;

	let paragraph = globalThis.window.document.createElement(""p"");
	let helloWorld = globalThis.window.document.createTextNode(""Hello, World!"");

	paragraph.appendChild(helloWorld);

	body.append(paragraph);
	
	DeleteEntry(paragraph);
}, true);

function DeleteEntry(li)
{
	li.remove();
}", file.TranslatedStr);
	}

	[Fact]
	public void Test_ClassFormating()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
namespace Test_ClassFormating;

public class Program
{
	public static void Main()
	{
		GlobalThis.Window.Document.AddEventListener(""DOMContentLoaded"", (Event e) =>
		{
			HTMLElement body = GlobalThis.Window.Document.Body;

			Element paragraph = GlobalThis.Window.Document.CreateElement(""p"");
			Text helloWorld = GlobalThis.Window.Document.CreateTextNode(""Hello, World!"");

			paragraph.AppendChild(helloWorld);

			(body as ParentNode).Append(paragraph);
			
			new Program().DeleteEntry(paragraph);
		}, true);
	}
	public void DeleteEntry(Element li)
	{
		(li as ChildNode).Remove();
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Program
{
	static Main()
	{
		globalThis.window.document.addEventListener(""DOMContentLoaded"", (e) =>
		{
			let body = globalThis.window.document.body;

			let paragraph = globalThis.window.document.createElement(""p"");
			let helloWorld = globalThis.window.document.createTextNode(""Hello, World!"");

			paragraph.appendChild(helloWorld);

			body.append(paragraph);
			
			new Program().DeleteEntry(paragraph);
		}, true);
	}
	DeleteEntry(li)
	{
		li.remove();
	}
}", file.TranslatedStr);
	}

	[Fact]
	public void Test_This()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.APIs.JS.Ecma;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
namespace Test_This;

public class Program
{
	private string _Time = new Date().ToISOString();
	public string Time
	{ 
		get
		{
			return _Time;
		} 
		set
		{
			_Time = value;
		}
	}
	public void Main()
	{
		Console.WriteLine(""Main"");

		var that = this;
		(GlobalThis.Window as WindowOrWorkerGlobalScope).SetInterval(() =>
		{
			that.Time = new Date().ToISOString();
		}, 1000);
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Program
{
	_Time = new Date().toISOString();
	get Time() 
		{
			return this._Time;
		} 
	set Time(value)
		{
			this._Time = value;
		}
	Main()
	{
		console.log(""Main"");

		let that = this;
		globalThis.window.setInterval(() =>
		{
			that.Time = new Date().toISOString();
		}, 1000);
	}
}", file.TranslatedStr);
	}

	[Fact]
	public void Test_ThisExplicit()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.APIs.JS.Ecma;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
namespace Test_ThisExplicit;

public class Program
{
	private string _Time = new Date().ToISOString();
	public void Main()
	{
		Console.WriteLine(this._Time);
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Program
{
	_Time = new Date().toISOString();
	Main()
	{
		console.log(this._Time);
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_StaticMethodCall()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.APIs.JS.Ecma;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
namespace Test_StaticMethodCall;

public static class Program
{
	public static void Main()
	{
		Console.WriteLine(Program.StatcicMethod());
	}
	public static int StatcicMethod()
	{
		return 1;
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Program
{
	static Main()
	{
		console.log(Program.StatcicMethod());
	}
	static StatcicMethod()
	{
		return 1;
	}
}", file.TranslatedStr);
	}

	//TODO! How?
	[Fact(Skip = "skip for now.")]
	public void Test_VirtualMethodCall()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.APIs.JS.Ecma;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
namespace Test_VirtualMethodCall;

public class Base
{
	public void Test(){}
}
public class Program : Base
{
	public void Main()
	{
		Test();
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Base
{
	Test(){}
}
class Program extends Base
{
	Main()
	{
		this.Test();
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_CallbackDelegate()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.APIs.JS.Ecma;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
namespace Test_CallbackDelegate;

public class Program
{
	public void Main()
	{
		MutationObserver observer = new MutationObserver(Callback);
	}
	public Undefined Callback(List<MutationRecord> mutations, MutationObserver observer)
	{
		return null;
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Program
{
	Main()
	{
		let observer = new MutationObserver(this.Callback);
	}
	Callback(mutations, observer)
	{
		return null;
	}
}", file.TranslatedStr);
	}

	[Fact]
	public void Test_ArrayCreationExpression()
	{
		FileData file = new()
		{
			SourceStr = @"int[] a = new int[]{1,2,3};"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"let a = new Array(1,2,3);", file.TranslatedStr);
	}

	[Fact]
	public void Test_SpecialSyntax()
	{
		FileData file = new()
		{
			SourceStr = @"//console.log(1);\\"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"console.log(1);", file.TranslatedStr);
	}
	private void ConsoleOutPut(object? obj)
	{
		_ConsoleStr = obj?.ToString() ?? "null";
	}

}

