namespace Supercell.Laser.Logic.Command.Home
{
    using Supercell.Laser.Logic.Helper;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Titan.DataStream;

    public class LogicPurchaseOfferCommand : Command
    {
        public int OfferIndex;
        public int DataGlobalId;
        public int Unknown;
        public int Currency;

        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);

            OfferIndex = stream.ReadVInt();
            DataGlobalId = ByteStreamHelper.ReadDataReference(stream);
            Unknown = ByteStreamHelper.ReadDataReference(stream);
            Currency = stream.ReadVInt();
        }

        public override int Execute(HomeMode homeMode)
        {
            homeMode.Home.PurchaseOffer(OfferIndex);
            return 0;
        }

        public override int GetCommandType()
        {
            return 519;
        }
    }
}
