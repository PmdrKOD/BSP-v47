using System.Linq;

namespace Supercell.Laser.Logic.Home
{
    using System;
    using System.Security.Cryptography;
    using Newtonsoft.Json;
    using System.Numerics;
    using Supercell.Laser.Logic.Command.Home;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Data.Helper;
    using Supercell.Laser.Logic.Helper;
    using Supercell.Laser.Logic.Home.Gatcha;
    using Supercell.Laser.Logic.Home.Items;
    using Supercell.Laser.Logic.Home.Quest;
    using Supercell.Laser.Logic.Home.Structures;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Message.Home;
    using Supercell.Laser.Titan.DataStream;
    using Supercell.Laser.Titan.Debug;
    using System.Text;
    using System.Reflection.Metadata;
    using System.Formats.Asn1;
    using System.Globalization;

    [JsonObject(MemberSerialization.OptIn)]
    public class ClientHome
    {
        public const int DAILYOFFERS_COUNT = 6;

        public static readonly int[] GoldPacksPrice = new int[]
        {
            20, 50, 140, 280
        };

        public static readonly int[] GoldPacksAmount = new int[]
        {
            150, 400, 1200, 2600
        };

        [JsonProperty] public long HomeId;
        [JsonProperty] public int ThumbnailId;
        [JsonProperty] public int NameColorId;
        [JsonProperty] public int[] CharacterIds;
        [JsonProperty] public int FavouriteCharacter;
        public int CharacterId => CharacterIds[0];

        [JsonIgnore] public List<OfferBundle> OfferBundles;

        [JsonProperty] public int TrophiesReward;
        [JsonProperty] public int TokenReward;
        [JsonProperty] public int StarTokenReward;

        [JsonProperty] public BigInteger BrawlPassProgress;
        [JsonProperty] public BigInteger PremiumPassProgress;
        [JsonProperty] public BigInteger BrawlPassPlusProgress;
        [JsonProperty] public int BrawlPassTokens;
        [JsonProperty] public bool HasPremiumPass;
        [JsonProperty] public List<int> UnlockedEmotes;
        [JsonProperty] public List<int> UnlockedThumbnails;
        [JsonProperty] public List<int> UnlockedTituls;
        [JsonProperty] public NotificationFactory NotificationFactory;
        [JsonProperty] public List<int> UnlockedSkins;

        [JsonProperty] public int TrophyRoadProgress;
        [JsonIgnore] public Quests Quests;
        [JsonProperty] public int EventId;
        [JsonProperty] public List<PlayerMap> PlayerMaps = new List<PlayerMap>();

        [JsonProperty] public BattleCard DefaultBattleCard;
        [JsonIgnore] public EventData[] Events;

        [JsonProperty] public int PreferredThemeId;

        [JsonProperty] public int RecruitTokens;
        [JsonProperty] public int RecruitBrawler;
        [JsonProperty] public int RecruitBrawlerCard;
        [JsonProperty] public int RecruitGemsCost;
        [JsonProperty] public int RecruitCost;
        [JsonProperty] public int ChromaticCoins; // after v52 - shit bcs not uset yet

        [JsonProperty] public List<string> OffersClaimed;

        [JsonProperty] public Dictionary<int, int> PlayerSelectedEmotes = new Dictionary<int, int>();


        [JsonProperty] public List<int> Brawlers;

        public PlayerThumbnailData Thumbnail => DataTables.Get(DataType.PlayerThumbnail).GetDataByGlobalId<PlayerThumbnailData>(ThumbnailId);
        public NameColorData NameColor => DataTables.Get(DataType.NameColor).GetDataByGlobalId<NameColorData>(NameColorId);

        public HomeMode HomeMode;

        [JsonProperty] public DateTime LastVisitHomeTime;

