using CSharpToJavaScript;
using System.Globalization;
using System.Threading;

namespace CSTOJS_Tests;

public class UnitTest_ThisExpression
{
	/*
	private readonly Engine _Engine = new(cfg => cfg.Culture(CultureInfo.InvariantCulture));
	private string _ConsoleStr = string.Empty;
	public UnitTest_ThisExpression()
	{
		//https://stackoverflow.com/a/45117890
		CultureInfo info = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentCulture = info;
		Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

		_Engine.SetValue("log", new Action<object>(ConsoleOutPut));
	}
	private void ConsoleOutPut(object? obj)
	{
		_ConsoleStr = obj?.ToString() ?? "null";
	}
	*/
	public UnitTest_ThisExpression()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
	}
	[Fact]
	public void Test_CastThis()
	{
		FileData file = new()
		{
			SourceStr = @"using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.APIs.JS.Ecma;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;

namespace Test_CastThis;

public class C
{
	public Node P { get; set; }

	public virtual void DeleteNode()
	{
		((ChildNode)P).Remove();
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	#_P_;
	get P(){return this.#_P_;}
	set P(value){this.#_P_ = value;}

	DeleteNode()
	{
		this.P.remove();
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_PrintTwoFields()
	{
		FileData file = new()
		{
			SourceStr = @"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS; 
namespace Test_PrintTwoFields;

public class Main
{
	private int F1 = 1;
	private int F2 = 2;
	
	public Main()
	{
		Console.WriteLine($""{F1} {F2}"");
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Main
{
	F1 = 1;
	F2 = 2;
	
	constructor()
	{
		console.log(`${this.F1} ${this.F2}`);
	}
}", file.TranslatedStr);

	}
	[Fact]
	public void Test_ExplicitThisWithImplicitThis()
	{
		FileData file = new()
		{
			SourceStr = @"using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;
using CSharpToJavaScript.APIs.JS; 
namespace Test_ExplicitThisWithImplicitThis;

public class Main
{
	private bool F1 = true;
	private bool F2 = true;
	
	public Main()
	{
		this.M(F1, this.F2);
	}
	public void M(bool b1, bool b2){}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class Main
{
	F1 = true;
	F2 = true;
	
	constructor()
	{
		this.M(this.F1, this.F2);
	}
	M(b1, b2){}
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
	public void Test_IfElseStatement()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_IfElseStatement;

public class C
{
	private int _F = 0;
	public C()
	{
		if(_F == 0){}else if(_F<0){}
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		if(this._F == 0){}else if(this._F<0){}
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_ForStatement()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_ForStatement;

public class C
{
	private int _F = 0;
	public C()
	{
		for(_F=0; _F<10; _F++){}
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		for(this._F=0; this._F<10; this._F++){}
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_WhileStatement()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_WhileStatement;

public class C
{
	private int _F = 0;
	public C()
	{
		while(_F == 1 - _F){}
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		while(this._F == 1 - this._F){}
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_DoWhileStatement()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_DoWhileStatement;

public class C
{
	private int _F = 0;
	public C()
	{
		do{}while(_F>0);
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		do{}while(this._F>0);
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_SwitchStatement()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_SwitchStatement;

public class C
{
	private int _F = 0;
	public C()
	{
		switch (_F)
		{
			case 0:
				Console.WriteLine(_F);
				break;
			default:
				break;
		}
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		switch (this._F)
		{
			case 0:
				console.log(this._F);
				break;
			default:
				break;
		}
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_TernaryOperator()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_TernaryOperator;

public class C
{
	private int _F = 0;
	public C()
	{
		int l = _F>0 ? _F : _F;
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		let l = this._F>0 ? this._F : this._F;
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_AssignmentRight()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_AssignmentRight;

public class C
{
	private int _F = 0;
	public C()
	{
		int l = _F;
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		let l = this._F;
	}
}", file.TranslatedStr);
	}
	[Fact]
	public void Test_AssignmentLeft()
	{
		FileData file = new()
		{
			SourceStr = @"namespace Test_AssignmentLeft;

public class C
{
	private int _F = 0;
	public C()
	{
		_F = 1;
	}
}"
		};
		file = CSTOJS.Translate(file);

		Assert.Equal(@"
class C
{
	_F = 0;
	constructor()
	{
		this._F = 1;
	}
}", file.TranslatedStr);
	}
}