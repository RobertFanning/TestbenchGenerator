module quantize_bif
  import intea_test_p::*;
  import intea_wic_test_p::*;
  import quantization_test_p::*;
  import quantize_type_pkg::*;
  import quantize_test_pkg::*;
  import uvm_pkg::*;
  import uvm_ml::*;
#(
  parameter IS_TOP = 1
)
  (
  //PORT MAP OF MODULE: 
  //nsertionPoint_Portmap
    input  logic  clk_i,
    input  logic  reseta_ni,

    input    logic [0:-17]  x_in_i,
    input    logic  x_in_rdy_i,
    input    intea_info_t  x_in_info_i,
    input    logic  x_in_ack_o,

    input    logic [0:-17]  x_predict_i,
    input    logic  x_predict_rdy_i,
    input    intea_info_t  x_predict_info_i,
    input    logic  x_predict_ack_o,

    input    logic [-1:-14]  scale_factor_i,
    input    logic  scale_factor_rdy_i,
    input    intea_info_t  scale_factor_info_i,
    input    logic  scale_factor_ack_o,

    input    logic [0:1] [4:0]  x_encoded_o,
    input    logic [1:0]  x_encoded_rdy_o,
    input    logic [0:1] [0:0]  x_encoded_info_o,
    input    logic [1:0]  x_encoded_ack_i,

    input    mult_cmd_t  mult_cmd_o,
    input    mult_rsp_t  mult_rsp_i

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
  packed_intea_info_t x_in_info_p;
  packed_intea_info_t x_predict_info_p;
  packed_intea_info_t scale_factor_info_p;
  packed_mult_cmd_t mult_cmd_p;
  packed_mult_rsp_t mult_rsp_p;
  packed_quantize_info_arr_t x_encoded_info_p;
  intea_info_t x_in_info;
  intea_info_t x_predict_info;
  intea_info_t scale_factor_info;
  mult_cmd_t mult_cmd;
  mult_rsp_t mult_rsp;
  quantize_data_arr_t x_encoded;
  quantize_info_arr_t x_encoded_info;
  quantize_rdy_ack_arr_t x_encoded_rdy;
  quantize_rdy_ack_arr_t x_encoded_ack;
  intea_info_t mult_req_info;
  intea_info_t mult_rsp_info;
  logic [$bits(mult_cmd_t)-1:0] mult_cmd_slv;
  logic [$bits(mult_rsp_t)-1:0] mult_rsp_slv;
  logic [$bits(intea_info_t)-1:0] mult_cmd_info_slv;


  //DECLARE VIRTUAL INTERFACES
  //FIXME!
  //feth_bif_instantiation:
  handshake_if#(.REQ_MSB(0), .REQ_LSB(-17), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(intea_info_t)-1),.REQ_META_LSB(0)) x_in_vif (.clk(local_clk),.rst_n(local_rst_n)); 
  handshake_if#(.REQ_MSB(0), .REQ_LSB(-17), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(intea_info_t)-1),.REQ_META_LSB(0)) x_predict_vif (.clk(local_clk),.rst_n(local_rst_n)); 
  handshake_if#(.REQ_MSB(-1), .REQ_LSB(-14), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(intea_info_t)-1),.REQ_META_LSB(0)) scale_factor_vif (.clk(local_clk),.rst_n(local_rst_n)); 
  handshake_if#(.REQ_MSB(0), .REQ_LSB(1), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(intea_info_t)-1),.REQ_META_LSB(0)) x_encoded_vif[2] (.clk(local_clk),.rst_n(local_rst_n)); 
  handshake_if#(.REQ_MSB($bits(quantize_type_pkg::table_idx_t)+$bits(quantize_type_pkg::scale_factor_t)-1), .REQ_LSB(0), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(intea_info_t)-1),.REQ_META_LSB(0), .RSP_MSB($left(mult_result_t)), .RSP_LSB($right(mult_result_t)), .RSPCONTAINSMETA(1), .RSP_META_MSB($bits(intea_info_t)-1), .RSP_META_LSB(0)) mult_vif (.clk(local_clk),.rst_n(local_rst_n));

  generate if (IS_TOP == 1) begin : SIGNAL_HOOKUP
    //FIXME!
    initial begin
      //sertionPoint_Metadata
      x_in_vif.metadata = '0;
      x_predict_vif.metadata = '0;
      scale_factor_vif.metadata = '0;
      mult_vif.rsp_meta = '0;
    end
    
    //sertionPoint_StreamUnpacked
    assign x_in_info_p = x_in_vif.metadata;
    assign {<<{x_in_info}} = x_in_info_p;

    assign x_predict_info_p = x_predict_vif.metadata;
    assign {<<{x_predict_info}} = x_predict_info_p;

    assign scale_factor_info_p = scale_factor_vif.metadata;
    assign {<<{scale_factor_info}} = scale_factor_info_p;

    for (genvar i = 0; i < 2; i++) begin
    assign x_encoded_vif[i].data = x_encoded[i];
    assign x_encoded_vif[i].req = x_encoded_rdy[i];
    assign {>>{x_encoded_info_p[i]}} = x_encoded_info[i];
    assign x_encoded_vif[i].metadata = x_encoded_info_p[i];
    assign x_encoded_vif[i].ack = x_encoded_ack[i];
    end

    assign {>>{mult_vif.metadata}} = mult_req_info;
    assign {<<{mult_rsp_info}} = mult_vif.rsp_meta;
    assign mult_rsp.info = mult_rsp_info;
    assign mult_rsp.ack = mult_vif.ack;
    assign {<<{mult_rsp.res}} = mult_vif.rsp_data;
    assign {>>{mult_cmd_slv}} = mult_cmd;
    assign {mult_vif.req,mult_vif.data,mult_cmd_info_slv} = mult_cmd_slv;
    assign mult_req_info = mult_cmd.info;
  end else begin
    //sertionPoint_NotTop
    assign x_in_vif.data = x_in_i;
    assign x_in_vif.req = x_in_rdy_i;
    assign {>>{x_in_info_p}} = x_in_info_i;
    assign x_in_vif.metadata = x_in_info_p;
    assign x_in_vif.ack = x_in_ack_o;

    assign x_predict_vif.data = x_predict_i;
    assign x_predict_vif.req = x_predict_rdy_i;
    assign {>>{x_predict_info_p}} = x_predict_info_i;
    assign x_predict_vif.metadata = x_predict_info_p;
    assign x_predict_vif.ack = x_predict_ack_o;

    assign scale_factor_vif.data = scale_factor_i;
    assign scale_factor_vif.req = scale_factor_rdy_i;
    assign {>>{scale_factor_info_p}} = scale_factor_info_i;
    assign scale_factor_vif.metadata = scale_factor_info_p;
    assign scale_factor_vif.ack = scale_factor_ack_o;

    for (genvar i = 0; i < 2; i++) begin
    assign x_encoded_vif[i].data = x_encoded_o[i];
    assign x_encoded_vif[i].req = x_encoded_rdy_o[i];
    assign {>>{x_encoded_info_p[i]}} = x_encoded_info_o[i];
    assign x_encoded_vif[i].metadata = x_encoded_info_p[i];
    assign x_encoded_vif[i].ack = x_encoded_ack_i[i];
    end

    // multiplier command/response
    assign {>>{mult_cmd_slv}} = mult_cmd_o;  // out mult_cmd_t;
    assign {mult_vif.req,mult_vif.data,mult_cmd_info_slv} = mult_cmd_slv;
    assign {>>{mult_vif.metadata}} = mult_cmd_o.info;
    assign {>>{mult_rsp_slv}} = mult_rsp_i;
    assign {mult_vif.ack,mult_vif.rsp_data,mult_vif.rsp_meta} = mult_rsp_slv;
  end endgenerate


  initial begin
    //FIXME!
    //fetch_if_configDB_interface:
  uvm_config_db#(x_in_handshake_vif_t)::set(null, "*", "quantize_x_in_vif", x_in_vif);
  uvm_config_db#(x_predict_handshake_vif_t)::set(null, "*", "quantize_x_predict_vif", x_predict_vif);
  uvm_config_db#(scale_factor_handshake_vif_t)::set(null, "*", "quantize_scale_factor_vif", scale_factor_vif);
  uvm_config_db#(mult_handshake_vif_t)::set(null, "*", "quantize_mult_vif", mult_vif);
    end
  generate for (genvar j=0; j<2; j++) begin
    initial begin
      uvm_config_db#(x_encoded_handshake_vif_t)::set(null, "*", $sformatf("quantize_x_encoded_vif_%0d",j), x_encoded_vif[j]);
    end
  end endgenerate




  generate if(IS_TOP==1) begin : DUT_HOOKUP
    //DECLARE DUT INSTANCE
    //FIXME!
    quantize quantize_i (
      //ssertionPoint_DUTConnection
      .clk_i  (local_clk),
      .reseta_ni  (local_rst_n),

      .x_in_i  ( x_in_vif.data ),
      .x_in_rdy_i  ( x_in_vif.req ),
      .x_in_info_i  ( x_in_info ),
      .x_in_ack_o  ( x_in_vif.ack ),

      .x_predict_i  ( x_predict_vif.data ),
      .x_predict_rdy_i  ( x_predict_vif.req ),
      .x_predict_info_i  ( x_predict_info ),
      .x_predict_ack_o  ( x_predict_vif.ack ),

      .scale_factor_i  ( scale_factor_vif.data ),
      .scale_factor_rdy_i  ( scale_factor_vif.req ),
      .scale_factor_info_i  ( scale_factor_info ),
      .scale_factor_ack_o  ( scale_factor_vif.ack ),

      .x_encoded_o  ( x_encoded ),
      .x_encoded_rdy_o  ( x_encoded_rdy ),
      .x_encoded_info_o  ( x_encoded_info ),
      .x_encoded_ack_i  ( x_encoded_ack ),

      .mult_cmd_o  ( mult_cmd ),
      .mult_rsp_i  ( mult_rsp )

    );

    initial begin
       //FIXME!
       string tops[2];

       tops[0]    = "SC:quantize";
       tops[1]    = "SV:random_test";
       
       uvm_ml_run_test(tops, "");
    end
  end endgenerate
endmodule
