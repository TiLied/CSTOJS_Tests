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

	//Test ToAttribute("None") too.
	[Theory]
	[InlineData("Console.WriteLine(PressureSource.Cpu);")]
	public void Test_EnumValueAttribute(string cs)
	{
		FileData file = new()
		{
			SourceStr = $@"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS;
{cs}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"console.log(""cpu"");", file.TranslatedStr);
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
}