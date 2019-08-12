module ${NAME}_bif
  import intea_test_p::*;
  import intea_wic_test_p::*;
  IncludePackages:
  import ${NAME}_type_pkg::*;
  import ${NAME}_test_pkg::*;
  import uvm_pkg::*;
  import uvm_ml::*;
#(
  parameter IS_TOP = 1
)
  (
  //PORT MAP OF MODULE: 
  //nsertionPoint_Portmap
  InsertionPoint_Portmap
  );
  timeunit 1ps;
  timeprecision 1ps;


  logic local_clk=0;
  logic local_rst_n=0;

 
  generate if(IS_TOP==1) begin
    //`USE_WIDEX_REPORT_SERVER
  end endgenerate


  generate if (IS_TOP == 1) begin : CLOCK_HOOKUP
    initial begin
      local_rst_n = 0;
      #20us;
      local_rst_n = 1;
    end
    
    initial begin
      local_clk = 0;
      forever begin
        local_clk = ~local_clk;
        #1us;
      end
    end
  end else begin
    assign local_clk = clk_i;
    assign local_rst_n = reseta_ni;
  end endgenerate

  //Unpacked variables need to be declared here.
  //nsertionPoint_UnpackedDeclared
  InsertionPoint_UnpackedDeclared:


  //DECLARE VIRTUAL INTERFACES
  //FIXME!
  //feth_bif_instantiation:
  Fetch_Interface:
bif_instantiation:

  generate if (IS_TOP == 1) begin : SIGNAL_HOOKUP
    //FIXME!
    initial begin
      //sertionPoint_Metadata
      InsertionPoint_Metadata
    end
    
    //sertionPoint_StreamUnpacked
    InsertionPoint_StreamUnpacked
  end else begin
    //sertionPoint_NotTop
    InsertionPoint_NotTop
  end endgenerate


  initial begin
    //FIXME!
    //fetch_if_configDB_interface:
bif_configDB_interface:




  generate if(IS_TOP==1) begin : DUT_HOOKUP
    //DECLARE DUT INSTANCE
    //FIXME!
    ${NAME} ${NAME}_i (
      //ssertionPoint_DUTConnection
      InsertionPoint_DUTConnection
    );

    initial begin
       //FIXME!
       string tops[2];

       tops[0]    = "SC:${NAME}";
       tops[1]    = "SV:random_test";
       
       uvm_ml_run_test(tops, "");
    end
  end endgenerate
endmodule