        public ClientHome()
        {
            Brawlers = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 34, 36, 37, 40, 42, 43, 45, 47, 48, 50, 52, 58, 61, 63, 64, 67, 69, 71, 73 };
            PlayerSelectedEmotes.Add(1, 28);
            PlayerSelectedEmotes.Add(2, 28);
            PlayerSelectedEmotes.Add(3, 28);
            PlayerSelectedEmotes.Add(4, 28);
            PlayerSelectedEmotes.Add(5, 94 - 3);
            RecruitBrawler = 1;
            RecruitBrawlerCard = 4;
            RecruitCost = 160;
            RecruitGemsCost = 29;
            RecruitTokens = 0;
            OffersClaimed = new List<string>();

            ThumbnailId = GlobalId.CreateGlobalId(28, 0);
            NameColorId = GlobalId.CreateGlobalId(43, 0);
            CharacterIds = new int[] { GlobalId.CreateGlobalId(16, 0), GlobalId.CreateGlobalId(16, 1), GlobalId.CreateGlobalId(16, 2) };
            FavouriteCharacter = GlobalId.CreateGlobalId(16, 0);

            OfferBundles = new List<OfferBundle>();
            UnlockedSkins = new List<int>();
            UnlockedEmotes = new List<int>();
            UnlockedTituls = new List<int>();
            UnlockedThumbnails = new List<int>();
            LastVisitHomeTime = DateTime.UnixEpoch;

            TrophyRoadProgress = 1;

            BrawlPassProgress = 1;
            PremiumPassProgress = 1;
            EventId = 1;
            UnlockedEmotes = new List<int>();
            DefaultBattleCard = new BattleCard();

            if (NotificationFactory == null) NotificationFactory = new NotificationFactory();

            PreferredThemeId = -1;
        }

