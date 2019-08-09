package quantize_test_pkg;
  import uvm_pkg::*;
  import quantize_base_test_pkg::*;
  import intea_sequences_pkg::*;

  ////////////////////////////////////
  // Random Test
  ///////////////////////////////////
  //Random test extended from base test. Will use the init_seq function to initialize the transaction sequence and then send random
  //transactions from the input interface. Will override the base sequence with its own transaction sequence.
  class random_test extends quantize_base_test;
    `uvm_component_utils(random_test)
    
    function new(string name = "random_test", uvm_component parent = null);
      super.new(name, parent);

    endfunction

    function void build_phase(uvm_phase phase);    
      intea_base_handshake_master_sequence::type_id::set_type_override(intea_handshake_master_sequence::get_type());

      super.build_phase(phase);

      test_config.number_of_transactions = 1000;
    endfunction: build_phase;
  endclass // random_test

/*
    Remember these phases can be used aswell to correctly time when to do something.
    //*********************
    // Run Phases:
    //task pre_reset_phase(uvm_phase phase);
    //  super.pre_reset_phase(phase);
    //endtask // pre_reset_phase

    task reset_phase(uvm_phase phase);
      super.reset_phase(phase);
    endtask // reset_phase

    //task post_reset_phase(uvm_phase phase);
    //  super.post_reset_phase(phase);
    //endtask // post_reset_phase

    //task pre_configure_phase(uvm_phase phase);
    //  super.pre_configure_phase(phase);
    //endtask // pre_configure_phase

    task configure_phase(uvm_phase phase);
      super.configure_phase(phase);
    endtask // configure_phase

    //task post_configure_phase(uvm_phase phase);
    //  super.post_configure_phase(phase);
    //endtask // post_configure_phase

    //task pre_main_phase(uvm_phase phase);
    //  super.pre_main_phase(phase);
    //endtask // pre_main_phase

    task main_phase(uvm_phase phase);
      super.main_phase(phase);
      assert(tsequence.randomize());
      phase.raise_objection(this);
        tsequence.start(environment.err_recstr_cfg.sequencer);
        \`uvm_info("TEST","transaction sequence_done",UVM_LOW);
      phase.drop_objection(this);
    endtask // main_phase

    //task post_main_phase(uvm_phase phase);
    //  super.post_main_phase(phase);
    //endtask // post_main_phase

    //task pre_shutdown_phase(uvm_phase phase);
    //  super.pre_shutdown_phase(phase);
    //endtask // pre_shutdown_phase

    task shutdown_phase(uvm_phase phase);
      super.shutdown_phase(phase);
    endtask // shutdown_phase

    //task post_shutdown_phase(uvm_phase phase);
    //  super.post_shutdown_phase(phase);
    //endtask // post_shutdown_phase

  endclass
*/  

endpackage
