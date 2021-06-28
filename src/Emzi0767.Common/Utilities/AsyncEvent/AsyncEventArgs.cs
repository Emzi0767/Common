// This file is part of Emzi0767.Common project
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

namespace Emzi0767.Utilities
{
    /// <summary>
    /// Contains arguments passed to an asynchronous event.
    /// </summary>
    public class AsyncEventArgs : EventArgs
    {
        /// <summary>
        /// <para>Gets or sets whether this event was handled.</para>
        /// <para>Setting this to true will prevent other handlers from running.</para>
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}
