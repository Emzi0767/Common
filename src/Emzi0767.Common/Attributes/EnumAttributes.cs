﻿// This file is a part of Emzi0767.Common project.
// 
// Copyright 2020 Emzi0767
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

namespace Emzi0767.Serialization
{
    /// <summary>
    /// <para>Specifies that this enum should be serialized and deserialized as its underlying numeric type.</para>
    /// <para>This is used to change the behaviour of enum serialization.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NumericEnumAttribute : Attribute
    { }

    /// <summary>
    /// <para>Specifies that this enum should be serialized and deserialized as its string representation.</para>
    /// <para>This is used to change the behaviour of enum serialization.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class StringEnumAttribute : Attribute
    { }
}
