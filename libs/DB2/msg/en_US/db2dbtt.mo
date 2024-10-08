  �  �      T   �  5  T   �  E  V   �  j  T   �    V   �  �  T   �  �  T   �  �  T   �  �  V   �    T   �  1  T   �  F  V   �  p  V   �  �  V   �  �  V   �  �  T   �  +  T   �  I  V   �  l  V   �  �  V   �  �  T   �    V   �  2  T   �  D  V   �  l  V   �  �  V     �  T   	  �  V   
  �  V     
  V     0  T     F  V     r  V     �  V     �  V       V     8  V     e  T     w  V   4  �  V   5  �  T   9  &	  V   ;  R	  V   U  l	  T   V  �	  T   W  �	  T   X  �	  T   Y  �	  T   f  �	  T   g  �	  T   h  
  T   i  #
  T   j  8
  T   k  R
  T   l  j
  T   m  y
  T   n  �
  T   o  �
  T   p  �
  T   q  �
  T   r  �
  T   s  �
  T   t  	  T   u    T   v  6  T   w  C  T   x  S  T   y  Z  T   z  f  T   {  x  T   |  �  T   }  �  T   ~  �  T     �  T   �  �  T   �  �  T   �  �  T   �     T   �    T   �  !  T   �  D  T   �  \  T   �  �  T   �  �  T   �  �  T   �    T   A  H  T   B  �  T   C  �  T   D  �  T   E  +  T   F    T   G    T   H  >  T   (#  l  V   )#  �  V   *#  �  V   +#  �  V   ,#  &  S   -#  Y  V   .#  x  S   /#  �  V   0#  �  V   1#  �  V   2#    V   3#  H  V   4#  �  V   5#    V   6#  3  S   7#  \  V   8#  �  V   9#  �  V   :#  �  S   ;#  �  T   <#  +  V   =#  b  V   >#  �  V   ?#  �  V   @#    V   �#  z  V   
Incorrect syntax. Correct the syntax and reissue the command.

db2tapemgr [DATABASE database-alias]
  [ON DBPARTITIONNUM db-partition-number]
  {{{{STORE | DOUBLE STORE}[Store-Option-Clause]|
   RETRIEVE [Retrieve-Option-Clause]|
   SHOW TAPE HEADER tape-device}
    [USING blocksize][EJECT]}|
   EJECT TAPE tape-device|
   DELETE TAPE LABEL tape-label|
   QUERY [For-Rollforward-Clause]} [TRACE]

Store-Option-Clause:
  ON tape-device [TAPE LABEL tape-label]
  [ALL LOGS|n LOGS][FORCE]

Retrieve-Option-Clause:
  {[For-Rollforward-Clause]
   FROM tape-device[TO directory]|
   {ALL LOGS|LOGS n TO m}
    FROM tape-device[TO directory]|
   HISTORY FILE
    FROM tape-device TO directory }

For-Rollforward-Clause:
  FOR ROLLFORWARD
   TO {END OF LOGS|isotime [USING LOCAL|GMT TIME]}
  [USING HISTORY FILE history-file]
 Rewinding tape. Rewinding tape failed. Reason: "%1S" Reading tape header. Reading tape header failed. Reason: "%1S" Tape header contents The FORCE option has been specified. Updating history. Updating history failed. Reason: "%1S" Empty tape detected. Writing tape header. Writing tape header failed. Reason: "%1S" Writing log file "%1S" to tape. Writing log file "%1S" to tape failed. Reason: "%2S" The number of log files is reduced to "%1S". Do you want to continue? Press Y to contine or N to end. Storing history file on tape. Deleting "%1S" log file from disk. Deleting "%1S" log file from disk failed. Reason: "%2S" Clearing location in history for log files currently on tape "%1S" failed. Reason: "%2S". Ejecting tape. Ejecting tape failed. Reason: "%1S". Positioning tape. Positioning tape failed. Reason: "%1S". Reading log file "%1S" from tape. Reading log file "%1S" from tape failed. Reason = "%2S". Required tapes. Insert tape "%1S". The tape "%1S" is not required. Retrieving log files from tape "%1S". Reading history file. Reading history file failed. Reason: "%1S". Unable to determine the database path. Reason: "%1S". Unable to read database configuration parameter OVERFLOWLOGPATH. Reason: "%1S". Parameter error. Reason: "%1S". Directory "%1S" does not exist. Creating tape header failed. Reason: "%1S".  Scanning history. Scanning history failed. Reason: "%1S". Unknown filetype "%1S". Unable to read the cpio header information. Each file written by db2tapemgr is encapsulated in a cpio archive. "%1S" bytes instead of "%2S" bytes written. File "%1S" is not a file. History entry not found. History access warning. Invalid tape device. Invalid block size. Invalid tape position. Permission denied. File or device does not exist. Interrupt detected. Disk error occurred. Memory allocation failed. File sharing violation. Access denied. File already exists. Invalid path. Error detected. Too many open system files. Too many open files. Error during directory deletion. Disk full. Device not ready. Device is write protected. Range error. Device is busy. Error. Seek error. Sector not found. Write failed. Disk changed. Invalid file handle. Not on disk. Lock violation. End of file reached. Drive locked. Network access denied. Invalid drive. Bad network path. Too many files opened for sharing. No more search handles. The directory of file cannot be created. The current directory cannot be removed. No resources to create process or thread. Press '1' to continue, '2' to decline, or '9' to quit the tool. Press '9' to quit or any other key to continue. Files and control structures were changed successfully. No changes to files or control structures were required. Database was catalogued successfully. Database may not have been catalogued successfully.  Must be done manually. There exists one or more containers that do not reside under the database directory and were not listed in the configuration file.  If this was not intentional, add these containers to the file and re-run this tool. Relocating database... Database relocation was successful. Unable to relocate database, cannot continue. An error occurred building library "%1S". An error occurred building symbol file "%1S". An error occurred executing command "%1S". An error occurred extracting files from archive "%1S". Begin processing for wrappers: %1S %2S %3S %4S %5S Copying file "%1S" as "%2S"... End processing for wrappers: %1S %2S %3S %4S %5S Error: Cannot change to directory "%1S". Error: Cannot create directory "%1S". Error: Directory "%1S" not found. Error: Environment variable "%1S" is not set. Error: Environment variable "%1S" is set to directory "%2S", which does not appear to have a valid client installation. Error: Environment variable "%1S" is set to directory "%2S", which was not found. Error: File "%1S" was not found. Found version "%1S" or higher of client. Library "%1S" was built successfully. Linking library "%1S" with version "%2S" of client... Messages are in file "%1S". Searching for version "%1S" of client... Verify that the client is properly installed. Environment variable "%1S" is set to "%2S" by default. Library "%1S" was not updated because it was not found in directory "%2S". "%1S" is not a valid option. Verify that environment variable "%1S" specifies a valid 64-bit client. This script requires that the "%1S" command be in one of the directories in environment variable $PATH. Error number "%1S" occurred.  For more information (in English), refer to file "%2S". 