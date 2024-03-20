This is the first case C# Challenge 

The PowerTradersPositions contains a solution with a single top-level script called Program.cs which is a console .NET 6.0 application that uses the PowerService dll to poll for day ahead prediction data and aggregate then save them to a .CSV file based on required file naming conventions. The data extraction is implemented using thread sleep for now but would require a timer for more precise polling at specific intervals. 
