//                               HANDSHAKE INTERFACE
//                          protocol-dependent insertions

//                                    bif.sv
// Below are the handshake protocol-dependent insertion points for the BIF package.
//-----------------------------------------------------------------------------

bif_instantiation:
  handshake_if#(.REQ_MSB($leftREQ), .REQ_LSB($rightREQ), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(intea_info_t)-1),.REQ_META_LSB(0)) ${if_name}_vif${array_sizeBrackets} (.clk(local_clk),.rst_n(local_rst_n)); 

//-----------------------------------------------------------------------------


//                              type_definition.sv
// Below are the handshake protocol-dependent insertion points for the type definition package.
//-----------------------------------------------------------------------------
type_pkg_virtual_interface:
  typedef virtual handshake_if#(.REQ_MSB($leftREQ), .REQ_LSB($rightREQ), .REQCONTAINSMETA(1),.REQ_META_MSB($bits(packed_intea_info_t)-1),.REQ_META_LSB(0)) ${if_name}_handshake_vif_t;

//-----------------------------------------------------------------------------