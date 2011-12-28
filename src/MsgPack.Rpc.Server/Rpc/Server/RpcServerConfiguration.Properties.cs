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
using System.Diagnostics.Contracts;
using System.Net;
using MsgPack.Rpc.Server.Dispatch;
using MsgPack.Rpc.Server.Protocols;

namespace MsgPack.Rpc.Server
{
	// This file generated from RpcServerConfiguration.tt T4Template.
	// Do not modify this file. Edit RpcServerConfiguration.tt instead.

	partial class RpcServerConfiguration
	{

		private int _minimumConnection = 2;
		
		/// <summary>
		/// 	Gets or sets the minimum connection to be pool for newly inbound connection.
		/// </summary>
		/// <value>
		/// 	The minimum connection to be pool for newly inbound connection. The default is 2.
		/// </value>
		public int MinimumConnection
		{
			get{ return this._minimumConnection; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateMinimumConnection( value );
				this._minimumConnection = value;
			}
		}
		
		/// <summary>
		/// 	Resets the MinimumConnection property value.
		/// </summary>
		public void ResetMinimumConnection()
		{
			this._minimumConnection = 2;
		}
		
		static partial void ValidateMinimumConnection( int value );

		private int _maximumConnection = 100;
		
		/// <summary>
		/// 	Gets or sets the maximum connection to be handle inbound connection.
		/// </summary>
		/// <value>
		/// 	The minimum connection to be handle inbound connection. The default is 100.
		/// </value>
		public int MaximumConnection
		{
			get{ return this._maximumConnection; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateMaximumConnection( value );
				this._maximumConnection = value;
			}
		}
		
		/// <summary>
		/// 	Resets the MaximumConnection property value.
		/// </summary>
		public void ResetMaximumConnection()
		{
			this._maximumConnection = 100;
		}
		
		static partial void ValidateMaximumConnection( int value );

		private int _minimumConcurrentRequest = 2;
		
		/// <summary>
		/// 	Gets or sets the minimum concurrency for the each clients.
		/// </summary>
		/// <value>
		/// 	The minimum concurrency for the each clients. The default is 2.
		/// </value>
		public int MinimumConcurrentRequest
		{
			get{ return this._minimumConcurrentRequest; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateMinimumConcurrentRequest( value );
				this._minimumConcurrentRequest = value;
			}
		}
		
		/// <summary>
		/// 	Resets the MinimumConcurrentRequest property value.
		/// </summary>
		public void ResetMinimumConcurrentRequest()
		{
			this._minimumConcurrentRequest = 2;
		}
		
		static partial void ValidateMinimumConcurrentRequest( int value );

		private int _maximumConcurrentRequest = 10;
		
		/// <summary>
		/// 	Gets or sets the maximum concurrency for the each clients.
		/// </summary>
		/// <value>
		/// 	The maximum concurrency for the each clients. The default is 10.
		/// </value>
		public int MaximumConcurrentRequest
		{
			get{ return this._maximumConcurrentRequest; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateMaximumConcurrentRequest( value );
				this._maximumConcurrentRequest = value;
			}
		}
		
		/// <summary>
		/// 	Resets the MaximumConcurrentRequest property value.
		/// </summary>
		public void ResetMaximumConcurrentRequest()
		{
			this._maximumConcurrentRequest = 10;
		}
		
		static partial void ValidateMaximumConcurrentRequest( int value );

		private EndPoint _bindingEndPoint = null;
		
		/// <summary>
		/// 	Gets or sets the local end point to be bound.
		/// </summary>
		/// <value>
		/// 	The local end point to be bound. The default is <c>null</c>. The server will select appropriate version IP and bind to it with port 0.
		/// </value>
		public EndPoint BindingEndPoint
		{
			get{ return this._bindingEndPoint; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateBindingEndPoint( value );
				this._bindingEndPoint = value;
			}
		}
		
		/// <summary>
		/// 	Resets the BindingEndPoint property value.
		/// </summary>
		public void ResetBindingEndPoint()
		{
			this._bindingEndPoint = null;
		}
		
		static partial void ValidateBindingEndPoint( EndPoint value );

		private int _listenBackLog = 100;
		
