using System.Collections.Generic;
using Xunit.Sdk;

namespace CSTOJS_Tests;

public class TestData : IXunitSerializable
{
	//CS Type Name
	public string CSType { get; set; } = string.Empty;
	//CS Value
	public string CSValue { get; set; } = string.Empty;
	//JS(Jint) Value (expected str)
	public string Expected { get; set; } = string.Empty;

	//method, reason
	public Dictionary<string, string> SkipMethods { get; set; } = new();

	public TestData() { }

	public TestData(string csType, string csValue, string expected) 
	{
		CSType = csType;
		CSValue = csValue;
		Expected = expected;
	}
	
	public void Deserialize(IXunitSerializationInfo info)
	{
		CSType = info.GetValue<string?>(nameof(CSType)) ?? string.Empty;
		CSValue = info.GetValue<string?>(nameof(CSValue)) ?? string.Empty;
		Expected = info.GetValue<string?>(nameof(Expected)) ?? string.Empty;

		SkipMethods = info.GetValue<Dictionary<string, string>?>(nameof(SkipMethods)) ?? new();
	}

	public void Serialize(IXunitSerializationInfo info)
	{
		info.AddValue(nameof(CSType), CSType);
		info.AddValue(nameof(CSValue), CSValue);
		info.AddValue(nameof(Expected), Expected);

		info.AddValue(nameof(SkipMethods), SkipMethods);
	}
	
	// You can customize how the class is displayed in parameters by overriding ToString
	//public override string ToString()
	//{
		//return $"{CSType} {CSValue} {Expected}";
	//}
}
