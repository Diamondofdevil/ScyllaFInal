            S      !   S      C   S      _   S      s   S      �   S      �   T      �   S      �   S      �   S   )     T   *   &  T   +   q  T   ,   �  T   -   �  T   .   �  T   /   �  T   0     T   1   f  T   Q   }  T   R   �  T   U   �  T   V   �  T      %1S has max size of %2S bytes    %1S has watermark of %2S bytes    %1S is of size %2S bytes    Total: %1S bytes Memory for database: %1S Memory for agent %1S Memory for instance Tracking Memory on: %1S at %2S Application Memory for database: %1S   Memory for application %1S Improper usage! Usage: db2mtrk -i | -d | -a | -p [-m | -w] [-v] [-r interval [count]] [-h] Usage: db2mtrk -i | -a | -p [-m | -w] [-v] [-r interval [count]] [-h] No memory allocated Instance not started No active databases No active agents Authorization check failed.  User does not have the authority 
to perform the requested command. No active applications    The following pools are always reported    The following pools are only reported when the '-all' flag is specified 

   -i  Display instance level memory usage
   -d  Display database level memory usage
   -a  Display application level memory usage
   -p  Display agent private memory usage
   -m  Display maximum usage information
   -w  Display watermark usage information
   -v  Display verbose memory usage information
   -r  Run in repeat mode
          interval  Amount of seconds to wait between reports
          count     Number of reports to generate before quitting
   -h  Display this help screen

Notes:

   1. One of -i -d -a -p must be specified.
   2. The -w and -m flags are optional.  An invocation of the application
      is invalid if both flags are specified.
   3. The -m flag reports the maximum allowable size for a given heap
      while the -w flag reports the largest amount of memory allocated
      from a given heap at some point in its history.

Usage scenarios:

   db2mtrk -i -d

      Report current memory usage for instance and all databases

   db2mtrk -i -p -m

      Report maximum allowable size for instance and agent private memory

   db2mtrk -p -r 1 5

      Report agent private memory five times at one second intervals

Heap Legend:

   When running in normal mode (i.e. -v flag not specified) heaps are named
   using the following codes:
 

   -i  Display instance level memory usage
   -a  Display application level memory usage
   -p  Display agent private memory usage
   -m  Display maximum usage information
   -w  Display watermark usage information
   -v  Display verbose memory usage information
   -r  Run in repeat mode
          interval  Amount of seconds to wait between reports
          count     Number of reports to generate before quitting
   -h  Display this help screen

Notes:

   1. One of -i -a -p must be specified.
   2. The -w and -m flags are optional.  An invocation of the applicaiton
      is invalid if both flags are specified.
   3. The -m flag reports the maximum allowable size for a given heap
      while the -w flag reports the largest amount of memory allocated
      from a given heap at some point in its history.

Usage scenarios:

   db2mtrk -i

      Report current memory usage for instance

   db2mtrk -i -p -m

      Report maximum allowable size for instance and agent private memory

   db2mtrk -p -r 1 5

      Report agent private memory five times at one second intervals

Heap Legend:

   When running in normal mode (i.e. -v flag not specified) heaps are named
   using the following codes:
 