﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Aequus.Core.Utilities;

public static class ReflectionHelper {
    public static IEnumerable<FieldInfo> GetConstants(Type t, BindingFlags flags = BindingFlags.Static | BindingFlags.Public) {
        foreach (var f in t.GetFields(flags)) {
            if (f.IsLiteral && !f.IsInitOnly) {
                yield return f;
            }
        }
    }

    public static IEnumerable<(T attributeInstance, MemberInfo memberInfo)> GetMembersWithAttribute<T>(Type t) where T : Attribute {
        var l = new List<(T, MemberInfo)>();
        foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            var attr = f.GetCustomAttribute<T>();
            if (attr != null) {
                l.Add((attr, f));
            }
        }
        foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            var attr = p.GetCustomAttribute<T>();
            if (attr != null) {
                l.Add((attr, p));
            }
        }
        return l;
    }

    public static T GetValue<T>(this PropertyInfo property, object obj) {
        return (T)property.GetValue(obj);
    }
    public static T GetValue<T>(this FieldInfo field, object obj) {
        return (T)field.GetValue(obj);
    }
}