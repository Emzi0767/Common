// This file is a part of Emzi0767.Common project.
// 
// Copyright 2019 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
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
        /// Converts a given object into a dictionary of property name to property value mappings.
        /// </summary>
        /// <typeparam name="T">Type of object to convert.</typeparam>
        /// <param name="obj">Object to convert.</param>
        /// <param name="useCachedModel">Whether a cached conversion model should be used. If you don't intend to reuse this method, set this to <see langword="false"/>.</param>
        /// <returns>Converted dictionary.</returns>
        public static IReadOnlyDictionary<string, object> ToDictionary<T>(this T obj, bool useCachedModel = true)
        {
            if (obj == null)
                throw new NullReferenceException();
            
            var t = typeof(T);
            if (useCachedModel)
            {
                if (!TypeModelCache.TryGetValue(t, out var model))
                    TypeModelCache.TryAdd(t, model = new TypeModel<T>(obj.GetReadableProperties()));

                return model.Convert(obj);
            }
            else
            {
                return new ReadOnlyDictionary<string, object>(typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(x => x.Name, x => x.GetValue(obj)));
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
                    var inst = Expression.Parameter(t, "instance");
                    var call = Expression.Call(inst, prop.GetMethod);
                    var cast = Expression.Convert(call, typeof(object));
                    yield return (prop.Name, Expression.Lambda<Func<T, object>>(cast, inst).Compile());
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
            {
                if (!(obj is TModel model))
                    return null;

                var dict = new Dictionary<string, object>();
                foreach (var (name, get) in this.Properties)
                    dict[name] = get(model);

                return new ReadOnlyDictionary<string, object>(dict);
            }
        }
    }
}
