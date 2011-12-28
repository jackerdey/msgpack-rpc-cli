﻿#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using MsgPack.Rpc.Protocols;
using System.Diagnostics.Contracts;
using System.Threading;

namespace MsgPack.Rpc.Server.Protocols
{
	partial class ServerTransport
	{
		/// <summary>
		///		Unpack request/notification message array header.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		internal bool UnpackRequestHeader( ServerRequestContext context )
		{
			Contract.Assert( context != null );

			if ( context.RootUnpacker == null )
			{
				context.UnpackingBuffer = new ByteArraySegmentStream( context.ReceivedData );
				context.RootUnpacker = Unpacker.Create( context.UnpackingBuffer, false );
				Interlocked.Increment( ref this._processing );
			}

			if ( !context.RootUnpacker.Read() )
			{
				Tracer.Protocols.TraceEvent( Tracer.EventType.NeedRequestHeader, Tracer.EventId.NeedRequestHeader, "Array header is needed." );
				return false;
			}

			if ( !context.RootUnpacker.IsArrayHeader )
			{
				this.HandleDeserializationError( context, "Invalid request/notify message stream. Message must be array.", () => context.UnpackingBuffer.ToArray() );
				return true;
			}

			if ( context.RootUnpacker.ItemsCount != 3 && context.RootUnpacker.ItemsCount != 4 )
			{
				this.HandleDeserializationError(
					context,
					String.Format(
						CultureInfo.CurrentCulture,
						"Invalid request/notify message stream. Message must be valid size array. Actual size is {0}.",
						context.RootUnpacker.ItemsCount
					),
					() => context.UnpackingBuffer.ToArray()
				);
				return true;
			}

			context.HeaderUnpacker = context.RootUnpacker.ReadSubtree();
			context.NextProcess = UnpackMessageType;
			return context.NextProcess( context );
		}

		/// <summary>
		///		Unpack Message Type part on request/notification message.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		private bool UnpackMessageType( ServerRequestContext context )
		{
			if ( !context.HeaderUnpacker.Read() )
			{
				Tracer.Protocols.TraceEvent( Tracer.EventType.NeedMessageType, Tracer.EventId.NeedMessageType, "Message Type is needed." );
				return false;
			}

			int numericType;
			try
			{
				numericType = context.HeaderUnpacker.Data.Value.AsInt32();
			}
			catch ( InvalidOperationException )
			{
				this.HandleDeserializationError( context, "Invalid request/notify message stream. Message Type must be Int32 compatible integer.", () => context.UnpackingBuffer.ToArray() );
				return true;
			}

			MessageType type = ( MessageType )numericType;
			context.MessageType = type;

			switch ( type )
			{
				case MessageType.Request:
				{
					context.NextProcess = this.UnpackMessageId;
					break;
				}
				case MessageType.Notification:
				{
					context.NextProcess = this.UnpackMethodName;
					break;
				}
				default:
				{
					this.HandleDeserializationError(
						context,
						String.Format( CultureInfo.CurrentCulture, "Unknown message type '{0:x8}'", numericType ),
						() => context.UnpackingBuffer.ToArray()
					);
					return true;
				}
			}

			return context.NextProcess( context );
		}

		/// <summary>
		///		Unpack Message ID part on request message.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		private bool UnpackMessageId( ServerRequestContext context )
		{
			if ( !context.HeaderUnpacker.Read() )
			{
				Tracer.Protocols.TraceEvent( Tracer.EventType.NeedMessageId, Tracer.EventId.NeedMessageId, "Message ID is needed." );
				return false;
			}

			try
			{
				context.Id = unchecked( ( int )context.HeaderUnpacker.Data.Value.AsUInt32() );
			}
			catch ( InvalidOperationException )
			{
				this.HandleDeserializationError(
					context,
					"Invalid request message stream. ID must be UInt32 compatible integer.",
					() => context.UnpackingBuffer.ToArray()
				);
				return true;
			}

			context.NextProcess = this.UnpackMethodName;
			return context.NextProcess( context );
		}

