using System.Diagnostics;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TileSample
{

    [ApiVersion(2, 1)]
    public class TileSample : TerrariaPlugin
    {
        public TileSample(Main game) : base(game)
        {
        }

        private const string DATA_KEY = nameof(TileSample), SHOW_COMMAND = "show", GET_COMMAND = "get";

        internal static readonly Dictionary<int, Item> ALL_ITEMS = new Dictionary<int, Item>();

        public override string Name => nameof(TileSample);

        public override Version Version => new Version(1, 0);

        public override string Author => "RaVen";

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
            GetDataHandlers.TileEdit += OnTileEdit;
            Commands.ChatCommands.Add(new Command("tilesample.use", GetSample, "sample"));
#if DEBUG 
            Commands.ChatCommands.Add(new Command("tilesample.admin", args => 
            {
                Debugger.Launch();
            }, "debug"));
#endif
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void OnGamePostInitialize(EventArgs args)
        {
            for (int i = 1; i < ItemID.Count; i++)
            {
                Item item = new Item();
                item.SetDefaults(i);
                ALL_ITEMS.Add(i, item);
            }
        }

        private void OnTileEdit(object? sender, TShockAPI.GetDataHandlers.TileEditEventArgs args)
        {
            if (args.Handled || !args.Player.ContainsData(DATA_KEY))
                return;

            TileItemInfo tileInfo = GetTileItemInfo(Main.tile[args.X, args.Y]);

            switch (args.Player.GetData<string>(DATA_KEY))
            {
                case SHOW_COMMAND:
                    tileInfo.Show(args.Player);
                    break;
                case GET_COMMAND:
                    tileInfo.Give(args.Player);
                    break;
                default:
                    TShock.Log.ConsoleError("Unimplemented command type.");
                    return;
            }

            args.Player.SendTileSquareCentered(args.X, args.Y, 3);
            args.Player.RemoveData(DATA_KEY);
            args.Handled = true;
        }

        private void GetSample(CommandArgs args)
        {
            string type;

            switch (args.Parameters.FirstOrDefault()?.ToLower())
            {
                case "s":
                case SHOW_COMMAND:
                case null:
                    type = SHOW_COMMAND;
                    break;
                case "g":
                case GET_COMMAND:
                    type = GET_COMMAND;
                    break;
                default:
                    args.Player.SendErrorMessage("/sample [show/get]");
                    return;
            }
            args.Player.SetData(DATA_KEY, type);
            args.Player.SendInfoMessage($"Edit some block to {type} items");
        }

        public static TileItemInfo GetTileItemInfo(ITile tile)
        {
            int tileType = -1, tileStyle = -1, wall = -1, liquid = -1;
            string slope = null!;
            List<int> wiring = new List<int>(), additionalItems = new List<int>();
            int tilePaint = -1, wallPaint = -1, tileCoating = -1, wallCoating = -1;
            bool isActuated = false;

            byte tileColor = tile.color(), wallColor = tile.wallColor();
            bool tileActive = tile.active(), wallActive = tile.wall > 0;

            foreach (Item item in ALL_ITEMS.Values)
            {
                if (tileActive)
                {
                    if (item.createTile == tile.type)
                        tileType = item.netID;
                    else if (tileColor > 0 && item.paint == tileColor)
                        tilePaint = item.netID;
                }
                if (wallActive)
                {
                    if (item.createWall == tile.wall)
                        wall = item.netID;
                    else if (wallColor > 0 && item.paint == wallColor)
                        wallPaint = item.netID;
                }
            }

            if (tile.liquid > 0)
                switch ((short)tile.liquidType())
                {
                    case LiquidID.Water: liquid = ItemID.BottomlessBucket; break;
                    case LiquidID.Lava: liquid = ItemID.BottomlessLavaBucket; break;
                    case LiquidID.Honey: liquid = ItemID.BottomlessHoneyBucket; break;
                    case LiquidID.Shimmer: liquid = ItemID.BottomlessShimmerBucket; break;
                }

            if (tileActive)
            {
                switch (tile.slope())
                {
                    case 1: slope = "◣"; break;
                    case 2: slope = "◢"; break;
                    case 3: slope = "◤"; break;
                    case 4: slope = "◥"; break;
                }
                if (tile.halfBrick())
                    slope = "▃";
            }

            if (tile.wire())
                wiring.Add(ItemID.Wrench);
            if (tile.wire2())
                wiring.Add(ItemID.BlueWrench);
            if (tile.wire3())
                wiring.Add(ItemID.GreenWrench);
            if (tile.wire4())
                wiring.Add(ItemID.YellowWrench);
            if (wiring.Any())
                additionalItems.Add(ItemID.Wire);

            if (tileActive && tile.inActive())
            {
                isActuated = true;
                additionalItems.Add(ItemID.Actuator);
            }
            
            if (tileActive)
            {
                if (tile.BlockColorAndCoating().Invisible)
                    tileCoating = ItemID.EchoCoating;
                if (tile.BlockColorAndCoating().FullBright)
                    tileCoating = ItemID.GlowPaint;
            }
            if (wallActive)
            {
                if (tile.WallColorAndCoating().Invisible)
                    wallCoating = ItemID.EchoCoating;
                if (tile.WallColorAndCoating().FullBright)
                    wallCoating = ItemID.GlowPaint;
            }
            
            return new TileItemInfo(tileType, tileStyle, wall, liquid, slope, isActuated, wiring, new PaintItemInfo(tilePaint, wallPaint, tileCoating, wallCoating), additionalItems);
        }
    }
}