pub use crate::errors::GameErrorCode;
use crate::state::player_data::PlayerData;
use crate::{ ChestVaultAccount, GameData };
use anchor_lang::{ prelude::*, system_program };
use session_keys::{ Session, SessionToken };
use solana_program::native_token::LAMPORTS_PER_SOL;

pub fn send_bird(ctx: Context<SendBird>, action: u8) -> Result<()> {
  let mut highest_snail_index: Option<usize> = None;
  let mut highest_value: u64 = u64::MIN;

  msg!("Snail amount {}", ctx.accounts.game_data.snails.len());

  for snail in &mut ctx.accounts.game_data.snails {
    snail.update_position(Clock::get()?.unix_timestamp);
  }

  // Iterate through the snails to find the highest one
  for (index, snail) in ctx.accounts.game_data.snails.iter().enumerate() {
    //if snail.authority == ctx.accounts.signer.key() {
    // Replace `snail.value` with the actual property you want to compare
    if snail.position > highest_value {
      highest_value = snail.position;
      highest_snail_index = Some(index);
    }
    //}
  }

  // If no snail is found, return an error
  if highest_snail_index.is_none() {
    return Err(GameErrorCode::SnailDoesNotExist.into());
  }

  ctx.accounts.game_data.last_snail_eaten = ctx.accounts.game_data.snails[
    highest_snail_index.unwrap()
  ].authority;
  ctx.accounts.game_data.last_snail_eaten_time = Clock::get()?.unix_timestamp;

  // Remove the highest snail from the array
  ctx.accounts.game_data.snails.remove(highest_snail_index.unwrap());
  let amount_per_snail = LAMPORTS_PER_SOL / (ctx.accounts.game_data.snails.len() as u64) / 10;

  for snail in &ctx.accounts.game_data.snails {
    // Skip the snail of the signer
    if snail.authority != ctx.accounts.signer.key() {
      **ctx.accounts.vault.to_account_info().try_borrow_mut_lamports()? -= amount_per_snail;
      **ctx.accounts.signer.to_account_info().try_borrow_mut_lamports()? += amount_per_snail;
    }
  }

  Ok(())
}

#[derive(Accounts, Session)]
#[instruction(level_seed: String)]
pub struct SendBird<'info> {
  #[session(
    // The ephemeral key pair signing the transaction
    signer = signer,
    // The authority of the user account which must have created the session
    authority = player.authority.key()
  )]
  // Session Tokens are passed as optional accounts
  pub session_token: Option<Account<'info, SessionToken>>,

  #[account(
      mut,
      seeds = [b"player".as_ref(), player.authority.key().as_ref()],
      bump,
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

  #[account(mut)]
  pub signer: Signer<'info>,
  #[account(init_if_needed, payer = signer, space = 8, seeds = [b"vault".as_ref()], bump)]
  pub vault: Account<'info, ChestVaultAccount>,

  pub system_program: Program<'info, System>,
}
