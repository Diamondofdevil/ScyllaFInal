  X        T     ò   T       T     X  T   x    T   y    T   z  ¬  T   {  Õ  T   |     T   }  Í  T   ~    T     ê  T     ¼  T     ø  T     æ  T     !  T     \  T     >"  T     z$  T     §%  T     Ä&  T     ''  T     '  T     ¤(  T      *  T     Á*  T     Ã+  T     f,  T     º0  T     ¦1  T     Ê2  T     â3  T     5  T     x6  T     x:  T     =  T     >  T     ¾>  T     ÝA  T      ZC  T   ¡  C  T   ¢  ÐC  T   £  ¦D  T   ¤  E  T   ¥  éE  T   ¦  nG  T   §  òH  T   ¨  OL  T   ©  gN  T   ª  O  T   
LIC1052E  You must be root to execute this program.

Explanation: 

This program can only be run under the root user ID. Special privileges
are required to execute this program.

User response: 

Login as root and issue the command again.

 
LIC1304E  Unexpected error.

Explanation: 

The tool encountered an unexpected system error.

User response: 

Contact your DB2 service representative.

 
LIC1305E  The profile registry was not found.

Explanation: 

The target machine does not have a profile registry setup.

User response: 

Create the registry on the target machine by installing DB2.

 
LIC1309E  System error.

Explanation: 

The tool encountered an operating system error.

User response: 

A system error was encountered during registry access. Ensure that there
is enough space on the file system where the registry is located, and
that there is a valid LAN connection if the registry is remote.

 
LIC1400N  The syntax of the db2licm command is incorrect. Run the
      db2licm -? command for more information.

Explanation: 

The db2licm tool performs basic license functions. It adds, removes,
lists, and modifies licenses installed on the local system. Execute
db2licm tool with -l parameter to find out the product identifier for
your product: 
 
 db2licm [-a filename]
         [-e product-identifier HARD | SOFT]
         [-p product-identifier
         REGISTERED | CONCURRENT | OFF]
         [-r product-identifier]
         [-u product-identifier num-users]
         [-c product-identifier num-connectors]
         [-l]
         [-v]
         [-?]
The command options are:

-a       

         Adds a license for a product. Specify a file name containing
         valid license information. This can be obtained from your
         licensed product CD or contact your IBM representative or
         authorized dealer.


-e       

         Updates the enforcement policy on the system. Valid values are:
         HARD and SOFT. HARD specifies that unlicensed requests will not
         be allowed. SOFT specifies that unlicensed requests will be
         logged but not restricted.


-p       

         Updates the license policy type to use on the system. The
         keywords CONCURRENT, REGISTERED, or CONCURRENT REGISTERED can
         be specified. Specify OFF to turn off all policies.


-r       

         Removes the license for a product. After the license is
         removed, the product functions in "Try & Buy" mode. To get the
         password for a specific product, invoke the command with the -l
         option.


-u       

         Updates the number of user entitlements that have been
         purchased. Specify the password of the product for which the
         entitlements were purchased and the number of users.


-c       

         Updates the number of connector entitlements that have been
         purchased. Specify the password of the product for which the
         entitlements were purchased and the number of connectors.


-l       

         Lists all the products with available license information,
         including the product identifier.


-v       

         Displays version information.


-?       

         Displays help information. When this option is specified, all
         other options are ignored, and only the help information is
         displayed.

User response: 

Enter the command again using the valid parameters.


   Related information
   db2licm - License management tool command

 
LIC1401I  Command line DB2 License Manager.

Explanation: 

The db2licm tool performs basic license functions. It adds, removes,
lists, and modifies licenses registered on the local system. Execute
db2licm tool with -l parameter to find out the product identifier for
your product:



 db2licm [-a filename]
         [-e product-identifier HARD | SOFT]
         [-p product-identifier
         CONCURRENT | OFF]
         [-r product-identifier]
         [-u product-identifier num-users]
         [-c product-identifier num-connectors]
         [-g filename]
         [-x]
         [-l][show detail]
         [-v]
         [-?]
The command options are:

-a       

         Adds a license for a product. Specify a file name containing
         valid license information. This can be obtained from your
         licensed product CD or contact your IBM representative or
         authorized dealer.


-e       

         Updates the enforcement policy on the system. Valid values are:
         HARD and SOFT. HARD specifies that unlicensed requests will not
         be allowed. SOFT specifies that unlicensed requests will be
         logged but not restricted.


