// ReSharper disable once CheckNamespace

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Struct)]
internal sealed class InlineArrayAttribute : Attribute
{

    public InlineArrayAttribute(int length) => Length = length;

    public int Length { get; }

}
