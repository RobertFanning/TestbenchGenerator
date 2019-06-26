-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Dennis True (DETR)
-- Date created:   July 2018
--
-- Description:    Intea codec source handling top file
--
-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-07-30  DETR   Initial file
--
-- Registered IO:  -
--
-- Comments:       -
--
-------------------------------------------------------------------------------

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

use work.pi_p.all;
use work.intea_pif_p.all;
use work.intea_p.all;
use work.intea_asrc_p.all;
use work.fixed_pkg.all;
use work.fixed_util_p.all;
use work.misc_util_p.all;
use work.intea_asrc_p.all;

entity async_track is
  generic(
    G_NODE_DUMP : node_dump_t := 0
    );
  port (
    -- clock & reset
    clk_i     : in std_ulogic;
    reseta_ni : in std_ulogic;          -- asynchronous reset (ver _5)

    -- parameters
    asrc_ctrl_i           : in  intea_asrc_ctrl_t;
    async_sample_strobe_o : out std_ulogic;
    pi_param_i            : in  intea_pif_in_t;

    offset_rx_o      : out offset_t;
    offset_rx_rdy_o  : out std_ulogic;
    offset_rx_info_o : out intea_info_t;
    offset_rx_ack_i  : in  std_ulogic;

    offset_tx_o      : out offset_t;
    offset_tx_rdy_o  : out std_ulogic;
    offset_tx_info_o : out intea_info_t;
    offset_tx_ack_i  : in  std_ulogic;

    rx_buffer_idx_o      : out buf_idx_t;
    rx_buffer_idx_rdy_o  : out std_ulogic;
    rx_buffer_idx_info_o : out intea_info_t;
    rx_buffer_idx_ack_i  : in  std_ulogic;

    test_en_i : in std_ulogic;
    scan_en_i : in std_ulogic
    );
end entity async_track;

architecture rtl of async_track is

  signal offset_tx_s             : offset_t;
  signal offset_tx_rdy_s         : std_ulogic;
  signal offset_tx_rdy_delayed_s : std_ulogic_vector(C_OFFSET_TX_RDY_CLK_CYC_DLY downto 0);
  signal offset_tx_info_s        : intea_info_t;
  signal offset_rx_s             : offset_t;
  signal offset_rx_rdy_s         : std_ulogic;
  signal offset_rx_rdy_delayed_s : std_ulogic_vector(C_OFFSET_RX_RDY_CLK_CYC_DLY downto 0);
  signal offset_rx_info_s        : intea_info_t;
  signal rx_buffer_idx_s         : buf_idx_t;
  signal rx_buffer_idx_suvi_s    : std_ulogic_vector(C_BUF_IDX_VEC_LEN-1 downto 0);
  signal rx_buffer_idx_suvo_s    : std_ulogic_vector(C_BUF_IDX_VEC_LEN-1 downto 0);
  signal rx_buffer_idx_rdy_s     : std_ulogic;
  signal rx_buffer_idx_info_s    : intea_info_t;
  
