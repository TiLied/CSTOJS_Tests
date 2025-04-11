using System;
using System.Collections.Generic;

namespace UnitTest7;

public class TestCl
{
	public string? Pstr { get; set; } = "1";
}

internal class UnitTest
{
	public string? str = null;
	public static TestCl? str2;
	
	

	public TestCl? name { get; set; } = null;

	public UnitTest()
	{
		var i2 = 1;
		string sss3 = "test!!!";
		double ddddd = 5f;
		
		Console.WriteLine($"{i2} 123 {sss3}");
		
		str2 = new TestCl() { Pstr = "" };

		string str33 = "lol";

		if (str2 == null)
		{
			var i = 0;
			i++;
			i--;
			i = 5 + 10;
		}
		else
		{
			str33 = $"{str2.Pstr}";
		}
		Console.WriteLine(Testo(str33));
		float f = 5f;
		str2 = new TestCl();
		if (f == 5)
			str33 += "5";

		p1();
		
		Console.WriteLine("");
	}

	private static int Run(int number)
	{
		int total = 0;
		for (var i = 1; i < number; i++)
			if (i % 3 == 0 || i % 5 == 0)
				total += i;
		return total;
	}
	
	
	
	string Testo(string naisu)
	{
		double firstCase = Math.Exp(1);
		double secondCase = Math.Exp(-1);
		double thirdCase = Math.Exp(0);

		Console.WriteLine(firstCase);
		Console.WriteLine(secondCase);
		Console.WriteLine(thirdCase);

		int First = 0, Next = 1, Result = 0, Sum = 0;

		while (Result < 4000000)
		{
			Result = First + Next;
			First = Next;
			Next = Result;
			if (Result % 2 == 0)
			{
				Sum += Result;
				Console.Write(Result + ", ");
			}
		}
		Console.WriteLine("\nSum :" + Sum);


		naisu += "123";
		return naisu;
	}

	public void p1()
	{
		Console.WriteLine("ProjectEuler Problem #1:");
		// var exeTime = Stopwatch.StartNew();
		// in Big O notation is O(n)
		//******************************************** */
		// Solution one to solve the problem
		decimal sum = 0;
		decimal n = 1000;
		int d1 = 3;
		int d2 = 5;
		for (int i = d1; i < 1000; i++)
		{
			if (i % d1 == 0 || i % d2 == 0)
				sum = sum + i;
		}
		Console.WriteLine($"Solution One- Answer for the Problem #1 is : {sum}");
		//  exeTime.Stop();
		//   Console.WriteLine("Execution Time : " + exeTime.ElapsedMilliseconds);

		//  Solution two  based on formula m=floor(n/d) and   for i=1 until m then Zigma(i*d) = d*((m*(m+1))/2)
		// O notation is O(1)

		sum = 0;

		n = n - 1;
		int d1byd2 = d1 * d2;
		decimal m1 = Math.Floor(n / d1);
		decimal m2 = Math.Floor(n / d2);
		decimal m3 = Math.Floor(n / d1byd2);
		// sum of multiples of 3 and 5 is Zigma(i*3)+Zigma(i*5) - Zigma(i*15)
		sum = d1 * (m1 * (m1 + 1) / 2) + d2 * (m2 * (m2 + 1) / 2) - d1byd2 * (m3 * (m3 + 1) / 2);
		Console.WriteLine($"Solution Two- Answer for the Problem #1 is :{sum}");
	}

	public static void Main2()
	{
		int[,] arr = new int[8, 8];
		Console.WriteLine("Pascal Triangle : ");
		for (int i = 0; i < 5; i++)
		{
			for (int k = 5; k > i; k--)
			{
				Console.Write(" ");
			}

			for (int j = 0; j < i; j++)
			{
				if (j == 0 || i == j)
					arr[i, j] = 1;
				else
				{
					arr[i, j] = arr[i - 1, j] + arr[i - 1, j - 1];
				}
				Console.Write(arr[i, j] + " ");
			}
			//Console.ReadLine();
		}
	}

}

class GFG
{

	// Consider a differential equation
	// dy/dx=(x + y + xy)
	static float func(float x, float y)
	{
		return x + y + x * y;
	}

	// Function for Euler formula
	static void euler(float x0, float y, float h, float x)
	{

		// Iterating till the point at which we
		// need approximation
		while (x0 < x)
		{
			y = y + h * func(x0, y);
			x0 = x0 + h;
		}

		// Printing approximation
		Console.WriteLine("Approximate solution at x = "
						  + x + " is " + y);
	}

	// Driver program
	public void Main()
	{

		// Initial Values
		float x0 = 0;
		float y0 = 1;
		float h = 0.025f;

		// Value of x at which we need
		// approximation
		float x = 0.1f;

		euler(x0, y0, h, x);
	}
	static void Main3()
	{
		bool[] doors = new bool[100];

		//The number of passes can be 1-based, but the number of doors must be 0-based.
		for (int p = 1; p <= 100; p++)
			for (int d = p - 1; d < 100; d += p)
				doors[d] = !doors[d];
		for (int d = 0; d < 100; d++)
			Console.WriteLine("Door #{0}: {1}", d + 1, doors[d] ? "Open" : "Closed");
		//Console.ReadKey(true);
	}
}