-p       

         Updates the license policy type to use on the system. The
         keyword CONCURRENT can be specified for concurrent user policy.
         Specify OFF to turn off all policies.


-r       

         Removes the license for a product. Specify the product
         identifier.


-u       

         Updates the number of user entitlements that have been
         purchased. Specify the product identifier and the number of
         users.


-c       

         Updates the number of connector entitlements that have been
         purchased. Specify the product identifier and the number of
         connector entitlements.


-g       

         Generates compliance report. Specify file name where output is
         to be stored.


-x       

         Resets license compliance information for the purposes of
         license compliance report.


-l[show detail]
         

         Lists all the products with available license information,
         including the product identifier. Specify [show detail] to view
         detailed information about licensed features (if any).


-v       

         Displays version information.


-?       

         Displays help information. When this option is specified, all
         other options are ignored, and only the help information is
         displayed.

User response: 




   Related information
   db2licm - License management tool command

 
LIC1402I  License added successfully.

 
LIC1403I  License removed successfully.

 
LIC1404N  Product identifier not found.

Explanation: 

The given identifier is either invalid, or a license for this product
was not found in the nodelock file.

User response: 

Issue this command with -l option to check that the identifier entered
is the correct product identifier for the product that you want to
perform this action on. If you are using nodelock passwords, check that
the license key for this product is installed in the nodelock file.

 
LIC1405I  License policy type updated successfully.

 
LIC1406N  Invalid license policy type.

Explanation: 

The license policy type that was entered was not valid for the product
specified.

User response: 

Please enter a valid license policy. Options are: 
*  CONCURRENT
*  OFF

 
LIC1407N  You are trying to register an invalid license certificate
      file, "<license-certificate-file-name>".

Explanation: 

Either the license file is not from the current version, or the license
file is corrupted. You can check the current version by running db2licm
-v

User response: 

Obtain the valid license file for the current version from the
Activation CD, and rerun the db2licm command. For example, db2licm -a
"<license-certificate-file-name>"

 
LIC1408N  The file "<file-name>" could not be opened. Enter the name of
      a file that exists and can be opened and try the command again.

Explanation: 

The file is not found or access to the file is denied.

User response: 

Enter the name of a file that exists and can be opened and try the
command again.

 
LIC1409N  Invalid enforcement policy type.

Explanation: 

The enforcement policy type specified is not valid for this product.

User response: 

Please enter a valid enforcement policy type that is supported by the
specified product.

 
LIC1410I  Concurrent entitlements updated successfully.

 
LIC1411I  Enforcement policy type updated successfully.

 
LIC1412W  A hard stop enforcement policy has been set. This enforcement
      policy stops unlicensed requests.

Explanation: 

You issued the db2licm command with the -e parameter, to update the
enforcement policy, and specified the value HARD. (For example, db2licm
-e db2ese HARD.) The value HARD specifies that unlicensed requests will
not be allowed.

User response: 

As a mechanism for you to keep track of, and differentiate, the DB2
database products and features installed on your system, it is
recommended that you register the license key for each DB2 database
product and feature.

If you want unlicensed requests to be logged but not restricted, change
the enforcement policy to SOFT. For example, db2licm -e db2ese SOFT

 
LIC1413W  A soft stop enforcement policy has been set. This enforcement
      policy specifies that unlicensed requests will be logged but not
      restricted.

Explanation: 

You issued the db2licm command with the -e parameter, to update the
enforcement policy, and specified the value SOFT. (For example, db2licm
-e db2ese SOFT.) The value SOFT specifies that unlicensed requests will
be logged but not restricted.

User response: 

If you want unlicensed requests to be stopped, you must change the
enforcement policy to HARD. For example, db2licm -e db2ese HARD.

 
LIC1416N  The license could not be added to the nodelock file
      automatically. The return code is "<return-code>".

User response: 

Please ensure the license certificate is readable. You may also enter
the license into the nodelock file manually. Please see the license file
for instructions.

 
LIC1417N  The license specified could not be removed from the nodelock
      file. The return code is "<return-code>". Check that the license
      for this product exists in the nodelock file.

User response: 

Ensure that the license for this product exists in the nodelock file.

 
LIC1418I  The number of licensed processors on this system has been
      updated successfully.

 
LIC1419N  There was an error updating the number of licensed processors.
      The return code is "<return-code>".

 
