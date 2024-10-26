**Directory Traversal and CSV Processing Project**

**Overview**

This .NET program traverses through all directories and subdirectories within a specified path, logging the names of directories and processing CSV files found within them. Each CSV file contains records that the program reads, validates, and enriches with a Date field based on the directory structure. Incomplete or invalid records are logged and counted as skipped, while valid rows are processed. Upon completion, the program logs execution details, including the total number of valid and skipped rows and the time taken for execution.

**Features**

**Directory Traversal**: Recursively searches for subdirectories and files, logging each directory name.
**CSV Processing:** Reads CSV files and validates each record, adding a Date value from the directory structure.
**Logging:** Logs all activities, including skipped rows and errors, to a file.
**Execution Summary:** Provides total execution time, valid and skipped row counts, enhancing data quality and process insights.
