-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Peter Staal (PSTA)
-- Date created:   June 2018
--
-- Description:    Top package for the intea (Inter Ear Audio) module
--
-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-06-07  PSTA   Initial file
--
-- Comments:       -
--
-------------------------------------------------------------------------------

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

use work.fixed_pkg.all;
use work.misc_util_p.all;               -- For log2
use work.pi_p.all;                      -- For PI type
use work.fir_filt_32_p.all;

-------------------------------------------------------------------------------

package intea_p is

  -----------------------------------------------------------------------------
  -- Constants and types for readability, ease of integraton and/or ruggedness
  -----------------------------------------------------------------------------

  -- audio to/from intea (from beamformer, baud and debug mux)
  subtype  audio_rxtx_t is sfixed(0 downto -17);
  constant C_AUDIO_RXTX_TYPE : audio_rxtx_t := to_sfixed(0, audio_rxtx_t'left, audio_rxtx_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_AUDIO_RXTX_ZERO : audio_rxtx_t := C_AUDIO_RXTX_TYPE;  -- use in clear statements

  subtype  audio_tctrl_t is sfixed(0 downto -23);
  constant C_AUDIO_TCTRL_TYPE : audio_tctrl_t := to_sfixed(0, audio_tctrl_t'left, audio_tctrl_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_AUDIO_TCTRL_ZERO : audio_tctrl_t := C_AUDIO_TCTRL_TYPE;  -- use in clear statements

  -- audio from incnv
  subtype  audio_incnv_t is sfixed(1 downto -26);
  constant C_AUDIO_INCNV_TYPE : audio_incnv_t := to_sfixed(0, audio_incnv_t'left, audio_incnv_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_AUDIO_INCNV_ZERO : audio_incnv_t := C_AUDIO_INCNV_TYPE;  -- use in clear statements

  -- audio from ble
  subtype  audio_ble_t is sfixed(1 downto -18);
  constant C_AUDIO_BLE_TYPE : audio_ble_t := to_sfixed(0, audio_ble_t'left, audio_ble_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_AUDIO_BLE_ZERO : audio_ble_t := C_AUDIO_BLE_TYPE;  -- use in clear statements

  -- x_decoded in predict
  subtype  x_decoded_t is sfixed(0 downto -17);
  constant C_X_DECODED_TYPE : x_decoded_t := to_sfixed(0, x_decoded_t'left, x_decoded_t'right);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_X_DECODED_ZERO : x_decoded_t := C_X_DECODED_TYPE;  -- use in clear statements

  -- encoded stream to/from nfmi
  constant C_ENC_TYPE_MSB : positive := 4;  -- encoded stream sign bit index
  subtype  enc_t is unsigned(C_ENC_TYPE_MSB downto 0);
  constant C_ENC_TYPE     : enc_t    := to_unsigned(0, enc_t'length);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
  constant C_ENC_ZERO     : enc_t    := C_ENC_TYPE;  -- use in clear statements

   -- encoded stream buffer index to/from nfmi
   constant C_BUF_IDX_VEC_LEN : natural   := 6;  -- length of buffer index vector
   subtype  buf_idx_t is unsigned(C_BUF_IDX_VEC_LEN-1 downto 0);
   constant C_BUF_IDX_TYPE    : buf_idx_t := to_unsigned(0, buf_idx_t'length);  -- type to use in combinatorial statements (avoid modelsim sensitivity list warning), can also be use for clear/reset assignments
   constant C_BUF_IDX_ZERO    : buf_idx_t := C_BUF_IDX_TYPE;  -- use in clear statements

   type seq_t is (IDLE, PROCESSING, STORING, PROCESSING_AND_STORING);

   type mod_seq_t is record
   state : seq_t;                      -- sequencer states
   store : std_ulogic;                 -- sequencer store flag
   count : natural range 0 to 63;  -- sequence clock count (64 equals maximum for a sample in 2MHz domain) 
   end record;

   subtype node_dump_t is natural range 0 to 1;

  -- src_handling
  constant C_LP_COEFS2 : t_fir_svec_array := (
    "0000000000",
    "0000000000",
    "1111111111",
    "1111111110",
    "0000000000",
    "0000000010",
    "0000000001",
    "1111111100",
    "1111111010",
    "0000000010",
    "0000001010",
    "0000000011",
    "1111101111",
    "1111101110",
    "0000010100",
    "0001001110",
    "0001101010"
    );

    -- frame scaling
  constant C_SCALING_VEC_LEN : positive := 3;
  subtype scaling_t is unsigned(C_SCALING_VEC_LEN-1 downto 0);
  constant C_SCALING_CLR : scaling_t := (others => '0');

  -----------------------------------------------------------------------------
  -- construction of intea_info_t
  -- (meta-data record that travels along with data through S-blocks)
  -----------------------------------------------------------------------------

  -- wic mode/function flag
  type     wic_mode_t is (ENCODE, DECODE);
  constant C_WIC_MODE_VEC_LEN : natural := 1;  -- one bit needed to represent mode
  type     wic_mode_vec_t is array (wic_mode_t'left to wic_mode_t'right) of std_ulogic;  -- use for sub-module interfaces where both ENC and DEC versions of a i/o exists

  -- samplecount range for sim/synth
  constant C_SAMPLECOUNT_VEC_LEN : natural :=
    -- pragma translate_off
    30;                                 -- for simulation purposes
  constant C_DUMMY_SMP_CNT_VEC_LEN : natural :=
    -- pragma translate_on
    1;   -- for synthesis : reduced range (not used - it should be pruned)

  subtype samplecount_t is natural range 0 to 2**C_SAMPLECOUNT_VEC_LEN - 1;

  type intea_info_t is record
    mode : wic_mode_t;                  -- Mode of wic sub-module
                                        --  - value = ENCODE OR DECODE
                                        --  - for deciding which S-blocks and/or Z^-1 elements in the shared sub-blocks to update
                                        --  - use also for general debugging of shared mode blocks

    enable : std_ulogic;                -- ENCODE/DECODE enable
                                        --  - value = '0' OR '1'
                                        --  - When low, any internal Z^-1 elements of mode="mode" in a processing block must be cleared
                                        --  - processing and storage update must continue

    buf_idx : buf_idx_t;                -- NFMI buffer index
                                        --  - points to tx buffer in nfmi for mode=ENCODE
                                        --  - points to rx buffer in nfmi for mode=DECODE

    scaling : scaling_t;                -- Input/Source scaling
                                        --  - relates to audio_in  for mode=ENCODE
                                        --  - relates to audio_out for mode=DECODE

    fsync : std_ulogic;                 -- Frame sync flag
                                        --  - '0' : idle
                                        --  - '1' : perform frame sync
                                        --    Example : src_handling sees a radio_strobe event and updates the scaling element.
                                        --              src_handling sets fsync=1 for the poly_filt block to synchronize the updated scaling
                                        --              element with the next frame (buf_idx=0 from async_track)
                                        --    Usage : 1. block N sets fsync=1 for block M
                                        --            2. block M performs task upon fsync and clears fsync

    rxlink_status_ok : std_ulogic;      -- Current health of RX link
                                        --  - 0 : bad
                                        --  - 1 : all good, we are receiving
                                        --  Usage : for syncing rxlink_status_ok with rx_buffer index from async_track (to intea_buffer)

    loopback : std_ulogic;              -- loopback enabled indicator
                                        --  - can either be source/input loop, asrc loop or codec loop
                                        --  - '0' : disabled
                                        --  - '1' : enable

    -- elements for simulation only :
    samplecount : samplecount_t;  -- for visibility, connect to pi_tb samplecount output
  end record;

    -- must be updated when elements are added to intea_info_t
    constant C_INTEA_INFO_VEC_LEN : natural := (C_WIC_MODE_VEC_LEN + 1 + C_BUF_IDX_VEC_LEN + C_SCALING_VEC_LEN + 1 + 1 + 1 + C_SAMPLECOUNT_VEC_LEN);

    -----------------------------------------------------------------------------
    -- NB: for adding an element to the intea_info_t the following must be done
    --
    --      1 : add element to intea_info_t record (DUH)
    --
    --      2 : assign a unique element index (eg. C_ENABLE_IDX == 1),
    --          (just take the next available, value is nonimportant)
    --
    --      3 : calculate the C_<element_name>_LSB and C_<element_name>_MSB constants
    --          (range in intea_info_vec_t)
    --
    --      4 : update the C_INTEA_INFO_VEC_LEN length constant
    --          (add the bit width of the new element)
    --
    --      5 : update the func_rec2vec and func_vec2rec and func_info_vec_idx functions
    --          (record <-> vector conversion)
    --
    --      6 : update the func_clr function
    --          (for intea_info_t)
    --
    --      7 : the new element should now be ready for use...
    --
    --          NB : some vhdl files does not make use of the func_clr function
    --          which will leave the newly added element unassigned
    --          and cause a synth warning. This should be fixed eventually
    -- 
    -----------------------------------------------------------------------------
    
    -- intea_info_t as vector
    subtype intea_info_vec_t is std_ulogic_vector(C_INTEA_INFO_VEC_LEN - 1 downto 0);
  
    -- indexes for elements in intea_info_t record (arbitrary definition)
    constant C_WIC_MODE_IDX         : natural := 0;
    constant C_ENABLE_IDX           : natural := 1;
    constant C_BUF_IDX_IDX          : natural := 2;
    constant C_SAMPLECOUNT_IDX      : natural := 3;
    constant C_LOOPBACK_IDX         : natural := 4;
    constant C_SCALING_IDX          : natural := 5;
    constant C_FSYNC_IDX            : natural := 6;
    constant C_RXLINK_STATUS_OK_IDX : natural := 7;
  
    -- lsb/msb encoding
    constant C_LSB : std_ulogic := '0';
    constant C_MSB : std_ulogic := '1';
  
    
  -- calculate lsb's and msb's of intea_info_t elements inside intea_info_vec_t
  constant C_WIC_MODE_LSB         : natural := 0;  -- mode lsb is lsb of vec
  constant C_WIC_MODE_MSB         : natural := C_WIC_MODE_VEC_LEN - 1;
  constant C_ENABLE_LSB           : natural := C_WIC_MODE_MSB + 1;
  constant C_ENABLE_MSB           : natural := C_ENABLE_LSB;     -- 1 bit
  constant C_BUF_IDX_LSB          : natural := C_ENABLE_MSB + 1;
  constant C_BUF_IDX_MSB          : natural := C_ENABLE_MSB + C_BUF_IDX_VEC_LEN;
  constant C_SCALING_LSB          : natural := C_BUF_IDX_MSB + 1;
  constant C_SCALING_MSB          : natural := C_BUF_IDX_MSB + C_SCALING_VEC_LEN;
  constant C_FSYNC_LSB            : natural := C_SCALING_MSB + 1;
  constant C_FSYNC_MSB            : natural := C_FSYNC_LSB;  -- 1 bit
  constant C_RXLINK_STATUS_OK_LSB : natural := C_FSYNC_MSB + 1;
  constant C_RXLINK_STATUS_OK_MSB : natural := C_RXLINK_STATUS_OK_LSB;  -- 1 bit
  constant C_LOOPBACK_LSB         : natural := C_RXLINK_STATUS_OK_MSB + 1;
  constant C_LOOPBACK_MSB         : natural := C_LOOPBACK_LSB;  -- 1 bit
  constant C_SAMPLECOUNT_LSB      : natural := C_LOOPBACK_MSB + 1;
  constant C_SAMPLECOUNT_MSB      : natural := C_SAMPLECOUNT_LSB + C_SAMPLECOUNT_VEC_LEN - 1;

  -- size of storage prefill value vector
  constant C_STORAGE_PREFILL_WIDTH : positive := 128;

  -- control interface for the asrc sub-module
  type intea_asrc_ctrl_t is record
    radio_strobe        : std_ulogic;     -- rxlink direction switch
    rxlink_status_ok    : std_ulogic;     -- rxlink health
    enable_enc          : std_ulogic;     -- enable encoder flag
    local_sample_strobe : std_ulogic;     -- local side sample rate pulse
    local_sample_count  : samplecount_t;  -- local side sample count
  end record;

  -----------------------------------------------------------------------------
  -- misc functions
  -----------------------------------------------------------------------------

  -- clear records/types
  function func_clr(src_i : string) return mod_seq_t;
  function func_clr(src_i : string) return intea_info_t;
  function func_clr(src_i : string; mode_i : wic_mode_t) return intea_info_t;
  function func_clr(src_i : string) return wic_mode_t;
  function func_clr(src_i : string) return wic_mode_vec_t;
  function func_clr(src_i : string) return intea_asrc_ctrl_t;

  -- intea_info_t record to std_ulogic_vector and vice versa
  function func_rec2vec(src_i      : string; rec_i : intea_info_t) return intea_info_vec_t;
  function func_vec2rec(src_i      : string; vec_i : std_ulogic_vector) return intea_info_t;
  function func_info_vec_idx(src_i : string; idx_i : natural; sb_i : std_ulogic) return natural;
  function func_prefill_val(src    : string; width : positive; info : intea_info_t; val : natural) return std_ulogic_vector;

  
  end intea_p;
  

