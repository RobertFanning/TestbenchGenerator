package quantize_env_config_pkg;
  import uvm_pkg::*;
  `include "uvm_macros.svh"
  import quantize_type_pkg::*;

  class quantize_env_config extends uvm_object;
    `uvm_object_utils(quantize_env_config)

    string LOG_NAME = "quantize Env Cfg";

    bit                          is_top              = 1;                 // set to 0 if this is instantiated in another evironment
    uvm_active_passive_enum      is_active           = UVM_ACTIVE;

    //ADD RELEVANT VIFS
    x_in_handshake_vif_t x_in_vif;
    x_predict_handshake_vif_t x_predict_vif;
    scale_factor_handshake_vif_t scale_factor_vif;
    x_encoded_handshake_vif_t x_encoded_vif[2];
    mult_handshake_vif_t mult_vif;
    
    function new(string name = "quantize_env_config");
      super.new(name);
    endfunction // new


    function lookup_interfaces(
      string x_in_vif_name="quantize_x_in_vif",
      string x_predict_vif_name="quantize_x_predict_vif",
      string scale_factor_vif_name="quantize_scale_factor_vif",
      string x_encoded_vif_name="quantize_x_encoded_vif",
      string mult_vif_name="quantize_mult_vif"
    );
      //ADD CONFIG DB GET FOR ALL INTERFACES
      //FIXME!
  if(!uvm_config_db#(x_in_handshake_vif_t)::get(null, "", x_in_vif_name, x_in_vif)) begin
    `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", x_in_vif_name));
  end
  if(!uvm_config_db#(x_predict_handshake_vif_t)::get(null, "", x_predict_vif_name, x_predict_vif)) begin
    `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", x_predict_vif_name));
  end
  if(!uvm_config_db#(scale_factor_handshake_vif_t)::get(null, "", scale_factor_vif_name, scale_factor_vif)) begin
    `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", scale_factor_vif_name));
  end
  for (int i = 0; i < 2; i++) begin
    if(!uvm_config_db#(x_encoded_handshake_vif_t)::get(null, "", $sformatf("%s_%0d",x_encoded_vif_name,i), x_encoded_vif[i])) begin
      `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", {x_encoded_vif_name,$sformatf("_%0d",i)}));
    end
  end
  if(!uvm_config_db#(mult_handshake_vif_t)::get(null, "", mult_vif_name, mult_vif)) begin
    `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", mult_vif_name));
  end
      

    endfunction
    
  endclass
  
endpackage
