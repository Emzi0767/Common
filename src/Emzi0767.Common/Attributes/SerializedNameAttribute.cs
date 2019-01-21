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

using System;

namespace Emzi0767.Attributes
{
    /// <summary>
    /// Declares name of a property in serialized data. This is used for mapping serialized data to object properties and fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SerializedNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the serialized name of the field or property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Declares name of a property in serialized data. This is used for mapping serialized data to object properties and fields.
        /// </summary>
        /// <param name="name">Name of the field or property in serialized data.</param>
        public SerializedNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
