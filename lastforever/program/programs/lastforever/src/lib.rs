pub use crate::errors::GameErrorCode;
pub use anchor_lang::prelude::*;
pub use session_keys::{ session_auth_or, Session, SessionError };
pub mod constants;
pub mod errors;
pub mod instructions;
pub mod state;
use instructions::*;

declare_id!("h4D4pns4hvYhFnqy9cZiZyZm65kjZbFqbfZWdrnVjYZ");

#[program]
pub mod lastforever {
  use super::*;

  pub fn init_player(ctx: Context<InitPlayer>, _level_seed: String) -> Result<()> {
    init_player::init_player(ctx)
  }

  pub fn interact_snail(
    ctx: Context<InteractSnail>,
    _level_seed: String,
    action: u8,
    counter: u16
  ) -> Result<()> {
    interact_snail::interact_snail(ctx, action)
  }

  pub fn send_bird(
    ctx: Context<SendBird>,
    _level_seed: String,
    action: u8,
    counter: u16
  ) -> Result<()> {
    send_bird::send_bird(ctx, action)
  }

  // This function lets the player chop a tree and get 1 wood. The session_auth_or macro
  // lets the player either use their session token or their main wallet. (The counter is only
  // there so that the player can do multiple transactions in the same block. Without it multiple transactions
  // in the same block would result in the same signature and therefore fail.)
  #[session_auth_or(
    ctx.accounts.player.authority.key() == ctx.accounts.signer.key(),
    GameErrorCode::WrongAuthority
  )]
  pub fn chop_tree(ctx: Context<ChopTree>, _level_seed: String, counter: u16) -> Result<()> {
    chop_tree::chop_tree(ctx, counter, 1)
  }
}