		/// <summary>
		/// 	Gets or sets the listen back log of each sockets.
		/// </summary>
		/// <value>
		/// 	The listen back log of each sockets. The default is 100.
		/// </value>
		public int ListenBackLog
		{
			get{ return this._listenBackLog; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateListenBackLog( value );
				this._listenBackLog = value;
			}
		}
		
		/// <summary>
		/// 	Resets the ListenBackLog property value.
		/// </summary>
		public void ResetListenBackLog()
		{
			this._listenBackLog = 100;
		}
		
		static partial void ValidateListenBackLog( int value );

		private int _portNumber = 10912;
		
		/// <summary>
		/// 	Gets or sets the listening port number.
		/// </summary>
		/// <value>
		/// 	The listening port number. The default is 10912.
		/// </value>
		public int PortNumber
		{
			get{ return this._portNumber; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidatePortNumber( value );
				this._portNumber = value;
			}
		}
		
		/// <summary>
		/// 	Resets the PortNumber property value.
		/// </summary>
		public void ResetPortNumber()
		{
			this._portNumber = 10912;
		}
		
		static partial void ValidatePortNumber( int value );

		private Func<ServiceTypeLocator> _serviceTypeLocatorProvider = () => new DefaultServiceTypeLocator();
		
		/// <summary>
		/// 	Gets or sets the factory function which creates new <see cref="ServiceTypeLocator" />.
		/// </summary>
		/// <value>
		/// 	The factory function which creates new <see cref="ServiceTypeLocator" />. The default is the delegate which creates <see cref="DefaultServiceTypeLocator" /> instance.
		/// </value>
		public Func<ServiceTypeLocator> ServiceTypeLocatorProvider
		{
			get{ return this._serviceTypeLocatorProvider; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateServiceTypeLocatorProvider( value );
				this._serviceTypeLocatorProvider = value;
			}
		}
		
		/// <summary>
		/// 	Resets the ServiceTypeLocatorProvider property value.
		/// </summary>
		public void ResetServiceTypeLocatorProvider()
		{
			this._serviceTypeLocatorProvider = () => new DefaultServiceTypeLocator();
		}
		
		static partial void ValidateServiceTypeLocatorProvider( Func<ServiceTypeLocator> value );

		private Func<Func<ServerRequestContext>, ObjectPool<ServerRequestContext>> _requestContextPoolProvider = ( factory ) => new StandardObjectPool<ServerRequestContext>( factory, null );
		
		/// <summary>
		/// 	Gets or sets the factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="ServerRequestContext" />.
		/// </summary>
		/// <value>
		/// 	The factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="ServerRequestContext" />. The default is the delegate which creates <see cref="StandardObjectPool{T}" /> instance with <c>null</c> configuration.
		/// </value>
		public Func<Func<ServerRequestContext>, ObjectPool<ServerRequestContext>> RequestContextPoolProvider
		{
			get{ return this._requestContextPoolProvider; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateRequestContextPoolProvider( value );
				this._requestContextPoolProvider = value;
			}
		}
		
		/// <summary>
		/// 	Resets the RequestContextPoolProvider property value.
		/// </summary>
		public void ResetRequestContextPoolProvider()
		{
			this._requestContextPoolProvider = ( factory ) => new StandardObjectPool<ServerRequestContext>( factory, null );
		}
		
		static partial void ValidateRequestContextPoolProvider( Func<Func<ServerRequestContext>, ObjectPool<ServerRequestContext>> value );

		private Func<Func<ServerResponseContext>, ObjectPool<ServerResponseContext>> _responseContextPoolProvider = ( factory ) => new StandardObjectPool<ServerResponseContext>( factory, null );
		
		/// <summary>
		/// 	Gets or sets the factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="ServerResponseContext" />.
		/// </summary>
		/// <value>
		/// 	The factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="ServerResponseContext" />. The default is the delegate which creates <see cref="StandardObjectPool{T}" /> instance with <c>null</c> configuration.
		/// </value>
		public Func<Func<ServerResponseContext>, ObjectPool<ServerResponseContext>> ResponseContextPoolProvider
		{
			get{ return this._responseContextPoolProvider; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateResponseContextPoolProvider( value );
				this._responseContextPoolProvider = value;
			}
		}
		