LIC1420N  This product does not support this type of license policy.

Explanation: 

The license policy specified does not apply to this product or is not
supported.

User response: 

Enter a valid license policy or select a product that supports this
policy.

 
LIC1421N  This product specified is not installed on this system.

Explanation: 

You can not configure a license policy for a product until the product
is installed.

User response: 

Install the product before running this command or specify the correct
product identifier. To list the products install on the system issue
db2licm -l command.

 
LIC1422N  The number of concurrent entitlements was not updated. The
      return code is "<return-code>".

User response: 

Please ensure the concurrent policy is enabled for this product.

 
LIC1423N  This option requires the creation of an instance.

Explanation: 

Features that are required to perform this action are only accessible
once an instance has been created.

User response: 

Please create an instance and issue this command again.

 
LIC1424N  An unexpected error occurred while accessing processor
      information.

Explanation: 

The return code is "<return-code>".

User response: 

None.

 
LIC1426I  This product is now licensed for use as outlined in your
      License Agreement.
 USE OF THE PRODUCT CONSTITUTES ACCEPTANCE OF THE TERMS OF THE IBM
      LICENSE AGREEMENT, LOCATED IN THE FOLLOWING DIRECTORY:

      "<dir-name>"------------------------------------------------------------------------
LIC1427I  This product is now licensed for use as outlined in your
      License Agreement.
 USE OF THE PRODUCT CONSTITUTES ACCEPTANCE OF THE TERMS OF THE IBM
      LICENSE AGREEMENT, LOCATED IN THE FOLLOWING DIRECTORY:

      "<dir-name>"------------------------------------------------------------------------
LIC1428N  There was an error updating the number of licensed processors.

Explanation: 

The number of licensed processors entered exceeds the number of maximum
licensed processors allowed for this product.

User response: 

Please enter number of licensed processors that does not exceed the
defined maximum. If the number of processors on your system exceeds the
maximum number of processors allowed for this product, please contact
your IBM representative or authorized dealer.

 
LIC1429N  This product does not support this combination of license
      policies.

User response: 

Please enter a valid combination of license policies. For example, you
can specify "CONCURRENT REGISTERED" as a valid combination.

 
LIC1430N  The license could not be added to the nodelock file because
      the license date is greater than operating system date.

User response: 

Please check your certificate file to ensure that the license start date
precedes the current date (the date set on the operating system).

 
LIC1431N  This user does not have sufficient authority to perform the
      specified action.

Explanation: 

This action can be run only by the root user ID or by a user ID with
SYSADM authority.

User response: 

Login with a user ID that has permission to run this command.

 
LIC1432N  The license could not be added to the nodelock file because
      this product has used the maximum number of evaluation licenses.
      The maximum number of evaluation licenses is "<lic-number>". Run
      this command again with a permanent license key.

Explanation: 

This product has used the maximum number of evaluation licenses.

User response: 

Run this command again with a permanent license key.

 
LIC1433N  The number of license entitlements was not updated.

Explanation: 

The specified number of license entitlements is not in the valid range.

User response: 

Run this command again using a valid number of license entitlements.

 
LIC1434N  DB2 has added the license entry to the nodelock file, however,
      this license entry is not active.

Explanation: 

DB2 failed to activate this license entry, therefore DB2 will run with
the previous license configuration until this license is activated.

User response: 

Try the command again and if it continues to fail, edit the nodelock
file manually or contact IBM support.

If you edit the nodelock file manually, move the new license entry to
the beginning of the license entries list.

The nodelock file could be found in the following locations:

Windows XP and Windows 2003
         

         X:\Documents and Settings\All Users\Application
         Data\IBM\DB2\<DB2 copy name>\license.


Windows Vista
         

         X:\ProgramData\IBM\DB2\<DB2 copy name>\license

Where 'X:' is the system drive.

On all other platforms the nodelock file is located in the installation
path of this product in the license directory.

Refer to the DB2 Information Center for more information on licensing.

 
LIC1435E  An I/O error occurred when accessing the nodelock file. The
      license could not be added.

Explanation: 

An error occurred when creating or accessing nodelock file. The file
access settings do not allow this action.

User response: 

Ensure that the nodelock file and the directory where nodelock file is
located allow read and write access to this program.

The nodelock file could be found in the following locations: 

Windows XP and Windows 2003
         X:\Documents and Settings\All Users\Application
         Data\IBM\DB2\<DB2 copy name>\license.

