use anchor_lang::prelude::*;

use crate::constants::MAX_WOOD_PER_TREE;

use super::player_data::PlayerData;

#[account]
pub struct GameData {
  pub total_wood_collected: u64,
  pub players: Vec<PlayerData>,
}

impl GameData {
  pub fn on_tree_chopped(&mut self, amount_chopped: u64) -> Result<()> {
    match self.total_wood_collected.checked_add(amount_chopped) {
      Some(v) => {
        if self.total_wood_collected >= MAX_WOOD_PER_TREE {
          self.total_wood_collected = 0;
          msg!("Tree successfully chopped. New Tree coming up.");
        } else {
          self.total_wood_collected = v;
          msg!("Total wood chopped: {}", v);
        }
      }
      None => {
        msg!("The ever tree is completly chopped!");
      }
    }

    Ok(())
  }
}
