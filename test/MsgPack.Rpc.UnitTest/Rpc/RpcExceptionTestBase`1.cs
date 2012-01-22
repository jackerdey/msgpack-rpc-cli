#region -- License Terms --
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using NUnit.Framework;

namespace MsgPack.Rpc
{
	/// <summary>
	///Tests the Rpc Exception 
	/// </summary>
	public abstract class RpcExceptionTestBase<TRpcException> : MarshalByRefObject
		where TRpcException : RpcException
	{
		protected abstract RpcError DefaultError
		{
			get;
		}

		protected virtual string DefaultMessage
		{
			get { return this.DefaultError.DefaultMessage; }
		}

		protected RpcException NewRpcException( RpcError rpcError, string message, string debugInformation )
		{
			return new RpcException( rpcError, message, debugInformation );
		}

		protected virtual TRpcException NewRpcException( ConstructorKind kind, IDictionary<string, object> properties )
		{
			switch ( kind )
			{
				case ConstructorKind.Serialization:
				case ConstructorKind.WithInnerException:
				{
					return ( TRpcException )Activator.CreateInstance( typeof( TRpcException ), GetRpcError( properties ), GetMessage( properties ), GetDebugInformation( properties ), GetInnerException( properties ) );
				}
				case ConstructorKind.Default:
				default:
				{
					return ( TRpcException )Activator.CreateInstance( typeof( TRpcException ), GetRpcError( properties ), GetMessage( properties ), GetDebugInformation( properties ) );
				}
			}
		}

		protected virtual TRpcException NewRpcException( RpcError rpcError, MessagePackObject unpackedException )
		{
			return ( TRpcException )typeof( TRpcException ).GetConstructor( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof( RpcError ), typeof( MessagePackObject ) }, null ).Invoke( new object[] { rpcError, unpackedException } );
		}

		protected virtual IDictionary<string, object> GetTestArguments()
		{
			return
				SetRpcError(
					new Dictionary<string, object>()
					{
						{ "Message", Guid.NewGuid().ToString() },
						{ "DebugInformation", Guid.NewGuid().ToString() },
						{ "InnerException", new Exception() }
					},
					this.DefaultError
				);
		}

		protected virtual IDictionary<string, object> GetTestArgumentsWithAllNullValue()
		{
			return
				SetRpcError(
					new Dictionary<string, object>()
					{
						{ "Message", null },
						{ "DebugInformation", null },
						{ "InnerException", null }
					},
					null
				);
		}

		protected virtual IDictionary<string, object> GetTestArgumentsWithDefaultValue()
		{
			return
				SetRpcError(
					new Dictionary<string, object>()
					{
						{ "Message", this.DefaultMessage },
						{ "DebugInformation", String.Empty },
						{ "InnerException", null }
					},
					this.DefaultError
				);
		}

		protected virtual void AssertProperties( TRpcException target, ConstructorKind kind, IDictionary<string, object> properties )
		{
			Assert.That( target.Message, Is.EqualTo( GetMessage( properties ) ), "Message" );
			if ( kind != ConstructorKind.WithoutDebugInformation )
			{
				Assert.That( target.DebugInformation, Is.EqualTo( GetDebugInformation( properties ) ), "DebugInformation" );
			}
			Assert.That( target.RpcError, Is.EqualTo( GetRpcError( properties ) ), "RpcError" );
			switch ( kind )
			{
				case ConstructorKind.WithInnerException:
				{
					Assert.That( target.InnerException, Is.SameAs( GetInnerException( properties ) ), "InnerException" );
					break;
				}
				case ConstructorKind.Serialization:
				{
					if ( GetInnerException( properties ) == null )
					{
						Assert.That( target.InnerException, Is.Null );
					}
					else
					{
						Assert.That( target.InnerException, Is.Not.Null, "InnerException" );
						Assert.That( target.InnerException.ToString(), Is.EqualTo( GetInnerException( properties ).ToString() ), "InnerException" );
					}
					break;
				}
			}
		}

		protected static string GetMessage( IDictionary<string, object> properties )
		{
			return ( string )properties[ "Message" ];
		}

		protected static string GetDebugInformation( IDictionary<string, object> properties )
		{
			return ( string )properties[ "DebugInformation" ];
		}

		protected static RpcError GetRpcError( IDictionary<string, object> properties )
		{
			if ( properties[ "RpcErrorIdentifier" ] == null && properties[ "RpcErrorCode" ] == null )
			{
				return null;
			}
			else
			{
				return RpcError.FromIdentifier( ( string )properties[ "RpcErrorIdentifier" ], ( int? )properties[ "RpcErrorCode" ] );
			}
		}

		protected static IDictionary<string, object> SetRpcError( IDictionary<string, object> properties, RpcError rpcError )
		{
			properties[ "RpcErrorIdentifier" ] = rpcError == null ? null : rpcError.Identifier;
			properties[ "RpcErrorCode" ] = rpcError == null ? default( int? ) : rpcError.ErrorCode;
			return properties;
		}

		protected static Exception GetInnerException( IDictionary<string, object> properties )
		{
			return ( Exception )properties[ "InnerException" ];
		}

		/// <summary>
		/// Tests the Constructor Rpc Exception Rpc Error Message Debug Information 
		/// </summary>
		[Test()]
		public void TestConstructor_RpcErrorMessageDebugInformation_Supplied_SetAsIs()
		{
			var properties = this.GetTestArguments();

			TRpcException target = this.NewRpcException( ConstructorKind.Default, properties );

			this.AssertProperties( target, ConstructorKind.Default, properties );
		}

		[Test()]
		public void TestConstructor_RpcErrorMessageDebugInformation_NotSupplied_SetDefault()
		{
			var properties = this.GetTestArgumentsWithAllNullValue();

			TRpcException target = this.NewRpcException( ConstructorKind.Default, properties );

			this.AssertProperties( target, ConstructorKind.Default, this.GetTestArgumentsWithDefaultValue() );
		}

		/// <summary>
		/// Tests the Constructor Rpc Exception Rpc Error Message Debug Information Inner 
		/// </summary>
		[Test()]
		public void TestConstructor_RpcErrorMessageDebugInformationInner_Supplied_SetAsIs()
		{
			var properties = this.GetTestArguments();

			TRpcException target = this.NewRpcException( ConstructorKind.WithInnerException, properties );

			this.AssertProperties( target, ConstructorKind.WithInnerException, properties );
		}

		[Test()]
		public void TestConstructor_RpcErrorMessageDebugInformationInner_NotSupplied_SetDefault()
		{
			var properties = this.GetTestArgumentsWithAllNullValue();

			TRpcException target = this.NewRpcException( ConstructorKind.WithInnerException, properties );

			this.AssertProperties( target, ConstructorKind.WithInnerException, this.GetTestArgumentsWithDefaultValue() );
		}

		[Test]
		public void TestSerialization_PropertiesSupplied_DeserializedNormally()
		{
			var properties = this.GetTestArguments();

			TRpcException target = this.NewRpcException( ConstructorKind.Serialization, properties );

			TRpcException result;
			using ( var buffer = new MemoryStream() )
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize( buffer, target );
				buffer.Position = 0;
				result = ( TRpcException )serializer.Deserialize( buffer );
			}

			this.AssertProperties( target, ConstructorKind.Serialization, properties );
		}

		[Test]
		public void TestSerialization_PropertiesAreNotSupplied_DeserializedNormally()
		{
			var properties = this.GetTestArgumentsWithAllNullValue();

			TRpcException target = this.NewRpcException( ConstructorKind.Serialization, properties );

			TRpcException result;
			using ( var buffer = new MemoryStream() )
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize( buffer, target );
				buffer.Position = 0;
				result = ( TRpcException )serializer.Deserialize( buffer );
			}

			this.AssertProperties( target, ConstructorKind.Serialization, this.GetTestArgumentsWithDefaultValue() );
		}

		[Test()]
		public void TestGetExceptionMessage_True_IncludesDebugInformation()
		{
			bool includesDebugInformation = true;
			var properties = this.GetTestArguments();

			TRpcException target = NewRpcException( ConstructorKind.WithInnerException, properties );
			MessagePackObject result = target.GetExceptionMessage( includesDebugInformation );
			var asDictionary = result.AsDictionary();

			Assert.That( asDictionary.Values.Any( item => item == GetRpcError( properties ).ErrorCode ) );
			Assert.That( asDictionary.Values.Any( item => item.IsTypeOf<string>().GetValueOrDefault() && item.AsString().Contains( GetMessage( properties ) ) ), Is.True );
			Assert.That( asDictionary.Values.Any( item => item.IsTypeOf<string>().GetValueOrDefault() && item.AsString().Contains( GetDebugInformation( properties ) ) ), Is.True );
		}

		/// <summary>
		/// Tests the Get Exception Message Includes Debug Information 
		/// </summary>
		[Test()]
		public void TestGetExceptionMessage_False_DoesNotIncludeDebugInformation()
		{
			bool includesDebugInformation = false;
			var properties = this.GetTestArguments();

			TRpcException target = NewRpcException( ConstructorKind.WithInnerException, properties );
			MessagePackObject result = target.GetExceptionMessage( includesDebugInformation );
			var asDictionary = result.AsDictionary();

			Assert.That( asDictionary.Values.Any( item => item == GetRpcError( properties ).ErrorCode ) );
			Assert.That( asDictionary.Values.Any( item => item.IsTypeOf<string>().GetValueOrDefault() && item.AsString().Contains( GetMessage( properties ) ) ), Is.False );
			Assert.That( asDictionary.Values.Any( item => item.IsTypeOf<string>().GetValueOrDefault() && item.AsString().Contains( GetRpcError( properties ).DefaultMessageInvariant ) ), Is.True );
			Assert.That( asDictionary.Values.Any( item => item.IsTypeOf<string>().GetValueOrDefault() && item.AsString().Contains( GetDebugInformation( properties ) ) ), Is.False );
		}

		[Test()]
		public void TestConstructor_RpcErrorMessagePackObject_True_IncludesDebugInformation()
		{
			bool includesDebugInformation = true;
			var properties = this.GetTestArguments();

			TRpcException target = NewRpcException( ConstructorKind.WithInnerException, properties );
			MessagePackObject unpackedException = target.GetExceptionMessage( includesDebugInformation );
			var result = NewRpcException( GetRpcError( properties ), unpackedException );

			this.AssertProperties( result, ConstructorKind.Default, properties );
		}

		[Test()]
		public void TestConstructor_RpcErrorMessagePackObject_False_DoesNotIncludeDebugInformation()
		{
			bool includesDebugInformation = true;
			var properties = this.GetTestArguments();

			TRpcException target = NewRpcException( ConstructorKind.WithInnerException, properties );
			MessagePackObject unpackedException = target.GetExceptionMessage( includesDebugInformation );
			var result = NewRpcException( GetRpcError( properties ), unpackedException );

			this.AssertProperties( result, ConstructorKind.WithoutDebugInformation, properties );
		}

		[Test]
		public void TestSerialization_PropertiesSupplied_PartialTrust_DeserializedNormally()
		{
			DoTestWithPartialTrust(
				( proxy ) => proxy.GetTestArguments(),
				( proxy, properties ) => proxy.NewRpcException( ConstructorKind.Serialization, properties ),
				( proxy, result, properties ) => proxy.AssertProperties( result, ConstructorKind.Serialization, properties )
			);
		}

		[Test]
		public void TestSerialization_PropertiesAreNotSupplied_PartialTrust_DeserializedNormally()
		{
			DoTestWithPartialTrust(
				( proxy ) => proxy.GetTestArgumentsWithAllNullValue(),
				( proxy, properties ) => proxy.NewRpcException( ConstructorKind.Serialization, properties ),
				( proxy, result, properties ) => proxy.AssertProperties( result, ConstructorKind.Serialization, proxy.GetTestArgumentsWithDefaultValue() )
			);
		}

		private static StrongName GetStrongName( Type type )
		{
			var assemblyName = type.Assembly.GetName();
			return new StrongName( new StrongNamePublicKeyBlob( assemblyName.GetPublicKey() ), assemblyName.Name, assemblyName.Version );
		}

		private void DoTestWithPartialTrust(
			Func<RpcExceptionTestBase<TRpcException>, IDictionary<string, object>> propertiesGetter,
			Func<RpcExceptionTestBase<TRpcException>, IDictionary<string, object>, TRpcException> instanceCreator,
			Action<RpcExceptionTestBase<TRpcException>, TRpcException, IDictionary<string, object>> assertion
		)
		{
			var appDomainSetUp = new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase };
			var evidence = new Evidence();
			evidence.AddHostEvidence( new Zone( SecurityZone.Internet ) );
			var permisions = SecurityManager.GetStandardSandbox( evidence );
			AppDomain workerDomain = AppDomain.CreateDomain( "PartialTrust", evidence, appDomainSetUp, permisions, GetStrongName( this.GetType() ) );
			try
			{
				var proxyHandle = Activator.CreateInstance( workerDomain, this.GetType().Assembly.FullName, this.GetType().FullName );
				var proxy = ( RpcExceptionTestBase<TRpcException> )proxyHandle.Unwrap();
				var properties = propertiesGetter( proxy );
				var result = instanceCreator( proxy, properties );
				assertion( proxy, result, properties );
			}
			catch ( SecurityException ex )
			{
				Assert.Fail(
					String.Format(
						CultureInfo.CurrentCulture,
						"{1}:{2}{0}" +
						"Action:{3}{0}" +
						"Demanded:{4}{0}" +
						"DenySetInstance:{5}{0}" +
						"FailedAssemblyInfo:{6}{0}" +
						"FirstPermissionThatFailed:{7}{0}" +
						"GrantedSet:{8}{0}" +
						"Method:{9}{0}" +
						"PermissionState:{10}{0}" +
						"PermissionType:{11}{0}" +
						"PermitOnlySetInstance:{12}{0}" +
						"RefusedSet:{13}{0}" +
						"Url:{14}{0}" +
						"Zone:{15}{0}" +
						"StackTrace:{0}{16}",
						Environment.NewLine,
						ex.GetType(),
						ex.Message,
						ex.Action,
						ex.Demanded,
						ex.DenySetInstance,
						ex.FailedAssemblyInfo,
						ex.FirstPermissionThatFailed,
						ex.GrantedSet,
						ex.Method,
						ex.PermissionState,
						ex.PermissionType,
						ex.PermitOnlySetInstance,
						ex.RefusedSet,
						ex.Url,
						ex.Zone,
						ex.StackTrace
					)
				);
			}
			finally
			{
				AppDomain.Unload( workerDomain );
			}
		}

		[Serializable]
		protected enum ConstructorKind
		{
			Default,
			WithInnerException,
			Serialization,
			WithoutDebugInformation
		}
	}
}