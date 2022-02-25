using System;

namespace Puerts
{
	
	//放置配置的类
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigureAttribute : Attribute
	{

	}

	//代码生成目录
	[AttributeUsage(AttributeTargets.Property)]
	public class CodeOutputDirectoryAttribute : Attribute
	{
	}

	//要在ts/js里头调用，必须放在标记了Configure的类里
	[AttributeUsage(AttributeTargets.Property)]
	public class BindingAttribute : Attribute
	{
	}

	//相比Binding，这标签仅生成ts声明
	[AttributeUsage(AttributeTargets.Property)]
	public class TypingAttribute : Attribute
	{
	}

	//对blittable值类型通过内存拷贝传递，需要开启unsafe编译选项
	[AttributeUsage(AttributeTargets.Property)]
	public class BlittableCopyAttribute : Attribute
	{

	}

	[AttributeUsage(AttributeTargets.Method)]
	public class FilterAttribute : Attribute
	{
	}
}