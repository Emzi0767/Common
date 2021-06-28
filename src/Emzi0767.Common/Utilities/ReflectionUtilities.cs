// This file is a part of Emzi0767.Common project.
// 
// Copyright (C) 2020-2021 Emzi0767
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Emzi0767.Utilities
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
        /// <returns>Converted dictionary.</returns>
        public static IReadOnlyDictionary<string, object> ToDictionary<T>(this T obj)
        {
            if (obj == null)
                throw new NullReferenceException();

            return new CharSpanLookupReadOnlyDictionary<object>(typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(obj))));
        }
    }
}
