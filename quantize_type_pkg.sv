package quantize_type_pkg;
  import quantization_test_p::*;
//DEFINE UNPACKED TYPES AS SYSTEMVERILOG FORMAT
typedef  enum bit[31:0] {ENCODE, DECODE} wic_mode_t_def;
typedef  logic [5:0]  buf_idx_t_def;
typedef  logic [2:0]  scaling_t_def;
typedef  logic [31:0]  samplecount_t_def;
typedef struct packed {
  wic_mode_t_def mode;
  logic   enable;
  buf_idx_t_def buf_idx;
  scaling_t_def scaling;
  logic   fsync;
  logic   rxlink_status_ok;
  logic   loopback;
  samplecount_t_def samplecount;
} packed_intea_info_t;
typedef  logic [3:0]  table_idx_t_def;
typedef  logic [-1:-14]  scale_factor_t_def;
typedef struct packed {
  logic   req;
  table_idx_t_def idx;
  scale_factor_t_def scf;
  packed_intea_info_t info;
} packed_mult_cmd_t;
typedef  logic [1:-20]  mult_result_t_def;
typedef struct packed {
  logic   ack;
  mult_result_t_def res;
  packed_intea_info_t info;
} packed_mult_rsp_t;
  typedef  packed_intea_info_t [0:1] packed_quantize_info_arr_t;

//DEFINE VIRTUAL INTERFACE TYPES HERE
  typedef virtual handshake_if#(.REQ_MSB(0), .REQ_LSB(-17), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(packed_intea_info_t)-1),.REQ_META_LSB(0)) x_in_handshake_vif_t;
  typedef virtual handshake_if#(.REQ_MSB(0), .REQ_LSB(-17), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(packed_intea_info_t)-1),.REQ_META_LSB(0)) x_predict_handshake_vif_t;
  typedef virtual handshake_if#(.REQ_MSB(-1), .REQ_LSB(-14), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(packed_intea_info_t)-1),.REQ_META_LSB(0)) scale_factor_handshake_vif_t;
  typedef virtual handshake_if#(.REQ_MSB(0), .REQ_LSB(1), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(packed_intea_info_t)-1),.REQ_META_LSB(0)) x_encoded_handshake_vif_t;
  typedef virtual handshake_if#(.REQ_MSB($bits(table_idx_t)+$bits(scale_factor_t)-1), .REQ_LSB(0), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(packed_intea_info_t)-1),.REQ_META_LSB(0), .RSP_MSB($left(mult_result_t)), .RSP_LSB($right(mult_result_t)), .RSPCONTAINSMETA(1), .RSP_META_MSB($bits(packed_intea_info_t)-1), .RSP_META_LSB(0)) mult_handshake_vif_t;


endpackage;
