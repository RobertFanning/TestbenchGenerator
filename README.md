# Testbench Generator

The Testbench Generator is a tool, written in C#, to maximise the efficiency of the UVM testbench's construction by minimising the input required from the verification engineer. It eliminates the need for the manual definition of design-specific components by automating the procedure. Its operation can be defined in two phases, interpretation and generation. 

The first phase interprets the DUT's specification, extracting all relevant information that would be required during a manual construction. Interpretation is achieved on three separate abstraction levels, the lowest level performs lexical analysis, the second level performs parsing and the third level performs any high-level deductions. 

The second phase then generates a testbench complying with the DUT's specification by utilising the information extracted during the interpretation phase. The generation phase requires a template of the UVM testbench on which to base generations. These template files are provided in the Template folder. 

<<<<<<< HEAD
## Files
=======
## Files
>>>>>>> 1ade78e2c3cd7e96f586c10a1d9d468dd20b0838

This repository specifies the complete description of the Testbench Generator. The main files contained within this repository are:

* **TestbenchGeneratorMain.cs**: This is the main function of the program that triggers all the required behaviour. It has been kept minimal. It currently only requests the input files from the user, concatenates them into a single character stream and starts the Parser with them. Then when the Parser is complete, it uses the output from the it to start the code generator.
* **Parser.cs**: The Parser is initiated by the main function. The Parser first initiates a Lexical Analyser that it processes the input stream in parallel with.
* **LexicalAnalyser.cs**: This file describes the Lexical Analyser that processes the input files into tokens and outputs these tokens to the Parser.
* **Generator.cs**: This file describes the code generator. The post-interpretation and mid-generation configurations are embedded in this file. The post-interpretation configurations are triggered before the code generator is run. The mid-generation configurations are triggered in parallel with the code generator. The output from the code generator are the generated packages.

Aside from the main files listed above, there are four folders containing necessary functions and Symbol Tables. The contents of these folders are listed below:

* **CustomDataTypes**: This folder contains all the necessary symbol table definitions to contain data interpreted from the Package files. 
* **DUTProductions**: This folder contains all the necessary symbol table definitions to contain data interpreted from the design module.
* **ExpressionParsing**: This folder contains all the methods for expression parsing. These are methods nested in a hierarchy according to their precedence. 
* **Deduction**: This folder contains the several class descriptions in which the deductions are computed. 


### Prerequisites

To run the Testbench Generator you must have all the mandatory input files. These are:
* Design files: The design module and package declarations in which custom data types are declared.
* Reusable Packages: These are the templates used by the code generator that contain Placeholders and Insertion Point flags.
* Insertion Point files: There is one mandatory insertion point file containing the independent insertions. Separate insertion point files are then needed for each interface protocol present in the design files. These protocol dependent insertion point files only specify two insertions each. 
* Output path: A path is required for the code generator to output the generated packages to.

The above inputs are mandatory. However, the reusable package files and insertion point files have already been provided. Therefore, the only inputs necessarily required are the design files and output. Insertion point files will need to be added if any new protocols are used.
The optional input is:
* Configuration file: This file specifies any configurations required to be made.

## Running the tests

To run the Testbench Generator you will need the .NET Framework.
In the TestbenchGenerator repository enter the command "dotnet run".
This will initiate the TestbenchGenerator.
You will then be prompted to provide the input files.

## Author

**Robert Fanning** 



