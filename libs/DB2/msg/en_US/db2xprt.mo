  �           T   
   S  T      �  S      �  S        S      -  S      E  S   #   c  S   (   �  T   2   �  T   <   �  T   F   )  T   P   d  S   Z   �  T   d   �  S   �    T   �  3  S   
 DB2XPRT [[/p path] | [/v]] [/n] infile [outfile]

 where:

   -p semicolon (;) separated path that points to the location(s) the binaries
      and PDB files are located.
   -v display version information.
   -n format data without regard to line number information.

Note:
   The output files are in XML format. The files can be viewed in their raw
   format or with a browser that supports XML format. If there are multiple
   dumps in the trap file, each dump will be placed in a separate [output] file
   in the following format:
      [output].xml.1, [output].xml.2 ... [output].xml.n

    Possible 32/64 bit trap file header mismatch detected. Use an appropriate  version of this tool to format the file.   Version information extracted from <%1S>  -->DB2 Release    %1S  [%2S]  -->Time of dump   %1S %2S  -->Failing Program %1S Could not open input file %1S Could not open output file %1S Invalid input file detected when reading DLL details Invalid input file detected when reading exception record Invalid input file detected when reading context record Invalid input file detected when reading code dump section Invalid input file detected when reading code call chain frame pointer = %1S Invalid input file detected when reading stack dump section Unable to load module %1S The DB2XPRT command completed successfully. The DB2XPRT command completed successfully with %1S warning(s). 