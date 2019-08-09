package quantize_environment_pkg;
  import uvm_pkg::*;
//  import quantize_scoreboard_pkg::*;
  import quantize_env_config_pkg::*;
  import handshake_agent_pkg::*;
  import handshake_config_pkg::*;
  import handshake_type_pkg::*;
  import handshake_sequencer_pkg::*;
  import quantize_type_pkg::*;
  import packet_pkg::*;
  import generic_producer_pkg::*;
  import generic_consumer_pkg::*;
  import uvm_misc::*;
  import objfilter_pkg::*;
  //-----------| ENVIRONMENT |---------------
  class quantize_environment extends uvm_env;
    `uvm_component_utils(quantize_environment)

    string LOG_NAME = "quantize Environment";

    quantize_env_config               env_cfg;

    //**********************
    //Interface Agents
    //**********************
    handshake_agent  #(x_in_handshake_vif_t)    x_in_agent;
    handshake_config #(x_in_handshake_vif_t)    x_in_cfg;
    handshake_agent  #(x_predict_handshake_vif_t)    x_predict_agent;
    handshake_config #(x_predict_handshake_vif_t)    x_predict_cfg;
    handshake_agent  #(scale_factor_handshake_vif_t)    scale_factor_agent;
    handshake_config #(scale_factor_handshake_vif_t)    scale_factor_cfg;
    handshake_agent  #(x_encoded_handshake_vif_t)    x_encoded_agent[2];
    handshake_config #(x_encoded_handshake_vif_t)    x_encoded_cfg[2];
    handshake_agent  #(mult_handshake_vif_t)    mult_agent;
    handshake_config #(mult_handshake_vif_t)    mult_cfg;

    //**********************
    //C++ Ref model interfaces
    //**********************
    generic_producer x_in_prod;
    generic_producer x_predict_prod;
    generic_producer scale_factor_prod;
    generic_consumer x_encoded_cons[2];
    generic_consumer mult_cons;

    //**********************
    //Sub module environments
    //**********************

    //**********
    //Scoreboard
    //**********
    pk_syoscb::cl_syoscb scb3;
    obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item)) x_encoded_ref_prefilter [2];
    tlm_filter#(base_data_pkg::base_data_item) x_encoded_ref_filter[2];
    obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item)) x_encoded_dut_prefilter [2];
    tlm_filter#(base_data_pkg::base_data_item) x_encoded_dut_filter[2];
    pk_syoscb::cl_syoscb scb4;
    obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item)) mult_ref_prefilter ;
    tlm_filter#(base_data_pkg::base_data_item) mult_ref_filter;
    obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item)) mult_dut_prefilter ;
    tlm_filter#(base_data_pkg::base_data_item) mult_dut_filter;

    function new(string name, uvm_component parent);
      super.new(name, parent);
      `uvm_info(get_type_name(),"SV env::env",UVM_LOW);
    endfunction // new

    function void set_knobs();
      //SETUP INTERFACES
      x_in_cfg.is_active      = env_cfg.is_active;
      x_in_cfg.mode           = MASTER;
      uvm_config_db#(handshake_config #(x_in_handshake_vif_t))::set(this, "*","x_in_agent_cfg", x_in_cfg);
      x_predict_cfg.is_active      = env_cfg.is_active;
      x_predict_cfg.mode           = MASTER;
      uvm_config_db#(handshake_config #(x_predict_handshake_vif_t))::set(this, "*","x_predict_agent_cfg", x_predict_cfg);
      scale_factor_cfg.is_active      = env_cfg.is_active;
      scale_factor_cfg.mode           = MASTER;
      uvm_config_db#(handshake_config #(scale_factor_handshake_vif_t))::set(this, "*","scale_factor_agent_cfg", scale_factor_cfg);
      for (int i = 0; i < 2; i++) begin
      x_encoded_cfg[i].is_active      = env_cfg.is_active;
      x_encoded_cfg[i].mode           = SLAVE;
      uvm_config_db#(handshake_config #(x_encoded_handshake_vif_t))::set(this, "*",$sformatf("x_encoded_agent_%0d_cfg",i), x_encoded_cfg[i]);
      end
      mult_cfg.is_active      = env_cfg.is_active;
      mult_cfg.mode           = SLAVE;
      uvm_config_db#(handshake_config #(mult_handshake_vif_t))::set(this, "*","mult_agent_cfg", mult_cfg);
    endfunction // set_knobs
  
    function void connect_interfaces();
      env_cfg.lookup_interfaces();
      //CONNECT VIF to CFG.VIF
      x_in_cfg.vif = env_cfg.x_in_vif;
      x_predict_cfg.vif = env_cfg.x_predict_vif;
      scale_factor_cfg.vif = env_cfg.scale_factor_vif;
      for (int i = 0; i < 2; i++) begin
      x_encoded_cfg[i].vif = env_cfg.x_encoded_vif[i];
      end
      mult_cfg.vif = env_cfg.mult_vif;
    endfunction // connect_interfaces

    function void build_phase(uvm_phase phase);
      super.build_phase(phase);
      // ************************************
      // Interface Environment Instantiations
      // ************************************
      x_in_agent = handshake_agent#(x_in_handshake_vif_t)::type_id::create(.name("x_in_agent"), .parent(this));
      x_in_cfg   = handshake_config#(x_in_handshake_vif_t)::type_id::create( "x_in_agent_cfg" );
      x_predict_agent = handshake_agent#(x_predict_handshake_vif_t)::type_id::create(.name("x_predict_agent"), .parent(this));
      x_predict_cfg   = handshake_config#(x_predict_handshake_vif_t)::type_id::create( "x_predict_agent_cfg" );
      scale_factor_agent = handshake_agent#(scale_factor_handshake_vif_t)::type_id::create(.name("scale_factor_agent"), .parent(this));
      scale_factor_cfg   = handshake_config#(scale_factor_handshake_vif_t)::type_id::create( "scale_factor_agent_cfg" );
      for (int i = 0; i < 2; i++) begin
      x_encoded_agent[i] = handshake_agent#(x_encoded_handshake_vif_t)::type_id::create(.name($sformatf("x_encoded_agent_%0d",i)), .parent(this));
      x_encoded_cfg[i]   = handshake_config#(x_encoded_handshake_vif_t)::type_id::create( $sformatf("x_encoded_agent_%0d_cfg",i) );
      end
      mult_agent = handshake_agent#(mult_handshake_vif_t)::type_id::create(.name("mult_agent"), .parent(this));
      mult_cfg   = handshake_config#(mult_handshake_vif_t)::type_id::create( "mult_agent_cfg" );

      // *************
      // Configuration cd 
      // *************
      // get configuration object for env
      if(!uvm_config_db#(quantize_env_config)::get(this, "", $sformatf("%s_cfg",get_name()), env_cfg)) begin
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
        pk_syoscb::cl_syoscb_cfg scb_cfg3;
        pk_syoscb::cl_syoscb_cfg scb_cfg4;
        // Specify standard queue type
        this.set_type_override_by_type(pk_syoscb::cl_syoscb_queue::get_type(),
                                       pk_syoscb::cl_syoscb_queue_std::get_type(),
                                       "*");

        // Specify in order compare
        this.set_type_override_by_type(pk_syoscb::cl_syoscb_compare_base::get_type(),
        pk_syoscb::cl_syoscb_compare_io::get_type(),
        "*");
        scb_cfg3 = pk_syoscb::cl_syoscb_cfg::type_id::create("scb_cfg3", this);
        scb_cfg4 = pk_syoscb::cl_syoscb_cfg::type_id::create("scb_cfg4", this);

        scb_cfg3.set_queues({"DUT", "REF"});
        void'(scb_cfg3.set_primary_queue("DUT"));
        for (int i = 0; i < 2; i++) begin
        void'(scb_cfg3.set_producer($sformatf("x_encoded_%0d",i), {"DUT", "REF"}));
        end
        scb_cfg4.set_queues({"DUT", "REF"});
        void'(scb_cfg4.set_primary_queue("DUT"));
        void'(scb_cfg4.set_producer("mult", {"DUT", "REF"}));

        this.scb3 = pk_syoscb::cl_syoscb::type_id::create("scb3", this);
        uvm_config_db#(pk_syoscb::cl_syoscb_cfg)::set(this, "scb3", "cfg", scb_cfg3);
        this.scb4 = pk_syoscb::cl_syoscb::type_id::create("scb4", this);
        uvm_config_db#(pk_syoscb::cl_syoscb_cfg)::set(this, "scb4", "cfg", scb_cfg4);

        for (int i = 0; i < 2; i++) begin
        x_encoded_ref_filter[i] = tlm_filter#(base_data_pkg::base_data_item)::type_id::create($sformatf("x_encoded_%0d_ref_filter",i), this);
        x_encoded_ref_prefilter[i] = obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item))::type_id::create($sformatf("x_encoded_%0d_ref_prefilter",i), this);
        x_encoded_ref_prefilter[i].tcnt = 1;
        x_encoded_dut_filter[i] = tlm_filter#(base_data_pkg::base_data_item)::type_id::create($sformatf("x_encoded_%0d_dut_filter",i), this);
        x_encoded_dut_prefilter[i] = obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item))::type_id::create($sformatf("x_encoded_%0d_dut_prefilter",i), this);
        end
        mult_ref_filter = tlm_filter#(base_data_pkg::base_data_item)::type_id::create("mult_ref_filter", this);
        mult_ref_prefilter = obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item))::type_id::create("mult_ref_prefilter", this);
        mult_ref_prefilter.tcnt = 1;
        mult_dut_filter = tlm_filter#(base_data_pkg::base_data_item)::type_id::create("mult_dut_filter", this);
        mult_dut_prefilter = obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item))::type_id::create("mult_dut_prefilter", this);

      end
      //-------------------------------------

      `uvm_info(get_type_name(),"SV env::build",UVM_LOW);


      // **************
      // ML Ref model interface
      // **************
      x_in_prod = generic_producer::type_id::create("x_in_prod", this);
      x_predict_prod = generic_producer::type_id::create("x_predict_prod", this);
      scale_factor_prod = generic_producer::type_id::create("scale_factor_prod", this);
      for (int i = 0; i < 2; i++) begin
      x_encoded_cons[i] = generic_consumer::type_id::create($sformatf("x_encoded_cons_%0d",i), this);
      end
      mult_cons = generic_consumer::type_id::create("mult_cons", this);
      //-------------------------------------
      // Set Knobs
      connect_interfaces();
      set_knobs();
    endfunction // phase

    // register the ML ports and sockets
    function void phase_ended(uvm_phase phase);
       if (phase.get_name() == "build") begin
        `uvm_info(get_type_name(),"SV svtop::build phase ended",UVM_LOW);
          uvm_ml::ml_tlm2#()::register(x_in_prod.nb_initiator_socket);
          uvm_ml::ml_tlm2#()::register(x_predict_prod.nb_initiator_socket);
          uvm_ml::ml_tlm2#()::register(scale_factor_prod.nb_initiator_socket);
          for (int i = 0; i < 2; i++) begin
          uvm_ml::ml_tlm2#()::register(x_encoded_cons[i].nb_target_socket);
          end
          uvm_ml::ml_tlm2#()::register(mult_cons.nb_target_socket);
       end
    endfunction // phase_ended

    function void connect_phase(uvm_phase phase);
      string sc_producer_x_encoded = "quantize.wrapper.x_encoded_%0d.";
      string sc_producer_mult = "quantize.wrapper.mult.";
      string sc_consumer_x_in = "quantize.wrapper.x_in.";
      string sc_consumer_x_predict = "quantize.wrapper.x_predict.";
      string sc_consumer_scale_factor = "quantize.wrapper.scale_factor.";

      super.connect_phase(phase);
      //Connect monitor analysis ports for all the input monitors. These are connected direcly to the reference models