		/// <summary>
		/// 	Resets the ResponseContextPoolProvider property value.
		/// </summary>
		public void ResetResponseContextPoolProvider()
		{
			this._responseContextPoolProvider = ( factory ) => new StandardObjectPool<ServerResponseContext>( factory, null );
		}
		
		static partial void ValidateResponseContextPoolProvider( Func<Func<ServerResponseContext>, ObjectPool<ServerResponseContext>> value );

		private Func<Func<ListeningContext>, ObjectPool<ListeningContext>> _listeningContextPoolProvider = ( factory ) => new StandardObjectPool<ListeningContext>( factory, null );
		
		/// <summary>
		/// 	Gets or sets the factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="ListeningContext" />.
		/// </summary>
		/// <value>
		/// 	The factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="ListeningContext" />. The default is the delegate which creates <see cref="StandardObjectPool{T}" /> instance with <c>null</c> configuration.
		/// </value>
		public Func<Func<ListeningContext>, ObjectPool<ListeningContext>> ListeningContextPoolProvider
		{
			get{ return this._listeningContextPoolProvider; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateListeningContextPoolProvider( value );
				this._listeningContextPoolProvider = value;
			}
		}
		
		/// <summary>
		/// 	Resets the ListeningContextPoolProvider property value.
		/// </summary>
		public void ResetListeningContextPoolProvider()
		{
			this._listeningContextPoolProvider = ( factory ) => new StandardObjectPool<ListeningContext>( factory, null );
		}
		
		static partial void ValidateListeningContextPoolProvider( Func<Func<ListeningContext>, ObjectPool<ListeningContext>> value );

		private Func<Func<TcpServerTransport>, ObjectPool<TcpServerTransport>> _tcpTransportPoolProvider = ( factory ) => new StandardObjectPool<TcpServerTransport>( factory, null );
		
		/// <summary>
		/// 	Gets or sets the factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="TcpServerTransport" />.
		/// </summary>
		/// <value>
		/// 	The factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="TcpServerTransport" />. The default is the delegate which creates <see cref="StandardObjectPool{T}" /> instance with <c>null</c> configuration.
		/// </value>
		public Func<Func<TcpServerTransport>, ObjectPool<TcpServerTransport>> TcpTransportPoolProvider
		{
			get{ return this._tcpTransportPoolProvider; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateTcpTransportPoolProvider( value );
				this._tcpTransportPoolProvider = value;
			}
		}
		
		/// <summary>
		/// 	Resets the TcpTransportPoolProvider property value.
		/// </summary>
		public void ResetTcpTransportPoolProvider()
		{
			this._tcpTransportPoolProvider = ( factory ) => new StandardObjectPool<TcpServerTransport>( factory, null );
		}
		
		static partial void ValidateTcpTransportPoolProvider( Func<Func<TcpServerTransport>, ObjectPool<TcpServerTransport>> value );

		private Func<Func<UdpServerTransport>, ObjectPool<UdpServerTransport>> _udpTransportPoolProvider = ( factory ) => new StandardObjectPool<UdpServerTransport>( factory, null );
		
		/// <summary>
		/// 	Gets or sets the factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="UdpServerTransport" />.
		/// </summary>
		/// <value>
		/// 	The factory function which creates new <see cref="ObjectPool{T}" /> of <see cref="UdpServerTransport" />. The default is the delegate which creates <see cref="StandardObjectPool{T}" /> instance with <c>null</c> configuration.
		/// </value>
		public Func<Func<UdpServerTransport>, ObjectPool<UdpServerTransport>> UdpTransportPoolProvider
		{
			get{ return this._udpTransportPoolProvider; }
			set
			{
				this.VerifyIsNotFrozen();
				ValidateUdpTransportPoolProvider( value );
				this._udpTransportPoolProvider = value;
			}
		}
		
		/// <summary>
		/// 	Resets the UdpTransportPoolProvider property value.
		/// </summary>
		public void ResetUdpTransportPoolProvider()
		{
			this._udpTransportPoolProvider = ( factory ) => new StandardObjectPool<UdpServerTransport>( factory, null );
		}
		
		static partial void ValidateUdpTransportPoolProvider( Func<Func<UdpServerTransport>, ObjectPool<UdpServerTransport>> value );
	}
}
