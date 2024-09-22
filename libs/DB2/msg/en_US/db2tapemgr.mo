  �         V         V         T         T      V  T      �  T   	   �  T   
   �  T        V      0  T      E  V      v  T      �  V      �  T      �  V      <  V      k  V      �  V      �  V        V      Z  T      v  T      �  V      �  T      �  T        T        V      M  V      m  V       �  V   !   �  V   #     T   $   #  V   %   _  V   &   �  V   '   �  V   (   	  V   )   e	  T   *   t	  V   +   �	  T   ,   �	  V   -   �	  V   .   
  V   /   ?
  V   0   �
  V   1   5  V   2   �  V   3     T   4   9  T   5   ]  T   6   ~  V   7   �  T   8   �  T   9   �  V   :   2  V   ;   W  V   <   }  T   =   �  V   >   �  V   ?   �  T   @   $  V   A   Y  V   B   u  V   C   �  T   D   7  V   E   �  T   F   �  V   G   �  V   H     V   I   -  V   J   _  T   K   q  V   d   �  V   e   �  T   f   �  V   g     V   h   $  V   i   M  V   j   r  V   k   �  V   l   �  V   m   �  V   n   �  V   o     V   p   >  V   q   `  V   r   �  V   s   �  V   t   �  V   u     V   v   0  T   w   b  V   x   �  T   y   �  V   z   �  V   {   �  V   |   
  T   }   ,  V   ~   Y  T   �   t  T   �   v  T   �   x  T   �   z  T   �   |  T   �   ~  T   Warning: "%1S" Error: "%1S" 
DB2 tape manager for log files
Usage:
db2tapemgr [DATABASE database-alias][ON DBPARTITIONNUM db-partition-number]
  {{{STORE | DOUBLE STORE}[Store-Option-Clause]|
    RETRIEVE [Retrieve-Option-Clause]|
    SHOW TAPE HEADER tape-device]}[USING blocksize][EJECT]}
   EJECT TAPE tape-device|
   DELETE TAPE LABEL tape-label|
   QUERY [For-Rollforward-Clause]

Store-Option-Clause:
  ON tape-device [TAPE LABEL tape-label][ALL LOGS|n LOGS][FORCE]

Retrieve-Option-Clause:
  {For-Rollforward-Clause FROM tape-device[TO directory]|
   {ALL LOGS|LOGS n TO m} FROM tape-device[TO directory]|
   HISTORY FILE           FROM tape-device TO directory }

For-Rollforward-Clause:
  [FOR ROLLFORWARD TO {END OF LOGS|isotime [USING LOCAL|GMT TIME]}
  [USING HISTORY FILE history-file]] DB2 tape manager for log files finished successfully. Warning: DB2 tape manager for log files finished with warnings. Error: DB2 tape manager for log files failed with errors. Error: Internal error. Reason: "%1" Rewinding tape. Error: Rewinding tape failed. Reason: "%1S" Reading tape header. Error: Reading tape header failed. Reason: "%1S" Tape header contents: Warning: "%1S" is not on disk as expected. No log files found for processing. Error: Label "%1S" already known, but not inserted in tape drive. Tape inserted is labeled "%2S". Error: Invalid value "%1S" for variable "%2S". Error: Tape not expired. Tape will expire at "%1S". Error: Log files on tape are of another database "%1S". Error: Log files on tape are of another database instance "%1S". Error: Log files on tape are of another database partition "%1S". Warning: Forcing overwrite. Updating history. Error: Updating history failed. Reason: "%1S" Empty tape detected. Error: Tape has not been used for storing log files before. Writing tape header. Error: Writing tape header failed. Reason: "%1S" Writing log file "%1S" to tape. Error: Writing log file "%1S" to tape failed. Reason: "%2S" Warning: Only "%1S" log files fit on tape. Reducing number of log files to "%1S" and retry. Storing history file on tape. Warning: Storing history file on tape failed. Reason: "%1S" Deleting "%1S" log file from disk. Error: Deleting "%1S" log file from disk failed. Reason: "%2S" Clearing location in history for log files currently on tape "%1S". Error: Clearing location in history for log files currently on tape "%1S" failed. Reason: "%2S" Ejecting tape. Error: Ejecting tape failed. Reason: "%1S" Positioning tape. Error: Positioning tape failed. Reason: "%1S" Reading log file "%1S" from tape. Error: Reading log file "%1S" from tape failed. Reason: "%2S" Error: Tape contains log files for partition "%1S", but tool executed for partition "%2S". Tape contains log files of database "%1S", but tool executed for database "%2S". Continue ? Press 'y' to continue, 'n' to decline or 'q' to quit the tool. Tape contains log files of instance "%1S", but tool executed for instance "%2S". Continue ? Press 'y' to continue, 'n' to decline or 'q' to quit the tool. Don't try to retrieve log file "%1S", because log file already on disk. Error: No matching backup found. No required log file on tape found. No required log file is on tape. Log files required for backup taken at "%1S": No tapes required. Required tapes: Please insert a required tape, for example "%1S".  Press 'q' to quit or any other key to continue. Inserted tape "%1S" is not required. Retrieving log files from tape "%1S". Reading history file. Error: Reading history file failed. Reason: "%1S" Working on database "%1S". Error: Database not specified on command line and DB2DBDFT not set. Error: Cannot determine database path. Reason: "%1S" Working on partition "%1S". Error: Failed to read database configuration parameter OVERFLOWLOGPATH. Reason: "%1S" Error: No directory specified at command line and database configuration parameter OVERFLOWLOGPATH not set. Error: Value "%1S" of database configuration parameter OVERFLOWLOGPATH is not a directory. Error: Cannot double store to same tape. Error: Parameter error. Reason: "%1S" Using automatic generated tape-label "%1S". Directory "%1S" does not exist. Error: creating tape header failed. Reason: "%1S" Scanning history. Error: Scanning history failed. Reason: "%1S" Unknown magic: "%1S" Cannot read filename in cpio header File name in cpio "%1S" does not match "%2S" Directory "%1S" does not exist File "%1S" found instead of cpio trailer "%1S" instead of "%2S" bytes written Database "%1S" is remote "%1S" is not a file Database "%1S" not found in database directory Parameter "%1S" expected Value "%2S" of parameter "%1S" is too long Value "%2S" of parameter "%1S" is too short Device "%1S" is not a tape device Device "%1S" is a rewind device History file "%1S" does not exist History file "%1S" does not end with "%2S" Value "%2S" of parameter "%1S" is not alpha numeric Value "%2S" of parameter "%1S" is out of range Parameter blocksize needs to be a multiple of 512 Value "%2S" of parameter "%1S" is not numeric No operation specified Unknown operation "%1S" specified Too many arguments starting with "%1S" Invalid time format "%1S" Cannot reduce number of log files "%1S" is not a tape header file, found "%2S" Invalid tape header format Y y N n Q q 