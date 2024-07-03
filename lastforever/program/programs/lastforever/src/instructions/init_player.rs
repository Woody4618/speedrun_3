pub use crate::errors::GameErrorCode;
use crate::state::game_data::SnailData;
use crate::state::player_data::PlayerData;
use crate::{ constants::MAX_ENERGY, GameData };
use anchor_lang::{ prelude::*, system_program };
use anchor_lang::system_program::transfer;
use solana_program::native_token::LAMPORTS_PER_SOL;

pub fn init_player(ctx: Context<InitPlayer>) -> Result<()> {
  ctx.accounts.player.energy = MAX_ENERGY;
  ctx.accounts.player.last_login = Clock::get()?.unix_timestamp;
  ctx.accounts.player.authority = ctx.accounts.signer.key();
  for snail in &ctx.accounts.game_data.snails {
    if snail.authority == ctx.accounts.signer.key() {
      return Err(GameErrorCode::SnailAlreadyExists.into());
    }
  }

  let snailData = SnailData {
    authority: ctx.accounts.signer.key(),
    avatar: Pubkey::default(),
    last_update_time: Clock::get()?.unix_timestamp,
    total_feed_delay: 0,
    armor_level: 0,
    weapon_level: 0,
    velocity: 100,
    position: 0,
  };
  ctx.accounts.game_data.snails.push(snailData);

  let cpi_context = CpiContext::new(
    ctx.accounts.system_program.to_account_info(),
    system_program::Transfer {
      from: ctx.accounts.signer.to_account_info().clone(),
      to: ctx.accounts.vault.to_account_info().clone(),
    }
  );

  transfer(cpi_context, LAMPORTS_PER_SOL)?;
  Ok(())
}

#[derive(Accounts)]
#[instruction(level_seed: String)]
pub struct InitPlayer<'info> {
  #[account(
    init,
    payer = signer,
    space = 1000, // 8+32+x+1+8+8+8 But taking 1000 to have space to expand easily.
    seeds = [b"player".as_ref(), signer.key().as_ref()],
    bump
  )]
  pub player: Account<'info, PlayerData>,

  #[account(
    init_if_needed,
    payer = signer,
    space = 1000, // 8 + 8 for anchor account discriminator and the u64. Using 1000 to have space to expand easily.
    seeds = [level_seed.as_ref()],
    bump
  )]
  pub game_data: Account<'info, GameData>,

  #[account(init_if_needed, payer = signer, space = 8, seeds = [b"vault".as_ref()], bump)]
  pub vault: Account<'info, ChestVaultAccount>,

  #[account(mut)]
  pub signer: Signer<'info>,
  pub system_program: Program<'info, System>,
}

#[account]
pub struct ChestVaultAccount {}
