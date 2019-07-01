
-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Peter Staal (PSTA)
-- Date created:   June 2018
--
-- Description:    package for the intea wic sub-module
--
-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-06-07  PSTA   Initial file
--
-- Comments:       -
--
-------------------------------------------------------------------------------

library PEnis;
library PEnis;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

use work.fixed_pkg.all;
use work.misc_util_p.all;               -- For log2
use work.intea_p.all;

-------------------------------------------------------------------------------

package intea_wic_p is

  -----------------------------------------------------------------------------
  -- Constants and types for readability, ease of integraton and/or ruggedness
  -----------------------------------------------------------------------------

  -----------------------------------------------------------------------------
  -- [S] box depths for wic (when it differs from default)
  constant C_DECODE_MUX_SBOX_DEPTH   : positive := 2;  -- as timing of nfmi and encoder is not necessarily correlated
  constant C_INV_QUANTIZE_SBOX_DEPTH : positive := 2;  -- as prediction and scale_update propagatoin delays might be slower than inv_quantize

  -- enc_t as vector
  subtype  enc_vec_t is std_ulogic_vector(C_ENC_TYPE_MSB downto 0);  -- for [S] ufixed data i/o
  constant C_ENC_VEC_ZERO : enc_vec_t := (others => '0');  -- use in clear statements

  -- wic scale_factor
  subtype  scale_factor_t is ufixed(-1 downto -14);
  constant C_SCALE_FACTOR_TYPE : scale_factor_t := to_ufixed(0, scale_factor_t'left, scale_factor_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_SCALE_FACTOR_ZERO : scale_factor_t := C_SCALE_FACTOR_TYPE;  -- use in clear statements
  type     scale_factor_dec_arr_t is array (wic_mode_t'left to wic_mode_t'right) of scale_factor_t;  -- scale_factor array - from update_scaling to inv_quantize
  type     scale_factor_dec_info_arr_t is array (wic_mode_t'left to wic_mode_t'right) of intea_info_t;  -- scale_factor info array - from update_scaling to inv_quantize

  -- prefill/reset value for scale_update = 0.25 (vector and natural representation = 0.25 * 2**abs(scale_factor_t'low))
  constant C_PREFILL_DATA_VAL_SCALE_FACTOR_0p25 : natural        := 4096;
  constant C_SCALE_FACTOR_DISABLE_VALUE_0p25    : scale_factor_t := to_ufixed(std_ulogic_vector(to_unsigned(C_PREFILL_DATA_VAL_SCALE_FACTOR_0p25, scale_factor_t'length)), C_SCALE_FACTOR_TYPE);

  -- quantization table element
  subtype  qtable_elem_t is ufixed(1 downto -10);
  constant C_QTABLE_ELEM_TYPE : qtable_elem_t := to_ufixed(0, qtable_elem_t'left, qtable_elem_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_QTABLE_ELEM_ZERO : qtable_elem_t := C_QTABLE_ELEM_TYPE;  -- use in clear statements

  -- inverse quantization output array
  constant C_INV_QUANTIZE_NUM_SBLKS : positive := 2;  -- number of [S] blocks in inv_quantize
  type     inv_quantize_data_arr_t is array (0 to C_INV_QUANTIZE_NUM_SBLKS - 1) of audio_rxtx_t;
  type     inv_quantize_info_arr_t is array (0 to C_INV_QUANTIZE_NUM_SBLKS - 1) of intea_info_t;
  subtype  inv_quantize_rdy_ack_arr_t is std_ulogic_vector(C_INV_QUANTIZE_NUM_SBLKS - 1 downto 0);

  -- quantization output array
  constant C_QUANTIZE_NUM_SBLKS : positive := 2;  -- number of [S] blocks in quantize
  type     quantize_data_arr_t is array (0 to C_QUANTIZE_NUM_SBLKS - 1) of enc_t;
  type     quantize_info_arr_t is array (0 to C_QUANTIZE_NUM_SBLKS - 1) of intea_info_t;
  subtype  quantize_rdy_ack_arr_t is std_ulogic_vector(C_QUANTIZE_NUM_SBLKS - 1 downto 0);

  -- predict coefficient memory
  subtype predict_coeffs_t is sfixed(1 downto -10);
  type    predict_coeff_mem_t is array(7 downto 0) of predict_coeffs_t;

  -- predict update engine
  subtype predict_upd_node_t is sfixed(predict_coeffs_t'high+3 downto predict_coeffs_t'low);

  -- predict COMPARATOR CIRCUIT
  constant C_EPSILON_POS : audio_rxtx_t := "000000000000000100";
  constant C_EPSILON_NEG : audio_rxtx_t := resize(0 - C_EPSILON_POS, C_AUDIO_RXTX_TYPE);
  subtype sign_err_predict_t is sfixed(1 downto 0);
  constant C_SIGN_ERR_NIL : sign_err_predict_t := (others => '0');
  constant C_SIGN_ERR_POS : sign_err_predict_t := to_sfixed( 1, sign_err_predict_t'high, sign_err_predict_t'low);
  constant C_SIGN_ERR_NEG : sign_err_predict_t := to_sfixed(-1, sign_err_predict_t'high, sign_err_predict_t'low);

end intea_wic_p;

package body intea_wic_p is

  -- TBD

end package body intea_wic_p;

