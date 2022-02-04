using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.CodeGeneration;

namespace TAO3.Internal.Commands.ConnectMSSQL
{
    internal class ConnectMSSQLOptions
    {
        public KernelInvocationContext Context { get; set; } = null!;
        public string? KernelName { get; set; }

        //
        // Summary:
        //     Gets or sets the connection string associated with the System.Data.Common.DbConnectionStringBuilder.
        //
        // Returns:
        //     The current connection string, created from the key/value pairs that are contained
        //     within the System.Data.Common.DbConnectionStringBuilder. The default value is
        //     an empty string.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     An invalid connection string argument has been supplied.
        [RefreshProperties(RefreshProperties.All)]
        public string? ConnectionString { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum number of connections allowed in the connection pool
        //     for this specific connection string.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.MinPoolSize
        //     property, or 0 if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Min Pool Size" key within the connection
        //     string.
        [DisplayName("Min Pool Size")]
        [RefreshProperties(RefreshProperties.All)]
        public int? MinPoolSize { get; set; }
        //
        // Summary:
        //     When true, an application can maintain multiple active result sets (MARS). When
        //     false, an application must process or cancel all result sets from one batch before
        //     it can execute any other batch on that connection. For more information, see
        //     [Multiple Active Result Sets (MARS)](https://msdn.microsoft.com//library/cfa084cz.aspx).
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.MultipleActiveResultSets
        //     property, or false if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "MultipleActiveResultSets" key within
        //     the connection string. ## Examples The following example explicitly disables
        //     the Multiple Active Result Sets feature. [!code-csharp[SqlConnectionStringBuilder_MultipleActiveResultSets.MARS#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_MultipleActiveResultSets.cs#1)]
        [DisplayName("Multiple Active Result Sets")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? MultipleActiveResultSets { get; set; }
        //
        // Summary:
        //     If your application is connecting to an AlwaysOn availability group (AG) on different
        //     subnets, setting MultiSubnetFailover=true provides faster detection of and connection
        //     to the (currently) active server. For more information about SqlClient support
        //     for Always On Availability Groups, see [SqlClient Support for High Availability,
        //     Disaster Recovery](/dotnet/framework/data/adonet/sql/sqlclient-support-for-high-availability-disaster-recovery).
        //
        // Value:
        //     Returns System.Boolean indicating the current value of the property.
        //
        // Remarks:
        //     To be added.
        [DisplayName("Multi Subnet Failover")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? MultiSubnetFailover { get; set; }
        //
        // Summary:
        //     Gets or sets the size in bytes of the network packets used to communicate with
        //     an instance of SQL Server.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.PacketSize
        //     property, or 8000 if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Packet Size" key within the connection
        //     string.
        [DisplayName("Packet Size")]
        [RefreshProperties(RefreshProperties.All)]
        public int? PacketSize { get; set; }
        //
        // Summary:
        //     Gets or sets the password for the SQL Server account.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Password
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The password was incorrectly set to null. See code sample below.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Password" and "pwd" keys within
        //     the connection string. If <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Password%2A>
        //     has not been set and you retrieve the value, the return value is <xref:System.String.Empty>.
        //     To reset the password for the connection string, pass null to the Item property.
        //     ## Examples The following example shows how to set <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Password%2A>.
        //     [!code-csharp[SqlConnectionStringBuilder_Password#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_Password.cs#1)]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        [RefreshProperties(RefreshProperties.All)]
        public string? Password { get; set; }
        //
        // Summary:
        //     Gets or sets the password for the SQL Server account.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Password
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The password was incorrectly set to null. See code sample below.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Password" and "pwd" keys within
        //     the connection string. If <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Password%2A>
        //     has not been set and you retrieve the value, the return value is <xref:System.String.Empty>.
        //     To reset the password for the connection string, pass null to the Item property.
        //     ## Examples The following example shows how to set <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Password%2A>.
        //     [!code-csharp[SqlConnectionStringBuilder_Password#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_Password.cs#1)]
        [DisplayName("PWD")]
        [PasswordPropertyText(true)]
        [RefreshProperties(RefreshProperties.All)]
        public string? PWD { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates if security-sensitive information,
        //     such as the password, is not returned as part of the connection if the connection
        //     is open or has ever been in an open state.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.PersistSecurityInfo
        //     property, or false if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Persist Security Info" and "persistsecurityinfo"
        //     keys within the connection string.
        [DisplayName("Persist Security Info")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? PersistSecurityInfo { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates whether the connection will be pooled
        //     or explicitly opened every time that the connection is requested.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Pooling
        //     property, or true if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Pooling" key within the connection
        //     string.
        [DisplayName("Pooling")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? Pooling { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates whether replication is supported
        //     using the connection.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Replication
        //     property, or false if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Replication" key within the connection
        //     string.
        [DisplayName("Replication")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? Replication { get; set; }
        //
        // Summary:
        //     Gets or sets a string value that indicates how the connection maintains its association
        //     with an enlisted System.Transactions transaction.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.TransactionBinding
        //     property, or String.Empty if none has been supplied.
        //
        // Remarks:
        //     ## Remarks The Transaction Binding keywords in a <xref:Microsoft.Data.SqlClient.SqlConnection.ConnectionString%2A>
        //     control how a <xref:Microsoft.Data.SqlClient.SqlConnection> binds to an enlisted
        //     <xref:System.Transactions.Transaction>. The following table shows the possible
        //     values for the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.TransactionBinding%2A>
        //     property: |Value|Description| |-----------|-----------------| |Implicit Unbind|The
        //     default. Causes the connection to detach from the transaction when it ends. After
        //     detaching, additional requests on the connection are performed in autocommit
        //     mode. The <xref:System.Transactions.Transaction.Current%2A> property is not checked
        //     when executing requests while the transaction is active. After the transaction
        //     has ended, additional requests are performed in autocommit mode.| |Explicit Unbind|Causes
        //     the connection to remain attached to the transaction until the connection is
        //     closed or until <xref:Microsoft.Data.SqlClient.SqlConnection.EnlistTransaction%2A>
        //     is called with a `null` (`Nothing` in Visual Basic) value. An <xref:System.InvalidOperationException>
        //     is thrown if <xref:System.Transactions.Transaction.Current%2A> is not the enlisted
        //     transaction or if the enlisted transaction is not active. This behavior enforces
        //     the strict scoping rules required for <xref:System.Transactions.TransactionScope>
        //     support.|
        [DisplayName("Transaction Binding")]
        [RefreshProperties(RefreshProperties.All)]
        public string? TransactionBinding { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether the channel will be encrypted while
        //     bypassing walking the certificate chain to validate trust.
        //
        // Value:
        //     A Boolean. Recognized values are true, false, yes, and no.
        //
        // Remarks:
        //     ## Remarks When `TrustServerCertificate` is set to `true`, the transport layer
        //     will use SSL to encrypt the channel and bypass walking the certificate chain
        //     to validate trust. If `TrustServerCertificate` is set to `true` and encryption
        //     is turned on, the encryption level specified on the server will be used even
        //     if `Encrypt` is set to `false`. The connection will fail otherwise. For more
        //     information, see [Encryption Hierarchy](/sql/relational-databases/security/encryption/encryption-hierarchy)
        //     and [Using Encryption Without Validation](/sql/relational-databases/native-client/features/using-encryption-without-validation).
        [DisplayName("Trust Server Certificate")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? TrustServerCertificate { get; set; }
        //
        // Summary:
        //     Gets or sets a string value that indicates the type system the application expects.
        //
        // Value:
        //     The following table shows the possible values for the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.TypeSystemVersion
        //     property:
        //     Value – Description
        //     SQL Server 2005 – Uses the SQL Server 2005 type system. No conversions are made
        //     for the current version of ADO.NET.
        //     SQL Server 2008 – Uses the SQL Server 2008 type system.
        //     Latest – Use the latest version than this client-server pair can handle. This
        //     will automatically move forward as the client and server components are upgraded.
        //
        // Remarks:
        //     ## Remarks The `TypeSystemVersion` property can be used to specify a down-level
        //     version of SQL Server for applications written against that version. This avoids
        //     possible problems with incompatible types in a newer version of SQL Server that
        //     may cause the application to break.
        [DisplayName("Type System Version")]
        [RefreshProperties(RefreshProperties.All)]
        public string? TypeSystemVersion { get; set; }
        //
        // Summary:
        //     Gets or sets the user ID to be used when connecting to SQL Server.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.UserID property,
        //     or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "User ID", "user", and "uid" keys
        //     within the connection string.
        [DisplayName("User ID")]
        [RefreshProperties(RefreshProperties.All)]
        public string? UserID { get; set; }
        //
        // Summary:
        //     Gets or sets the user ID to be used when connecting to SQL Server.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.UserID property,
        //     or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "User ID", "user", and "uid" keys
        //     within the connection string.
        [DisplayName("User")]
        [RefreshProperties(RefreshProperties.All)]
        public string? User { get; set; }
        //
        // Summary:
        //     Gets or sets the user ID to be used when connecting to SQL Server.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.UserID property,
        //     or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "User ID", "user", and "uid" keys
        //     within the connection string.
        [DisplayName("UID")]
        [RefreshProperties(RefreshProperties.All)]
        public string? UID { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether to redirect the connection from the
        //     default SQL Server Express instance to a runtime-initiated instance running under
        //     the account of the caller.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.UserInstance
        //     property, or False if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "User Instance" key within the connection
        //     string. > [!NOTE] > This feature is available only with the SQL Server Express
        //     Edition. For more information on user instances, see [SQL Server Express User
        //     Instances](/dotnet/framework/data/adonet/sql/sql-server-express-user-instances).
        [DisplayName("User Instance")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? UserInstance { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the workstation connecting to SQL Server.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.WorkstationID
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Workstation ID" and "wsid" keys
        //     within the connection string.
        [DisplayName("Workstation ID")]
        [RefreshProperties(RefreshProperties.All)]
        public string? WorkstationID { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the workstation connecting to SQL Server.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.WorkstationID
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Workstation ID" and "wsid" keys
        //     within the connection string.
        [DisplayName("WSID")]
        [RefreshProperties(RefreshProperties.All)]
        public string? WSID { get; set; }
        //
        // Summary:
        //     The blocking period behavior for a connection pool.
        //
        // Value:
        //     The available blocking period settings.
        //
        // Remarks:
        //     ## Remarks When connection pooling is enabled and a timeout error or other login
        //     error occurs, an exception will be thrown and subsequent connection attempts
        //     will fail for the next five seconds, the "blocking period". If the application
        //     attempts to connect within the blocking period, the first exception will be thrown
        //     again. Subsequent failures after a blocking period ends will result in a new
        //     blocking period that is twice as long as the previous blocking period, up to
        //     a maximum of one minute. Attempting to connect to Azure SQL databases can fail
        //     with transient errors which are typically recovered within a few seconds. However,
        //     with the connection pool blocking period behavior, you may not be able to reach
        //     your database for extensive periods even though the database is available. This
        //     is especially problematic for apps that need to render fast. The **PoolBlockingPeriod**
        //     enables you to select the blocking period best suited for your app. See the <xref:Microsoft.Data.SqlClient.PoolBlockingPeriod>
        //     enumeration for available settings.
        [DisplayName("Pool Blocking Period")]
        [RefreshProperties(RefreshProperties.All)]
        public PoolBlockingPeriod? PoolBlockingPeriod { get; set; }
        //
        // Summary:
        //     Gets or sets the column encryption settings for the connection string builder.
        //
        // Value:
        //     The column encryption settings for the connection string builder.
        //
        // Remarks:
        //     To be added.
        [DisplayName("Column Encryption Setting")]
        [RefreshProperties(RefreshProperties.All)]
        public SqlConnectionColumnEncryptionSetting? ColumnEncryptionSetting { get; set; }
        //
        // Summary:
        //     Gets or sets the maximum number of connections allowed in the connection pool
        //     for this specific connection string.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.MaxPoolSize
        //     property, or 100 if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Max Pool Size" key within the connection
        //     string.
        [DisplayName("Max Pool Size")]
        [RefreshProperties(RefreshProperties.All)]
        public int? MaxPoolSize { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum time, in seconds, for the connection to live in the
        //     connection pool before being destroyed.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.LoadBalanceTimeout
        //     property, or 0 if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Load Balance Timeout" and "connection
        //     lifetime" keys within the connection string.
        [DisplayName("Connection Lifetime")]
        [RefreshProperties(RefreshProperties.All)]
        public int? ConnectionLifetime { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum time, in seconds, for the connection to live in the
        //     connection pool before being destroyed.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.LoadBalanceTimeout
        //     property, or 0 if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Load Balance Timeout" and "connection
        //     lifetime" keys within the connection string.
        [DisplayName("Load Balance Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        public int? LoadBalanceTimeout { get; set; }
        //
        // Summary:
        //     Set/Get the value of Attestation Protocol.
        //
        // Returns:
        //     Returns Attestation Protocol.
        [DisplayName("Attestation Protocol")]
        [RefreshProperties(RefreshProperties.All)]
        public SqlConnectionAttestationProtocol? AttestationProtocol { get; set; }
        //
        // Summary:
        //     Gets or sets the enclave attestation Url to be used with enclave based Always
        //     Encrypted.
        //
        // Value:
        //     The enclave attestation Url.
        //
        // Remarks:
        //     To be added.
        [DisplayName("Enclave Attestation Url")]
        [RefreshProperties(RefreshProperties.All)]
        public string? EnclaveAttestationUrl { get; set; }
        //
        // Summary:
        //     Declares the application workload type when connecting to a database in an SQL
        //     Server Availability Group. You can set the value of this property with Microsoft.Data.SqlClient.ApplicationIntent.
        //     For more information about SqlClient support for Always On Availability Groups,
        //     see [SqlClient Support for High Availability, Disaster Recovery](/dotnet/framework/data/adonet/sql/sqlclient-support-for-high-availability-disaster-recovery).
        //
        // Value:
        //     Returns the current value of the property (a value of type Microsoft.Data.SqlClient.ApplicationIntent).
        //
        // Remarks:
        //     To be added.
        [DisplayName("Application Intent")]
        [RefreshProperties(RefreshProperties.All)]
        public ApplicationIntent? ApplicationIntent { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the application associated with the connection string.
        //
        // Value:
        //     The name of the application, or ".NET SqlClient Data Provider" if no name has
        //     been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Application Name" and "app" keys
        //     within the connection string. ## Examples The following example creates a new
        //     <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> and assigns a connection
        //     string in the object's constructor. The code displays the parsed and recreated
        //     version of the connection string, and then modifies the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ApplicationName%2A>
        //     property of the object. Finally, the code displays the new connection string,
        //     including the new key/value pair. [!code-csharp[SqlConnectionStringBuilder.ApplicationName#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_ApplicationName.cs#1)]
        //     The sample displays the following text in the console window: ``` Original: Data
        //     Source=(local);Initial Catalog=AdventureWorks;Integrated Security=True ApplicationName=".Net
        //     SqlClient Data Provider" Modified: Data Source=(local);Initial Catalog=AdventureWorks;Integrated
        //     Security=True;Application Name="My Application" ```
        [DisplayName("Application Name")]
        [RefreshProperties(RefreshProperties.All)]
        public string? ApplicationName { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the application associated with the connection string.
        //
        // Value:
        //     The name of the application, or ".NET SqlClient Data Provider" if no name has
        //     been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Application Name" and "app" keys
        //     within the connection string. ## Examples The following example creates a new
        //     <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> and assigns a connection
        //     string in the object's constructor. The code displays the parsed and recreated
        //     version of the connection string, and then modifies the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ApplicationName%2A>
        //     property of the object. Finally, the code displays the new connection string,
        //     including the new key/value pair. [!code-csharp[SqlConnectionStringBuilder.ApplicationName#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_ApplicationName.cs#1)]
        //     The sample displays the following text in the console window: ``` Original: Data
        //     Source=(local);Initial Catalog=AdventureWorks;Integrated Security=True ApplicationName=".Net
        //     SqlClient Data Provider" Modified: Data Source=(local);Initial Catalog=AdventureWorks;Integrated
        //     Security=True;Application Name="My Application" ```
        [DisplayName("App")]
        [RefreshProperties(RefreshProperties.All)]
        public string? App { get; set; }
        //
        // Summary:
        //     Gets or sets a string that contains the name of the primary data file. This includes
        //     the full path name of an attachable database.
        //
        // Value:
        //     The value of the AttachDBFilename property, or String.Empty if no value has been
        //     supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "AttachDBFilename", "extended properties",
        //     and "initial file name" keys within the connection string. `AttachDBFilename`
        //     is only supported for primary data files with an .mdf extension. An error will
        //     be generated if a log file exists in the same directory as the data file and
        //     the 'database' keyword is used when attaching the primary data file. In this
        //     case, remove the log file. Once the database is attached, a new log file will
        //     be automatically generated based on the physical path. ## Examples The following
        //     example creates a new <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     instance, and sets the `AttachDBFilename` property in order to specify the name
        //     of an attached data file. [!code-csharp[DataWorks SqlConnectionStringBuilder_AttachDBFilename#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_AttachDBFilename.cs#1)]
        [DisplayName("AttachDbFilename")]
        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [RefreshProperties(RefreshProperties.All)]
        public string? AttachDBFilename { get; set; }
        //
        // Summary:
        //     Gets or sets a string that contains the name of the primary data file. This includes
        //     the full path name of an attachable database.
        //
        // Value:
        //     The value of the AttachDBFilename property, or String.Empty if no value has been
        //     supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "AttachDBFilename", "extended properties",
        //     and "initial file name" keys within the connection string. `AttachDBFilename`
        //     is only supported for primary data files with an .mdf extension. An error will
        //     be generated if a log file exists in the same directory as the data file and
        //     the 'database' keyword is used when attaching the primary data file. In this
        //     case, remove the log file. Once the database is attached, a new log file will
        //     be automatically generated based on the physical path. ## Examples The following
        //     example creates a new <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     instance, and sets the `AttachDBFilename` property in order to specify the name
        //     of an attached data file. [!code-csharp[DataWorks SqlConnectionStringBuilder_AttachDBFilename#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_AttachDBFilename.cs#1)]
        [DisplayName("Extended Properties")]
        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [RefreshProperties(RefreshProperties.All)]
        public string? ExtendedProperties { get; set; }
        //
        // Summary:
        //     Gets or sets a string that contains the name of the primary data file. This includes
        //     the full path name of an attachable database.
        //
        // Value:
        //     The value of the AttachDBFilename property, or String.Empty if no value has been
        //     supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "AttachDBFilename", "extended properties",
        //     and "initial file name" keys within the connection string. `AttachDBFilename`
        //     is only supported for primary data files with an .mdf extension. An error will
        //     be generated if a log file exists in the same directory as the data file and
        //     the 'database' keyword is used when attaching the primary data file. In this
        //     case, remove the log file. Once the database is attached, a new log file will
        //     be automatically generated based on the physical path. ## Examples The following
        //     example creates a new <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     instance, and sets the `AttachDBFilename` property in order to specify the name
        //     of an attached data file. [!code-csharp[DataWorks SqlConnectionStringBuilder_AttachDBFilename#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_AttachDBFilename.cs#1)]
        [DisplayName("Initial File Name")]
        [Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [RefreshProperties(RefreshProperties.All)]
        public string? InitialFileName { get; set; }
        //
        // Summary:
        //     Gets the authentication of the connection string.
        //
        // Value:
        //     The authentication of the connection string.
        //
        // Remarks:
        //     To be added.
        [DisplayName("Authentication")]
        [RefreshProperties(RefreshProperties.All)]
        public SqlAuthenticationMethod? Authentication { get; set; }
        //
        // Summary:
        //     The number of reconnections attempted after identifying that there was an idle
        //     connection failure. This must be an integer between 0 and 255. Default is 1.
        //     Set to 0 to disable reconnecting on idle connection failures. An System.ArgumentException
        //     will be thrown if set to a value outside of the allowed range.
        //
        // Value:
        //     The number of reconnections attempted after identifying that there was an idle
        //     connection failure.
        [DisplayName("Connect Retry Count")]
        [RefreshProperties(RefreshProperties.All)]
        public int? ConnectRetryCount { get; set; }
        //
        // Summary:
        //     Gets or sets the length of time (in seconds) to wait for a connection to the
        //     server before terminating the attempt and generating an error.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout
        //     property, or 15 seconds if no value has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Connect Timeout", "connection timeout",
        //     and "timeout" keys within the connection string. When opening a connection to
        //     a Azure SQL Database, set the connection timeout to 30 seconds. ## Examples The
        //     following example first displays the contents of a connection string that does
        //     not specify the "Connect Timeout" value, sets the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout%2A>
        //     property, and then displays the new connection string. [!code-csharp[SqlConnectionStringBuilder_ConnectTimeout#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_ConnectTimeout.cs#1)]
        [DisplayName("Connect Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        public int? ConnectTimeout { get; set; }
        //
        // Summary:
        //     Gets or sets the length of time (in seconds) to wait for a connection to the
        //     server before terminating the attempt and generating an error.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout
        //     property, or 15 seconds if no value has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Connect Timeout", "connection timeout",
        //     and "timeout" keys within the connection string. When opening a connection to
        //     a Azure SQL Database, set the connection timeout to 30 seconds. ## Examples The
        //     following example first displays the contents of a connection string that does
        //     not specify the "Connect Timeout" value, sets the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout%2A>
        //     property, and then displays the new connection string. [!code-csharp[SqlConnectionStringBuilder_ConnectTimeout#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_ConnectTimeout.cs#1)]
        [DisplayName("Connection Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        public int? ConnectionTimeout { get; set; }
        //
        // Summary:
        //     Gets or sets the length of time (in seconds) to wait for a connection to the
        //     server before terminating the attempt and generating an error.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout
        //     property, or 15 seconds if no value has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Connect Timeout", "connection timeout",
        //     and "timeout" keys within the connection string. When opening a connection to
        //     a Azure SQL Database, set the connection timeout to 30 seconds. ## Examples The
        //     following example first displays the contents of a connection string that does
        //     not specify the "Connect Timeout" value, sets the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout%2A>
        //     property, and then displays the new connection string. [!code-csharp[SqlConnectionStringBuilder_ConnectTimeout#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_ConnectTimeout.cs#1)]
        [DisplayName("Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        public int? Timeout { get; set; }
        //
        // Summary:
        //     Gets or sets the SQL Server Language record name.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.CurrentLanguage
        //     property, or String.Empty if no value has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Current Language" and "language"
        //     keys within the connection string.
        [DisplayName("Current Language")]
        [RefreshProperties(RefreshProperties.All)]
        public string? CurrentLanguage { get; set; }
        //
        // Summary:
        //     Gets or sets the SQL Server Language record name.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.CurrentLanguage
        //     property, or String.Empty if no value has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Current Language" and "language"
        //     keys within the connection string.
        [DisplayName("Language")]
        [RefreshProperties(RefreshProperties.All)]
        public string? Language { get; set; }
        //
        // Summary:
        //     Amount of time (in seconds) between each reconnection attempt after identifying
        //     that there was an idle connection failure. This must be an integer between 1
        //     and 60. The default is 10 seconds. An System.ArgumentException will be thrown
        //     if set to a value outside of the allowed range.
        //
        // Value:
        //     Amount of time (in seconds) between each reconnection attempt after identifying
        //     that there was an idle connection failure.
        [DisplayName("Connect Retry Interval")]
        [RefreshProperties(RefreshProperties.All)]
        public int? ConnectRetryInterval { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates whether SQL Server uses SSL encryption
        //     for all data sent between the client and server if the server has a certificate
        //     installed.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Encrypt
        //     property, or false if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Encrypt" key within the connection
        //     string.
        [DisplayName("Encrypt")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? Encrypt { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates whether the SQL Server connection
        //     pooler automatically enlists the connection in the creation thread's current
        //     transaction context.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Enlist property,
        //     or true if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Enlist" key within the connection
        //     string.
        [DisplayName("Enlist")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? Enlist { get; set; }
        //
        // Summary:
        //     Gets or sets the name or address of the partner server to connect to if the primary
        //     server is down.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.FailoverPartner
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     To be added.
        [DisplayName("Failover Partner")]
        [RefreshProperties(RefreshProperties.All)]
        public string? FailoverPartner { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the database associated with the connection.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.InitialCatalog
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Initial Catalog" and "database"
        //     keys within the connection string. ## Examples The following example creates
        //     a simple connection string and then uses the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     class to add the name of the database to the connection string. The code displays
        //     the contents of the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.InitialCatalog%2A>
        //     property, just to verify that the class was able to convert from the synonym
        //     ("Database") to the appropriate property value. [!code-csharp[SqlConnectionStringBuilder_InitialCatalog#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_InitialCatalog.cs#1)]
        [DisplayName("Initial Catalog")]
        [RefreshProperties(RefreshProperties.All)]
        public string? InitialCatalog { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the database associated with the connection.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.Database
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Initial Catalog" and "database"
        //     keys within the connection string. ## Examples The following example creates
        //     a simple connection string and then uses the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     class to add the name of the database to the connection string. The code displays
        //     the contents of the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.InitialCatalog%2A>
        //     property, just to verify that the class was able to convert from the synonym
        //     ("Database") to the appropriate property value. [!code-csharp[SqlConnectionStringBuilder_InitialCatalog#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_InitialCatalog.cs#1)]
        [DisplayName("Database")]
        [RefreshProperties(RefreshProperties.All)]
        public string? Database { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates whether User ID and Password are
        //     specified in the connection (when false) or whether the current Windows account
        //     credentials are used for authentication (when true).
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.IntegratedSecurity
        //     property, or false if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Integrated Security" and "trusted_connection"
        //     keys within the connection string. ## Examples The following example converts
        //     an existing connection string from using SQL Server Authentication to using integrated
        //     security. The example does its work by removing the user name and password from
        //     the connection string and then setting the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.IntegratedSecurity%2A>
        //     property of the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> object.
        //     > [!NOTE] > This example includes a password to demonstrate how <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     works with connection strings. In your applications, we recommend that you use
        //     Windows Authentication. If you must use a password, do not include a hard-coded
        //     password in your application. [!code-csharp[SqlConnectionStringBuilder_IntegratedSecurity#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_IntegratedSecurity.cs#1)]
        [DisplayName("Integrated Security")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? IntegratedSecurity { get; set; }
        //
        // Summary:
        //     Gets or sets a Boolean value that indicates whether User ID and Password are
        //     specified in the connection (when false) or whether the current Windows account
        //     credentials are used for authentication (when true).
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.IntegratedSecurity
        //     property, or false if none has been supplied.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Integrated Security" and "trusted_connection"
        //     keys within the connection string. ## Examples The following example converts
        //     an existing connection string from using SQL Server Authentication to using integrated
        //     security. The example does its work by removing the user name and password from
        //     the connection string and then setting the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder.IntegratedSecurity%2A>
        //     property of the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> object.
        //     > [!NOTE] > This example includes a password to demonstrate how <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder>
        //     works with connection strings. In your applications, we recommend that you use
        //     Windows Authentication. If you must use a password, do not include a hard-coded
        //     password in your application. [!code-csharp[SqlConnectionStringBuilder_IntegratedSecurity#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_IntegratedSecurity.cs#1)]
        [DisplayName("Trusted Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public bool? TrustedConnection { get; set; }
        //
        // Summary:
        //     Gets or sets the name or network address of the instance of SQL Server to connect
        //     to.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.DataSource
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Data Source", "server", "address",
        //     "addr", and "network address" keys within the connection string. Regardless of
        //     which of these values has been supplied within the supplied connection string,
        //     the connection string created by the `SqlConnectionStringBuilder` will use the
        //     well-known "Data Source" key. ## Examples The following example demonstrates
        //     that the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> class converts
        //     synonyms for the "Data Source" connection string key into the well-known key:
        //     [!code-csharp[SqlConnectionStringBuilder_DataSource#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_DataSource.cs#1)]
        [DisplayName("Data Source")]
        [RefreshProperties(RefreshProperties.All)]
        public string? DataSource { get; set; }
        //
        // Summary:
        //     Gets or sets the name or network address of the instance of SQL Server to connect
        //     to.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.DataSource
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Data Source", "server", "address",
        //     "addr", and "network address" keys within the connection string. Regardless of
        //     which of these values has been supplied within the supplied connection string,
        //     the connection string created by the `SqlConnectionStringBuilder` will use the
        //     well-known "Data Source" key. ## Examples The following example demonstrates
        //     that the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> class converts
        //     synonyms for the "Data Source" connection string key into the well-known key:
        //     [!code-csharp[SqlConnectionStringBuilder_DataSource#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_DataSource.cs#1)]
        [DisplayName("Server")]
        [RefreshProperties(RefreshProperties.All)]
        public string? Server { get; set; }
        //
        // Summary:
        //     Gets or sets the name or network address of the instance of SQL Server to connect
        //     to.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.DataSource
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Data Source", "server", "address",
        //     "addr", and "network address" keys within the connection string. Regardless of
        //     which of these values has been supplied within the supplied connection string,
        //     the connection string created by the `SqlConnectionStringBuilder` will use the
        //     well-known "Data Source" key. ## Examples The following example demonstrates
        //     that the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> class converts
        //     synonyms for the "Data Source" connection string key into the well-known key:
        //     [!code-csharp[SqlConnectionStringBuilder_DataSource#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_DataSource.cs#1)]
        [DisplayName("Address")]
        [RefreshProperties(RefreshProperties.All)]
        public string? Address { get; set; }
        //
        // Summary:
        //     Gets or sets the name or network address of the instance of SQL Server to connect
        //     to.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.DataSource
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Data Source", "server", "address",
        //     "addr", and "network address" keys within the connection string. Regardless of
        //     which of these values has been supplied within the supplied connection string,
        //     the connection string created by the `SqlConnectionStringBuilder` will use the
        //     well-known "Data Source" key. ## Examples The following example demonstrates
        //     that the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> class converts
        //     synonyms for the "Data Source" connection string key into the well-known key:
        //     [!code-csharp[SqlConnectionStringBuilder_DataSource#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_DataSource.cs#1)]
        [DisplayName("Addr")]
        [RefreshProperties(RefreshProperties.All)]
        public string? Addr { get; set; }
        //
        // Summary:
        //     Gets or sets the name or network address of the instance of SQL Server to connect
        //     to.
        //
        // Value:
        //     The value of the Microsoft.Data.SqlClient.SqlConnectionStringBuilder.DataSource
        //     property, or String.Empty if none has been supplied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     To set the value to null, use System.DBNull.Value.
        //
        // Remarks:
        //     ## Remarks This property corresponds to the "Data Source", "server", "address",
        //     "addr", and "network address" keys within the connection string. Regardless of
        //     which of these values has been supplied within the supplied connection string,
        //     the connection string created by the `SqlConnectionStringBuilder` will use the
        //     well-known "Data Source" key. ## Examples The following example demonstrates
        //     that the <xref:Microsoft.Data.SqlClient.SqlConnectionStringBuilder> class converts
        //     synonyms for the "Data Source" connection string key into the well-known key:
        //     [!code-csharp[SqlConnectionStringBuilder_DataSource#1](~/../sqlclient/doc/samples/SqlConnectionStringBuilder_DataSource.cs#1)]
        [DisplayName("Network Address")]
        [RefreshProperties(RefreshProperties.All)]
        public string? NetworkAddress { get; set; }

        public string GetConnectionString()
        {
            if (ConnectionString != null)
            {
                return ConnectionString;
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            if (MinPoolSize != null)
            {
                builder.MinPoolSize = MinPoolSize.Value;
            }

            if (MultipleActiveResultSets != null)
            {
                builder.MultipleActiveResultSets = MultipleActiveResultSets.Value;
            }

            if (MultiSubnetFailover != null)
            {
                builder.MultiSubnetFailover = MultiSubnetFailover.Value;
            }

            if (PacketSize != null)
            {
                builder.PacketSize = PacketSize.Value;
            }

            if (Password != null)
            {
                builder.Password = Password;
            }

            if (PersistSecurityInfo != null)
            {
                builder.PersistSecurityInfo = PersistSecurityInfo.Value;
            }

            if (Pooling != null)
            {
                builder.Pooling = Pooling.Value;
            }

            if (Replication != null)
            {
                builder.Replication = Replication.Value;
            }

            if (TransactionBinding != null)
            {
                builder.TransactionBinding = TransactionBinding;
            }

            if (TrustServerCertificate != null)
            {
                builder.TrustServerCertificate = TrustServerCertificate.Value;
            }

            if (TypeSystemVersion != null)
            {
                builder.TypeSystemVersion = TypeSystemVersion;
            }

            if (UserID != null)
            {
                builder.UserID = UserID;
            }

            if (User != null)
            {
                builder.UserID = User;
            }

            if (UID != null)
            {
                builder.UserID = UID;
            }

            if (UserInstance != null)
            {
                builder.UserInstance = UserInstance.Value;
            }

            if (WorkstationID != null)
            {
                builder.WorkstationID = WorkstationID;
            }

            if (WSID != null)
            {
                builder.WorkstationID = WSID;
            }

            if (PoolBlockingPeriod != null)
            {
                builder.PoolBlockingPeriod = PoolBlockingPeriod.Value;
            }

            if (ColumnEncryptionSetting != null)
            {
                builder.ColumnEncryptionSetting = ColumnEncryptionSetting.Value;
            }

            if (MaxPoolSize != null)
            {
                builder.MaxPoolSize = MaxPoolSize.Value;
            }

            if (ConnectionLifetime != null)
            {
                builder.LoadBalanceTimeout = ConnectionLifetime.Value;
            }

            if (LoadBalanceTimeout != null)
            {
                builder.LoadBalanceTimeout = LoadBalanceTimeout.Value;
            }

            if (AttestationProtocol != null)
            {
                builder.AttestationProtocol = AttestationProtocol.Value;
            }

            if (EnclaveAttestationUrl != null)
            {
                builder.EnclaveAttestationUrl = EnclaveAttestationUrl;
            }

            if (ApplicationIntent != null)
            {
                builder.ApplicationIntent = ApplicationIntent.Value;
            }

            if (App != null)
            {
                builder.ApplicationName = App;
            }

            if (ApplicationName != null)
            {
                builder.ApplicationName = ApplicationName;
            }

            if (ExtendedProperties != null)
            {
                builder.AttachDBFilename = ExtendedProperties;
            }

            if (InitialFileName != null)
            {
                builder.AttachDBFilename = InitialFileName;
            }

            if (AttachDBFilename != null)
            {
                builder.AttachDBFilename = AttachDBFilename;
            }

            if (Authentication != null)
            {
                builder.Authentication = Authentication.Value;
            }

            if (ConnectRetryCount != null)
            {
                builder.ConnectRetryCount = ConnectRetryCount.Value;
            }

            if (ConnectTimeout != null)
            {
                builder.ConnectTimeout = ConnectTimeout.Value;
            }

            if (ConnectionTimeout != null)
            {
                builder.ConnectTimeout = ConnectionTimeout.Value;
            }

            if (Timeout != null)
            {
                builder.ConnectTimeout = Timeout.Value;
            }

            if (CurrentLanguage != null)
            {
                builder.CurrentLanguage = CurrentLanguage;
            }

            if (Language != null)
            {
                builder.CurrentLanguage = Language;
            }

            if (ConnectRetryInterval != null)
            {
                builder.ConnectRetryInterval = ConnectRetryInterval.Value;
            }

            if (Encrypt != null)
            {
                builder.Encrypt = Encrypt.Value;
            }

            if (Enlist != null)
            {
                builder.Enlist = Enlist.Value;
            }

            if (FailoverPartner != null)
            {
                builder.FailoverPartner = FailoverPartner;
            }

            if (InitialCatalog != null)
            {
                builder.InitialCatalog = InitialCatalog;
            }

            if (Database != null)
            {
                builder.InitialCatalog = Database;
            }

            if (IntegratedSecurity != null)
            {
                builder.IntegratedSecurity = IntegratedSecurity.Value;
            }

            if (TrustedConnection != null)
            {
                builder.IntegratedSecurity = TrustedConnection.Value;
            }

            if (DataSource != null)
            {
                builder.DataSource = DataSource;
            }

            if (Server != null)
            {
                builder.DataSource = Server;
            }

            if (Address != null)
            {
                builder.DataSource = Address;
            }

            if (Addr != null)
            {
                builder.DataSource = Addr;
            }

            if (NetworkAddress != null)
            {
                builder.DataSource = NetworkAddress;
            }

            return builder.ConnectionString;
        }

        public string GetkernelName()
        {
            if (KernelName != null)
            {
                return KernelName;
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = GetConnectionString();

            return IdentifierUtils.ToCSharpIdentifier(builder.InitialCatalog);
        }
    }
}