        public int TimerMath(DateTime timer_start, DateTime timer_end)
        {
            {
                DateTime timer_now = DateTime.Now;
                if (timer_now > timer_start)
                {
                    if (timer_now < timer_end)
                    {
                        int time_sec = (int)(timer_end - timer_now).TotalSeconds;
                        return time_sec;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        public void HomeVisited()
        {

            OfferBundles.RemoveAll(bundle => bundle.IsTrue);
            GenerateOffer(
                new DateTime(2024, 5, 9, 12, 0, 0), new DateTime(2025, 5, 10, 11, 0, 0),
                10000, 999, 130, ShopItem.Coin,
                1, 9999999, 1,
                "уцкйвыаукйцу", "Подарок", "offer_generic",
                 0, false,
                false, false, 0,
                0, 0, 0, 0, 0
            );

            GenerateOffer(
             new DateTime(2024, 5, 9, 12, 0, 0), new DateTime(2025, 5, 10, 11, 0, 0),
             10000, 999, 52, ShopItem.Skin,
             1, 9999999, 1,
             "уцкйвыаукйцу", "Подарок", "offer_generic",
              0, false,
             false, false, 0,
             0, 0, 0, 0, 0
         );


            LastVisitHomeTime = DateTime.UtcNow;

            if (Quests == null && TrophyRoadProgress >= 11)
            {
                Quests = new Quests();
                Quests.AddRandomQuests(HomeMode.Avatar.Heroes, 8);
            }
            else if (Quests != null)
            {
                if (Quests.QuestList.Count < 8) // New quests adds at 07:00 AM UTC
                {
                    Quests.AddRandomQuests(HomeMode.Avatar.Heroes, 8 - Quests.QuestList.Count);
                }
            }
        }

        public void Tick()
        {
            LastVisitHomeTime = DateTime.UtcNow;
            TokenReward = 0;
            TrophiesReward = 0;
            StarTokenReward = 0;
        }

        public void PurchaseOffer(int index)
        {
            if (index < 0 || index >= OfferBundles.Count) return;

            OfferBundle bundle = OfferBundles[index];
            if (bundle.Purchased) return;

            if (bundle.Currency == 0)
            {
                if (!HomeMode.Avatar.UseDiamonds(bundle.Cost)) return;
            }
            else if (bundle.Currency == 1)
            {
                if (!HomeMode.Avatar.UseGold(bundle.Cost)) return;
            }

            bundle.Purchased = true;

            if (bundle.Claim == "debug")
            {

            }
            else
            {
                OffersClaimed.Add(bundle.Claim);
            }



            LogicGiveDeliveryItemsCommand command = new LogicGiveDeliveryItemsCommand();
            Random rand = new Random();
            foreach (Offer offer in bundle.Items)
            {

                if (offer.Type == ShopItem.BrawlBox || offer.Type == ShopItem.FreeBox)
                {
                    for (int x = 0; x < offer.Count; x++)
                    {
                        DeliveryUnit unit = new DeliveryUnit(10);
                        HomeMode.SimulateGatcha(unit);
                        if (x + 1 != offer.Count)
                        {
                            command.Execute(HomeMode);
                        }
                        command.DeliveryUnits.Add(unit);
                    }
                }
                else if (offer.Type == ShopItem.HeroPower)
                {
                    DeliveryUnit unit = new DeliveryUnit(100);
                    GatchaDrop reward = new GatchaDrop(6);
                    reward.DataGlobalId = offer.ItemDataId;
                    reward.Count = offer.Count;
                    unit.AddDrop(reward);
                    command.DeliveryUnits.Add(unit);
                }
                else if (offer.Type == ShopItem.BigBox)
                {
                    for (int x = 0; x < offer.Count; x++)
                    {
                        DeliveryUnit unit = new DeliveryUnit(12);
                        HomeMode.SimulateGatcha(unit);
                        if (x + 1 != offer.Count)
                        {
                            command.Execute(HomeMode);
                        }
                        command.DeliveryUnits.Add(unit);
                    }
                }
                else if (offer.Type == ShopItem.MegaBox)
                {
                    for (int x = 0; x < offer.Count; x++)
                    {
                        DeliveryUnit unit = new DeliveryUnit(11);
                        HomeMode.SimulateGatcha(unit);
                        if (x + 1 != offer.Count)
                        {
                            command.Execute(HomeMode);
                        }
                        command.DeliveryUnits.Add(unit);
                    }
                }
                else if (offer.Type == ShopItem.Skin)
                {
                    DeliveryUnit unit = new DeliveryUnit(100);
                    GatchaDrop reward = new GatchaDrop(9);
                    reward.SkinGlobalId = GlobalId.CreateGlobalId(29, offer.SkinDataId);
                    unit.AddDrop(reward);
                    command.DeliveryUnits.Add(unit);
                }
                else if (offer.Type == ShopItem.Gems)
                {
                    DeliveryUnit unit = new DeliveryUnit(100);
                    GatchaDrop reward = new GatchaDrop(8);
                    reward.Count = offer.Count;
                    unit.AddDrop(reward);
                    command.DeliveryUnits.Add(unit);
                }
                else if (offer.Type == ShopItem.Coin)
                {
                    DeliveryUnit unit = new DeliveryUnit(100);
                    GatchaDrop reward = new GatchaDrop(7);
                    reward.Count = offer.Count;
                    unit.AddDrop(reward);
                    command.DeliveryUnits.Add(unit);
                }
                else if (offer.Type == ShopItem.CoinDoubler)
                {
                    DeliveryUnit unit = new DeliveryUnit(100);
                    GatchaDrop reward = new GatchaDrop(2);
                    reward.Count = offer.Count;
                    unit.AddDrop(reward);
                    command.DeliveryUnits.Add(unit);
                }
                else
                {
                    // todo...
                }

                command.Execute(HomeMode);


                void NewCommand(SkinData skinData)
                {
                    if (skinData == null) return;
                    DeliveryUnit unit = new DeliveryUnit(100);
                    GatchaDrop reward = new GatchaDrop(9);
                    foreach (EmoteData emoteData in DataTables.Get(DataType.Emote).GetDatas())
                    {

                        if (emoteData.Skin == skinData.Name)
                        {
                            GatchaDrop reward1 = new GatchaDrop(11);
                            reward1.DataGlobalId = DataTables.Get(DataType.Emote).GetData<EmoteData>(emoteData.Name).GetGlobalId();
                            reward1.Count = 1;
                            unit.AddDrop(reward1);
                        }

                    }
                    foreach (PlayerThumbnailData playerThumbnailData in DataTables.Get(DataType.PlayerThumbnail).GetDatas())
                    {
                        if (playerThumbnailData.CatalogPreRequirementSkin == skinData.Name)
                        {
                            GatchaDrop reward1 = new GatchaDrop(11);
                            reward1.DataGlobalId = DataTables.Get(DataType.PlayerThumbnail).GetData<PlayerThumbnailData>(playerThumbnailData.Name).GetGlobalId();
                            reward1.Count = 1;
                            unit.AddDrop(reward1);
                        }

                    }
                    foreach (SprayData sprayData in DataTables.Get(DataType.Spray).GetDatas())
                    {
                        if (sprayData.Skin == skinData.Name)
                        {
                            GatchaDrop reward1 = new GatchaDrop(11);
                            reward1.DataGlobalId = DataTables.Get(DataType.Spray).GetData<SprayData>(sprayData.Name).GetGlobalId();
                            reward1.Count = 1;
                            unit.AddDrop(reward1);
                        }
                    }
                    command.DeliveryUnits.Add(unit);
                    command.Execute(HomeMode);

                    AvailableServerCommandMessage message = new AvailableServerCommandMessage();
                    message.Command = command;
                    HomeMode.GameListener.SendMessage(message);
                }
            }
            AvailableServerCommandMessage message = new AvailableServerCommandMessage();
            message.Command = command;
            HomeMode.GameListener.SendMessage(message);
        }


        public void GenerateOffer(
            DateTime OfferStart,
            DateTime OfferEnd,
            int Count,
            int BrawlerID,
            int Extra,
            ShopItem Item,
            int Cost,
            int OldCost,
            int Currency,
            string Claim,
            string Title,
            string BGR,
            int DailyOfferType,
            bool OneTimeOffer,
            bool LoadOnStartup,
            bool Processed,
            int TypeBenefit,
            int Benefit,

            int panelClass,
            int panelType,

            int styleClass,
            int styleType

            )
        {

            OfferBundle bundle = new OfferBundle();
            bundle.IsDailyDeals = false;
            bundle.IsTrue = true;
            bundle.EndTime = OfferEnd;
            bundle.Cost = Cost;
            bundle.OldCost = OldCost;
            bundle.Currency = Currency;
            bundle.Claim = Claim;
            bundle.Title = Title;
            bundle.BackgroundExportName = BGR;

            bundle.OfferType = DailyOfferType;
            bundle.OneTimeOffer = OneTimeOffer;
            bundle.LoadOnStartup = LoadOnStartup;
            bundle.Processed = Processed;
            bundle.TypeBenefit = TypeBenefit;
            bundle.Benefit = Benefit;
            bundle.ShopPanelLayoutClass = panelClass;
            bundle.ShopPanelLayoutType = panelType;
            bundle.ShopStyleSetClass = styleClass;
            bundle.ShopStyleSetType = styleType;




            //  if (OffersClaimed.Contains(bundle.Claim))
            //  {
            //      bundle.Purchased = true;
            /// }
            //  if (TimerMath(OfferStart, OfferEnd) == -1)
            //  {
            //     bundle.Purchased = true;
            //  }
            // if (HomeMode.HasHeroUnlocked(16000000 + BrawlerID))
            // {
            //     bundle.Purchased = true;
            // }

            Offer offer = new Offer(Item, Count, (16000000 + BrawlerID), Extra);
            bundle.Items.Add(offer);

            OfferBundles.Add(bundle);
        }


        public void LogicDailyData(ByteStream encoder, DateTime utcNow)
        {
            encoder.WriteVInt(2000000);
            encoder.WriteVInt(0);

            encoder.WriteVInt(utcNow.Year * 1000 + utcNow.DayOfYear); // 0x78d4b8
            encoder.WriteVInt(utcNow.Hour * 3600 + utcNow.Minute * 60 + utcNow.Second); // 0x78d4cc
            encoder.WriteVInt(HomeMode.Avatar.Trophies); // 0x78d4e0
            encoder.WriteVInt(HomeMode.Avatar.HighestTrophies); // 0x78d4f4
            encoder.WriteVInt(HomeMode.Avatar.HighestTrophies); // highest trophy again?
            encoder.WriteVInt(TrophyRoadProgress);
            encoder.WriteVInt(1909); // experience
            ByteStreamHelper.WriteDataReference(encoder, Thumbnail);
            ByteStreamHelper.WriteDataReference(encoder, NameColorId);


            encoder.WriteVInt(27);
            for (int i = 0; i < 27; i++)
                encoder.WriteVInt(i);

            int skins = 0;
            foreach (Hero hero in HomeMode.Avatar.Heroes)
            {
                if (hero.SelectedSkinId != 0) skins++;
            }

            encoder.WriteVInt(skins); // Selected Skins
            foreach (Hero hero in HomeMode.Avatar.Heroes)
            {
                if (hero.SelectedSkinId != 0) ByteStreamHelper.WriteDataReference(encoder, GlobalId.CreateGlobalId(29, hero.SelectedSkinId));
            }
            encoder.WriteVInt(0); // Randomizer Skin Selected

            encoder.WriteVInt(0); // Current Random Skin

            encoder.WriteVInt(UnlockedSkins.Count); // Played game modes
            foreach (int s in UnlockedSkins)
            {
                ByteStreamHelper.WriteDataReference(encoder, s);
            }

            encoder.WriteVInt(0); // Unlocked Skin Purchase Option

            encoder.WriteVInt(0);////New Item State

            encoder.WriteVInt(0);
            encoder.WriteVInt(HomeMode.Avatar.HighestTrophies);
            encoder.WriteVInt(0);
            encoder.WriteVInt(1);
            encoder.WriteBoolean(true);
            encoder.WriteVInt(0);
            encoder.WriteVInt(8000);////TrophyRoad Timer
            encoder.WriteVInt(0);////PowerPlay Timer
            encoder.WriteVInt(80000);////Brawl Pass Timer

            encoder.WriteVInt(141);
            encoder.WriteVInt(135);

            encoder.WriteVInt(5);

            encoder.WriteVInt(93);
            encoder.WriteVInt(206);
            encoder.WriteVInt(456);
            encoder.WriteVInt(792);
            encoder.WriteVInt(729);

            encoder.WriteBoolean(false);////Offer 1
            encoder.WriteBoolean(false);////Offer 2
            encoder.WriteBoolean(true); // Token Doubler Enabled
            encoder.WriteVInt(2);  // Token Doubler New Tag State
            encoder.WriteVInt(2);  // Event Tickets New Tag State
            encoder.WriteVInt(2);  // Coin Packs New Tag State
            encoder.WriteVInt(0);  // Change Name Cost
            encoder.WriteVInt(0);  // Timer For the Next Name Change

            encoder.WriteVInt(OfferBundles.Count); // Shop offers at 0x78e0c4
            foreach (OfferBundle offerBundle in OfferBundles)
            {
                offerBundle.Encode(encoder);        // ес че эта хуйня крашит
            }


            encoder.WriteVInt(200);
            encoder.WriteVInt(-1);

            encoder.WriteVInt(0);

            encoder.WriteVInt(0);
            encoder.WriteVInt(0);

            encoder.WriteByte(1); // brawler selected count
            ByteStreamHelper.WriteDataReference(encoder, CharacterIds[0]);
            //ByteStreamHelper.WriteDataReference(encoder, CharacterIds[1]);
            //ByteStreamHelper.WriteDataReference(encoder, CharacterIds[2]);

            encoder.WriteString("RU"); // Location
            encoder.WriteString("BSP"); // Content Creator

            encoder.WriteVInt(13); // какая то залупа которая первее IntValueEntries
            {
                encoder.WriteInt(6);
                encoder.WriteInt(0); // демо аккаунт

                encoder.WriteInt(18);
                encoder.WriteInt(0); // кнопка киберспорт (v33)

                encoder.WriteInt(9);
                encoder.WriteInt(0); // отображать старпойнты

                encoder.WriteInt(3);
                encoder.WriteInt(0); // бп токены получены

                encoder.WriteInt(4);
                encoder.WriteInt(0); // получены трофеи

                encoder.WriteInt(8);
                encoder.WriteInt(0); // получены старпойнты

                encoder.WriteInt(20);
                encoder.WriteInt(0); // получены гемы (v33?)

                encoder.WriteInt(14);
                encoder.WriteInt(0); // получено монет 

                encoder.WriteInt(10);
                encoder.WriteInt(0); // получено трофеев силовой гонки

                encoder.WriteInt(23);
                encoder.WriteInt(0); // получено клубных трофеев (v40)

                encoder.WriteInt(7);
                encoder.WriteInt(1); // визуальное отображение кнопки "не беспокоить"

                encoder.WriteInt(17);
                encoder.WriteInt(1); // визуальное отображение кнопки "заглушить текстовой чат"

                encoder.WriteInt(15);
                encoder.WriteInt(0); // экран выбора возраста (1 - показывать, 2 - социальные функции не ограничены, 3 - социальные фукнции ограничены)
            }

            encoder.WriteVInt(0);

            encoder.WriteVInt(1);
            {
                encoder.WriteVInt(15);
                encoder.WriteVInt(1);
                encoder.WriteVInt(0);
                encoder.WriteVInt(0);
                encoder.WriteByte(2);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteByte(1);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteVInt(0);
            }


            encoder.WriteBoolean(true); // Quests
            encoder.WriteVInt(0);
            encoder.WriteVInt(0);
            encoder.WriteVInt(0);
            encoder.WriteVInt(0);

            encoder.WriteBoolean(true);
            encoder.WriteVInt(UnlockedEmotes.Count + UnlockedThumbnails.Count); // Played game modes
            foreach (int Emote in UnlockedEmotes)
            {
                ByteStreamHelper.WriteDataReference(encoder, Emote);
                encoder.WriteVInt(0);
            }
            foreach (int Thumbnail in UnlockedThumbnails)
            {
                ByteStreamHelper.WriteDataReference(encoder, Thumbnail);
                encoder.WriteVInt(0);
            }


            encoder.WriteBoolean(false); // Power League Season Data
            encoder.WriteInt(164);
        }


        public void LogicConfData(ByteStream encoder, DateTime utcNow)
        {
            encoder.WriteVInt(-15789);

            encoder.WriteVInt(32); // Played game modes
            for (int i = 0; i < 32; i++)
            {
                encoder.WriteVInt(i);
            }

            encoder.WriteVInt(Events.Length);
            foreach (EventData e in Events)
            {
                e.Encode(encoder);
            }

            encoder.WriteVInt(0); // Comming Events

            encoder.WriteVInt(10); //Brawler Upgrade Cost
            {
                encoder.WriteVInt(20); // 2 Level
                encoder.WriteVInt(35); // 3 Level
                encoder.WriteVInt(75); // 4 Level
                encoder.WriteVInt(140); // 5 Level
                encoder.WriteVInt(290); // 6 Level
                encoder.WriteVInt(480); // 7 Level
                encoder.WriteVInt(800); // 8 Level
                encoder.WriteVInt(1250); // 9 Level
                encoder.WriteVInt(1875); // 10 Level
                encoder.WriteVInt(2800); // 11 Level
            }


            encoder.WriteVInt(4); //Shop Coins Price
            {
                encoder.WriteVInt(20);
                encoder.WriteVInt(50);
                encoder.WriteVInt(140);
                encoder.WriteVInt(280);
            }

            encoder.WriteVInt(4); //Shop Coins Count
            {
                encoder.WriteVInt(150); // Count
                encoder.WriteVInt(400); // Count
                encoder.WriteVInt(1200); // Count
                encoder.WriteVInt(2600); // Count  
            }


            encoder.WriteVInt(0); // Locked For Chronos

            encoder.WriteVInt(19); // IntValueEntries (значение тут указывает на колво функций в блоке)
            {
                // --- настройки ui и т.д ---
                encoder.WriteInt(1);
                encoder.WriteInt(GlobalId.CreateGlobalId(41, 59)); // тема в лобби

                encoder.WriteInt(37);
                encoder.WriteInt(0); // убрать кнопку с бп (1 - кнопка пропадет с лобби)

                encoder.WriteInt(16);
                encoder.WriteInt(1); // убрать вкладку видео в новостях (1 - вкладка пропадает с новостей)

                encoder.WriteInt(46);
                encoder.WriteInt(1); // отключение подсказок для новичков (1 - отключает)

                encoder.WriteInt(5);
                encoder.WriteInt(0); // закрытие магазина (1 - закрывает магазин)

                encoder.WriteInt(6);
                encoder.WriteInt(0); // запретить ящики (не работает для v27+ потому что добавили батл пасс) (1 - запрещает ящики)

                encoder.WriteInt(15);
                encoder.WriteInt(0); // убрать коды автора (1 - убирает)

                // --- шоп ---
                encoder.WriteInt(29);
                encoder.WriteInt(21); // баннер скинов в шопе

                // --- визуал в батле ---
                encoder.WriteInt(17);
                encoder.WriteInt(1); // активирует дизайн банок лунного нового года

                // --- ивенты ---
                encoder.WriteInt(14);
                encoder.WriteInt(0); // врубить ивент удвоение жетонов (1 - включает)

                encoder.WriteInt(31);
                encoder.WriteInt(1); // врубить ивент лавина монет (1 - включает)

                encoder.WriteInt(10046);
                encoder.WriteInt(1);  // клубная лига (1 - включает)

                // --- Brawl Pass ---
                encoder.WriteInt(41);
                encoder.WriteInt(228); // цена прохождения уровня бп

                encoder.WriteInt(38);
                encoder.WriteInt(199); // цена обычного бп

                encoder.WriteInt(40);
                encoder.WriteInt(279); // цена бп+

                encoder.WriteInt(39);
                encoder.WriteInt(228); // цена бонусного ящика в бп

                encoder.WriteInt(1100);
                encoder.WriteInt(228); // цена квеста для 1 заполнителя квеста

                encoder.WriteInt(1003);
                encoder.WriteInt(1); // делает 1 заполнитель квестом бравл пасс

                encoder.WriteInt(50);
                encoder.WriteInt(5); // колво заполнителей для будущих квестов
            }

            encoder.WriteVInt(0); // Timed Int Entry Count
            encoder.WriteVInt(0); // Custom Event

            encoder.WriteVInt(0);
            encoder.WriteVInt(0);
            encoder.WriteVInt(0);

            encoder.WriteVInt(6); //Brawler Cost Gems
            {
                encoder.WriteVInt(0);
                encoder.WriteVInt(29);
                encoder.WriteVInt(79);
                encoder.WriteVInt(169);
                encoder.WriteVInt(349);
                encoder.WriteVInt(699);
            }


            encoder.WriteVInt(6); //Chromatic Brawler Cost Chromacredits
            {
                encoder.WriteVInt(0);
                encoder.WriteVInt(160);
                encoder.WriteVInt(450);
                encoder.WriteVInt(500);
                encoder.WriteVInt(1500);
                encoder.WriteVInt(4500);
            }

        }

        public void Encode(ByteStream encoder)
        {
            DateTime utcNow = DateTime.UtcNow;
            LogicDailyData(encoder, utcNow);
            LogicConfData(encoder, utcNow);
            encoder.WriteLong(HomeId); // хз дичь какая то
            encoder.WriteVInt(0);
            encoder.WriteVInt(-64);
            encoder.WriteBoolean(false);
            encoder.WriteVInt(0); // Gatcha Drop
            encoder.WriteVInt(0);
            encoder.WriteVInt(0);
            encoder.WriteBoolean(false);
            encoder.WriteVInt(0); // New Function V46
            encoder.WriteBoolean(false);
        }

    }
}
