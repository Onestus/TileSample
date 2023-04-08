using Microsoft.Xna.Framework;
using TShockAPI;

namespace TileSample
{
    public class TileItemInfo
    {
        private static readonly Color PrimaryColor = Color.LawnGreen, SecondaryColor = Color.ForestGreen;

        public int TileType, TileStyle, Wall, Liquid;
        public string Slope;
        public bool IsActuated;
        public List<int> Wiring, AdditionalItems;
        public PaintItemInfo Paint;

        public TileItemInfo(int tileType, int tileStyle, int wall, int liquid, string slope, bool isActuated, List<int> wiring, PaintItemInfo paint, List<int> additionalItems)
        {
            TileType = tileType;
            TileStyle = tileStyle;
            Wall = wall;
            Liquid = liquid;
            Slope = slope;
            IsActuated = isActuated;
            Wiring = wiring;
            Paint = paint;
            AdditionalItems = additionalItems;
        }

        public void Show(TSPlayer player)
        {
            List<string> items = new List<string>();
            bool color = false;
            Color GetRowColor() => (color = !color) ? PrimaryColor : SecondaryColor;

            if (TileType >= 0)
                items.Add(ConvertIDToTag(TileType));
            if (Paint.TilePaint >= 0)
                items.Add(ConvertIDToTag(Paint.TilePaint));
            if (Paint.TileCoating >= 0)
                items.Add(ConvertIDToTag(Paint.TileCoating));
            if (items.Any())
                HandleShowItems(player, "Blocks", items, GetRowColor());

            if (Wall >= 0)
                items.Add(ConvertIDToTag(Wall));
            if (Paint.WallPaint >= 0)
                items.Add(ConvertIDToTag(Paint.WallPaint));
            if (Paint.WallCoating >= 0)
                items.Add(ConvertIDToTag(Paint.WallCoating));
            if (items.Any())
                HandleShowItems(player, "Walls", items, GetRowColor());

            if (Wiring.Any())
                HandleShowItems(player, "Wiring", Wiring.Select(w => ConvertIDToTag(w)).ToList(), GetRowColor());

            player.SendMessage($"Actuated: {IsActuated.ToString().ToLower()}", GetRowColor());

            if (Slope is not null)
                player.SendMessage($"Slope: {Slope}", GetRowColor());

            if (Liquid >= 0)
                player.SendMessage($"Liquid: {ConvertIDToTag(Liquid)}", GetRowColor());
        }

        public void Give(TSPlayer player)
        {
            Show(player);

            int skippedItems = 0;

            if (TileType >= 0)
                skippedItems += player.GiveItemCheck(TileType, "", TileSample.ALL_ITEMS[TileType].maxStack) ? 0 : 1;
            if (TileStyle >= 0)
                skippedItems += player.GiveItemCheck(TileStyle, "", TileSample.ALL_ITEMS[TileStyle].maxStack) ? 0 : 1;
            if (Wall >= 0)
                skippedItems += player.GiveItemCheck(Wall, "", TileSample.ALL_ITEMS[Wall].maxStack) ? 0 : 1;
            if (Paint.TilePaint >= 0)
                skippedItems += player.GiveItemCheck(Paint.TilePaint, "", TileSample.ALL_ITEMS[Paint.TilePaint].maxStack) ? 0 : 1;
            if (Paint.TileCoating >= 0)
                skippedItems += player.GiveItemCheck(Paint.TileCoating, "", TileSample.ALL_ITEMS[Paint.TileCoating].maxStack) ? 0 : 1;
            if (Paint.WallPaint >= 0)
                skippedItems += player.GiveItemCheck(Paint.WallPaint, "", TileSample.ALL_ITEMS[Paint.WallPaint].maxStack) ? 0 : 1;
            if (Paint.WallCoating >= 0)
                skippedItems += player.GiveItemCheck(Paint.WallCoating, "", TileSample.ALL_ITEMS[Paint.WallCoating].maxStack) ? 0 : 1;
            foreach (int item in AdditionalItems)
                skippedItems += player.GiveItemCheck(item, "", TileSample.ALL_ITEMS[item].maxStack) ? 0 : 1;

            if (skippedItems > 0)
                player.SendErrorMessage($"{skippedItems} item{(skippedItems == 1 ? " was" : "s were")} not given");
                
        }

        private void HandleShowItems(TSPlayer player, string category, List<string> items, Color color)
        {
            player.SendMessage($"{category}: {string.Join(", ", items)}", color);
            items.Clear();
        }

        private string ConvertIDToTag(int id) => $"[i:{id}]";
    }
}