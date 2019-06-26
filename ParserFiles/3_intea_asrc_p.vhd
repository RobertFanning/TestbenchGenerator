-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Jan Baadsgaard
-- Date created:   31th July 2018

-- Description:    Top file package for INTEA module ASRC

-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-07-31  JANB   Initial file

-- Comments: ASRC(Asyncronous Sample Rate Converter)
-------------------------------------------------------------------------------

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

use work.fixed_pkg.all;
use work.misc_util_p.all;               -- for log2 etc.
use work.intea_p.all;

package intea_asrc_p is

  -- coeff_update coefficient and array
  subtype  coeff_update_coeff_t is sfixed(2 downto -17);
  constant C_COEFF_UPDATE_COEFF_TYPE : coeff_update_coeff_t := to_sfixed(0, coeff_update_coeff_t'left, coeff_update_coeff_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_COEFF_UPDATE_COEFF_ZERO : coeff_update_coeff_t := C_COEFF_UPDATE_COEFF_TYPE;  -- use in clear statements
  constant C_NUM_COEFF_UPDATE_COEFFS : positive             := 4;  -- number of coeff_update coefficients
  type     coeff_update_coeff_arr_t is array (0 to C_NUM_COEFF_UPDATE_COEFFS-1) of coeff_update_coeff_t;
  subtype  coeff_update_coeff_vec_t is std_ulogic_vector(C_NUM_COEFF_UPDATE_COEFFS*coeff_update_coeff_t'length - 1 downto 0);

  -- coeff_update to poly_filt
  type asrc_coeff_arr_t is array (wic_mode_t'left to wic_mode_t'right) of coeff_update_coeff_arr_t;
  type asrc_info_arr_t is array (wic_mode_t'left to wic_mode_t'right) of intea_info_t;

  -- poly_filt coefficient format
  type coeff_rdy_t is array (0 to C_NUM_COEFF_UPDATE_COEFFS-1) of std_ulogic;
  type coeff_array_rdy_t is array (wic_mode_t'left to wic_mode_t'right) of coeff_rdy_t;
  -- poly_filt coefficient format  
  type coeff_inf_t is array (0 to C_NUM_COEFF_UPDATE_COEFFS-1) of intea_info_t;
  type coeff_array_inf_t is array (wic_mode_t'left to wic_mode_t'right) of coeff_inf_t;
  
  -- offset_rx/tx clock cycle delay to achieve same rtl behaviour as HASIM
  -- This  ensures that coefficients updated on the async_strobe (ENCODE) or local strobe (ENCODE) are used with the right offset
  -- (async_track operates on local_sample_strobe and generates the async_sample_strobe)
  constant C_OFFSET_TX_RDY_CLK_CYC_DLY : positive := 1;
  constant C_OFFSET_RX_RDY_CLK_CYC_DLY : positive := 20;
  
  -- clear functions
  function func_clr(src_i : string) return coeff_update_coeff_arr_t;

  -- functions for converting coefficient array to vector and back
  function func_coeff2vec(src : string; arr : coeff_update_coeff_arr_t) return coeff_update_coeff_vec_t;
  function func_vec2coeff(src : string; vec : coeff_update_coeff_vec_t) return coeff_update_coeff_arr_t;

  subtype offset_t is ufixed(0 downto -8);

end package intea_asrc_p;
-------------------------------------------------------------------------------

package body intea_asrc_p is

  function func_clr(src_i : string) return coeff_update_coeff_arr_t is
    variable ret_s : coeff_update_coeff_arr_t;
  begin
    ret_s := (others => C_COEFF_UPDATE_COEFF_ZERO);
    return ret_s;
  end;

  -- convert coefficient array to vector
  function func_coeff2vec(src : string; arr : coeff_update_coeff_arr_t) return coeff_update_coeff_vec_t is
    variable vec_s : coeff_update_coeff_vec_t;
  begin
    vec_s := (others => '0');
    for coeff in 0 to C_NUM_COEFF_UPDATE_COEFFS-1 loop
      vec_s((coeff+1)*coeff_update_coeff_t'length - 1 downto coeff*coeff_update_coeff_t'length) := to_suv(arr(coeff));
    end loop;
    return vec_s;
  end;

  -- convert vector to coefficient array
  function func_vec2coeff(src : string; vec : coeff_update_coeff_vec_t) return coeff_update_coeff_arr_t is
    variable arr_s : coeff_update_coeff_arr_t;
  begin
    arr_s := func_clr(src);
    for coeff in 0 to C_NUM_COEFF_UPDATE_COEFFS-1 loop
      arr_s(coeff) := to_sfixed(vec((coeff+1)*coeff_update_coeff_t'length - 1 downto coeff*coeff_update_coeff_t'length), C_COEFF_UPDATE_COEFF_TYPE);
    end loop;
    return arr_s;
  end;

end intea_asrc_p;
