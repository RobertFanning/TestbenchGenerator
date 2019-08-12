//                          Independent Insertion Points
//
// Below are all the independent insertion points for the Testbench Generator.
// These insertions are consistent for all interface protocols.
// They are defined below in order of the packages in which they occur. 


//                                    bif.sv
// Below are the insertion points for the BIF package.
//-----------------------------------------------------------------------------
bif_configDB_interface:
    uvm_config_db#(${if_name}_handshake_vif_t)::set(null, "*", "${NAME}_${if_name}_vif", ${if_name}_vif);

bif_configDB_interface_array:
  generate for (genvar j=0; j<${arraySize};; j++) begin
    initial begin
      uvm_config_db#(${if_name}_handshake_vif_t)::set(null, "*", $sformatf("${NAME}_${if_name}_vif_%0d",j), ${if_name}_vif[j]);
    end
  end endgenerate
  
//-----------------------------------------------------------------------------



//                                environment.sv
// Below are the insertion points for the Environment package.
//-----------------------------------------------------------------------------
environment_pkg_agent_config:
    handshake_agent  #(${if_name}_handshake_vif_t)    ${if_name}_agent${array_sizeBrackets};
    handshake_config #(${if_name}_handshake_vif_t)    ${if_name}_cfg${array_sizeBrackets};

environment_pkg_ref_model:
    generic_${ProducerConsumer} ${if_name}_${ProdCons}${array_sizeBrackets};

environment_pkg_interface_setup:
      ${forloop_begin}
      ${if_name}_cfg${array_filledBrackets}.is_active      = env_cfg.is_active;
      ${if_name}_cfg${array_filledBrackets}.mode           = ${MasterSlave};
      uvm_config_db#(handshake_config #(${if_name}_handshake_vif_t))::set(this, "*",${sformatOpen}"${if_name}_agent${%0d}_cfg"${sformatClose}, ${if_name}_cfg${array_filledBrackets});
      ${forloop_end}

environment_pkg_connect_interfaces:
      ${forloop_begin}
      ${if_name}_cfg${array_filledBrackets}.vif = env_cfg.${if_name}_vif${array_filledBrackets};
      ${forloop_end}

environment_pkg_interface_environment:
      ${forloop_begin}
      ${if_name}_agent${array_filledBrackets} = handshake_agent#(${if_name}_handshake_vif_t)::type_id::create(.name(${sformatOpen}"${if_name}_agent${%0d}"${sformatClose}), .parent(this));
      ${if_name}_cfg${array_filledBrackets}   = handshake_config#(${if_name}_handshake_vif_t)::type_id::create( ${sformatOpen}"${if_name}_agent${%0d}_cfg"${sformatClose} );
      ${forloop_end}

environment_pkg_scoreboard_OnlyOutput:
    pk_syoscb::cl_syoscb scb${IterationCounter};
    obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item)) ${if_name}_ref_prefilter ${array_sizeBrackets};
    tlm_filter#(base_data_pkg::base_data_item) ${if_name}_ref_filter${array_sizeBrackets};
    obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item)) ${if_name}_dut_prefilter ${array_sizeBrackets};
    tlm_filter#(base_data_pkg::base_data_item) ${if_name}_dut_filter${array_sizeBrackets};

environment_pkg_scoreboard1_OnlyOutput:
        pk_syoscb::cl_syoscb_cfg scb_cfg${IterationCounter};

environment_pkg_scoreboard2_OnlyOutput:
        scb_cfg${IterationCounter} = pk_syoscb::cl_syoscb_cfg::type_id::create("scb_cfg${IterationCounter}", this);

environment_pkg_scoreboard3_OnlyOutput:
        scb_cfg${IterationCounter}.set_queues({"DUT", "REF"});
        void'(scb_cfg${IterationCounter}.set_primary_queue("DUT"));
        ${forloop_begin}
        void'(scb_cfg${IterationCounter}.set_producer(${sformatOpen}"${if_name}${%0d}"${sformatClose}, {"DUT", "REF"}));
        ${forloop_end}

environment_pkg_scoreboard4_OnlyOutput:
        this.scb${IterationCounter} = pk_syoscb::cl_syoscb::type_id::create("scb${IterationCounter}", this);
        uvm_config_db#(pk_syoscb::cl_syoscb_cfg)::set(this, "scb${IterationCounter}", "cfg", scb_cfg${IterationCounter});

environment_pkg_scoreboard5_OnlyOutput:
        ${forloop_begin}
        ${if_name}_ref_filter${array_filledBrackets} = tlm_filter#(base_data_pkg::base_data_item)::type_id::create(${sformatOpen}"${if_name}${%0d}_ref_filter"${sformatClose}, this);
        ${if_name}_ref_prefilter${array_filledBrackets} = obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item))::type_id::create(${sformatOpen}"${if_name}${%0d}_ref_prefilter"${sformatClose}, this);
        ${if_name}_ref_prefilter${array_filledBrackets}.tcnt = 1;
        ${if_name}_dut_filter${array_filledBrackets} = tlm_filter#(base_data_pkg::base_data_item)::type_id::create(${sformatOpen}"${if_name}${%0d}_dut_filter"${sformatClose}, this);
        ${if_name}_dut_prefilter${array_filledBrackets} = obj_filter_base#(.IN(handshake_sequencer_pkg::handshake_seq_item),.OUT(base_data_pkg::base_data_item))::type_id::create(${sformatOpen}"${if_name}${%0d}_dut_prefilter"${sformatClose}, this);
        ${forloop_end}

