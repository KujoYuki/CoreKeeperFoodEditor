using CKFoodMaker.Model;
using CKFoodMaker.Resource;
using CKFoodMaker;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace CKFoodMakerTest
{
    [TestClass]
    public class AnalyzeTest
    {
        [TestMethod]
        public void AnalyzeReipeTest()
        {
            string winUserName = Environment.UserName;
            string id = "449703638";
            int saveDataSlotNo = 0;
            string SaveDataPath = $@"C:\Users\{winUserName}\AppData\LocalLow\Pugstorm\Core Keeper\Steam\{id}\saves\{saveDataSlotNo.ToString()}.json";
            string saveDataContents = File.ReadAllText(SaveDataPath);
            // conditionsList中のInfinity文字列により例外が出るのを回避する
            saveDataContents = SaveDataManager.SanitizeJsonString(saveDataContents);

            JsonObject _saveData = JsonNode.Parse(saveDataContents)!.AsObject();


            // 料理のコモンとレアのカテゴリIDリスト
            List<int> allCookedCategoryId = StaticResource.AllCookedBaseCategories
                .SelectMany(c => new[] { c.Info.objectID, c.Info.objectID + (int)CookRarity.Rare })
                .OrderBy(id => id)
                .ToList();

            // レア化させる食材のIDリスト
            List<int> rarerizeMaterials = Enumerable.Range(8100, 11).Append(9733).ToList();

            // 全ての食材と料理の表示名を取得
            var foodMaterials = StaticResource.AllFoodMaterials
                .Concat(StaticResource.ObsoleteFoodMaterials)   // レシピには載らないが料理は作成できるため含める
                .Select(item => (objectID: item.Info.objectID, DisplayName: item.DisplayName))
                .ToList();

            var allFoodMaterials = StaticResource.AllCookedBaseCategories
                .Select(item =>
                {
                    return new[] { item.Info.objectID, item.Info.objectID + (int)CookRarity.Rare, item.Info.objectID + (int)CookRarity.Epic }
                    .Select(id => (objectID: id, DisplayName: item.DisplayName));
                })
                .SelectMany(food => food).ToList()
                .Concat(foodMaterials)
                .ToDictionary();

            // 全ての発見済みアイテムからレシピのみを抽出し、表示名を含めてcsv形式のレコード変換
            List<string> discoveredAllRecipe = _saveData["discoveredObjects2"]!.AsArray()
                .Select(obj => JsonSerializer.Deserialize<DiscoveredObjects>(obj)!)
                .Where(o => allCookedCategoryId.Contains(o.objectID))   // 非料理アイテムは除外
                .Where(o => o.variation > 0 && (uint)o.variation <= uint.MaxValue)   // variationが0か32bitで表現できない場合は除外
                .OrderBy(o => o.variation)
                .ThenBy(o => o.objectID)

                // 料理の鉄人でレア化されたレシピが含まれている場合は除外
                // 各グループの最初の要素のみを選択（脱法料理とレアしか知らない料理を除外できないことに注意）
                .GroupBy(o => o.variation)
                .Select(g => g.First())

                // 食材に対しての検証
                .Where(r =>
                {
                    Form1.ReverseCalcurateVariation(r.variation, out int materialA, out int materialB);
                    int[] materials = [materialA, materialB];
                    foreach (var material in materials)
                    {
                        // 食材じゃないものが食材の場合を除外（シーズンアイテム系など）
                        if (!allFoodMaterials.ContainsKey(material))
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .Select(r =>
                {
                    // csv用レコードに変換
                    Form1.ReverseCalcurateVariation(r.variation, out int materialA, out int materialB);
                    return (materialA, materialB, r.objectID, materialNameA:allFoodMaterials[materialA], materialNameB:allFoodMaterials[materialB], recepeCategoryName:allFoodMaterials[r.objectID]);
                })
                .OrderBy(r => r.materialA)
                .ThenBy(r => r.materialB)
                .Select(r => string.Join(',', r.materialA, r.materialB, r.objectID, r.materialNameA, r.materialNameB, r.recepeCategoryName))
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("食材A_Id,食材B_Id,料理_Id,食材A_名前,食材B_名前,料理名");
            foreach (var item in discoveredAllRecipe)
            {
                sb.AppendLine(item);
            }
            string currentPath = Path.Combine(Directory.GetCurrentDirectory(), "analyzedRecipe.csv")!;
            File.WriteAllText(currentPath, sb.ToString());
        }
    }
}
