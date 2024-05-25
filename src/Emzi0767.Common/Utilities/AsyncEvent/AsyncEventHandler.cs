// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2024 Emzi0767
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

using System.Threading.Tasks;

namespace Emzi0767.Utilities;

/// <summary>
/// Handles an asynchronous event of type <see cref="AsyncEvent{TSender, TArgs}"/>. The handler will take an instance of <typeparamref name="TArgs"/> as its arguments.
/// </summary>
/// <typeparam name="TSender">Type of the object that dispatches this event.</typeparam>
/// <typeparam name="TArgs">Type of the object which holds arguments for this event.</typeparam>
/// <param name="sender">Object which raised this event.</param>
/// <param name="e">Arguments for this event.</param>
/// <returns></returns>
public delegate Task AsyncEventHandler<in TSender, in TArgs>(TSender sender, TArgs e) where TArgs : AsyncEventArgs;
