namespace Supercell.Laser.Logic.Avatar
{
    using Newtonsoft.Json;
    using Supercell.Laser.Titan.DataStream;
    using Supercell.Laser.Logic.Friends;
    using Supercell.Laser.Logic.Helper;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Logic.Home.Structures;
    using Supercell.Laser.Titan.DataStream;
    using Supercell.Laser.Titan.Debug;
    using System.IO;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Battle.Objects;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Security.Principal;

    public enum AllianceRole
    {
        None = 0,
        Member = 1,
        Leader = 2,
        Elder = 3,
        CoLeader = 4
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ClientAvatar
    {
        [JsonProperty] public long AccountId;
        [JsonProperty] public string PassToken;

        [JsonProperty] public string Name;
        [JsonProperty] public bool NameSetByUser;
        [JsonProperty] public int TutorialsCompletedCount = 2;

        [JsonIgnore] public HomeMode HomeMode;

        [JsonProperty] public int Gold;
        [JsonProperty] public int Diamonds;

        [JsonProperty] public List<Hero> Heroes;

        [JsonProperty] public int TrioWins;
        [JsonProperty] public int DuoWins;
        [JsonProperty] public int SoloWins;

        [JsonProperty] public int Tokens;
        [JsonProperty] public int StarTokens;
        [JsonProperty] public int StarPoints;
        [JsonProperty] public int ClubPoints;
        [JsonProperty] public int PowerPoints;
        [JsonProperty] public int ChromaticCoins;
        [JsonProperty] public int RareTokens;

        [JsonProperty] public bool IsDev;
        [JsonProperty] public bool IsPremium;

        [JsonProperty] public long AllianceId;
        [JsonProperty] public AllianceRole AllianceRole;

        [JsonProperty] public DateTime LastOnline;

        [JsonProperty] public List<Friend> Friends;
        [JsonProperty] public bool Banned;

        [JsonIgnore] public int PlayerStatus;
        [JsonIgnore] public long TeamId;

        [JsonIgnore] public long BattleId;
        [JsonIgnore] public long UdpSessionId;
        [JsonIgnore] public int TeamIndex;
        [JsonIgnore] public int OwnIndex;

        [JsonProperty] public int HighestTrophies;

        [JsonProperty] public int RollsSinceGoodDrop;

        public int Trophies
        {
            get
            {
                int result = 0;
                foreach (Hero hero in Heroes.ToArray())
                {
                    result += hero.Trophies;
                }
                return result;
            }
        }

        public void AddTrophies(int t)
        {
            HighestTrophies = Math.Max(HighestTrophies, Trophies + t);
        }

        public int GetUnlockedBrawlersCountWithRarity(string rarity)
        {
            return Heroes.Count(x => x.CardData.Rarity == rarity);
        }

        public void ResetTrophies()
        {
            foreach (Hero hero in Heroes.ToArray())
            {
                hero.Trophies = 0;
                hero.HighestTrophies = 0;
            }
        }

        public int GetUnlockedHeroesCount()
        {
            return Heroes.Count;
        }

        public void UnlockHero(int characterId)
        {
            Hero heroEntry = new Hero(characterId);
            Heroes.Add(heroEntry);
        }

        public void UpgradeHero(int characterId)
        {
            Hero heroEntry = GetHero(characterId);
            if (heroEntry.SelectedOverChargeId == 0)
            {
                CardData o = heroEntry.GetDefaultMetaForHero(6);
                if (o != null) heroEntry.SelectedOverChargeId = o.GetInstanceId();
            }
        }

        public void RemoveHero(int characterId)
        {
            Heroes.RemoveAll(x => x.CharacterId == characterId);
        }

        public bool HasHero(int characterId)
        {
            return Heroes.Find(x => x.CharacterId == characterId) != null;
        }

        public void SetEmoteForBrawler(int characterId, int slot, int pin)
        {
            Hero heroEntry = GetHero(characterId);
            heroEntry.emote[slot] = pin;
        }
        public Hero GetHero(int characterId)
        {
            return Heroes.Find(x => x.CharacterId == characterId);
        }
        public Hero GetHeroForCard(CardData cardData)
        {
            //Debugger.Print(DataTables.Get(16).GetData<CharacterData>(cardData.Target).GetInstanceId() + "");
            return GetHero(DataTables.Get(16).GetData<CharacterData>(cardData.Target).GetInstanceId() + 16000000);
        }

        public bool UseDiamonds(int count)
        {
            if (count > Diamonds) return false;

            Diamonds -= count;
            return true;
        }

        public bool UseBlings(int count)
        {
            if (count > StarPoints) return false;

            StarPoints -= count;
            return true;
        }

        public bool UseGold(int count)
        {
            if (count > Gold) return false;

            Gold -= count;
            return true;
        }

        public void AddDiamonds(int count)
        {
            Diamonds += count;
        }

        public void AddGold(int count)
        {
            Gold += count;
        }

        public bool UseTokens(int count)
        {
            if (count > Tokens) return false;

            Tokens -= count;
            return true;
        }

        public void AddTokens(int count)
        {
            HomeMode.Home.BrawlPassTokens += count;
        }

        public bool UseStarTokens(int count)
        {
            if (count > StarTokens) return false;

            StarTokens -= count;
            return true;
        }

        public void AddStarTokens(int count)
        {
            StarTokens += count;
        }

        public void AddPowerPoints(int count)
        {
            PowerPoints += count;
        }

        public void AddRareTokens(int count)
        {
            RareTokens += count;
        }

        public void AddBlings(int count)
        {
            StarPoints += count;
        }

        public ClientAvatar()
        {
            Name = "Brawler";

            Gold = 100;
            Diamonds = 0;

            Heroes = new List<Hero>();

            IsDev = false;
            IsPremium = false;

            AllianceRole = AllianceRole.None;
            AllianceId = -1;

            LastOnline = DateTime.UtcNow;
            Friends = new List<Friend>();
        }

        public void SkipTutorial()
        {
            TutorialsCompletedCount = 2;
        }

        public bool IsTutorialState()
        {
            return TutorialsCompletedCount < 2;
        }

        public Friend GetRequestFriendById(long id)
        {
            return Friends.Find(friend => friend.AccountId == id && friend.FriendState != 4);
        }

        public Friend GetAcceptedFriendById(long id)
        {
            return Friends.Find(friend => friend.AccountId == id && friend.FriendState == 4);
        }
        public EmoteData GetDefaultEmoteForCharacter(string Character, string Type)
        {
            foreach (EmoteData emoteData in DataTables.Get(DataType.Emote).GetDatas())
            {
                if (emoteData.Character == Character && emoteData.EmoteType == Type) return emoteData;
            }
            return null;
        }
        public Friend GetFriendById(long id)
        {
            return Friends.Find(friend => friend.AccountId == id);
        }
        public void Refresh()
        {
            for (int i = 0; ; i++)
            {
                CharacterData character = DataTables.Get(16).GetDataWithId<CharacterData>(i);
                if (character.Disabled && !character.LockedForChronos)
                {
                    RemoveHero(character.GetGlobalId());
                    continue;
                }
                if (!HasHero(16000000 + i))
                {
                    if (!character.IsHero()) break;
                    UnlockHero(character.GetGlobalId());
                }
            }
        }
        public int Checksum
        {
            get
            {
                ChecksumEncoder encoder = new ChecksumEncoder();

                return encoder.GetCheckSum();
            }
        }
        //64 vint 36 int 32 boolean
        //124 VInt 108 Int 104 Boolean
        public void Encode(ByteStream Stream)
        {

            Stream.WriteVLong(AccountId);
            Stream.WriteVLong(AccountId);
            Stream.WriteVLong(AccountId);
            Stream.WriteString(Name);
            Stream.WriteBoolean(NameSetByUser);
            Stream.WriteInt(-1);


            Stream.WriteVInt(15); ; //Commodity Array Count

            Stream.WriteVInt(9 + Heroes.Count);
            {
                foreach (Hero hero in Heroes)
                {
                    ByteStreamHelper.WriteDataReference(Stream, hero.CardData);
                    Stream.WriteVInt(-1);
                    Stream.WriteVInt(1);
                }
                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 2));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(11111); 

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 8));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(Gold); // HeroLvlUpMaterial -- vпиздец старая версия

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 10));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(StarPoints); // LegendaryTrophies -- v19

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 13));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(ClubPoints);  // ClubCoins -- v40

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 18));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(5);  // SpraySlot -- v43

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 19));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(0);  // RecruitTokens -- v47

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 20));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(ChromaticCoins);  // ChromaticTokens -- v47

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 21));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(0);  // Fame -- v47

                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(5, 22));
                Stream.WriteVInt(-1);
                Stream.WriteVInt(PowerPoints);  // PowerPoints -- v47
            }

            Stream.WriteVInt(Heroes.Count);//2
            foreach (Hero hero in Heroes)
            {
                ByteStreamHelper.WriteDataReference(Stream, hero.CharacterData);
                Stream.WriteVInt(-1);
                Stream.WriteVInt(hero.Trophies);
            }

            Stream.WriteVInt(Heroes.Count);//3
            foreach (Hero hero in Heroes)
            {
                ByteStreamHelper.WriteDataReference(Stream, hero.CharacterData);
                Stream.WriteVInt(-1);
                Stream.WriteVInt(hero.HighestTrophies);
            }

            Stream.WriteVInt(Heroes.Count);//4
            foreach (Hero hero in Heroes)
            {
                ByteStreamHelper.WriteDataReference(Stream, hero.CharacterData);
                Stream.WriteVInt(-1);
                Stream.WriteVInt(RandomNumberGenerator.GetInt32(4));
            }

            Stream.WriteVInt(Heroes.Count);//5
            foreach (Hero hero in Heroes)
            {
                ByteStreamHelper.WriteDataReference(Stream, hero.CharacterData);
                Stream.WriteVInt(-1);
                Stream.WriteVInt(hero.PowerPoints);
            }

            Stream.WriteVInt(Heroes.Count);//6
            foreach (Hero hero in Heroes)
            {
                ByteStreamHelper.WriteDataReference(Stream, hero.CharacterData);
                Stream.WriteVInt(-1);
                //Stream.WriteVInt(hero.PowerLevel);
                Stream.WriteVInt(1);
            }


            //Get spgs
            {
                List<int> spgs = new List<int>();
                foreach (CardData carddata in DataTables.Get(DataType.Card).GetDatas())
                {
                    if (carddata.MetaType == 4 || carddata.MetaType == 5 || carddata.MetaType == 6)
                    {
                        spgs.Add(carddata.GetInstanceId());
                    }
                }
                List<int> spgsChosen = new List<int>();
                foreach (Hero hero in Heroes)
                {
                    spgsChosen.Add(hero.SelectedGadgetId);
                    spgsChosen.Add(hero.SelectedStarPowerId);
                    spgsChosen.Add(hero.SelectedOverChargeId);
                }
                Stream.WriteVInt(spgs.Count);//7
                for (int i = 0; i < spgs.Count; i++)
                {
                    ByteStreamHelper.WriteDataReference(Stream, 23000000 + spgs[i]);
                    Stream.WriteVInt(-1);
                    Stream.WriteVInt(0);//0 lock 1 unlock 2 chosen
                }

            }


            Stream.WriteVInt(0); // HeroSeenState

            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array
            Stream.WriteVInt(0); // Array

            Stream.WriteVInt(Diamonds); // Diamonds
            Stream.WriteVInt(0); // Free Diamonds
            Stream.WriteVInt(999); // Player Level
            Stream.WriteVInt(100);
            Stream.WriteVInt(0); // CumulativePurchasedDiamonds or Avatar User Level Tier | 10000 < Level Tier = 3 | 1000 < Level Tier = 2 | 0 < Level Tier = 1
            Stream.WriteVInt(0); // Battle Count
            Stream.WriteVInt(0); // WinCount
            Stream.WriteVInt(0); // LoseCount
            Stream.WriteVInt(0); // WinLooseStreak
            Stream.WriteVInt(0); // NpcWinCount
            Stream.WriteVInt(0); // NpcLoseCount
            Stream.WriteVInt(2); // TutorialState | shouldGoToFirstTutorialBattle = State == 0
            Stream.WriteVInt(12);
            Stream.WriteVInt(0);
            Stream.WriteVInt(0);
            Stream.WriteString("");
            Stream.WriteVInt(0);
            Stream.WriteVInt(0);
            Stream.WriteVInt(1);



        }
    }

}
