package quantize_scoreboard_pkg;
  import uvm_pkg::*;
  import quantize_interface_sequencer_pkg::*; //Providing the sequence item for the interface.
  import quantize_ref_model_pkg::*; //Package containing the reference model.

  class quantize_scoreboard extends uvm_scoreboard;
    `uvm_component_utils(quantize_scoreboard)

    string LOG_NAME = "quantize SCOREBOARD*";

    uvm_tlm_analysis_fifo #(quantize_interface_seq_item)  agent_fifo;
    uvm_tlm_analysis_fifo #(quantize_interface_seq_item)  ref_model_fifo;

    quantize_interface_seq_item agent_item;
    quantize_interface_seq_item ref_item;
    int transaction_count=0;

    quantize_ref_model_t              ref_model;

    protected function new(string name, uvm_component parent);
      super.new(name, parent);
    endfunction 
  
    function void build_phase(uvm_phase phase);
      super.build_phase(phase);

      //Create Ref Model
      ref_model = quantize_ref_model_t::type_id::create(.name("ref_model"), .parent(this));

      //Agent Connect
      agent_fifo = new("quantize_if_agent_fifo",this);
      agent_item = quantize_interface_seq_item::type_id::create("agent_item");
      //Reference Model Connect
      ref_model_fifo = new("ref_ap",this);
      ref_item = quantize_interface_seq_item::type_id::create("ref_item");

    endfunction

    function void connect_phase(uvm_phase phase);
      super.connect_phase(phase);

      //The output of the ref model is connected to the scoreboard and is used to compare the results with the output interface
      ref_model.results_ap.connect(ref_model_fifo.analysis_export);

    endfunction // phase

    task run_phase(uvm_phase phase);
      string s;

      fork
        begin
          sign_of_life();
        end
        begin
          check();
        end
      join_any;
    endtask // run_phase

    task check();
      forever begin
        `uvm_info("Scoreboard Check Task", "Waiting for item in agent_fifo", UVM_DEBUG);
        agent_fifo.get(agent_item);
        `uvm_info("Scoreboard Check Task", "Waiting for item in ref_ap", UVM_DEBUG);
        ref_model_fifo.get(ref_item);
        transaction_count++;
        if (!agent_item.compare(ref_item)) begin
          //Report
          s ={"\n_______________________________ ________ __________  _______________________________________ _____________________\n",
                "\\_   _____/\\______   \\______   \\\\_____  \\\\______   \\ \\______   \\_   _____/\\______   \\_____  \\\\______   \\__    ___/\n",
                " |    __)_  |       _/|       _/ /   |   \\|       _/  |       _/|    __)_  |     ___//   |   \\|       _/ |    |   \n",
                " |        \\ |    |   \\|    |   \\/    |    \\    |   \\  |    |   \\|        \\ |    |   /    |    \\    |   \\ |    |   \n",
                "/_______  / |____|_  /|____|_  /\\_______  /____|_  /  |____|_  /_______  / |____|   \\_______  /____|_  / |____|   \n",
                "        \\/         \\/        \\/         \\/       \\/          \\/        \\/                   \\/       \\/           \n"};
          `uvm_info(LOG_NAME,s,UVM_LOW);
          `uvm_info( LOG_NAME,$sformatf("Transaction Match: ERROR \n\t\t\tAgent Interface: %s\n\t\t\tReference Model: %s",agent_item.convert2string(),ref_item.convert2string()),UVM_LOW);
          `uvm_fatal(LOG_NAME, "ERROR");

        end else begin
          `uvm_info(LOG_NAME,$sformatf("Transaction Match: SUCCESS %s",agent_item.convert2string()),UVM_FULL);
        end
      end
    endtask // run

    task sign_of_life();
      forever begin
        #20us;
        `uvm_info(LOG_NAME,$sformatf("Transactions Checked: %0d",transaction_count),UVM_HIGH);
      end
    endtask


    function void report_phase( uvm_phase phase );
      string s;
      $sformat(s, "\tThe Testcase has completed Successfully!\n");
      $sformat(s, "%s\tReport:\n",s);
      $sformat(s, "%s\tNumber of transactions checked by scoreboard:\t%12d\n",s,transaction_count);
      `uvm_info(LOG_NAME,s,UVM_LOW);
    endfunction

  endclass

endpackage