		/// <summary>
		///		Unpack Method Name part on request/notification message.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		private bool UnpackMethodName( ServerRequestContext context )
		{
			if ( !context.HeaderUnpacker.Read() )
			{
				Tracer.Protocols.TraceEvent( Tracer.EventType.NeedMethodName, Tracer.EventId.NeedMethodName, "Method Name is needed." );
				return false;
			}

			try
			{
				context.MethodName = context.HeaderUnpacker.Data.Value.AsString();
			}
			catch ( InvalidOperationException )
			{
				this.HandleDeserializationError(
					context,
					"Invalid request/notify message stream. Method name must be UTF-8 string.",
					() => context.UnpackingBuffer.ToArray()
				);
				return true;
			}

			context.NextProcess = this.UnpackArgumentsHeader;
			return context.NextProcess( context );
		}

		/// <summary>
		///		Unpack array header of Arguments part on request/notification message.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		private bool UnpackArgumentsHeader( ServerRequestContext context )
		{
			if ( !context.HeaderUnpacker.Read() )
			{
				Tracer.Protocols.TraceEvent( Tracer.EventType.NeedArgumentsArrayHeader, Tracer.EventId.NeedArgumentsArrayHeader, "Arguments array header is needed." );
				return false;
			}

			if ( !context.HeaderUnpacker.IsArrayHeader )
			{
				this.HandleDeserializationError(
					context,
					"Invalid request/notify message stream. Arguments must be array.",
					() => context.UnpackingBuffer.ToArray()
				);
				return true;
			}

			context.ArgumentsBufferUnpacker = context.HeaderUnpacker.ReadSubtree();

			if ( Int32.MaxValue < context.ArgumentsBufferUnpacker.ItemsCount )
			{
				this.HandleDeserializationError(
					context,
					RpcError.MessageTooLargeError,
					"Too many arguments.",
					context.ArgumentsBufferUnpacker.ItemsCount.ToString( "#,0", CultureInfo.CurrentCulture ),
					() => context.UnpackingBuffer.ToArray()
				);
				return true;
			}

			context.ArgumentsCount = unchecked( ( int )( context.ArgumentsBufferUnpacker.ItemsCount ) );
			context.ArgumentsBufferPacker = Packer.Create( context.ArgumentsBuffer, false );
			context.ArgumentsBufferPacker.PackArrayHeader( context.ArgumentsCount );
			context.UnpackedArgumentsCount = 0;
			context.NextProcess = this.UnpackArguments;
			return context.NextProcess( context );
		}

		/// <summary>
		///		Unpack array elements of Arguments part on request/notification message.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		private bool UnpackArguments( ServerRequestContext context )
		{
			while ( context.UnpackedArgumentsCount < context.ArgumentsCount )
			{
				if ( !context.ArgumentsBufferUnpacker.Read() )
				{
					Tracer.Protocols.TraceEvent( Tracer.EventType.NeedArgumentsElement, Tracer.EventId.NeedArgumentsElement, "Arguments array element is needed. {0}/{1}", context.UnpackedArgumentsCount, context.ArgumentsCount );
					return false;
				}

				context.ArgumentsBufferPacker.Pack( context.ArgumentsBufferUnpacker.Data.Value );
				context.UnpackedArgumentsCount++;
			}

			context.ArgumentsBuffer.Position = 0;
			context.ArgumentsUnpacker = Unpacker.Create( context.ArgumentsBuffer, false );
			context.NextProcess = this.Dispatch;
			return context.NextProcess( context );
		}

		/// <summary>
		///		Dispatch request/notification message via the <see cref="MessageRecieved"/> event.
		/// </summary>
		/// <param name="context">Context information.</param>
		/// <returns>
		///		<c>true</c>, if the pipeline is finished;
		///		<c>false</c>, the pipeline is interruppted because extra data is needed.
		/// </returns>
		private bool Dispatch( ServerRequestContext context )
		{
			context.State = ServerProcessingState.Reserved;
			context.ClearBuffers();

			var messageId = context.MessageType == MessageType.Notification ? default( int? ) : unchecked( ( int )context.Id );
			try
			{
				// TODO: Pooling
				this.OnMessageReceivedCore(
					new RpcMessageReceivedEventArgs(
						this,
						context.MessageType,
						messageId,
						context.MethodName,
						context.ArgumentsUnpacker
					)
				);
			}
			finally
			{
				context.ClearDispatchContext();

				if ( messageId == null )
				{
					this.OnProcessFinished();
				}
			}

			return true;
		}
	}
}
