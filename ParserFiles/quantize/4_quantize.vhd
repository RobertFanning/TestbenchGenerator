-------------------------------------------------------------------------------
-- Widex A/S (HQ), Global R&D
-------------------------------------------------------------------------------
-- Author/Ini:     Peter Staal (PSTA)
-- Date created:   June 2018
--
-- Description:    Intea Codec Forward Quantization sub-module top
--
-- Revision Log:   Rev:  Date:       Init:  Change:
--                 001   2018-06-26  PSTA   Initial file
--
-- Registered IO:  Output registers in [S] elements
--
-- Comments:       Instantiates [P]rocessing entity
--                 and implements [S]torage blocks
--
-------------------------------------------------------------------------------

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

use work.intea_p.all;
use work.intea_wic_p.all;
use work.quantization_p.all;
use work.fixed_pkg.all;
use work.fixed_util_p.all;
use work.misc_util_p.all;

entity quantize is
  generic(
    G_NODE_DUMP      : node_dump_t := 0;
    G_X_IN_SHIFT_CLR : natural range 0 to 31  -- input ramp init value
    );
  port (
    -- clock & reset
    clk_i     : in std_ulogic;
    reseta_ni : in std_ulogic;                -- asynchronous reset (ver _5)

    -- audio in
    x_in_i      : in  audio_rxtx_t;
    x_in_rdy_i  : in  std_ulogic;
    x_in_info_i : in  intea_info_t;
    x_in_ack_o  : out std_ulogic;

    -- predicted value
    x_predict_i      : in  audio_rxtx_t;
    x_predict_rdy_i  : in  std_ulogic;
    x_predict_info_i : in  intea_info_t;
    x_predict_ack_o  : out std_ulogic;

    -- scale factor
    scale_factor_i      : in  scale_factor_t;
    scale_factor_info_i : in  intea_info_t;
    scale_factor_rdy_i  : in  std_ulogic;
    scale_factor_ack_o  : out std_ulogic;

    -- multiplier command/response
    mult_cmd_o : out mult_cmd_t;
    mult_rsp_i : in  mult_rsp_t;

    -- encoded output
    x_encoded_o      : out quantize_data_arr_t;
    x_encoded_info_o : out quantize_info_arr_t;
    x_encoded_rdy_o  : out quantize_rdy_ack_arr_t;
    x_encoded_ack_i  : in  quantize_rdy_ack_arr_t
    );
end entity quantize;

architecture rtl of quantize is

  ----------------------------
  -- entity instance hierachy
  constant C_INST : string :=
    -- pragma translate_off
    quantize'instance_name;
  constant C_DUMMY : string :=
    -- pragma translate_on
    "";

  -----------------------------------------------------------------------------
  -- local constants, (sub-)types and signals
  -----------------------------------------------------------------------------
  signal x_encoded_s     : enc_t;
  signal x_encoded_rdy_s : std_ulogic;  -- local rdy bit for [S] elements

begin

  -----------------------------------------------------------------------------
  -- [P]rocessing
  -----------------------------------------------------------------------------
  quantize_dsp_inst : entity work.quantize_dsp
    generic map (
      G_X_IN_SHIFT_CLR => G_X_IN_SHIFT_CLR
      )
    port map (
      clk_i               => clk_i,
      reseta_ni           => reseta_ni,
      x_in_i              => x_in_i,
      x_in_rdy_i          => x_in_rdy_i,
      x_in_info_i         => x_in_info_i,
      x_in_ack_o          => x_in_ack_o,
      x_predict_i         => x_predict_i,
      x_predict_rdy_i     => x_predict_rdy_i,
      x_predict_info_i    => x_predict_info_i,
      x_predict_ack_o     => x_predict_ack_o,
      scale_factor_i      => scale_factor_i,
      scale_factor_info_i => scale_factor_info_i,
      scale_factor_rdy_i  => scale_factor_rdy_i,
      scale_factor_ack_o  => scale_factor_ack_o,
      mult_cmd_o          => mult_cmd_o,
      mult_rsp_i          => mult_rsp_i,
      x_encoded_o         => x_encoded_s,
      x_encoded_rdy_o     => x_encoded_rdy_s);

  -----------------------------------------------------------------------------
  -- [S]torage block
  -----------------------------------------------------------------------------
  b_storage : block
    type   quantize_data_vec_arr_t is array (0 to C_QUANTIZE_NUM_SBLKS - 1) of enc_vec_t;
    signal data_in_s  : quantize_data_vec_arr_t;
    signal data_out_s : quantize_data_vec_arr_t;
    signal info_out_s : quantize_info_arr_t;
    signal ack_s      : quantize_rdy_ack_arr_t;
  begin

    ---------------------------------------------------------------------------
    -- storage
    ---------------------------------------------------------------------------
    g_storage : for sblk in 0 to C_QUANTIZE_NUM_SBLKS - 1 generate
    begin
      sbox_suv_intea_inst : entity work.sbox_suv_intea
        generic map (
          G_MSB => C_ENC_TYPE_MSB,
          G_LSB => 0
          )
        port map (
          clk_i     => clk_i,
          reseta_ni => reseta_ni,
          sclr_i    => '0',
          rdy_i     => x_encoded_rdy_s,
          data_i    => data_in_s(sblk),
          info_i    => x_in_info_i,
          rdy_o     => x_encoded_rdy_o(sblk),
          ack_i     => ack_s(sblk),
          data_o    => data_out_s(sblk),
          info_o    => info_out_s(sblk)
          );

      -- [S] input
      data_in_s(sblk) <= std_ulogic_vector(x_encoded_s);
      ack_s(sblk)     <= x_encoded_ack_i(sblk);

      -- [S] output(s)
      x_encoded_o(sblk) <= unsigned(data_out_s(sblk));
      p_info : process(info_out_s(sblk))
      begin
        x_encoded_info_o(sblk)      <= info_out_s(sblk);
        x_encoded_info_o(sblk).mode <= ENCODE;  -- output mode is hardwired
      end process;

    end generate;

  end block b_storage;

  -----------------------------------------------------------------------------
  -- for simulation only :
  -----------------------------------------------------------------------------
  -- pragma translate_off
  b_sim : block

    signal node0_s     : std_ulogic_vector(enc_t'length-1 downto 0);
    signal nodes_rdy_s : std_ulogic_vector(0 downto 0);

  begin

    -- local assertions
    p_assert : process(reseta_ni, clk_i)
    begin
      if reseta_ni = '0' then
        null;
      elsif rising_edge(clk_i) then

        -- check all info modes = ENCODE
        assert x_in_info_i.mode = ENCODE report "expected x_in_info_i.mode=ENCODE (although output mode is hardwired)" severity error;
        assert x_predict_info_i.mode = ENCODE report "expected x_predict_info_i.mode=ENCODE (although output mode is hardwired)" severity error;
        assert scale_factor_info_i.mode = ENCODE report "expected scale_factor_info_i.mode=ENCODE (although output mode is hardwired)" severity error;

        -- check readiness
        if x_in_rdy_i = '1' then
          assert x_predict_rdy_i = '1' report "predictor must signal ready when x_in_rdy flag rises" severity error;
          assert scale_factor_rdy_i = '1' report "scale_update must signal ready when x_in_rdy flag rises" severity error;
        end if;
        
      end if;
    end process;

    -- node dump(s)
    node_dump_gen : if (G_NODE_DUMP = 1) generate
      quantize_fw_inst : entity work.quantize_fw
        port map (
          reseta_ni => reseta_ni,
          clk_i     => clk_i
          );
    end generate node_dump_gen;

  end block b_sim;
  -- pragma translate_on

end architecture rtl;
