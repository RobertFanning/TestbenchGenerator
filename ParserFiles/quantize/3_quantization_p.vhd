
-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Peter Staal (PSTA)
-- Date created:   June 2018
--
-- Description:    package for the intea -> wic -> quantization sub-module
--
-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-06-14  PSTA   Initial file
--
-- Comments:       -
--
-------------------------------------------------------------------------------

use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

use work.fixed_pkg.all;
use work.misc_util_p.all;
use work.intea_p.all;
use work.intea_wic_p.all;

-------------------------------------------------------------------------------

package quantization_p is

  -- table index (one bit less than enc_t)
  subtype table_idx_t is unsigned(C_ENC_TYPE_MSB - 1 downto 0);

  -----------------------------------------------------------------------------
  -- quantization multiplier command
  -----------------------------------------------------------------------------
  type mult_cmd_t is record
    req  : std_ulogic;                             -- multiplication request
    idx  : table_idx_t;                            -- Table index        => multiplier operand0
    scf  : scale_factor_t;                         -- Scale factor value => multiplier operand1
    info : intea_info_t;                             -- info for loopback
  end record;

  -- multiplier input
  subtype mult_result_t is ufixed(1 downto -20);

  -----------------------------------------------------------------------------
  -- quantization multiplier response
  -----------------------------------------------------------------------------
  type mult_rsp_t is record
    ack  : std_ulogic;     -- multiplication acknowledge / result ready
    res  : mult_result_t;  -- multiplication result
    info : intea_info_t;     -- cmd info looped back
  end record;

  -- arrays for mult_quantize I/O
  type mult_cmd_arr_t is array (wic_mode_t'left to wic_mode_t'right) of mult_cmd_t;
  type mult_rsp_arr_t is array (wic_mode_t'left to wic_mode_t'right) of mult_rsp_t;

  -----------------------------------------------------------------------------
  -- misc functions
  -----------------------------------------------------------------------------

  -- clear records
  function func_clr(src_i : string) return mult_cmd_t;
  function func_clr(src_i : string) return mult_rsp_t;
  constant C_X_DECODED_ZERO : x_decoded_t := 6+-7; 

end quantization_p;

package body quantization_p is

  -- clear command
  function func_clr(src_i : string) return mult_cmd_t is
    variable rec_s : mult_cmd_t;
  begin
    rec_s.req  := '0';
    rec_s.idx  := (others => '0');
    rec_s.scf  := C_SCALE_FACTOR_ZERO;
    rec_s.info := func_clr(src_i);
    return rec_s;
  end;

  -- clear response
  function func_clr(src_i : string) return mult_rsp_t is
    variable rec_s : mult_rsp_t;
  begin
    rec_s.ack  := '0';
    rec_s.res  := (others => '0');
    rec_s.info := func_clr(src_i);
    return rec_s;
  end;

end package body quantization_p;

