package ${NAME}_base_test_pkg;
  import uvm_pkg::*;
  import ${NAME}_environment_pkg::*;
  import ${NAME}_env_config_pkg::*;
  import handshake_sequences_pkg::*;
  import ${NAME}_type_pkg::*;
  import intea_sequences_pkg::*;
import packet_pkg::*;
import uvm_ml::*;
  ////////////////////////////////////
  // Test Config
  ///////////////////////////////////
  class ${NAME}_test_config extends uvm_object;
    `uvm_object_utils( ${NAME}_test_config ) ;

    rand bit reconfigure = 0;
    rand bit sreset = 0;

    //Transactions.
    int number_of_transactions = 500;
    rand int min_delay_between_transactions = 0;
    rand int max_delay_between_transactions = 0;

    function new( string name = "" );
      super.new( name );
    endfunction

    constraint c_reconfigure{
      soft reconfigure dist { 
        0:= 4, 
        1:= 1};
    }

    constraint c_reset{
      soft sreset dist { 
        0:= 4, 
        1:= 1};
    }

    constraint c_delay {
      soft max_delay_between_transactions >= min_delay_between_transactions;
      soft min_delay_between_transactions dist { 
        0       := 9, 
        [1 : 5] :/ 1 };
      soft max_delay_between_transactions dist { 
        0        := 9, 
        [1 : 20] :/ 1 };
    }

    function string convert2string();
      string s;
      $sformat(s,"%s\tTest Configuration:\n",s);
      if (reconfigure === 0) begin
        $sformat(s,"%s\tRuntime Reconfiguration:\t\tOFF\n",s);
      end else begin
        $sformat(s,"%s\tRuntime Reconfiguration:\t\t ON\n",s);
      end
      if (sreset === 0) begin
        $sformat(s,"%s\tSoft Reset:\t\tOFF\n",s);
      end else begin
        $sformat(s,"%s\tSoft Reset:\t\t ON\n",s);
      end
      $sformat(s,"%s\tNumber of transactions to send:\t\t%0d\n",s,number_of_transactions);
      $sformat(s,"%s\tDelay between transactions -> Max: %3d -> Min: %3d\n",s, max_delay_between_transactions, min_delay_between_transactions);

      return s;
    endfunction
  endclass

  ////////////////////////////////////
  // Base Test
  ///////////////////////////////////
  //The base test is setting up the basic testing environment
  class ${NAME}_base_test extends uvm_test;

    `uvm_component_utils(${NAME}_base_test)

    ${NAME}_test_config      test_config;
    
    ${NAME}_environment      environment;
    ${NAME}_env_config       env_cfg;

    intea_base_handshake_master_sequence tsequence;

    function new(string name = "${NAME}_base_test", uvm_component parent = null);
      super.new(name, parent);
    endfunction
    
    function void build_phase(uvm_phase phase);
      super.build_phase(phase);

      environment = ${NAME}_environment::type_id::create(.name("environment"), .parent(this));
      env_cfg     = ${NAME}_env_config::type_id::create( "environment_cfg" );

      test_config = ${NAME}_test_config::type_id::create( "test_config" );

      tsequence = intea_base_handshake_master_sequence::type_id::create("tsequence");

      test_config.randomize();

      uvm_config_db#(${NAME}_env_config)::set(null, "*", "environment_cfg", env_cfg);  // overwriting default values and store it in uvm_config_db
    endfunction: build_phase

    task run_phase(uvm_phase phase);
      Fetch_Interface:
base_test_pkg_sequence_OnlyInput:
      super.run_phase(phase);
      `uvm_info("TEST CASE SETUP*",test_config.convert2string(),UVM_LOW);
    endtask


    // connect the ML ports and sockets
    function void connect_phase(uvm_phase phase);
      super.connect_phase(phase);
    endfunction // connect_phase

    task main_phase(uvm_phase phase);
      super.main_phase(phase);
      assert(tsequence.randomize());
      phase.raise_objection(this);
      fork begin
        for (integer i = 0; i < 1000; i++) begin
  	  #1 uvm_ml::synchronize();
        end;      
      end join_none;
      //FIXME! POINT to interface for which the sequence shall start.
      Fetch_Interface:
base_test_pkg_start_seq_OnlyInput:

      `uvm_info("TEST","transaction sequence_done",UVM_LOW);
      phase.drop_objection(this);
    endtask // main_phase

    //initialize base sequence
    Fetch_Interface:
base_test_pkg_func_OnlyInput:

  endclass
  

endpackage
