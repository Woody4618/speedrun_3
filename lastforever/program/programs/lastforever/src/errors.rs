use anchor_lang::error_code;

#[error_code]
pub enum GameErrorCode {
  #[msg("Not enough energy")]
  NotEnoughEnergy,
  #[msg("Wrong Authority")]
  WrongAuthority,
  #[msg("Snail already exists")]
  SnailAlreadyExists,
  #[msg("Invalid action")]
  InvalidAction,
  #[msg("Snail does not exist")]
  SnailDoesNotExist,
}
