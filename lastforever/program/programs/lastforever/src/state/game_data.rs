use anchor_lang::prelude::*;

use crate::constants::MAX_WOOD_PER_TREE;

#[account]
pub struct GameData {
  pub total_wood_collected: u64,
  pub snails: Vec<SnailData>,
  pub last_snail_eaten: Pubkey,
  pub last_snail_eaten_time: i64,
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

#[derive(Default, AnchorDeserialize, AnchorSerialize, Clone, Debug, PartialEq)]
pub struct SnailData {
  /// The authority who controls this snail
  pub authority: Pubkey,

  /// The avatar associated with this snail
  pub avatar: Pubkey,

  /// The last recorded time of the crawl
  pub last_update_time: i64,

  /// The total delay due to feeding
  pub total_feed_delay: u64,

  /// The base velocity of the snail, in units per second
  pub velocity: u64,

  /// The current position of the snail
  pub position: u64,

  /// The armor level which reduces the effective velocity
  pub armor_level: u8,

  /// The weapon level of the snail
  pub weapon_level: u8,
}

impl SnailData {
  /// Calculates the effective velocity considering the armor level
  pub fn effective_velocity(&self) -> u64 {
    self.velocity.saturating_sub(self.armor_level as u64)
  }

  /// Updates the snail's position based on the current time
  pub fn update_position(&mut self, current_time: i64) {
    let elapsed_time = (current_time - self.last_update_time) as u64;
    let effective_velocity = self.effective_velocity();
    self.position += elapsed_time.saturating_sub(self.total_feed_delay) * effective_velocity;
    self.last_update_time = current_time;
    self.total_feed_delay = 0; // Reset feed delay after applying
  }

  /// Feeds the snail, adding to the total feed delay
  pub fn feed(&mut self, timestamp: i64, delay: u64) {
    self.total_feed_delay += delay;
    self.update_position(timestamp); // Update position before applying the new feed delay
  }

  /// Feeds the snail, adding to the total feed delay
  pub fn slime(&mut self, timestamp: i64, delay: u64) {
    self.total_feed_delay -= delay;
    self.update_position(timestamp); // Update position before applying the new feed delay
  }

  /// Upgrades the armor level
  pub fn upgrade_armor(&mut self, timestamp: i64, new_level: u8) {
    self.update_position(timestamp); // Update position before applying the new armor level
    self.armor_level = new_level;
  }
}

// #[derive(Default, AnchorDeserialize, AnchorSerialize, Clone, Debug, PartialEq)]
// pub struct Events {
//   time_stamp: i64,
//   duration: i64,
// }
