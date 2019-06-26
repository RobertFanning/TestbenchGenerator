-- DeltaData	837	837	56
-- Default libraries
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
-- library delta_lib;-- ?? 
-- library work; -- ??
use work.pi_p.all;
use work.misc_util_p.all;
use work.fixed_util_p.all;
 
package intea_pif_p is
-- Type declarations
-- input
subtype INTEA_InvBufferSize_t                  is unsigned(16-1 downto 0);
subtype INTEA_M_SourceSelect_t                 is unsigned(3-1 downto 0);
subtype INTEA_Prolong_Conn_t                   is unsigned(8-1 downto 0);
subtype INTEA_WDC_RadioDelayCompensation_t     is unsigned(12-1 downto 0);
subtype INTEA_WIC_Qtable_elem_t                    is unsigned(12-1 downto 0);
type INTEA_WIC_Qtable_t                        is array(0 to 15) of INTEA_WIC_Qtable_elem_t;
subtype NFMI_Wlink4_Intvl_t                    is unsigned(9-1 downto 0);
subtype Remote_INTEA_Enable_Loop_t             is unsigned(2-1 downto 0);
-- output

-- Input record declaration
type intea_pif_in_t is record
   INTEA_InvBufferSize                    : INTEA_InvBufferSize_t;
   INTEA_M_SourceSelect                   : INTEA_M_SourceSelect_t;
   INTEA_Prolong_Conn                     : INTEA_Prolong_Conn_t;
   INTEA_WDC_RadioDelayCompensation       : INTEA_WDC_RadioDelayCompensation_t;
   INTEA_WIC_Qtable                       : INTEA_WIC_Qtable_t;
   NFMI_Wlink4_Intvl                      : NFMI_Wlink4_Intvl_t;
   Remote_INTEA_Enable_Loop               : Remote_INTEA_Enable_Loop_t;
end record; -- input

-- Output record declaration
type intea_pif_out_t is record
   Meas_INTEA_AudioRx_Overflow       : std_logic;
   Meas_INTEA_AudioTx_Underflow      : std_logic;
end record; -- output

type intea_pif_rd_str_t is record
   Meas_INTEA_AudioRx_Overflow            : std_logic;
   Meas_INTEA_AudioTx_Underflow           : std_logic;
end record; -- fbreadstrobes


-- Global parameters from def file header
constant VERSION           : integer := 837;

-- Commands

-- FB_PARAM NULL constant
constant INTEA_PIF_CLR: intea_pif_out_t := (
   Meas_INTEA_AudioRx_Overflow       => '0',
   Meas_INTEA_AudioTx_Underflow      => '0');

constant INTEA_PIF_OUT_LENGTH : integer :=
    1
   +1;

subtype intea_pif_SLV is std_logic_vector(intea_pif_out_length-1 downto 0);
function intea_pif_out_to_slv (fbrec:intea_pif_out_t) return intea_pif_SLV;
function slv_to_intea_pif_out (fb_slv: intea_pif_SLV) return intea_pif_out_t;
-- pragma synthesis_off 
signal db_param: intea_pif_in_t;
signal db_fbparam: intea_pif_out_t;
-- pragma synthesis_on 

end intea_pif_p;	-- Package Header 

package body intea_pif_p is 
function intea_pif_out_to_slv (fbrec:intea_pif_out_t) return intea_pif_SLV is
   variable result : intea_pif_SLV := (others => '0');
   variable index:integer := 0;
begin
   result(index) := fbrec.Meas_INTEA_AudioRx_Overflow;index:=index+1;
   result(index) := fbrec.Meas_INTEA_AudioTx_Underflow;index:=index+1;

   return result;
end intea_pif_out_to_slv;

function slv_to_intea_pif_out (fb_slv: intea_pif_SLV) return intea_pif_out_t is
   variable result : intea_pif_out_t := intea_pif_clr;
   variable index:integer := 0;
begin
   result.Meas_INTEA_AudioRx_Overflow              := fb_slv(index);index:=index+1;
   result.Meas_INTEA_AudioTx_Underflow             := fb_slv(index);index:=index+1;

   return result;
end slv_to_intea_pif_out;
end intea_pif_p;

