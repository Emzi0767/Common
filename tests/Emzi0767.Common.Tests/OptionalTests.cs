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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class OptionalTests
    {
        private Optional<int> Optional42_0 { get; } = Optional.FromValue(42);
        private Optional<int> Optional42_1 { get; } = Optional.FromValue(42);
        private Optional<int> Optional69 { get; } = Optional.FromValue(69);
        private Optional<int> OptionalDefaultInt { get; } = Optional.FromDefaultValue<int>();
        private Optional<int> OptionalNoValueInt { get; } = Optional.FromNoValue<int>();

        private Optional<string> OptionalString42 { get; } = Optional.FromValue("42");
        private Optional<string> OptionalDefaultString { get; } = Optional.FromDefaultValue<string>();
        private Optional<string> OptionalNoValueString { get; } = Optional.FromNoValue<string>();

        private int Int42 { get; } = 42;
        private int Int69 { get; } = 69;
        private int Int0 { get; } = 0;
        private string String42 { get; } = "42";
        private string String69 { get; } = "69";
        private string StringNull { get; } = null;

        private Optional<Nullable<int>> OptionalNullableInt_0 { get; } = Optional.FromValue<Nullable<int>>(null);
        private Optional<Nullable<int>> OptionalNullableInt_1 { get; } = Optional.FromValue<Nullable<int>>(null);
        private Nullable<int> NullableInt { get; } = null;

        [TestMethod]
        public void TestOptionalEquality()
        {
            // Test that optionals with similar values are equal
            Assert.IsTrue(this.Optional42_0 == this.Optional42_1);
            Assert.IsTrue(this.Optional42_1 == this.Optional42_0);
            Assert.IsTrue(this.OptionalNullableInt_0 == this.OptionalNullableInt_1);

            // Test that optionals with different values are not equal
            Assert.IsFalse(this.Optional42_0 == this.Optional69);
            Assert.IsFalse(this.Optional69 == this.Optional42_0);

            Assert.IsFalse(this.Optional42_1 == this.Optional69);
            Assert.IsFalse(this.Optional69 == this.Optional42_1);

            Assert.IsFalse(this.Optional42_0 == this.OptionalDefaultInt);
            Assert.IsFalse(this.OptionalDefaultInt == this.Optional42_0);

            Assert.IsFalse(this.Optional42_1 == this.OptionalDefaultInt);
            Assert.IsFalse(this.OptionalDefaultInt == this.Optional42_1);

            Assert.IsFalse(this.Optional69 == this.OptionalDefaultInt);
            Assert.IsFalse(this.OptionalDefaultInt == this.Optional69);

            // Test that optionals with no value are not equal to optionals with value
            Assert.IsFalse(this.OptionalNoValueInt == this.Optional42_0);
            Assert.IsFalse(this.OptionalNoValueInt == this.Optional42_1);
            Assert.IsFalse(this.OptionalNoValueInt == this.Optional69);
            Assert.IsFalse(this.OptionalNoValueInt == this.OptionalDefaultInt);

            // Test that optionals of different types are not equal
            Assert.IsFalse(this.OptionalString42.Equals(this.Optional42_0));
            Assert.IsFalse(this.OptionalNoValueString.Equals(this.OptionalNoValueInt));
            Assert.IsFalse(this.OptionalNoValueString.Equals(this.Optional42_0));
            Assert.IsFalse(this.OptionalDefaultString.Equals(this.OptionalNoValueInt));
        }

        [TestMethod]
        public void TestValueEquality()
        {
            // Test that optionals are equal to their values
            Assert.IsTrue(this.Optional42_0 == this.Int42);
            Assert.IsTrue(this.Optional42_1 == this.Int42);
            Assert.IsTrue(this.Optional69 == this.Int69);
            Assert.IsTrue(this.OptionalDefaultInt == this.Int0);

            Assert.IsTrue(this.OptionalString42 == this.String42);

            Assert.IsTrue(this.OptionalNullableInt_0 == this.NullableInt);
            Assert.IsTrue(this.OptionalNullableInt_1 == this.NullableInt);

            // Test that optionals are not equal to other values
            Assert.IsFalse(this.Optional42_0 == this.Int69);
            Assert.IsFalse(this.Optional42_0 == this.Int0);

            Assert.IsFalse(this.Optional42_1 == this.Int69);
            Assert.IsFalse(this.Optional42_1 == this.Int0);

            Assert.IsFalse(this.Optional69 == this.Int42);
            Assert.IsFalse(this.Optional69 == this.Int0);

            Assert.IsFalse(this.OptionalDefaultInt == this.Int42);
            Assert.IsFalse(this.OptionalDefaultInt == this.Int69);

            Assert.IsFalse(this.OptionalString42 == this.String69);
            Assert.IsFalse(this.OptionalDefaultString == this.String69);

            Assert.IsFalse(this.OptionalNullableInt_0 == this.Int42);
            Assert.IsFalse(this.OptionalNullableInt_0 == this.Int69);
            Assert.IsFalse(this.OptionalNullableInt_0 == this.Int0);

            Assert.IsFalse(this.OptionalNullableInt_1 == this.Int42);
            Assert.IsFalse(this.OptionalNullableInt_1 == this.Int69);
            Assert.IsFalse(this.OptionalNullableInt_1 == this.Int0);

            // Test that unintialized optionals are equal to nulls
            Assert.IsTrue(this.OptionalNoValueInt.Equals(null));
            Assert.IsTrue(this.OptionalNoValueString.Equals(null));

            // Test that nullable type optionals initialized with null are equal to null
            Assert.IsTrue(this.OptionalDefaultString == null);
        }

        [TestMethod]
        public void TestImplicitConversion()
        {
            // Test that implicitly-converted optional is equal to an optional with the same value
            Optional<int> fourtyTwo = 42;
            Assert.IsTrue(fourtyTwo == this.Optional42_0);
            Assert.IsTrue(fourtyTwo == this.Optional42_1);
        }

        [TestMethod]
        public void TestTruthiness()
        {
            // Test that optionals with a value are truthy
            if (this.Optional42_0) { } else { Assert.Fail(); }
            if (this.Optional42_1) { } else { Assert.Fail(); }
            if (this.Optional69) { } else { Assert.Fail(); }
            if (this.OptionalDefaultInt) { } else { Assert.Fail(); }
            
            if (this.OptionalString42) { } else { Assert.Fail(); }
            if (this.OptionalDefaultString) { } else { Assert.Fail(); }

            // Test that optionals with no value are falsey
            if (this.OptionalNoValueInt) { Assert.Fail(); }
            if (this.OptionalNoValueString) { Assert.Fail(); }
        }
    }
}
