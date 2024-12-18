// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2025 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Emzi0767.Utilities;

/// <summary>
/// Contains various utilities for use with .NET's reflection.
/// </summary>
public static class ReflectionUtilities
{
    /// <summary>
    /// <para>Creates an empty, uninitialized instance of specified type.</para>
    /// <para>This method will not call the constructor for the specified type. As such, the object might not be properly initialized.</para>
    /// </summary>
    /// <remarks>
    /// This method is intended for reflection use only.
    /// </remarks>
    /// <param name="t">Type of the object to instantiate.</param>
    /// <returns>Empty, uninitialized object of specified type.</returns>
    public static object CreateEmpty(this Type t)
        => FormatterServices.GetUninitializedObject(t);

    /// <summary>
    /// <para>Creates an empty, uninitialized instance of type <typeparamref name="T"/>.</para>
    /// <para>This method will not call the constructor for type <typeparamref name="T"/>. As such, the object might not be proerly initialized.</para>
    /// </summary>
    /// <remarks>
    /// This method is intended for reflection use only.
    /// </remarks>
    /// <typeparam name="T">Type of the object to instantiate.</typeparam>
    /// <returns>Empty, uninitalized object of specified type.</returns>
    public static T CreateEmpty<T>()
        => (T)FormatterServices.GetUninitializedObject(typeof(T));

    /// <summary>
    /// Converts a given object into a dictionary of property name to property value mappings. This method supports
    /// caching object models, however note that this only provides a performance benefit when this method is called
    /// many times (on the order of couple hundred times at least) on a given object type.
    /// </summary>
    /// <typeparam name="T">Type of object to convert.</typeparam>
    /// <param name="obj">Object to convert.</param>
    /// <param name="useCachedModel">
    /// Whether a cached conversion model should be used. If you don't intend to reuse this method, set this to
    /// <see langword="false"/>.
    /// </param>
    /// <returns>Converted dictionary.</returns>
    public static IReadOnlyDictionary<string, object> ToDictionary<T>(this T obj, bool useCachedModel = false)
    {
        if (obj == null)
            throw new NullReferenceException();
        
        var t = obj.GetType(); // T might not be equal
        if (useCachedModel)
        {
            if (!TypeModelCache.TryGetValue(t, out var model))
                TypeModelCache.TryAdd(t, model = new TypeModel<T>(obj.GetReadableProperties()));

            return model.Convert(obj);
        }
        else
        {
            return new CharSpanLookupReadOnlyDictionary<object>(
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(obj)))
            );
        }
    }

    private static IEnumerable<(string, Func<T, object>)> GetReadableProperties<T>(this T obj)
    {
        var t = typeof(T);
        var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.GetMethod != null && x.GetCustomAttribute<CompilerGeneratedAttribute>() == null);

        foreach (var prop in props)
        {
            if (!prop.PropertyType.IsValueType)
            {
                yield return (prop.Name, prop.GetMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(t, prop.PropertyType)) as Func<T, object>);
            }
            else
            {
                var boxedGetterType = typeof(BoxedGetter<,>).MakeGenericType(t, prop.PropertyType);
                var boxedGetter = Activator.CreateInstance(boxedGetterType, prop.GetMethod) as BoxedGetter<T>;
                yield return (prop.Name, boxedGetter.GetBoxed);
            }
        }
    }

    private static ConcurrentDictionary<Type, ITypeModel> TypeModelCache { get; } = new ConcurrentDictionary<Type, ITypeModel>();
    private interface ITypeModel
    {
        IReadOnlyDictionary<string, object> Convert<TObj>(TObj obj);
    }

    private sealed class TypeModel<TModel> : ITypeModel
    {
        private ImmutableArray<(string Name, Func<TModel, object> GetAccessor)> Properties { get; }

        public TypeModel(IEnumerable<(string, Func<TModel, object>)> props)
        {
            this.Properties = props.ToImmutableArray();
        }

        public IReadOnlyDictionary<string, object> Convert<TObj>(TObj obj)
            => obj is not TModel model
            ? null
            : new CharSpanLookupReadOnlyDictionary<object>(
                this.Properties
                    .Select(x => new KeyValuePair<string, object>(x.Name, x.GetAccessor(model)))
            ) as IReadOnlyDictionary<string, object>;
    }

    private abstract class BoxedGetter<TObject>
    {
        public abstract object GetBoxed(TObject instance);
    }

    private sealed class BoxedGetter<TObject, TValue> : BoxedGetter<TObject>
    {
        private readonly Func<TObject, TValue> _getter;

        public BoxedGetter(MethodInfo getter)
        {
            this._getter = getter.CreateDelegate(typeof(Func<TObject, TValue>)) as Func<TObject, TValue>;
        }

        public override object GetBoxed(TObject instance)
            => this._getter(instance);
    }
}
