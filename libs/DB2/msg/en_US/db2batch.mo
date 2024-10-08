  �         T      )  T      }  T      �"  T      �*  T      �-  T   d   �-  S   e   �-  T   f   �-  S   g   �.  S   h   �.  T   i   4/  S   j   U/  S   k   s/  S   l   �/  S   m   �/  S   n   �/  S   o   q0  S   p   �0  S   q   �0  S   r   �0  S   ,  �0  T   -  1  T   .  L1  T   /  �1  T   0  �1  T   1  ^2  T   �   3  T   �  q3  T   �  �3  T   �  �3  T   �  �3  T   �  &4  T   �  V4  T   �  �4  T   �  �4  T   �  �4  S   �  
5  S   �   5  S      C5  S     a5  T     �5  T     �5  T     06  T     z6  T     �6  S     �6  T     �6  T   	  7  T   
  L7  T     �7  S     �7  T     �7  T     8  T     A8  T     w8  S     �8  T      �8  T   !  �8  T   "  �8  T   #  �8  T   $  �8  T   %  �8  T   &  �8  T   '  9  T   (  9  T   )  &9  T   >  49  T   ?  >9  T   R  D9  T   S  H9  T   T  R9  T   U  Z9  T   �  h9  T   �  �9  T   �  �9  T   �   :  T   �  `:  T   �  �:  T   �  �:  T   �  ;  T   �  c;  T   �  �;  T   �  �;  T   �  -<  T   �  m<  T   �  �<  T   �  �<  T   �  =  S   �  $=  S   �  C=  S     Y=  T     �=  T     �=  T     �=  T      �=  T   !  >  T   "  Q>  T   #  �>  T   $  �>  T   %  �>  T   &  ?  T   '  0?  T   (  O?  T   )  q?  T   *  �?  T   +  �?  T   ,  �?  T   -  �?  T   .  @  T   /  <@  T   0  j@  T   1  �@  T   2  �@  T   3  �@  T   4  �@  T   5  A  T   �  ?A  T   �  jA  S   �  �A  S   �  �A  S   �  SB  S   �  �B  S   �  C  S   �  AC  T   �  �C  T   �  �C  T   �  �C  T   �  D  T   �  ID  T   �  �D  T   �  �D  S   �  E  T   �  wE  T   �  �E  S   �  �E  T   �  �E  T   �  *F  T   �  \F  T   �  �F  T   �  �F  T   �  G  T   �  bG  T   �  �G  T   �  �G  T   �  H  S   �  -H  S   Usage:

