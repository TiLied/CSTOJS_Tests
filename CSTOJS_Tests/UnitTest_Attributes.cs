using CSharpToJavaScript;

namespace CSTOJS_Tests.ECMA;

public class UnitTest_Attributes
{
	[Theory]
	[InlineData("Element e = (GlobalThis.Window.Document as ParentNode).QuerySelector(\"test\");")]
	[InlineData("HTMLElement e = (GlobalThis.Window.Document as ParentNode).QuerySelector<HTMLElement>(\"test\");")]
	public void Test_ToAttribute(string cs)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"let e = globalThis.window.document.querySelector(""test"");", file.TranslatedStr);
	}
	[Theory]
	[InlineData("Document d = GlobalThis.Window.Document;")]
	public void Test_ValueAttribute(string cs)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"let d = globalThis.window.document;", file.TranslatedStr);
	}

	[Theory]
	[InlineData(@"namespace Test_ToAttributeNone;
[To(ToAttribute.None)] public class Test{
	[Value(""Test1"")] 
	public static int TestField = 1;
	public Test() { Console.WriteLine(Test.TestField); }
}", @"class Test{
	static TestField = 1;
	constructor() { console.log(.Test1); }
}")]
	[InlineData(@"namespace Test_ToAttributeNone;
[To(ToAttribute.NoneWithTailingDotRemoved)] public class Test{
	[Value(""Test2"")] 
	public static int TestField = 1;
	public Test() { Console.WriteLine(Test.TestField); }
}", @"class Test{
	static TestField = 1;
	constructor() { console.log(Test2); }
}")]
	[InlineData(@"namespace Test_ToAttributeNone;
[Value(""Test3"")] public class Test{
	[To(ToAttribute.NoneWithLeadingDotRemoved)]
	public static int TestField = 1;
	public Test() { Console.WriteLine(Test.TestField); }
}", @"class Test{
	static TestField = 1;
	constructor() { console.log(Test3); }
}")]
	public void Test_ToAttributeNone(string cs, string expectedJS)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.Utils;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(expectedJS, file.TranslatedStr);
	}
	
	[Theory]
	[InlineData("if(EqualsStrict(1, 2)) {};")]
	public void Test_BinaryAttribute(string cs)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"if(1 === 2) {};", file.TranslatedStr);
	}

	[Theory]
	[InlineData("Delete(1);")]
	public void Test_UnaryAttribute(string cs)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"delete 1;", file.TranslatedStr);
	}
	
	[Theory]
	[InlineData(@"MutationObserverInit a = new MutationObserverInit() 
	{ Attributes = true, ChildList = true };")]
	public void Test_ToObjectAttribute(string cs)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"let a = 
	{ attributes : true, childList : true };", file.TranslatedStr);
	}
}