Windows Vista
         X:\ProgramData\IBM\DB2\<DB2 copy name>\license

 Where 'X:' is the system drive.

On all other platforms the nodelock file is located in the installation
path of this product in the license directory.

 
LIC1436I  Duplicate license was found in nodelock file.

Explanation: 

DB2 has determined that this license has already been registered in the
nodelock file for this installation of DB2.

User response: 

No further action is necessary.

 
LIC1437I  License entitlements updated successfully.

 
LIC1438E  An I/O error occurred when accessing the nodelock file. The
      license could not be removed.

Explanation: 

An error occurred when creating or accessing nodelock file. The file
access settings do not allow this action.

User response: 

Ensure that the nodelock file and the directory where nodelock file is
located allow read and write access to this program.

The nodelock file could be found in the following locations: 

Windows XP and Windows 2003
         X:\Documents and Settings\All Users\Application
         Data\IBM\DB2\<DB2 copy name>\license.

Windows Vista
         X:\ProgramData\IBM\DB2\<DB2 copy name>\license

 Where 'X:' is the system drive.

On all other platforms the nodelock file is located in the installation
path of this product in the license directory.

 
LIC1439I  DB2 server has detected that "<product-name>" is installed on
      this system. Products and functions obtained via this offering may
      only be used for testing or development purposes as outlined in
      your License Agreement. The License Agreement for this offering is
      located in the 'license' directory in the installation path for
      this product.

 
LIC1440I  License compliance report generated successfully.

 
LIC1441I  License compliance information was reset.

 
LIC1442E  An error occurred when generating compliance report.

Explanation: 

Compliance report could not be created.

User response: 

Ensure this program is able to write to the file specified and try
again.

 
LIC1443E  An error occurred when resetting compliance information.

Explanation: 

Compliance information could not be reset.

User response: 

Ensure this program is able to write to the license directory in the
installation path and try again.

 
LIC1444E  An I/O error occurred. The return code is. "<return-code>" .

 
LIC1445E  An error occurred when generating the compliance report.

Explanation: 

An unexpected error occurred when generating the compliance report. The
compliance report could not be created.

User response: 


*  Ensure at least one valid DB2 instance is created.
*  Ensure the DB2 global registry is not corrupted.
*  Ensure the DB2 Administration Server was started successfully.

 
LIC1446I  The license certificate "<license-certificate-file-name>" for
      SA MP was successfully installed.

Explanation: 

The IBM Tivoli System Automation for Multiplatforms (SA MP) requires a
valid license certificate to work with the DB2 High Availability (HA)
feature. This license certificate was successfully installed or updated.

User response: 

No response is required.

 
LIC1447N  The license certificate "<license-certificate-file-name>" for
      SA MP was not successfully installed.

Explanation: 

The IBM Tivoli System Automation for Multiplatforms (SA MP) requires a
valid license certificate to work with the DB2 High Availability (HA)
feature. This license certificate was not successfully installed or
updated.

If you used the DB2 installer to install or update this license
certificate, you can find more detailed information about why the
install or update failed in the DB2 install log file.

User response: 

To manually install or update this license certificate for the SA MP
Base Component, issue the command:

*  samlicm -i license-certificate-file-name

For more information about the samlicm command, see the SA MP Base
Component documentation.


   Related information
   SA MP Base Component documentation

 
LIC1448I  This license was automatically applied at install time in
      order to enable you to start working with DB2.

Explanation: 

To be fully licensed, this product requires a license appropriate to
your purchased license policy.

User response: 

A license can be downloaded from Passport Advantage or may be found on a
separate CD in your product package. Both the download and the CD are
titled "Activation CD".

For more information on licensing your product search the Information
Center using terms such as "licensing".

 
LIC1449N  The license was not installed due to a platform restriction.

Explanation: 

This DB2 product is only supported in trial mode, also known as "Try and
Buy" mode, on this platform.

User response: 

Continue to use this product in trial mode, or install one which is
fully supported on this platform.

 
LIC1450I  The product licensed by the certificate "<file-name>" was not
      found in the DB2 copy.

Explanation: 

Additional licenses may be added to a DB2 copy prior to additional
product installation. The license has been successfully added but it
will not be shown until the corresponding product is installed.

User response: 

No action is necessary. If you subsequently install the product covered
by this certificate, you do not need to re-register the license.

 