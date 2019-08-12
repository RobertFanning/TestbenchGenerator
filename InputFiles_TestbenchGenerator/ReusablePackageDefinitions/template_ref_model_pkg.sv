package ${NAME}_ref_model_pkg;
import uvm_pkg::*;

  class ${NAME}_ref_model_t extends uvm_component;//Could use subscriber if it only needs a single analysis port
    `uvm_component_utils(${NAME}_ref_model_t);

    //Analysis port from the monitors on the ${NAME} inputs.
    uvm_tlm_analysis_fifo #(test_sequence_item)   interface_a_fifo;
    uvm_tlm_analysis_fifo #(test_sequence_item)   interface_b_fifo;

    //This is the analysis ports which transmits the final results of the reference model.
    uvm_analysis_port #(${NAME}_interface_seq_item) results_ap;

    //Sequence items to/from each of the analysis ports
    test_sequence_item            interface_a_transaction;
    test_sequence_item            interface_b_transaction;

    //Collect report for each packet which goes through the reference model.
    string report_collection [$];
    //Printout Indentation.
    string spacing = "\t\t\t";

    protected function new (string name, uvm_component parent);
      super.new(name, parent);
    endfunction

    function void build_phase(uvm_phase phase);
      //Initialize the results analysis ports
      results_ap = new("results_ap",this);

      //Initialize the inputs
      interface_a_fifo = new("interface_a_fifo",this);
      interface_a_transaction = test_sequence_item::type_id::create("interface_a_transaction");

      interface_b_fifo = new("interface_b_fifo",this);
      interface_b_transaction = test_sequence_item::type_id::create("interface_b_transaction");

    endfunction

    task run_phase(uvm_phase phase);
      ${NAME}_interface_seq_item ref_item;
      ${NAME}_interface_seq_item ref_item_clone;

      forever begin
        `uvm_info("Ref Model Run Task","Waiting for item in interface_a_fifo", UVM_DEBUG);
        interface_a_fifo.get(interface_a_transaction);
        `uvm_info("Ref Model Run Task","Waiting for item in interface_b_fifo",UVM_DEBUG);
        interface_b_fifo.get(interface_b_transaction);

        `uvm_info("Ref Model Run Task","got items",UVM_DEBUG);

        //Start the ${NAME} ref model.
        ref_item = ref_model();

        //Put the result from the reference model into the output analysis port.
        $cast(ref_item_clone,ref_item.clone());
        results_ap.write(ref_item_clone);
      end
    endtask // run

    function void connect_phase(uvm_phase phase);
      super.connect_phase(phase);

    endfunction 

    //The ${NAME} Reference Model
    function ${NAME}_interface_seq_item ref_model ();
      //Output sequence item
      ${NAME}_interface_seq_item item_ref = ${NAME}_interface_seq_item::type_id::create(.name("item_ref"),.contxt(""));

      item_ref.match = interface_a_transaction.compare(interface_b_transaction);
      result_reporter(item_ref,interface_a_transaction,interface_b_transaction);

      return item_ref;
    endfunction

    function void result_reporter(${NAME}_interface_seq_item ref_item,test_sequence_item interface_a_transaction, test_sequence_item interface_b_transaction);
      string s;
      string newline = $sformatf("\n%s",spacing);

      s = $sformatf("%s%s",s,newline);
      s = $sformatf("%s-----------%s",s,newline);
      s = $sformatf("%sRef Model Result%s",s,newline);
      s = $sformatf("%s-----------%s",s,newline);


      s = $sformatf("%s%s",s,interface_a_transaction.convert2string());
      s = $sformatf("%s%s%s",s,newline,newline);
      s = $sformatf("%s%s",s,interface_b_transaction.convert2string());
      s = $sformatf("%s%s%s",s,newline,newline);
      s = $sformatf("%s ref_item.match =\t %d%s",s,ref_item.match);
      report_collection.push_back(s);
    endfunction

  endclass

endpackage