db2batch
    [-d dbname] [-a userid[/password]]
    [-f sql_file] [-m parameters_file] [-r result_file[,summary_file]]
    [-v on||off] [-s on||off] [-q on||off||del] [-g on||off]
    [-l stmt_delimiter] [-t col_delimiter] [-w col_width]
    [-c on||off] [-iso rr||rs||cs||ur] [-car [cc||wfo] [-cli [cache_size]]
    [-o [option value] [...]] [-time on||off] [-i short||long||complete]
    [-msw [switch on||off||hold] [...]] [-mss [snapshot] [...]]
    [-z output_file[,summary_file]]
    [-h] [-u] [-?]

    ('db2batch -h' for extended help)
 
Where:  (All options and values are case insensitive)

  -d    Name of database to connect to.
        Default is to use the name set in $DB2DBDFT.
  -a    Authentication host user id with optional password.
        Default is to not specify a user id or password.
  -f    Input file containing SQL statements.
        Default is to use stdin.
  -m    Input file containing parameter values to bind to SQL statement
        parameter markers before executing a statement.
        Default is to not bind parameters.
  -r    Output file to contain the query results. If 'summary_file' is
        specified, it will contain the summary table.
        Default is to stdout. (screen)
  -v    Verbose mode.  Reports extra information during processing.
        Default is off.
  -s    Summary table.  Provides a summary of elapsed times with
        arithmetic and geometric means, complete with the number of
        query rows fetched and output.
        Default is on.
  -q    Query output only.  The output options are:
            off - Output information and headings with query results.
            on  - Output only query results in non-delimited format.
            del - Output only query results in delimited format.
        Default is off.
  -g    Group block output.  When 'on', takes one snapshot for the
        entire block and reports only block timing in the summary table.
        When 'off', takes a snapshot and reports summary table timing
        for each statement executed in the block.
        Default is on.
  -l    SQL Statement Delimiter.  Changes SQL statement separator.
        Default is ';'.
  -t    Single character output SQL column separator.
        Specify -t TAB for tab or -t SPACE for space.
        Default is SPACE for '-q on' and ',' for '-q del'.
  -w    Maximum Result Set Column Width.  Truncates all data to this
        width for displaying, unless data cannot be truncated.
        The width may be increased in order to eliminate the warning
        CLI0002W and thus get more accurate Fetch time.
        Default is 32768, ranging from 0 to 2G.	
  -c    Automatically commit after each SQL statement.
        Default is on.
  -iso  Isolation Level.
            rr - Repeatable Read (ODBC Serializable)
            rs - Read Stability (ODBC Repeatable Read)
            cs - Cursor Stability (ODBC Read Committed)
            ur - Uncommitted Read (ODBC Read Uncommitted)
        Default is rr.
  -car  Concurrent Access Resolution
            cc  - Currently Commited
            wfo - Wait for Outcome
  -cli  CLI Execution.  This option exists for backwards compatibility
        with versions where the default execution mode was not CLI.
        The optional cache size argument is also ignored.
  -o    Control option and value pairs. The possible options are: 
            r <rows output>  (Maximum query result rows shown)
                Default is -1. (All)
            f <rows fetched>  (Maximum query result rows fetched)
                Default is -1. (All)
            p <performance detail>  (A detail level between 0 and 5)
                0 - No timing or monitoring snapshots.
                1 - Timing information.
                2 - 1 and agent_id snapshot.
                3 - 2 and db2, dbase snapshots.
                4 - 3 and dbase_applinfo snapshot.
                5 - 4 and dbase_tablespaces, dbase_tables,
                    dbase_bufferpools snapshots.
                Default is 1.
            o <query optimization class> (A class of 0-3, 5, 7, or 9)
                Default is -1. (Server default)
            e <explain mode>
                no      - Execute query but do not explain.
                explain - Explain query but do not execute.
                yes     - Explain and execute the query.
                Default is no.
            s <error stop>
                no      - Continue when encountering non-critical errors
                yes     - Stop when encountering errors
                Default is no.                
  -time SQL statement timing.
        Default is on.
  -i    Elapsed time interval measurement.
            short    - elapsed time to run the statement.
            long     - elapsed time to run the statement including 
                       overhead between statements. 
            complete - elapsed time to run the statement where the prepare, 
                       execute, and fetch times are reported separately.
        Default is short.
  -msw  Monitor Switches.  Each switch specified should be followed by
        either 'on', 'off', or 'hold' to turn on, turn off, or
        keep the switch's previous state respectively.  The possible
        switches are:
            uow, statement, table, bufferpool, lock, sort, timestamp
        The special switch 'all' sets all of the above switches.
        By default all switches are set to 'hold'.
  -mss  Monitor Snapshots.  List all monitoring snapshots that should be
        taken after each statement or block is executed.  More than one
        snapshot can be taken at a time, with information from all
        snapshots combined into one large snapshot before printing.
        The possible snapshots are:
            applinfo_all, dbase_applinfo, dcs_applinfo_all, db2, dbase,
            dbase_all, dcs_dbase, dcs_dbase_all, dbase_remote,
            dbase_remote_all, agent_id, dbase_appls, appl_all,
            dcs_appl_all, dcs_appl_handle, dcs_dbase_appls,
            dbase_appls_remote, appl_remote_all, dbase_tables,
            appl_locks_agent_id, dbase_locks, dbase_tablespaces,
            bufferpools_all, dbase_bufferpools, dynamic_sql
        The special snapshot 'all' takes all of the above snapshots.
        Any snapshots involving an appl ID are not supported in favour of
        their agent ID (appl handle) equivalents.
        Default is to not take any monitoring snapshots.
  -z    Redirect all output to a output file. It is similar to the -r option, 
        but includes any messages or error codes with the output. 
        If 'summary_file' is specified, it will contain the summary table. 
        Default is to stdout. 
  -h    Display this help screen.
  -u    Alias for -h.
  -?    Alias for -h.

 
SQL Input File Format:

  SELECT * FROM TABLE;     (all SQL statements must end with a delimiter)
  HELP;                    (display this help message and db2batch notes)
  QUIT;                    (stop executing any more SQL statements)
  -- comment               (comments are ignored)
  --#COMMENT comment       (comment appears in the output)
  --#BGBLK [repeat_count]  (start of a block of queries)
  --#EOBLK                 (end of a block of queries)
  --#PARAM <value> [...]   (values for a parameter in next SQL statement)
  --#SET <option> <value>  (see following table)

      -----------------------------------------------------------------
      option        value     default   description
      -----------------------------------------------------------------
      ROWS_FETCH    -1 to n   -1        -1 fetches all the rows
      ROWS_OUT      -1 to n   -1        -1 shows all fetched rows
      PERF_DETAIL   0 to 5    1         Performance detail level
      TIMING        on/off    on        Print timing information
      SNAPSHOT      string    none      Monitoring snapshots to take
      DELIMITER     string    ;         SQL statement delimiter
      PAUSE         n/a       n/a       Stop and wait for user input
      TIMESTAMP     n/a       n/a       Output a timestamp
      SLEEP         1 to n    n/a       Sleep for x seconds
      ERROR_STOP    no/yes    no        Stop when encountering errors

Note:

  Blocks of queries can be looped over multiple times by specifying a
  repeat count when defining the block.  The statement(s) in the block
  will only be prepared the first time.

  If a parameters file is used, then each line specifies the parameter
  values for a given statement and a given iteration of a block.
  If instead #PARAM directives are used, multiple values and even
  parameter ranges are specified in advance for each parameter of each
  statement, and on each iteration of the block a random value is chosen
  from the specified sets for each parameter.  #PARAM directives and a
  parameters file cannot be mixed.

 
Parameter Value Format:

  -36.6       'DB2'         X'0AB2'    G'...'   NULL
  12          'batch'       x'32ef'    N'...'   null
  +1.345E-6   'db2 batch'   X'afD4'    g'...'   Null

  Each parameter is defined like a SQL constant, and is separated from
  other parameters by whitespace.  Non-delimited text represents a
  number, plain delimited (') text represents a single byte character
  string, x/X prefixed delimited (') text represents a binary string
  encoded as pairs of hex digits, g/G/n/N prefixed delimited (') text
  represents a graphic string composed of double byte characters, and
  NULL (case insensitive) represents a null value.

Parameter Input File Format:

  Line X lists the set of parameters to supply to the Xth SQL statement
  that is executed in the input file.  If blocks of statements are not
  repeated, then this corresponds to the Xth SQL statement that is listed
  in the input file. A blank line represents no parameters for the 
  corresponding SQL statement. The number of parameters and their types 
  must agree with the number of parameters and the types expected by the 
  SQL statement.

Parameter Directive Format:

  --#PARAM [single | start:end | start:step:end] [...]

  Each parameter directive specifies a set of parameter values from which
  one random value is selected for each execution of the query.  Sets are
  composed of both single parameter values and parameter value ranges.
  Parameter value ranges are specified by placing a colon (':')
  between two valid parameter values, with whitespace being an optional
  separator.  A third parameter value can be placed between the start and
  end values to be used as a step size which overrides the default.  Each
  parameter range is the equivalent of specifying the single values of
  'start, start+step, start+2*step, ... start+n*step' where n is chosen
  such that 'start+n*step <= end' but 'start+(n+1)*step > end'.
  While parameter directives can be used to specify sets of values for
  any type of parameter (even NULL), ranges are only supported on
  numerical parameter values (integers and decimal numbers).

 
General Notes:

 1. All SQL statements MUST be terminated by a delimiter (default ';')
    set by the --#SET DELIMITER command.  This delimiter can be 1 or 2
    characters.
 2. SQL statement length is limited only by available memory and the
    interface used.  Statements can break over multiple lines, but
    multiple statements are not allowed on a single line.
 3. Input file line length is limited only be available memory.
 4. db2batch issues its own connect and connect reset.
 5. PAUSE & SLEEP are timed when in long timing mode.
 6. Explain tables must be created before explain options can be used.
 7. All command line options and SQL file statements are case insensitive
    with respects to db2batch.
 * Type 'db2batch -h' for help.
 * Timestamp: %1S
 * Summary Table:
 
* Total Entries:        %1S
* Total Time:           %2S seconds
* Minimum Time:         %3S seconds
* Maximum Time:         %4S seconds
* Arithmetic Mean Time: %5S seconds
* Geometric Mean Time:  %6S seconds
 * List of %1S supplied parameter(s):
 *   
* db2batch is paused
*   
* Press ENTER to continue...
 * Sleeping for %1S second(s)...
 
* SQL Statement Number %1S:
 
* %1S row(s) fetched, %2S row(s) output.

 * Elapsed Time is: %1S seconds

 * Elapsed Time is: %1S seconds (long)

 * Prepare Time is: %1S seconds
* Execute Time is: %2S seconds
* Fetch Time is:   %3S seconds
* Elapsed Time is: %4S seconds (complete)

 * Comment: "%1S"
 * Start of Block Number %1S:
 * Block Iteration %1S of %2S:
 * End of Block Number %1S
 ** The following warnings were issued:
 ** All arguments before the first command line option are being ignored
 ** Warning, cannot pause db2batch when SQL input is from STDIN
** The pause command is being ignored
 ** Warning, sleeping ended early due to signal interruption
 ** Warning, no delimiter was found for the final SQL statement in the input.  This statement will be ignored.
 ** Warning, no end of block directive encountered for matching start of block directive in the input.  All statements and actions in this block will be ignored.
 *** No database specified and empty $DB2DBDFT environment variable
*** A database must be specified to continue
 *** Unable to allocate sufficient memory
 *** Unable to get current locale's decimal character
 *** Unable to initialize data structure
 *** Unable to print summary table headings
 *** Command line options cannot be zero length
 *** Invalid or unsupported short command line option
 *** Invalid or unsupported long command line option
 *** Invalid argument(s) for command line option
 *** For argument "%1S"
 *** For option "%1S"
 *** Unable to open %1S file "%2S"
 *** Unable to close %1S file
 *** Data value ranges are not supported for non-numeric parameters
 *** Parameter data decimal range must specify a step size
 *** Parameter data range composed of something other than integers or decimals
 *** Parameter data range has minimum value larger than its maximum value
 *** Sets of parameter data values cannot be empty
 *** For range '%1S%2S%3S:%4S'
 *** Unable to print monitoring snapshot
 *** Cannot nest blocks of statements
 *** End of statement block needs matching begin
 *** Cannot give parameter directives when using a parameters file
 *** For comment directive '%1S%2S'
 *** Parse error, invalid #SET directive
 *** Parse error, invalid comment directive
 *** Parse error, invalid arguments to comment directive
 *** Unable to configure snapshot monitoring switches
 *** Parse Error: "%1S"
 *** Unable to initialize API libraries
 Type Number Repetitions Total Time (s) Min Time (s) Max Time (s) Arithmetic Mean Geometric Mean Row(s) Fetched Row(s) Output Statement Block SQL parameter results summary table *** Error, unable to convert monitoring data to a number
 *** Error, monitoring data overflowed during number conversion
 *** Error, monitoring element is not a timestamp as expected
 *** Error, monitoring element is not a long string as expected
 *** Error, monitoring element is not a text string as expected
 *** Error, cannot convert monitoring timestamp to locale format
 *** Error, monitoring element is not a switch as expected
 *** Error, monitoring element is not a hexidecimal number as expected
 *** Error, monitoring element is not hexidecimal text as expected
 *** Error, monitoring element is not a FCM Node group as expected
 *** Error, monitoring element is not a tablespace name as expected
 *** Error, monitoring element is not a cursor name as expected
 ** Warning, skipping monitoring element which cannot be printed
 *** Invalid snapshot monitor self describing data stream
             Monitoring Information
 ** CLI warning in %1S:
 *** CLI invalid handle in %1S
 ** CLI error in %1S:
 retrieval of agent ID (application handle) set environment handle attribute set connection handle attribute set statement handle attribute removing statement's existing parameter bindings retrieving the number of parameter markers in the statement retrieving information on a parameter marker in the statement binding parameter to statement parameter marker retrieving number of query result columns retrieving query result column information binding query result column fetching next query result row allocating the environment handle allocating the database connection handle establishing the database connection allocating the statement handle preparing the SQL statement executing the SQL statement closing SQL statement result cursor unbinding of all SQL statement result columns deallocating the statement handle commiting SQL statements disconnecting from the database deallocating the database connection handle deallocating the environment handle checking whether the connection is active *** CLI error, invalid agent ID retrieved
 ** CLI attribute number %1S
 *** CLI error, supplied %1S parameter(s) for a SQL statement containing %2S parameter marker(s)
 *** CLI error, attempting to assign NULL to statement parameter marker %1S which cannot have a NULL value
 ** CLI warning, unknown type for parameter marker %1S, cannot guarantee proper parameter value data conversion
 *** CLI error, attempt to bind statement parameter marker %1S to data of a different type
 ** Statement parameter marker %1S
 ** CLI query result column has unsupported SQL data type, skipping it
 *** CLI error in printing query result headings
 *** CLI error in printing query result row
 *** CLI error in resetting monitoring statistics
 *** CLI error in capturing monitoring statistics
 *** Error, delimited parameter value is missing a closing delimiter
 *** Error, expecting a single parameter data value but received a range of parameter data values instead
 *** For parameter data value %1S
 *** Error, parameter data value has a different type than the rest of the values in the set
 *** Error, invalid parameter data value range
 *** For parameter token %1S: "%2S"
 ** Warning, host variable string truncated
 ** Warning, authorization name longer than 8 bytes
 ** Warning, removed Null argument(s) to function
 ** Warning, number of columns and host variables does not match
 ** Warning, missing WHERE in UPDATE or DELETE statement
 ** Warning, adjusted date calculation to avoid impossible date
 ** Warning, the DYN_QUERY_MGMT database configuration parameter is enabled
 ** Warning, substituted unconvertable character
 ** Warning, ignored errornous arithmetic expression(s)
 ** Warning, conversion error for a data value in the SQLCA
 SQLCODE=%1S SQLSTATE=%2S SQLERRP=%3S
 SQLERRP=%1S  %2S
 