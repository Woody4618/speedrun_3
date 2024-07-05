using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Solana.Unity;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using Lastforever;
using Lastforever.Program;
using Lastforever.Errors;
using Lastforever.Accounts;
using Lastforever.Types;

namespace Lastforever
{
    namespace Accounts
    {
        public partial class ChestVaultAccount
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 9406927803919968769UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{1, 42, 101, 100, 255, 30, 140, 130};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "CJrgsQPtJV";
            public static ChestVaultAccount Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                ChestVaultAccount result = new ChestVaultAccount();
                return result;
            }
        }

        public partial class GameData
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 13758009850765924589UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{237, 88, 58, 243, 16, 69, 238, 190};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "ghYLwVtPH73";
            public ulong TotalWoodCollected { get; set; }

            public SnailData[] Snails { get; set; }

            public PublicKey LastSnailEaten { get; set; }

            public long LastSnailEatenTime { get; set; }

            public static GameData Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                GameData result = new GameData();
                result.TotalWoodCollected = _data.GetU64(offset);
                offset += 8;
                int resultSnailsLength = (int)_data.GetU32(offset);
                offset += 4;
                result.Snails = new SnailData[resultSnailsLength];
                for (uint resultSnailsIdx = 0; resultSnailsIdx < resultSnailsLength; resultSnailsIdx++)
                {
                    offset += SnailData.Deserialize(_data, offset, out var resultSnailsresultSnailsIdx);
                    result.Snails[resultSnailsIdx] = resultSnailsresultSnailsIdx;
                }

                result.LastSnailEaten = _data.GetPubKey(offset);
                offset += 32;
                result.LastSnailEatenTime = _data.GetS64(offset);
                offset += 8;
                return result;
            }
        }

        public partial class PlayerData
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 9264901878634267077UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{197, 65, 216, 202, 43, 139, 147, 128};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "ZzeEvyxXcpF";
            public PublicKey Authority { get; set; }

            public string Name { get; set; }

            public byte Level { get; set; }

            public ulong Xp { get; set; }

            public ulong Wood { get; set; }

            public ulong Energy { get; set; }

            public long LastLogin { get; set; }

            public ushort LastId { get; set; }

            public static PlayerData Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                PlayerData result = new PlayerData();
                result.Authority = _data.GetPubKey(offset);
                offset += 32;
                offset += _data.GetBorshString(offset, out var resultName);
                result.Name = resultName;
                result.Level = _data.GetU8(offset);
                offset += 1;
                result.Xp = _data.GetU64(offset);
                offset += 8;
                result.Wood = _data.GetU64(offset);
                offset += 8;
                result.Energy = _data.GetU64(offset);
                offset += 8;
                result.LastLogin = _data.GetS64(offset);
                offset += 8;
                result.LastId = _data.GetU16(offset);
                offset += 2;
                return result;
            }
        }
    }

    namespace Errors
    {
        public enum LastforeverErrorKind : uint
        {
            NotEnoughEnergy = 6000U,
            WrongAuthority = 6001U,
            SnailAlreadyExists = 6002U,
            InvalidAction = 6003U,
            SnailDoesNotExist = 6004U
        }
    }

    namespace Types
    {
        public partial class SnailData
        {
            public PublicKey Authority { get; set; }

            public PublicKey Avatar { get; set; }

            public long LastUpdateTime { get; set; }

            public ulong TotalFeedDelay { get; set; }

            public ulong Velocity { get; set; }

            public ulong Position { get; set; }

            public byte ArmorLevel { get; set; }

            public byte WeaponLevel { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(Authority, offset);
                offset += 32;
                _data.WritePubKey(Avatar, offset);
                offset += 32;
                _data.WriteS64(LastUpdateTime, offset);
                offset += 8;
                _data.WriteU64(TotalFeedDelay, offset);
                offset += 8;
                _data.WriteU64(Velocity, offset);
                offset += 8;
                _data.WriteU64(Position, offset);
                offset += 8;
                _data.WriteU8(ArmorLevel, offset);
                offset += 1;
                _data.WriteU8(WeaponLevel, offset);
                offset += 1;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out SnailData result)
            {
                int offset = initialOffset;
                result = new SnailData();
                result.Authority = _data.GetPubKey(offset);
                offset += 32;
                result.Avatar = _data.GetPubKey(offset);
                offset += 32;
                result.LastUpdateTime = _data.GetS64(offset);
                offset += 8;
                result.TotalFeedDelay = _data.GetU64(offset);
                offset += 8;
                result.Velocity = _data.GetU64(offset);
                offset += 8;
                result.Position = _data.GetU64(offset);
                offset += 8;
                result.ArmorLevel = _data.GetU8(offset);
                offset += 1;
                result.WeaponLevel = _data.GetU8(offset);
                offset += 1;
                return offset - initialOffset;
            }
        }
    }

    public partial class LastforeverClient : TransactionalBaseClient<LastforeverErrorKind>
    {
        public LastforeverClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ChestVaultAccount>>> GetChestVaultAccountsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = ChestVaultAccount.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ChestVaultAccount>>(res);
            List<ChestVaultAccount> resultingAccounts = new List<ChestVaultAccount>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => ChestVaultAccount.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ChestVaultAccount>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameData>>> GetGameDatasAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = GameData.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameData>>(res);
            List<GameData> resultingAccounts = new List<GameData>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => GameData.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameData>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<PlayerData>>> GetPlayerDatasAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = PlayerData.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<PlayerData>>(res);
            List<PlayerData> resultingAccounts = new List<PlayerData>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => PlayerData.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<PlayerData>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>> GetChestVaultAccountAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>(res);
            var resultingAccount = ChestVaultAccount.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<GameData>> GetGameDataAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<GameData>(res);
            var resultingAccount = GameData.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<GameData>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<PlayerData>> GetPlayerDataAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<PlayerData>(res);
            var resultingAccount = PlayerData.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<PlayerData>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeChestVaultAccountAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, ChestVaultAccount> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                ChestVaultAccount parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = ChestVaultAccount.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeGameDataAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, GameData> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                GameData parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = GameData.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribePlayerDataAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, PlayerData> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                PlayerData parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = PlayerData.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendInitPlayerAsync(InitPlayerAccounts accounts, string levelSeed, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.LastforeverProgram.InitPlayer(accounts, levelSeed, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendEnterRaceAsync(EnterRaceAccounts accounts, string levelSeed, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.LastforeverProgram.EnterRace(accounts, levelSeed, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInteractSnailAsync(InteractSnailAccounts accounts, string levelSeed, byte action, ushort counter, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.LastforeverProgram.InteractSnail(accounts, levelSeed, action, counter, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSendBirdAsync(SendBirdAccounts accounts, string levelSeed, byte action, ushort counter, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.LastforeverProgram.SendBird(accounts, levelSeed, action, counter, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendChopTreeAsync(ChopTreeAccounts accounts, string levelSeed, ushort counter, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.LastforeverProgram.ChopTree(accounts, levelSeed, counter, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<LastforeverErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<LastforeverErrorKind>>{{6000U, new ProgramError<LastforeverErrorKind>(LastforeverErrorKind.NotEnoughEnergy, "Not enough energy")}, {6001U, new ProgramError<LastforeverErrorKind>(LastforeverErrorKind.WrongAuthority, "Wrong Authority")}, {6002U, new ProgramError<LastforeverErrorKind>(LastforeverErrorKind.SnailAlreadyExists, "Snail already exists")}, {6003U, new ProgramError<LastforeverErrorKind>(LastforeverErrorKind.InvalidAction, "Invalid action")}, {6004U, new ProgramError<LastforeverErrorKind>(LastforeverErrorKind.SnailDoesNotExist, "Snail does not exist")}, };
        }
    }

    namespace Program
    {
        public class InitPlayerAccounts
        {
            public PublicKey Player { get; set; }

            public PublicKey GameData { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Signer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class EnterRaceAccounts
        {
            public PublicKey Player { get; set; }

            public PublicKey GameData { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Signer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class InteractSnailAccounts
        {
            public PublicKey SessionToken { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey GameData { get; set; }

            public PublicKey Signer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class SendBirdAccounts
        {
            public PublicKey SessionToken { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey GameData { get; set; }

            public PublicKey Signer { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class ChopTreeAccounts
        {
            public PublicKey SessionToken { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey GameData { get; set; }

            public PublicKey Signer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class LastforeverProgram
        {
            public static Solana.Unity.Rpc.Models.TransactionInstruction InitPlayer(InitPlayerAccounts accounts, string levelSeed, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameData, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Vault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4819994211046333298UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(levelSeed, offset);
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction EnterRace(EnterRaceAccounts accounts, string levelSeed, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameData, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Vault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2266413783460574962UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(levelSeed, offset);
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InteractSnail(InteractSnailAccounts accounts, string levelSeed, byte action, ushort counter, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SessionToken == null ? programId : accounts.SessionToken, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameData, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15617493602183854855UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(levelSeed, offset);
                _data.WriteU8(action, offset);
                offset += 1;
                _data.WriteU16(counter, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SendBird(SendBirdAccounts accounts, string levelSeed, byte action, ushort counter, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SessionToken == null ? programId : accounts.SessionToken, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameData, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Vault, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(11289621917330069412UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(levelSeed, offset);
                _data.WriteU8(action, offset);
                offset += 1;
                _data.WriteU16(counter, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ChopTree(ChopTreeAccounts accounts, string levelSeed, ushort counter, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SessionToken == null ? programId : accounts.SessionToken, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameData, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2027946759707441272UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(levelSeed, offset);
                _data.WriteU16(counter, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}
