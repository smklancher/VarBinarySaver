# VarBinarySaver
Save SSMS VarBinary Results to Files - Quick utility to save arbitrary VARBINARY results to files.

Expected format per line: filename 0xFFF
Where 0xFFF is any amount of binary hex and filename is the file it will be saved to in the same folder as the exe.
This is suitable for pasting the results of a SQL return that returns two columns: VARCHAR, then VARBINARY.

