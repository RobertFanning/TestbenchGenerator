package ${NAME}_env_config_pkg;
  import uvm_pkg::*;
  `include "uvm_macros.svh"
  import ${NAME}_type_pkg::*;

  class ${NAME}_env_config extends uvm_object;
    `uvm_object_utils(${NAME}_env_config)

    string LOG_NAME = "${NAME} Env Cfg";

    bit                          is_top              = 1;                 // set to 0 if this is instantiated in another evironment
    uvm_active_passive_enum      is_active           = UVM_ACTIVE;

    //ADD RELEVANT VIFS
    Fetch_Interface:
env_config_vif:
    
    function new(string name = "${NAME}_env_config");
      super.new(name);
    endfunction // new


    function lookup_interfaces(
      Fetch_Interface:
env_config_lookup:
    );
      //ADD CONFIG DB GET FOR ALL INTERFACES
      //FIXME!
      Fetch_Interface:
env_config_configdb:
      

    endfunction
    
  endclass
  
endpackage