environment_pkg_scoreboard_string_OnlyOutput:
      string sc_producer_${if_name} = "${NAME}.wrapper.${if_name}${%0d}.";

environment_pkg_scoreboard_string_OnlyInput:
      string sc_consumer_${if_name} = "${NAME}.wrapper.${if_name}${%0d}.";

environment_pkg_scoreboard_analysis_export_OnlyInput:
      ${forloop_begin}
      ${if_name}_agent${array_filledBrackets}.req_dap.connect(${if_name}_prod${array_filledBrackets}.analysis_export);
      ${forloop_begin}

environment_pkg_scoreboard_analysis_export_OnlyOutput:
        ${forloop_begin}
        subscriber = this.scb${IterationCounter}.get_subscriber("DUT", ${sformatOpen}"${if_name}${%0d}"${sformatClose});
        this.${if_name}_agent${array_filledBrackets}.req_dap.connect(this.${if_name}_dut_prefilter${array_filledBrackets}.in_port);
        this.${if_name}_dut_prefilter${array_filledBrackets}.out_port.connect(this.${if_name}_dut_filter${array_filledBrackets}.in_port);
        this.${if_name}_dut_filter${array_filledBrackets}.out_port.connect(subscriber.analysis_export);
        subscriber = this.scb${IterationCounter}.get_subscriber("REF", ${sformatOpen}"${if_name}${%0d}"${sformatClose});
        this.${if_name}_cons${array_filledBrackets}.ap.connect(this.${if_name}_ref_prefilter${array_filledBrackets}.in_port);
        this.${if_name}_ref_prefilter${array_filledBrackets}.out_port.connect(this.${if_name}_ref_filter${array_filledBrackets}.in_port);
        this.${if_name}_ref_filter${array_filledBrackets}.out_port.connect(subscriber.analysis_export);
        ${forloop_end}

environment_pkg_ref_interface:
      ${forloop_begin}
      ${if_name}_${ProdCons}${array_filledBrackets} = generic_${ProducerConsumer}::type_id::create(${sformatOpen}"${if_name}_${ProdCons}${%0d}"${sformatClose}, this);
      ${forloop_end}

environment_pkg_ports_sockets:
          ${forloop_begin}
          uvm_ml::ml_tlm2#()::register(${if_name}_${ProdCons}${array_filledBrackets}.nb_${InitiatorTarget}_socket);
          ${forloop_end}

environment_pkg_connect_ports_OnlyInput:
      ${forloop_begin}
      if(!uvm_ml::connect(${if_name}_prod${array_filledBrackets}.nb_initiator_socket.get_full_name(), ${sformatOpen}{sc_consumer_${if_name}${array_filledBrackets}, "nb_tsocket"}${sformatClose})) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;
      ${forloop_end}

environment_pkg_connect_ports_OnlyOutput:
      ${forloop_begin}
      if(!uvm_ml::connect(${sformatOpen}{sc_producer_${if_name}${array_filledBrackets}, "nb_isocket"}${sformatClose}, ${if_name}_cons${array_filledBrackets}.nb_target_socket.get_full_name())) begin
	 `uvm_fatal("MLCONN", "uvm_ml connect failed");
      end;
      ${forloop_end}


//-----------------------------------------------------------------------------



//                             environment_config.sv
// Below are the insertion points for the Environment configuration object package.
//-----------------------------------------------------------------------------
env_config_vif:
    ${if_name}_handshake_vif_t ${if_name}_vif${array_sizeBrackets};

env_config_lookup:
      string ${if_name}_vif_name="${NAME}_${if_name}_vif",
  
env_config_configdb:
  if(!uvm_config_db#(${if_name}_handshake_vif_t)::get(null, "", ${if_name}_vif_name, ${if_name}_vif)) begin
    `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", ${if_name}_vif_name));
  end

env_config_configdb_array:
  ${forloop_begin}
    if(!uvm_config_db#(${if_name}_handshake_vif_t)::get(null, "", $sformatf("%s_%0d",${if_name}_vif_name,i), ${if_name}_vif[i])) begin
      `uvm_fatal(LOG_NAME, $sformatf("Interface lookup: %s not found!", {${if_name}_vif_name,$sformatf("_%0d",i)}));
    end
  ${forloop_end}

//-----------------------------------------------------------------------------



//                                base_test.sv
// Below are the insertion points for the Base Test package.
//-----------------------------------------------------------------------------
base_test_pkg_sequence_OnlyInput:
      init_${if_name}(tsequence);

base_test_pkg_start_seq_OnlyInput:
      tsequence.start(environment.${if_name}_cfg.sequencer);

base_test_pkg_func_OnlyInput:
    function void init_${if_name}(handshake_base_sequence tseq);
      tseq.transactions = test_config.number_of_transactions;
      tseq.gap_min = test_config.min_delay_between_transactions;
      tseq.gap_max = test_config.max_delay_between_transactions;
      env_cfg.${if_name}_vif.init_seq(tseq);
    endfunction

//-----------------------------------------------------------------------------