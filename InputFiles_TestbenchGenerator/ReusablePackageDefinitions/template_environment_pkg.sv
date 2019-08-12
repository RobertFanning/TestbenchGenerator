package ${NAME}_environment_pkg;
  import uvm_pkg::*;
//  import ${NAME}_scoreboard_pkg::*;
  import ${NAME}_env_config_pkg::*;
  import handshake_agent_pkg::*;
  import handshake_config_pkg::*;
  import handshake_type_pkg::*;
  import handshake_sequencer_pkg::*;
  import ${NAME}_type_pkg::*;
  import packet_pkg::*;
  import generic_producer_pkg::*;
  import generic_consumer_pkg::*;
  import uvm_misc::*;
  import objfilter_pkg::*;
  //-----------| ENVIRONMENT |---------------
  class ${NAME}_environment extends uvm_env;
    `uvm_component_utils(${NAME}_environment)

    string LOG_NAME = "${NAME} Environment";

    ${NAME}_env_config               env_cfg;

    //**********************
    //Interface Agents
    //**********************
	Fetch_Interface:
environment_pkg_agent_config:

    //**********************
    //C++ Ref model interfaces
    //**********************
	Fetch_Interface:
environment_pkg_ref_model:

    //**********************
    //Sub module environments
    //**********************

    //**********
    //Scoreboard
    //**********
	Fetch_Interface:
environment_pkg_scoreboard_OnlyOutput:

    function new(string name, uvm_component parent);
      super.new(name, parent);
      `uvm_info(get_type_name(),"SV env::env",UVM_LOW);
    endfunction // new

    function void set_knobs();
      //SETUP INTERFACES
    Fetch_Interface:
environment_pkg_interface_setup:
    endfunction // set_knobs
  
    function void connect_interfaces();
      env_cfg.lookup_interfaces();
      //CONNECT VIF to CFG.VIF
      Fetch_Interface:
environment_pkg_connect_interfaces:
    endfunction // connect_interfaces

    function void build_phase(uvm_phase phase);
      super.build_phase(phase);
      // ************************************
      // Interface Environment Instantiations
      // ************************************
      Fetch_Interface:
environment_pkg_interface_environment:

      // *************
      // Configuration cd 
      // *************
      // get configuration object for env
      if(!uvm_config_db#(${NAME}_env_config)::get(this, "", $sformatf("%s_cfg",get_name()), env_cfg)) begin
        `uvm_fatal(LOG_NAME, "\t ----| NO CONFIG FOUND |---- \n");         
      end else begin
        `uvm_info(LOG_NAME,  env_cfg.convert2string(), UVM_DEBUG);
      end

      // ************************************
      // Sub Module Environment Instantiations
      // ************************************

      // **************
      // Scoreboard
      // **************
      //FIXME!
      // Create and configure scoreboard
      if(/*this.env_cfg.has_scoreboard == 1*/1) begin
      Fetch_Interface:
environment_pkg_scoreboard1_OnlyOutput:
        // Specify standard queue type
        this.set_type_override_by_type(pk_syoscb::cl_syoscb_queue::get_type(),
                                       pk_syoscb::cl_syoscb_queue_std::get_type(),
                                       "*");

        // Specify in order compare
        this.set_type_override_by_type(pk_syoscb::cl_syoscb_compare_base::get_type(),
        pk_syoscb::cl_syoscb_compare_io::get_type(),
        "*");
      Fetch_Interface:
environment_pkg_scoreboard2_OnlyOutput:

      Fetch_Interface:
environment_pkg_scoreboard3_OnlyOutput:

      Fetch_Interface:
environment_pkg_scoreboard4_OnlyOutput:

      Fetch_Interface:
environment_pkg_scoreboard5_OnlyOutput:

      end
      //-------------------------------------

      `uvm_info(get_type_name(),"SV env::build",UVM_LOW);


      // **************
      // ML Ref model interface
      // **************
      Fetch_Interface:
environment_pkg_ref_interface:
      //-------------------------------------
      // Set Knobs
      connect_interfaces();
      set_knobs();
    endfunction // phase

    // register the ML ports and sockets
    function void phase_ended(uvm_phase phase);
       if (phase.get_name() == "build") begin
        `uvm_info(get_type_name(),"SV svtop::build phase ended",UVM_LOW);
        Fetch_Interface:
environment_pkg_ports_sockets:
       end
    endfunction // phase_ended

    function void connect_phase(uvm_phase phase);
      Fetch_Interface:
environment_pkg_scoreboard_string_OnlyOutput:
      Fetch_Interface:
environment_pkg_scoreboard_string_OnlyInput:

      super.connect_phase(phase);
      //Connect monitor analysis ports for all the input monitors. These are connected direcly to the reference models
//      tst_agent.data_analysis_port.connect(scoreboard.ref_model.input_port_fifo.analysis_export);

      //The output interface monitor is connected to the scoreboard.
      Fetch_Interface:
environment_pkg_scoreboard_analysis_export_OnlyInput:

      if(1/*this.env_cfg.has_scoreboard == 1*/) begin
        pk_syoscb::cl_syoscb_subscriber subscriber;

        Fetch_Interface:
environment_pkg_scoreboard_analysis_export_OnlyOutput:


//        subscriber = this.scb2.get_subscriber("DUT", "X_DECODED");
//        this.x_decoded_agent.req_dap.connect(this.x_decoded_dut_filter.in_port);
//        this.x_decoded_dut_filter.out_port.connect(subscriber.analysis_export);
//
//        subscriber = this.scb2.get_subscriber("REF", "X_DECODED");
//        this.gcons2.ap.connect(this.x_decoded_ref_filter.in_port);
//        this.x_decoded_ref_filter.out_port.connect(subscriber.analysis_export);

      end

      `uvm_info(get_type_name(),"SV ::connect ",UVM_LOW);

      //CONNECT ALL ML PORTS TO REF MODEL
      Fetch_Interface:
environment_pkg_connect_ports_OnlyInput:

      Fetch_Interface:
environment_pkg_connect_ports_OnlyOutput:

    endfunction // phase

  endclass 
  
endpackage
