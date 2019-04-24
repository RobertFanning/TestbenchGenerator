-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Dennis True
-- Date created:   8th of June 2018

-- Description:    This is top file for predict, contains dsp, storage, and not-synthesized 
-- filewriters


-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-06-08  DETR   Initial file

-- Comments:
-------------------------------------------------------------------------------

library ieee;
--use ieee.std_logic_1164.all;
--use ieee.numeric_std.all;
--use work.fixed_pkg.all;
--use work.fixed_util_p.all;
use work.intea_p.all;
--use work.intea_wic_p.all;


entity predict is
  generic(
    G_NODE_DUMP             : node_dump_t := 0
  );
  port (
    clk_i                   : in std_ulogic;
    reseta_ni               : in std_ulogic;
        
    -- inputs here
    err_recstr_i            : in audio_rxtx_t;
    err_recstr_rdy_i        : in std_ulogic;
    err_recstr_info_i       : in intea_info_t;
    err_recstr_ack_o        : out std_ulogic;
    
    x_predict_o             : out audio_rxtx_t;
    x_predict_rdy_o         : out std_ulogic; 
    x_predict_info_o        : out intea_info_t;
    x_predict_ack_i         : in std_ulogic;
    
    x_decoded_o             : out audio_rxtx_t;
    x_decoded_rdy_o         : out std_ulogic;
    x_decoded_info_o        : out intea_info_t;
    x_decoded_ack_i         : in std_ulogic;
    
    test_en_i               : in std_ulogic;
    scan_en_i               : in std_ulogic
    
    );
end predict;
