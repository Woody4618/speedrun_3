pub use crate::errors::GameErrorCode;
use crate::state::game_data::SnailData;
use crate::state::player_data::PlayerData;
use crate::{ GameData };
use anchor_lang::prelude::*;
use session_keys::{ Session, SessionToken };

pub fn interact_snail(ctx: Context<InteractSnail>, action: u8, snail_id: Pubkey) -> Result<()> {
  let mut snailData: Option<&mut SnailData> = None;

  for snail in &mut ctx.accounts.game_data.snails {
    if snail.authority == snail_id {
      snailData = Some(snail);
    }
  }
  if snailData.is_none() {
    return Err(GameErrorCode::SnailDoesNotExist.into());
  }

  match snailData {
    Some(snail_data) => {
      match action {
        0 => {
          // Feed Snail
          snail_data.feed(Clock::get()?.unix_timestamp, 10);
          ctx.accounts.player.update_energy()?;
          ctx.accounts.player.energy -= 20;
        }
        1 => {
          // Speedup Snail
          snail_data.slime(Clock::get()?.unix_timestamp, 10);
          ctx.accounts.player.energy -= 30;
        }
        2 => {
          snail_data.armor_level += 1;
          // Upgrade armor
          snail_data.upgrade_armor(Clock::get()?.unix_timestamp, snail_data.armor_level);
          ctx.accounts.player.energy -= 55;
        }
        3 => {
          // TODO: implement weapon upgrade
          snail_data.armor_level += 1;
          // Upgrade armor
          snail_data.upgrade_armor(Clock::get()?.unix_timestamp, snail_data.armor_level);
          ctx.accounts.player.energy -= 55;
        }
        _ => {
          return Err(GameErrorCode::InvalidAction.into());
        }
      }
    }
    None => {
      return Err(GameErrorCode::SnailDoesNotExist.into());
    }
  }

  Ok(())
}

#[derive(Accounts, Session)]
#[instruction(level_seed: String)]
pub struct InteractSnail<'info> {
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
  pub system_program: Program<'info, System>,
}
