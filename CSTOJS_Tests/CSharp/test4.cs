using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject.GlobalThis;
using Console = System.Console;

namespace UnitTest4
{
	public class TestClass
	{
		public string StrTest { get; set; } = string.Empty;

		public TestClass(string test = "")
		{
			//Console.Log("asd");
			Console.WriteLine("asd");
			var a = Window.Name;
			StrTest = test;
		}

		public void Math(string testClass, string stt = "") 
		{
			
		}
	}

	public class Test4
	{
		private TestClass TestClassInTest4 = new();

		private string _StrStrInTest4;
		public string StrStrInTest4
		{ 
			get
			{
				return _StrStrInTest4;
			}
			set 
			{
				_StrStrInTest4 = value;
				TestClassInTest4.Math(TestClassInTest4.StrTest);
			}
		}
		
		public Test4() 
		{
			TestClassInTest4 = new(TestClassInTest4.StrTest);

			var a = new TestClass();
			TestClass b = new("123");

			TestClassInTest4.Math(TestClassInTest4.StrTest, TestClassInTest4.StrTest);
		}
	}
}
