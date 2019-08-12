//-----------------------------------------------------------------------------
//                              Configuration file
//                                   --------
//                              TestbenchGenerator
//                                   --------
// All configurations that wish to be made to the Testbench Generator's behaviour 
// are specified in this file. To configure the Testbench Generator, simply fill
// out the fields below corresponding to the configurations you require. This 
// file must then be passed as an input to the Testbench Generator.
//
// Configurations occur in one of two phases:
//    1. Post-Interpretation
//    2. Mid-generation
//
// These two phases are clearly denoted by flags below.
//
// If you wish to configure mulitple interfaces simply duplicate the field 
// headings below and specify a different name for the begin and end flags
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Package Configurations:
// Specify any extra packages you wish to include in the generated testbench.
IncludePackages:
  

//-----------------------------------------------------------------------------
// Post-Interpretation Configurations
//-----------------------------------------------------------------------------

// The field below is for denoting the start of an interface's configuration.
// After "begin" specify a unique identifier for the configuration.
// This same identifier will be used to terminate the configuration at the end.
InterfaceConfiguration:
begin 

// Specify the name of the interface being configured under the field below.
// This will ensure that it is correctly added by the Testbench Generator.
InterfaceName:

// Specify the protocol of the interface under the field below.
InterfaceType: 

// Specify the direction of the interface under the field below.
InterfaceDirection: 

// Specify the signals to be groupped within the interface under the field below.
Signals: 

//--------------------------------------------
//Mid-Generation Configurations
//--------------------------------------------
// The mid-generation configurations consist only of insertion point overrides.
// Under the heading "InsertionPointOverrides:" write the names of all insetion
// points you wish to override followed by the override you wish to specify.
// These insertion point overrides exist within the interface's begin and end
// flags and therefore only apply to that interface. 
InsertionPointOverrides:


// Specifies the end of an interface's configuration.
// Use the same flag as after the begin.
end
