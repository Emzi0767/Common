﻿// This file is part of Emzi0767.Common project
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

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System
{
    /// <summary>
    /// Provides a cryptographically-secure pseudorandom number generator (CSPRNG) implementation compatible with <see cref="Random"/>.
    /// </summary>
    public sealed class SecureRandom : Random, IDisposable
    {
        private RandomNumberGenerator RNG { get; } = RandomNumberGenerator.Create();

        private volatile bool _isDisposed = false;

        /// <summary>
        /// Creates a new instance of <see cref="SecureRandom"/>.
        /// </summary>
        public SecureRandom()
        { }

        /// <summary>
        /// Finalizes this <see cref="SecureRandom"/> instance by disposing it.
        /// </summary>
        ~SecureRandom()
        {
            this.Dispose();
        }

        /// <summary>
        /// Fills a supplied buffer with random bytes.
        /// </summary>
        /// <param name="buffer">Buffer to fill with random bytes.</param>
        public void GetBytes(byte[] buffer)
        {
            this.RNG.GetBytes(buffer);
        }

        /// <summary>
        /// Fills a supplied buffer with random nonzero bytes.
        /// </summary>
        /// <param name="buffer">Buffer to fill with random nonzero bytes.</param>
        public void GetNonZeryBytes(byte[] buffer)
        {
            this.RNG.GetNonZeroBytes(buffer);
        }
        
        /// <summary>
        /// Fills a supplied memory region with random bytes.
        /// </summary>
        /// <param name="buffer">Memmory region to fill with random bytes.</param>
        public void GetBytes(Span<byte> buffer)
        {
#if NETCOREAPP2_2
            this.RNG.GetBytes(buffer);
#else
            var buff = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                var buffSpan = buff.AsSpan(0, buffer.Length);
                this.RNG.GetBytes(buff);
                buffSpan.CopyTo(buffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buff);
            }
#endif
        }
        
        /// <summary>
        /// Fills a supplied memory region with random nonzero bytes.
        /// </summary>
        /// <param name="buffer">Memmory region to fill with random nonzero bytes.</param>
        public void GetNonZeroBytes(Span<byte> buffer)
        {
#if NETCOREAPP2_2
            this.RNG.GetNonZeroBytes(buffer);
#else
            var buff = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                var buffSpan = buff.AsSpan(0, buffer.Length);
                this.RNG.GetNonZeroBytes(buff);
                buffSpan.CopyTo(buffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buff);
            }
#endif
        }

        /// <summary>
        /// Generates a signed 8-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="sbyte.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public sbyte GetInt8(sbyte min = 0, sbyte max = sbyte.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            var offset = (sbyte)(min < 0 ? -min : 0);
            min += offset;
            max += offset;

            return (sbyte)(Math.Abs(this.Generate<sbyte>()) % (max - min) + min - offset);
        }

        /// <summary>
        /// Generates a unsigned 8-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="byte.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public byte GetUInt8(byte min = 0, byte max = byte.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            return (byte)(this.Generate<byte>() % (max - min) + min);
        }

        /// <summary>
        /// Generates a signed 16-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="short.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public short GetInt16(short min = 0, short max = short.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            var offset = (short)(min < 0 ? -min : 0);
            min += offset;
            max += offset;

            return (short)(Math.Abs(this.Generate<short>()) % (max - min) + min - offset);
        }

        /// <summary>
        /// Generates a unsigned 16-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="ushort.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public ushort GetUInt16(ushort min = 0, ushort max = ushort.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            return (ushort)(this.Generate<ushort>() % (max - min) + min);
        }

        /// <summary>
        /// Generates a signed 32-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="int.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public int GetInt32(int min = 0, int max = int.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            var offset = min < 0 ? -min : 0;
            min += offset;
            max += offset;

            return Math.Abs(this.Generate<int>()) % (max - min) + min - offset;
        }

        /// <summary>
        /// Generates a unsigned 32-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="uint.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public uint GetUInt32(uint min = 0, uint max = uint.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            return this.Generate<uint>() % (max - min) + min;
        }

        /// <summary>
        /// Generates a signed 64-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="long.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public long GetInt64(long min = 0, long max = long.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            var offset = min < 0 ? -min : 0;
            min += offset;
            max += offset;

            return Math.Abs(this.Generate<long>()) % (max - min) + min - offset;
        }

        /// <summary>
        /// Generates a unsigned 64-bit integer within specified range.
        /// </summary>
        /// <param name="min">Minimum value to generate. Defaults to 0.</param>
        /// <param name="max">Maximum value to generate. Defaults to <see cref="ulong.MaxValue"/>.</param>
        /// <returns>Generated random value.</returns>
        public ulong GetUInt64(ulong min = 0, ulong max = ulong.MaxValue)
        {
            if (max <= min)
                throw new ArgumentException("Maximum needs to be greater than minimum.", nameof(max));

            return this.Generate<ulong>() % (max - min) + min;
        }

        /// <summary>
        /// Generates a 32-bit floating-point number between 0.0 and 1.0.
        /// </summary>
        /// <returns>Generated 32-bit floating-point number.</returns>
        public float GetSingle()
        {
            var (i1, i2) = ((float)this.GetInt32(), (float)this.GetInt32());
            return i1 / i2 % 1.0F;
        }

        /// <summary>
        /// Generates a 64-bit floating-point number between 0.0 and 1.0.
        /// </summary>
        /// <returns>Generated 64-bit floating-point number.</returns>
        public double GetDouble()
        {
            var (i1, i2) = ((double)this.GetInt64(), (double)this.GetInt64());
            return i1 / i2 % 1.0;
        }

        /// <summary>
        /// Generates a 32-bit integer between 0 and <see cref="int.MaxValue"/>. Upper end exclusive.
        /// </summary>
        /// <returns>Generated 32-bit integer.</returns>
        public override int Next()
            => this.GetInt32();

        /// <summary>
        /// Generates a 32-bit integer between 0 and <paramref name="maxValue"/>. Upper end exclusive.
        /// </summary>
        /// <param name="maxValue">Maximum value of the generated integer.</param>
        /// <returns>Generated 32-bit integer.</returns>
        public override int Next(int maxValue)
            => this.GetInt32(0, maxValue);

        /// <summary>
        /// Generates a 32-bit integer between <paramref name="minValue"/> and <paramref name="maxValue"/>. Upper end exclusive.
        /// </summary>
        /// <param name="minValue">Minimum value of the generate integer.</param>
        /// <param name="maxValue">Maximum value of the generated integer.</param>
        /// <returns>Generated 32-bit integer.</returns>
        public override int Next(int minValue, int maxValue)
            => this.GetInt32(minValue, maxValue);

        /// <summary>
        /// Generates a 64-bit floating-point number between 0.0 and 1.0. Upper end exclusive.
        /// </summary>
        /// <returns>Generated 64-bit floating-point number.</returns>
        public override double NextDouble()
            => this.GetDouble();

        /// <summary>
        /// Fills specified buffer with random bytes.
        /// </summary>
        /// <param name="buffer">Buffer to fill with bytes.</param>
        public override void NextBytes(byte[] buffer)
            => this.GetBytes(buffer);

        /// <summary>
        /// Fills specified memory region with random bytes.
        /// </summary>
        /// <param name="buffer">Memory region to fill with bytes.</param>
        #if NETCOREAPP2_2 
        override 
        #endif 
        public void NextBytes(Span<byte> buffer)
            => this.GetBytes(buffer);

        /// <summary>
        /// Disposes this <see cref="SecureRandom"/> instance and its resources.
        /// </summary>
        public void Dispose()
        {
            if (this._isDisposed)
                return;

            this._isDisposed = true;
            this.RNG.Dispose();
        }

        /// <summary>
        /// Generates a random 64-bit floating-point number between 0.0 and 1.0. Upper end exclusive.
        /// </summary>
        /// <returns>Generated 64-bit floating-point number.</returns>
        protected override double Sample()
            => this.GetDouble();

        private T Generate<T>() where T : struct
        {
            var size = Unsafe.SizeOf<T>();
            Span<byte> buff = stackalloc byte[size];
            this.GetBytes(buff);
            return MemoryMarshal.Read<T>(buff);
        }
    }
}