//      tst_agent.data_analysis_port.connect(scoreboard.ref_model.input_port_fifo.analysis_export);

      //The output interface monitor is connected to the scoreboard.
      x_in_agent.req_dap.connect(x_in_prod.analysis_export);
      x_predict_agent.req_dap.connect(x_predict_prod.analysis_export);
      scale_factor_agent.req_dap.connect(scale_factor_prod.analysis_export);

      if(1/*this.env_cfg.has_scoreboard == 1*/) begin
        pk_syoscb::cl_syoscb_subscriber subscriber;

        for (int i = 0; i < 2; i++) begin
        subscriber = this.scb3.get_subscriber("DUT", $sformatf("x_encoded_%0d",i));
        this.x_encoded_agent[i].req_dap.connect(this.x_encoded_dut_prefilter[i].in_port);
        this.x_encoded_dut_prefilter[i].out_port.connect(this.x_encoded_dut_filter[i].in_port);
        this.x_encoded_dut_filter[i].out_port.connect(subscriber.analysis_export);
        subscriber = this.scb3.get_subscriber("REF", $sformatf("x_encoded_%0d",i));
        this.x_encoded_cons[i].ap.connect(this.x_encoded_ref_prefilter[i].in_port);
        this.x_encoded_ref_prefilter[i].out_port.connect(this.x_encoded_ref_filter[i].in_port);
        this.x_encoded_ref_filter[i].out_port.connect(subscriber.analysis_export);
        end
        subscriber = this.scb4.get_subscriber("DUT", "mult");
        this.mult_agent.req_dap.connect(this.mult_dut_prefilter.in_port);
        this.mult_dut_prefilter.out_port.connect(this.mult_dut_filter.in_port);
        this.mult_dut_filter.out_port.connect(subscriber.analysis_export);
        subscriber = this.scb4.get_subscriber("REF", "mult");
        this.mult_cons.ap.connect(this.mult_ref_prefilter.in_port);
        this.mult_ref_prefilter.out_port.connect(this.mult_ref_filter.in_port);
        this.mult_ref_filter.out_port.connect(subscriber.analysis_export);


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
      if(!uvm_ml::connect(x_in_prod.nb_initiator_socket.get_full_name(), {sc_consumer_x_in, "nb_tsocket"})) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;
      if(!uvm_ml::connect(x_predict_prod.nb_initiator_socket.get_full_name(), {sc_consumer_x_predict, "nb_tsocket"})) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;
      if(!uvm_ml::connect(scale_factor_prod.nb_initiator_socket.get_full_name(), {sc_consumer_scale_factor, "nb_tsocket"})) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;

      for (int i = 0; i < 2; i++) begin
      if(!uvm_ml::connect($sformatf({sc_producer_x_encoded[i], "nb_isocket"},i), x_encoded_cons[i].nb_target_socket.get_full_name())) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;
      end
      if(!uvm_ml::connect({sc_producer_mult, "nb_isocket"}, mult_cons.nb_target_socket.get_full_name())) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;

    endfunction // phase

  endclass 
  
endpackage
