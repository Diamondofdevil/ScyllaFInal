-----------------------------------------------------------------------
About
-----------------------------------------------------------------------
Scylla is a great tool :) it does a lot of stuff and it will do even more! i promisse, i'm working in a lot of ideas to make it faster, more reliable, better, much better. I hope you help us with this project :)

This project is property of 2Secure S.A.S and is releases on the "Don't be a asshole" Licence:
	Get what you need, do what you need, but don't be an asshole, don't sell what's mine, tell people that i did it (unless you copy a couple of lines :P) and then do what ever you want, just don't be a jerk.
	
Main Developer:
	diamondofdevil -> diamondofdevil 

	

-----------------------------------------------------------------------
Install (Or compile) readme
-----------------------------------------------------------------------

Well, fist of all, if this is your first time, maybe this would be a pain in the ass, so pay attention. (plz, leave the bin directory as is)

-----------------------------------------------------------------------
Pre reqs
-----------------------------------------------------------------------
You will need to download and install the following:

Visual Studio 2012 (yeah, for compiling and all that stuff, there's a version for VS 2010, but i used 2012 cus it's free... for now :P)
	http://www.microsoft.com/visualstudio/11/en-us/downloads

.NET Framework 4.0 (maybe you should download 3.5 either)
	It comes with VS 2012

ODA.NET (Oracle Data Access for .Net): Used for oracle connections, i download the ODAC1122021Xcopy_32bit
	http://www.oracle.com/technetwork/database/windows/downloads/index-101290.html
	To install, just follow instructions, unzip and do something like this:
	   install.bat odp.net2 c:\oracle odac

IBM Data Server Runtime Clients (all the .net stuff): I prefer 32 bit version (i used this one)
	https://www14.software.ibm.com/webapp/iwm/web/reg/pick.do?source=swg-idsrc11&S_TACT=appddnet&S_CMP=ibm_im&lang=en_US

OpenSSL for windows (Please, install it in the default dir!)
	http://slproweb.com/products/Win32OpenSSL.html
	
-----------------------------------------------------------------------
The code
-----------------------------------------------------------------------
Well, if you are reading this you must have the code, if not, get it from here

Opening the code is just easy, click the Scylla.sln file and your are done :)

-----------------------------------------------------------------------
Dealing with dependencies
-----------------------------------------------------------------------
This is the main reason why i prefer letting you compile the code than giving you a binary, cus dependencies are a pain in the ass! (I know, this can be easy to solve, but i don't do it because fuck you. Your time dealing with this shit would be welcomed :) ). If you don't have it, start going to "VIEW/SOLUTION EXPLORER"

Lets begin with the C++ Code:
	The main dependency here is OpenSSL. So, let's step into each of the following projects:
		OpenSSL_Wrapper
		SSHLogin
		SSHLoginWrapper
	
	Then, right click on then and go to properties, the go into "Configuration Properties/C/C++" and in the field "Additional Include Directory" add the following (try to find a similar path in your pc):
		C:\OpenSSL-Win32\include;%(AdditionalIncludeDirectories)
			Maybe you can have problems finding some files, if so, you would need the Windows SDK, i got it from here: http://msdn.microsoft.com/en-us/windows/desktop/hh852363.aspx
			Then add the following path's:
			C:\Program Files (x86)\Windows Kits\8.0\Include\shared;
			C:\Program Files (x86)\Windows Kits\8.0\Include\um;
		
	Go into "Linker" and in the field "Additional Library Directory" add the following:
		C:\OpenSSL-Win32\lib\VC;C:\OpenSSL-Win32\lib;%(AdditionalLibraryDirectories)
			If you need to download de winSDK, maybe you should ned to add this:
			C:\Program Files (x86)\Windows Kits\8.0\Lib\win8\um\x86;
			
	That's all :) now do it in the other projects

Here comes the harder part, the databases dependencies (DB2 and Oracle, and others):
	First of all, a few comments: I only could make db2 work if there's a bin directory, also, if you see, in ScyllaMain directory, there's a folder that says "msg", it's needed for DB2, also, in the bin directory, there are 2 .dll's (db2app.dll and db2app64.dll), I think that if you got this one, you are done :).
	If you succesfully installed the IBM-someShit, i hope you don't have any problems :).
	
	Well, most .dll's dependencies are in a directory named \Scylla\libs, so you would not have any problem with that. This part is a pain in the ass, i know :S. But you gotta expand the following projects:
		DatabaseBrowser
		DBManagement and
		Scylla
		
	Then expand the References item, you should find a warning in the following references (maybe more, but the others are solved when you compile :) ):
		IBM.Data.DB2
		MySQL.Data
		Npgsql
		Oracle.DataAccess
		System.Data.SqlServerCe
		
	so, for each one of those, right click/remove and then right click in the "References forlder"/add reference. In that form go to Browse and then click in the "Browse..." button, go to the libs folder, find the reference you have just deleted and add it.

-----------------------------------------------------------------------
Compile
-----------------------------------------------------------------------
Now compile =D (if you got trouble here, try compiling each project alone, starting with the C++ projects). To compile just press F6. To compile each project alone, right click in the project and press "Compile"

-----------------------------------------------------------------------
Run Scylla :)
-----------------------------------------------------------------------
After you compile, you got 2 options (i prefer the first one cus u'll be able to help me with bug dealing).
	1) Just click in VS the run button
	2) Go to: \Scylla\ScyllaMain\bin\Debug, you'll find a file named "Scylla.exe", double click it.