begin

  async_track_dsp_inst : entity work.async_track_dsp
    port map(
      clk_i                => clk_i,
      reseta_ni            => reseta_ni,
      pi_param_i           => pi_param_i,
      asrc_ctrl_i          => asrc_ctrl_i,
      offset_tx_o          => offset_tx_s,
      offset_tx_rdy_o      => offset_tx_rdy_s,
      offset_tx_info_o     => offset_tx_info_s,
      offset_rx_o          => offset_rx_s,
      offset_rx_rdy_o      => offset_rx_rdy_s,
      offset_rx_info_o     => offset_rx_info_s,
      rx_buffer_idx_o      => rx_buffer_idx_s,
      rx_buffer_idx_rdy_o  => rx_buffer_idx_rdy_s,
      rx_buffer_idx_info_o => rx_buffer_idx_info_s,
      test_en_i            => test_en_i,
      scan_en_i            => scan_en_i
      );

  async_sample_strobe_o <= offset_tx_rdy_s;

  -- offset_rx into sbox is delayed, such that it coincides with coeff_rx for polyfilt correct behaviour
  -- in other words : fsync (first buffer index) of both coeff and offset MUST coincide
  delay_offset_rx : process(clk_i, reseta_ni)
  begin
    if reseta_ni = '0' then
      offset_rx_rdy_delayed_s <= (others => '0');
    elsif rising_edge(clk_i) then
      offset_rx_rdy_delayed_s <= offset_rx_rdy_delayed_s(C_OFFSET_RX_RDY_CLK_CYC_DLY-1 downto 0) & offset_rx_rdy_s;
    end if;
  end process;

  -- offset_tx into sbox is delayed, such that it coincides with coeff_tx for polyfilt correct behaviour
  -- in other words : fsync (first buffer index) of both coeff and offset MUST coincide
  delay_tx_offset : process(clk_i, reseta_ni)
  begin
    if reseta_ni = '0' then
      offset_tx_rdy_delayed_s <= (others => '0');
    elsif rising_edge(clk_i) then
      offset_tx_rdy_delayed_s <= offset_tx_rdy_delayed_s(C_OFFSET_TX_RDY_CLK_CYC_DLY-1 downto 0) & offset_tx_rdy_s;
    end if;
  end process;


  s_offset_tx : entity work.sbox_ufx_intea
    generic map(
      G_DEPTH => 1,
      G_MSB   => offset_t'high,
      G_LSB   => offset_t'low
      )
    port map(
      clk_i     => clk_i,
      sclr_i    => '0',
      reseta_ni => reseta_ni,
      rdy_i     => offset_tx_rdy_delayed_s(0),
      data_i    => offset_tx_s,
      info_i    => offset_tx_info_s,

      rdy_o  => offset_tx_rdy_o,
      ack_i  => offset_tx_ack_i,
      data_o => offset_tx_o,
      info_o => offset_tx_info_o
      );

  s_offset_rx : entity work.sbox_ufx_intea
    generic map(
      G_DEPTH => 1,
      G_MSB   => offset_t'high,
      G_LSB   => offset_t'low
      )
    port map(
      clk_i     => clk_i,
      sclr_i    => '0',
      reseta_ni => reseta_ni,
      rdy_i     => offset_rx_rdy_delayed_s(C_OFFSET_RX_RDY_CLK_CYC_DLY),
      data_i    => offset_rx_s,
      info_i    => offset_rx_info_s,
      rdy_o     => offset_rx_rdy_o,
      ack_i     => offset_rx_ack_i,
      data_o    => offset_rx_o,
      info_o    => offset_rx_info_o
      );

  s_buffer_idx_rx : entity work.sbox_suv_intea
    generic map(
      G_MSB => C_BUF_IDX_VEC_LEN-1
      )
    port map(
      clk_i     => clk_i,
      sclr_i    => '0',
      reseta_ni => reseta_ni,
      rdy_i     => rx_buffer_idx_rdy_s,
      data_i    => rx_buffer_idx_suvi_s,
      info_i    => rx_buffer_idx_info_s,
      rdy_o     => rx_buffer_idx_rdy_o,
      ack_i     => rx_buffer_idx_ack_i,
      data_o    => rx_buffer_idx_suvo_s,
      info_o    => rx_buffer_idx_info_o
      );

  rx_buffer_idx_suvi_s <= std_ulogic_vector(rx_buffer_idx_s);
  rx_buffer_idx_o      <= unsigned(rx_buffer_idx_suvo_s);

  -- pragma translate_off
  -- FW only for test
  node_dump : if G_NODE_DUMP = 1 generate
    async_track_writer : entity work.async_track_fw
      port map(
        clk_i     => clk_i,
        reseta_ni => reseta_ni
        );
  end generate node_dump;
  -- pragma translate_on

end architecture rtl;
