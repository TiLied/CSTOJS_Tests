using CSharpToJavaScript.APIs.JS;
using CSharpToJavaScript.Utils;
using System;
using System.Collections.Generic;
using Math = CSharpToJavaScript.APIs.JS.Ecma.Math;

namespace UnitTest6
{
	public enum EColors
	{
		RED = 5,
		GREEN = 6,
		BLUE = 7
	}

	public partial class Person
	{
		public required FullName Name { get; set; }
	}

	public class FullName
	{
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		[Value("asd")]
		public void Write() 
		{
			if (FirstName != null)
			{
				//GM2.DeleteValue2("");
			}
			Console.WriteLine(GM.Info.Script.Namespace_);

			Console.WriteLine($"{RequestMode.Cors}");

			Console.WriteLine($"{FirstName} {LastName}");

			string[] strA = new string[5];
		}
	}

	[Value("asd")]
	internal class Test6
	{
		public static readonly EColors _Test = EColors.BLUE;
		[Value("asd")]
		public readonly int _ROInt = 0;
		public required int ReqInt { get; set; } = 1;
		[Value("asd")]
		public string Prop { get; set; } = string.Empty;
		[Value("asd")]
		public Test6() 
		{
			Prop = "1";
			Prop = "2";
			EColors e = ReqInt == 1 ? EColors.RED : EColors.GREEN;

			int a, b, c;
			a = 7;
			b = a;
			c = b++;
			b = a + b * c;
			c = a >= 100 ? b : c / 10;
			a = (int)Math.Sqrt(b * b + c * c);

			string s = "String literal";
			char l = s[s.Length - 1];

			List<int> numbers = new();
			b = numbers.FindLast((n) => n > 1);

			Person? person = null;
			person?.Name.Write();

			int? a2 = null;
			Console.Write(a2 ?? 3);

			int aMV = 0;
			int bit = aMV << 3;
			bit = aMV >> 3;
			bit = aMV | 3;

			uint a1 = 2;
			uint b1 = 4;
			uint c1 = a1 & b1;

			uint au = 2;
			uint bu = ~au;

			uint INITIAL_VALUE = 4;

			uint au2 = INITIAL_VALUE;
			au2 &= 2;

			au2 = INITIAL_VALUE;
			au2 |= 4;

			au2 = INITIAL_VALUE;
			au2 ^= 2;

			au2 = INITIAL_VALUE;
			au2 <<= 2;

			try
			{
				au2 = INITIAL_VALUE;
				au2 >>= 4;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				//throw ex;
			}
			finally 
			{
				Console.WriteLine("ex");
			}
			do 
			{ 
				au2 = INITIAL_VALUE; 
				au2 >>= 4;
				Console.WriteLine("");
			} while( au2 != 0 );
		}
	}
}
