  �  �      T   �  \   T   �  �   T   �  �   T   �    T   �  G  T   �  `  T   �  v  T   �  &  T   �  u	  T   �  �
  T   �  �  T   �  �  T   �  �  T   �  H  T   �  �  T   �    T   �  x  T   �  �  T   �  "   T   �  m"  T   �  N#  T   �  �$  T   �  �&  T   �  4(  T   �  �+  T   �  n-  T   �  /  T   �   1  T   �  �1  T   �  3  T   �  �3  T   �  �4  T   �  �6  T   �  U8  T   �  P9  T   �  :  T   �  ;  T   �  <  T   �  A=  T   �  4>  T   �  �?  T   �  �D  T   �  eG  T   �  �I  T   �  �K  T   �  �P  T   �  �Q  T   �  �R  T   �  UT  T   �  6V  T   �  �W  T   �  �X  T   �  �Y  T   �  �Z  T   �  i[  T   �  "\  T   �  G]  T   �  9^  T   �  h_  T   �  �`  T   �  qb  T   �  [c  T   �  �d  T   �  _e  T   �  �g  T   �  i  T   �  \j  T   �  �k  T   �  Xl  T   �  &m  T   �  �m  T   �  �n  T   �  *p  T   �  �p  T   �  �q  T   �  �r  T   
CCA1001I  Use alphanumeric characters only. The first character cannot
      be numeric.

 
CCA1002I  Use decimal numbers, 0 to 9, only.

 
CCA1003I  No DB2 system was found on the network.

 
CCA1004I  No DB2 instance was found on the selected system(s).

 
CCA1005I  No DB2 database was found on the selected instance(s).

 
CCA2001W  No files were found matching the file pattern specified.

Explanation: 

A request was made to read from files matching the specified file
pattern. No files were found matching the pattern.

User response: 

Correct the file pattern specified and retry the operation.

 
CCA2002W  An error was encountered while updating the TCP/IP services
      file.

Explanation: 

An attempt to add a service name and port number to the TCP/IP services
file failed or Network Information Services is being used and only the
local services file was updated. The port number was used to catalog the
node instead of the service name.

User response: 

To use the service name instead of the port number in the node directory
entry, the node must be manually uncataloged and then recataloged using
the service name. The services file must also be updated manually. If
Network Information Services (NIS) is being used, the local services
file may have been updated but the NIS server must be updated manually.
In this case, the node was also cataloged using the port number.

 
CCA2003W  The Discover request did not return data for one or more DB2
      systems.

Explanation: 

The discover request did not return data for one or more of the
requested DB2 systems. One of the following may have occurred: 
*  The Administration Server to which the discover request was sent has
   not been started.
*  An error occurred attempting to perform the discover request.
*  The DB2 system to which the discover request was sent is not
   configured for discovery.

User response: 

Verify that the DB2 system to which the discover request was sent is
enabled for discovery. If enabled for discovery, ensure that the
Administration Server on the DB2 system is running.

 
CCA2004W  The nname value specified is not unique.

Explanation: 

The nname value specified is already being used by another NetBIOS
application on the network.

User response: 

Select 'YES' to use the specified nname or 'NO' to cancel the request.
If 'YES' is selected, any application using the existing nname will be
affected.

 
CCA2005W  The socket number specified is not unique.

Explanation: 

The socket number specified is being used by another DB2 instance on the
workstation.

User response: 

Select 'YES' to use the specified socket or 'NO' to cancel the request.
If 'YES' is selected, any application using the existing socket number
will be affected.

 
CCA2006W  An entry already exists in the TCP/IP services file for the
      service name and port number specified.

Explanation: 

An entry already exists in the TCP/IP services file for the service name
and port number specified. Another application may be using the entry.

User response: 

Select 'YES' to use the existing entry or 'NO' to cancel the request. If
'YES' is selected, any application using the existing entry will be
affected.

 
CCA2007W  The port number specified is being used with a different
      service name.

Explanation: 

The TCP/IP services file contains an entry that uses the port number
specified but the associated service name does not match the service
name specified.

User response: 

Select 'YES' to use the specified service name and port number or 'NO'
to cancel the request. If 'YES' is selected, a new entry will be added
to the services file. Any applications using the entry with the existing
port number may be affected.

 
CCA2008W  The service name specified is being used with a different port
      number.

Explanation: 

The TCP/IP services file contains an entry that uses the service name
specified but the associated port number does not match the port number
specified.

User response: 

Select 'YES' to use the specified service name and port number or 'NO'
to cancel the request. If 'YES' is selected, the existing entry in the
services file, using the service name, will be updated to use the port
number specified. This may affect any applications using the existing
entry.

 
CCA2009W  The request was canceled by the user.

Explanation: 

The request was canceled by the user.

User response: 

None.

 
CCA2010W  An attempt to update the APPC stack failed.

Explanation: 

An attempt to add the Transaction Program name to the APPC stack failed.

User response: 

The APPC stack must be manually updated. If the Transaction Program name
is not added to the stack, remote connections to the server will not be
possible.

 
CCA2011W  An attempt to add the service name and port number to the
      TCP/IP services file failed.

Explanation: 

An attempt to add the service name and port number to the TCP/IP
services file failed. The database manager configuration file has been
updated with the service name specified.

User response: 

The service name and port number must be manually added to the TCP/IP
service file. If the entry is not added to the services file, remote
connections to the server will not be possible.

 
CCA2012W  The Discover request did not find any DB2 systems.

Explanation: 

A request to search the network for DB2 systems completed successfully
but no DB2 systems were found. Following is a list of possible reasons
why no DB2 systems were found: 
*  Search discovery was not enabled on any DB2 systems (that is, in the
   DBM configuration file of the administration server on the DB2
   system, DISCOVER = SEARCH was not specified).
*  The DB2 system was not set up with the appropriate discovery protocol
   for the client to find it (that is, DISCOVER_COMM on the
   administration server does not contain a protocol that matches one
   specified for DISCOVER_COMM on the client).
*  The DB2 system is on the other side of a router or bridge, and the
   routers and bridges on the network were configured such that the
   discovery packet was filtered out and not passed on.

User response: 

Following is a list of possible actions to take in order for discovery
to find DB2 systems: 
*  Set DISCOVER = SEARCH in the DBM configuration file of the
   administration server on all DB2 systems that you wish to be found.
*  Set DISCOVER_COMM for the administration server to include the
   protocol that the client will use to issue a discover request (that
   is, set DISCOVER_COMM to include at least one of the protocols
   specified in DISCOVER_COMM on the client).
*  Have the network administrator reconfigure the router or bridge to
   allow discovery packets (for the specified protocol) to be passed
   through.

 
CCA2013W  Remote database cataloged using APPC but the stack was not
      configured.

Explanation: 

A request to catalog a database resulted in a node being cataloged that
uses the APPC protocol. The node was cataloged using the symbolic
destination name which was retrieved from the profile specified. The
APPC stack was not configured because there was insufficient information
in the profile to configure the stack or APPC was not detected on the
DB2 system. It was not possible to use a different protocol because no
other matching protocol was detected on the client.

User response: 

If APPC is not installed on the client, uncatalog the database and
recatalog the database manually using a protocol that is available on
both the client and server. If APPC is installed, configure the stack if
it has not already been configured.

 
CCA2014W  The transaction program name specified is not unique or has
      already been configured.

Explanation: 

The transaction program name specified is already being used by another
DB2 instance or a non DB2 application on this server.

User response: 

Select 'YES' to use the specified transaction program name or 'NO' to
cancel the request. If "YES" is selected, for all applications using the
transaction program name concurrently, APPC will only be operational for
the first one started. If any new APPC parameter values have been
specified, the APPC stack will be updated with these values.

 
CCA2015W  The service name and port number specified are being used in
      different entries in the services file.

Explanation: 

The TCP/IP services file contains entries that use the service name and
port number specified but they are not being used in the same entry.

User response: 

Select 'YES' to use the specified service name and port number or 'NO'
to cancel the request. If 'YES' is selected, the existing entry in the
services file, using the service name, will be updated to use the port
number specified. This may affect any applications using the existing
entries.

 
CCA2016W  The password will be saved as clear text.

Explanation: 

The password will be saved as clear text in db2cli.ini file.

User response: 

If password security is a concern, deselect the 'Save password' check
box.

 
CCA3000C  An internal error has occurred. Reason code "<reason-code>".

Explanation: 

An unexpected internal error has occurred.

User response: 

Turn trace on and retry the steps that caused the error. If the problem
reoccurs, save the trace information to a file and contact IBM Support
with the following information: 
*  Problem description
*  Message number
*  Reason code
*  Trace file

 
CCA3001N  The specified service name and port number conflicts with
      existing values in the TCP/IP services file.

Explanation: 

The service name and port number entered by the user conflicts with
existing values in the TCP/IP services file. The service name may
already be used with a different port number, the port number may
already be used with a different service name or both.

User response: 

Specify a service name and port number that does not conflict with
existing entries in the services file.

 
CCA3002N  An I/O error occurred.

Explanation: 

An error was encountered while attempting to open, read, change the file
position or close a file.

User response: 

If a file name was specified, verify that the file name is valid and
that the user has permission to access the file. Also check for any disk
and operating system errors.

 
CCA3003N  The format of the file is not valid.

Explanation: 

An error was encountered while reading from a file. The format of the
file is not valid. Possible errors include: 
*  The file contains invalid data.
*  The file does not contain expected data.
*  The order of the data in the file is incorrect.

User response: 

If a file name was specified, and the file has been modified by the
user, regenerate the file and retry the operation. If the problem
persists, and the file was not modified by the user, or the problem
occurred during a Discover request, turn trace on and retry the steps
that caused the error. If the problem reoccurs, save the trace
information to a file and contact IBM Support with the following
information: 
*  Problem description
*  Message number
*  Trace file
*  File which is causing the error if a file name was specified

 
CCA3004N  An attempt to allocate memory failed.

Explanation: 

An error was detected while attempting to allocate memory.

User response: 

Terminate other applications running on the system that may be using
large amounts of memory. If the problem persists, turn trace on and
retry the operation. If the problem reoccurs, save the trace information
to a file and contact IBM Support with the following information: 
*  Problem description
*  Message number
*  Trace file

 
CCA3005N  An error was encountered while writing to a file.

Explanation: 

An error was detected while writing to a profile. The error could also
be encountered when updating a host system password and errors are being
recorded in the file db2pem.log.

User response: 

Verify that the file system on which the file resides is not full and is
not damaged. Also check for any operating system errors.

 
CCA3006N  No matching communication protocol was detected.

Explanation: 

The database cannot be cataloged because none of the protocols available
on the client match any of the protocols available at the server.

User response: 

Ensure that the client and server have at least one matching
communication protocol that can be detected on both. If a matching
protocol is installed on both the client and the server, the protocol
could not be detected. In this case, catalog the database and node
manually.

 
CCA3007N  The database alias name specified is not valid.

Explanation: 

The length of the database alias specified is not a valid or the alias
contains invalid characters.

User response: 

Correct the alias name and resubmit the request.

 
CCA3009N  The application requestor name specified is not valid.

Explanation: 

The length of the application requestor name specified is not valid or
the name contains invalid characters.

User response: 

Correct the application requestor and resubmit the request.

 
CCA3010N  The length of the parameter value specified is not valid.

Explanation: 

The length of the parameter value specified for the application
requestor is not valid.

User response: 

Correct the parameter value and resubmit the request.

 
CCA3011N  The target database name specified is not valid.

Explanation: 

The length of the target database name specified is not valid or the
name contains invalid characters.

User response: 

Correct the target database name and resubmit the request.

 
CCA3012N  Add ODBC data source failed.

Explanation: 

A request to add an ODBC data source failed. The error could be caused
by an out of memory error, a disk full condition or a disk failure.

User response: 

Verify that the disk on which the ODBC.INI and DB2CLI.INI files reside
is not full and that the disk is not damaged. In addition, if other
applications are using large amounts of memory, terminate the
applications and retry the operation.

 
CCA3013N  Remove ODBC data source failed.

Explanation: 

A request to remove an ODBC data source failed. The error could be
caused by an out of memory condition or a disk failure.

User response: 

If other applications are using large amounts of memory, terminate the
applications and retry the operation. Also verify that the disk on which
the ODBC.INI and DB2CLI.INI files reside is not damaged.

 
CCA3014N  The bind request cannot be processed.

Explanation: 

The bind request cannot be processed because another bind operation is
already in progress.

User response: 

Complete or terminate the bind in progress and resubmit the bind
request.

 
CCA3015N  The adapter specified is not valid.

Explanation: 

The adapter specified was not detected on the DB2 system.

User response: 

Specify an adapter that is available and resubmit the request.

 
CCA3016N  The nname value specified is not unique.

Explanation: 

The nname value specified is already being used by another NetBIOS
application on the network.

User response: 

Specify a unique nname and retry the operation.

 
CCA3017N  The path specified for the file is not valid.

Explanation: 

An attempt was made to open the specified file, but the path specified
is invalid or does not exists.

User response: 

Ensure that the path specified is valid and the path for the file
exists.

 
CCA3018N  The user does not have sufficient authority to access the
      file.

Explanation: 

An attempt was made to access the requested file, but the user does not
have the required authority to access the file.

User response: 

Ensure that the user has the required authority to access the file.

 
CCA3019N  The file name specified is a directory.

Explanation: 

An attempt to access the file specified failed because the name
specified is a directory and not a file.

User response: 

Specify a valid file name and retry the operation.

 
CCA3020N  An attempt to access the specified file failed because of a
      share violation.

Explanation: 

An attempt to access the file specified failed because of a share
violation. Another process may have the file opened in exclusive mode.

User response: 

The file is currently being accessed by another process in exclusive
mode. Ensure that no other process is accessing the file and retry the
operation or specify another file name.

 
CCA3021N  An attempt to retrieve, add or remove variable
      "<variable-name>" from the DB2 Profile Registry failed with Return
      Code "<return-code>".

Explanation: 

An attempt to retrieve, add or remove the indicated variable from the
DB2 Profile Registry failed. The return code indicates the cause of the
problem. The possible return codes are as follows: 
*   -2 The specified parameter is invalid
*   -3 Insufficient memory to process the request
*   -4 Variable not found in the registry
*   -7 DB2 Profile Registry is not found on this DB2 system
*   -8 Profile not found for the given instance
*   -9 Profile not found for the given node
*  -10 UNIX registry file lock time-out

User response: 

For return code: 
*   -2 Ensure that the parameter has been specified correctly.
*   -3 Terminate other applications using large amounts of memory and
   retry the operation.
*   -4 Ensure that the variable is set in the DB2 Profile Registry.
*   -7 Ensure that the DB2 Profile Registry has been created.
*   -8 Ensure that the profile has been created for the instance.
*   -9 Ensure that the profile has been created for the node.
*   -10 Ensure that the registry file is not locked by another process.

 
CCA3022C  An attempt to retrieve the address of the function
      "<procedure-name>" from library "<library-name>" failed with
      Return code "<return-code>".

Explanation: 

An attempt to retrieve the address of a function from the indicated
library failed.

User response: 

Verify that the correct version of the library is being used. If the
incorrect version is being used, install the correct version. If the
problem persists, turn trace on and retry the steps that caused the
error. If the problem reoccurs, save the trace information to a file and
contact IBM Support with the following information: 
*  Problem description
*  Message number
*  Return code
*  Trace file

 
CCA3023C  An attempt to load library "<library-name>" failed with Return
      code "<return-code>".

Explanation: 

An attempt to load the indicated library failed.

User response: 

Verify that the path in which the library resides is included in the
library path. Also ensure that there is enough memory available to load
the library. If the problem persists, turn trace on and retry the steps
that caused the error. If the problem reoccurs, save the trace
information to a file and contact IBM Support with the following
information: 
*  Problem description
*  Message number
*  Return code
*  Trace file

 
CCA3024C  An attempt to unload library "<library-name>" failed with
      Return code "<return-code>".

Explanation: 

An attempt to unload the indicated library failed.

User response: 

Turn trace on and retry the steps that caused the internal error. If the
problem reoccurs, save the trace information to a file and contact IBM
Support with the following information: 
*  Problem description
*  Message number
*  Return code
*  Trace file

 
CCA3025N  One or more of the IPX/SPX parameters specified is not valid.

Explanation: 

One or more of the input parameters is not valid. Following is a list of
possible errors:

*  One or more of the fileserver, objectname and ipx_socket parameters
   is NULL.
*  Only the fileserver parameter or objectname parameter is set to "*".
*  The fileserver and/or objectname parameters are not set to "*" for
   Windows and Solaris.
*  The objectname value specified is not unique.
*  The ipx_socket value specified is not unique.
*  The ipx_socket value specified is not in the valid range.
*  The DB2 system failed to attach to the fileserver specified.

User response: 

Verify the following:

*  The fileserver, objectname and ipx_socket parameters are not NULL.
*  If the value specified for fileserver is "*", the value for
   objectname must also be "*".
*  On Windows and Solaris, both fileserver and objectname must be "*".
*  The value specified for objectname, if not "*", is unique for all DB2
   instances and IPX/SPX applications registered at the fileserver.
*  The value specified for ipx_socket is unique across all DB2 instances
   on the DB2 system.
*  The value specified for ipx_socket is within the valid range.
*  The fileserver specified exists and that it is up and running.

Correct all errors and retry the operation.

 
CCA3026N  No available NetBIOS adapters detected.

Explanation: 

A NetBIOS adapter was not detected on the DB2 system. The database
cannot be cataloged.

User response: 

Catalog the database and node manually if an adapter is available on the
DB2 system.

 
CCA3027N  The port number specified is out of range.

Explanation: 

The TCP/IP port number specified is out of range. The maximum value that
can be specified for the port number if 65534.

User response: 

Specify a port number that does not exceed the maximum value and retry
the operation.

 
CCA3028N  The DB2INSTANCE variable is not valid.

Explanation: 

The DB2INSTANCE environment variable is not set or is set to the
Administration Server instance. The Configuration Assistant cannot run
under the Administration Server Instance.

User response: 

Set the DB2INSTANCE variable to an instance other than the
Administration Server instance.

 
CCA3029N  Updating of the ODBC data source settings failed.

Explanation: 

A request to update the settings of an ODBC data source failed. The
error could be caused by an out of memory error, a disk full condition
or a disk failure.

User response: 

Verify that the disk on which the DB2CLI.INI file resides is not full
and that the disk is not damaged. In addition, if other applications are
using large amounts of memory, terminate the applications and retry the
operation.

 
CCA3030N  Values missing for configuring APPC.

Explanation: 

A request was made to catalog a database using APPC or to configure a
server instance for APPC. The request could not be completed because one
or more parameters were not specified.

User response: 

Ensure that all required parameters have been specified and retry the
operation.

 
CCA3031N  The APPC stack has not been configured for the database
      selected.

Explanation: 

The database selected is using APPC for the database connection.
However, the APPC stack has not been configured for the connection.

User response: 

Configure the APPC stack for the database selected.

 
CCA3051N  A "<protocol>" protocol interface failure has occurred with
      Return code "<return-code>".

Explanation: 

The failure occurred while attempting to access the protocol interface.

User response: 

Verify that the protocol is operational.

 
CCA3052N  The specified item "<item-name>" was not found.

Explanation: 

The specified item name could not be found in the configuration data.

User response: 

Verify that you have specified the item name correctly.

 
CCA3053N  The ODBC DSN specified "<DSN-name>" is invalid.

Explanation: 

The ODBC DSN specified is an invalid name.

User response: 

Ensure that you are using valid characters for the ODBC DSN name.

 
CCA3054N  The ODBC DSN could not be registered.

Explanation: 

The ODBC DSN registration attempt failed.

User response: 

Verify that ODBC is installed properly and is functional.

 
CCA3055N  The specified item "<item-name>" already exists.

Explanation: 

The specified item name already exists in the configuration data.

User response: 

Verify that you have specified the item name correctly. Use a different
item name or delete the old item and resubmit the request.

 
CCA3056N  The hostname "<host-name>" was not found.

Explanation: 

The specified hostname could not be resolved on the network.

User response: 

Ensure that the hostname, as specified, is correct and is a valid
hostname on your network.

 
CCA3057N  The service name "<service-name>" was not found.

Explanation: 

The specified service name was not found in the local services file.

User response: 

Ensure that the service name, as specified, is correct and that there is
a valid entry for that service name in your local services file.

 
CCA3058N  The local system object is not allowed to be removed.

Explanation: 

The local system object appears if this is a server installation, and
does not appear if this is a client installation. You have no direct
control over the removal of this object because it has special
properties that are required by the server installation type.

User response: 

No action is required.

 
CCA3059N  The local system object is not allowed to be changed.

Explanation: 

The local system object appears if this is a server installation, and
does not appear if this is a client installation. You have no direct
control over the changing of this object because it has special
properties that are required by the server installation type.

User response: 

No action is required.

 
CCA3060N  The selected system object is not allowed to be changed.

Explanation: 

The selected system is using communication protocol that is not
supported by the DB2 Administration Tools.

User response: 

No action is required.

 
CCA3061N  Incomplete server configuration.

Explanation: 

The server configuration information contained in the server profile is
missing data necessary to complete the requested operation. Refer to the
db2diag log file for details.

User response: 

Contact your system administrator to verify that the server
configuration is correct.

 
CCA3062N  "Common" is a reserved data source name.

Explanation: 

"Common" is a reserved data source name by DB2 CLI.

User response: 

Re-enter another data source name.

 
CCA3063N  The specified service name and port number conflicts with
      existing values in the TCP/IP services file. Do you wish to
      overwrite the existing values in the services file?

Explanation: 

The service name and port number entered by the user conflicts with
existing values in the TCP/IP services file. The service name may
already be used with a different port number, the port number may
already be used with a different service name or both.

User response: 

Click Yes to overwrite the existing values in the services file with the
new values.

Click No to cancel the action and keep the existing values in the
services file.

 
CCA3064N  The new data source name specified already exists.

Explanation: 

A data source entry already exists with the same name and its content
does not match the specification of the new entry. Therefore, it cannot
be reused.

User response: 

Use a different data source name.

 
CCA3065N  The requested operation is not available in offline (OFFLINE)
      mode.

Explanation: 

An operation or task has been requested which is not valid or not
applicable while the mode is offline (OFFLINE). The operation cannot
proceed.

User response: 

Change the mode from offline (OFFLINE) mode before retrying this
operation.

 
CCA3066N  The requested operation is not available in remote (REMOTE)
      mode.

Explanation: 

An operation or task has been requested which is not valid or not
applicable while the mode is remote (REMOTE). The operation cannot
proceed.

User response: 

Change the mode from remote (REMOTE) mode before retrying this
operation.

 
CCA5000N  The user ID specified is not valid.

Explanation: 

The user ID specified does not exist.

User response: 

Enter the correct user ID and resubmit the request.

 
CCA5001N  The password specified is incorrect.

Explanation: 

The password specified for the user ID is incorrect.

User response: 

Enter the correct password for the user ID and resubmit the request.

 
CCA5002N  The password for the user ID has expired.

Explanation: 

The password for the user ID has expired and cannot be updated.

User response: 

Contact your system administrator to have the password reset.

 
CCA5003N  The new password specified is not valid.

Explanation: 

The new password specified is not valid.

User response: 

Enter a valid password and resubmit the request.

 
CCA5004N  An unexpected error occurred.

Explanation: 

An unexpected error occurred while attempting to update the password for
the user ID specified. Additional information may have been written to
the file db2pem.log in the instance directory.

User response: 

Contact your system administrator for further assistance and provide the
information from the file db2pem.log.

 
CCA5005N  The new password does not match the verify password.

Explanation: 

The new password does not match the verify password.

User response: 

Type the new password in both text boxes again.

 
CCA5006N  The port number specified is not valid.

Explanation: 

The port number specified is out of range. It should be greater than
zero and less than 65535.

User response: 

Type the new port number and retry the operation.

 
CCA5007N  The parameter value specified is not valid.

Explanation: 

The parameter value specified is out of range.

User response: 

Type the new parameter value and retry the operation.

 
CCA5008N  The adapter number specified is not valid.

Explanation: 

The adapter number specified is out of range. It should be between 0 and
255.

User response: 

Type the new adapter number and retry the operation.

 