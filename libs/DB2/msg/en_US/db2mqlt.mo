  `   �      S   �      S   �  �  S     �  S     �  T     �  S   $  h	  S   .  .
  S   
Usage: db2mqlsn %1S <options>. 
%1S <configuration database>
[%2S <configDB user ID> %3S <configDB password>]
        Database in which configuration information is stored.  Authorization
        information is optional.  If none is supplied, an implicit connection
        will be attempted.

[%4S <configuration name>]
        Individual tasks are grouped in configurations, which allows them
        to be run together.  If no configuration name is specified, the
        default configuration is used. 
Usage: db2mqlsn <command> <options>
Command:
        %1S    -- print this message
        %2S     -- start listener
        %3S     -- add tasks to configuration
        %4S  -- remove tasks from configuration
        %5S    -- display configuration information
        %6S   -- send admin command to running listener

Options depend on <command>.  For more information, run
        db2mqlsn %1S <command> 
[%1S <admin queue name> [%2S <admin queue manager>]]
        Queue to listen to for admin commands.  If no queue manager
        is given, the configured default queue manager will be used.
        If no admin queue is specified, app will not receive
        admin (e.g., shutdown, restart) commands via message queue.  
%1S <input queue name> [%2S <queue manager name>]
        Queue on which this task will listen for messages.  If no queue
        manager is specified, the default queue manager will be used.
        (The combination of an input queue and queue manager must be unique
        within a configuration.)

%3S <stored procedure schema> %4S <stored procedure name>
%5S <stored procedure database>
%6S <dbName user ID> %7S <dbName password>
        Information required to specify the stored procedure to which messages
        from the specified queue will be passed.  This information includes
        the schema and name of the stored procedure and the database in which
        to find it, as well as the user on whose behalf to execute the stored
        procedure, and authorization for that user.

[%8S]
        Integrate message reading/writing into the stored procedure
        transaction, coordinated by MQ.  By default, message manipulations
        are not executed transactionally.

[%9S <number of instances to run>]
        The number of duplicate instances of this task to run in this
        configuration.  If no value is specified, only one instance is run. 
%1S <input queue name> [%2S <queue manager name>]
        The task to remove from this configuration.  (The combination of input
        queue and queue manager are unique within a configuration.) 
%1S <admin queue name>
%2S <namelist of admin queue names>
[%3S <admin queue manager>]
        The queue or list of queues on which to send admin commands.
        Only one of -adminQueue or -adminQueueList may be given.
        If no queue mnager is given, the configured default queue manager
        will be used.

%4S <admin command>
      One of shutdown, restart.
      Shutdown exits a running listener when it has finished processing
      the current message.
      Restart performs a shutdown, then re-reads its configuration and restarts. 