using UnityEngine;
using System;

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
    public NamedArrayAttribute(Type enumType) { names = Enum.GetNames(enumType); }

}
