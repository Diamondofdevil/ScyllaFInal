  `   @      T   A  e  V   B  �  V   C    V   D  6  V   J  �  T   K  <  T   L  �  T   
      ____________________________________________________________________

                      _____     D B 2 C K L O G     _____

                            DB2 Check Log File tool
                                 I    B    M


          The db2cklog tool is a utility can be used to test the integrity
        of an archive log file and to determine whether or not the log file
                  can be used in the rollforward database command.

      ____________________________________________________________________


________________________________________________________________________________
 
========================================================
"db2cklog": Processing log file header of "%1S".
 
"db2cklog": Processing log pages of "%1S" (total log pages: "%2S").
             ==> page "%1S" ... 
"db2cklog": Finished processing log file "%1S". Return code: "%2S".
========================================================
 
db2cklog (DB2 Check Log File tool)
------------------------------------------------------------------------------
Syntax: DB2CKLOG [ CHECK ] <log-file-number1> [ TO <log-file-number2> ]
        [ ARCHLOGPATH <archive-log-path> ]

Command parameters:

   CHECK                           - Verifies the integrity of the log file.
                                     This is default option.
 
   <log-file-number1>              - Specifies the number of the log
                                     file that should be verified. If
                                     the log-file-number2 is specified,
                                     the log-file-number1 represents the
                                     start of the log file range to check.
                                     E.g. 2 is for S0000002.LOG file.
 
   TO <log-file-number2>           - Specifies a range of log
                                     file numbers for which the log files
                                     should be verified.
                                     If the log-file-number2 is less than
                                     the log-file-number1, only the
                                     log-file-number1 will be verified.

   ARCHLOGPATH <archive-log-path>  - Specifies the path where the archive
                                     log files reside. The default is the
                                     current directory.
 