using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CKFoodMaker.Model;
using CKFoodMaker.Model.ItemAux;
using CKFoodMaker.Resource;

namespace CKFoodMaker
{
    /// <summary>
    /// 単一のセーブデータに対する読み書きモジュール
    /// </summary>
    public sealed class SaveDataManager
    {
        public static int LoadItemLimit;

        private static SaveDataManager? _singletonInstance;

        public static SaveDataManager GetInstance() => _singletonInstance ??= new();

        private string _saveDataPath = string.Empty;
        public string SaveDataPath
        {
            get
            {
                return _saveDataPath;
            }
            set
            {
                _saveDataPath = value;
                LoadItemLimit = Program.IsDeveloper ? 86 : 50;
                _saveData = LoadInventory(out var items);
                Items = items;
            }
        }

        public List<(ItemBase item, string objectName, ItemAuxData auxData)> Items { get; private set; } = [];
        private JsonObject _saveData = [];

        private SaveDataManager()
        {
        }

        private static string SanitizeJsonString(string origin)
        {
            return origin.Replace("Infinity", "\"Infinity\"");
        }

        private static string RestoreJsonString(string processed)
        {
            return processed.Replace("\"Infinity\"", "Infinity");
        }

        private JsonObject LoadInventory(out List<(ItemBase item, string objectName, ItemAuxData auxData)> items)
        {

            string saveDataContents = File.ReadAllText(SaveDataPath);
            // conditionsList中のInfinity文字列により例外が出るのを回避する
            saveDataContents = SanitizeJsonString(saveDataContents);

            _saveData = JsonNode.Parse(saveDataContents)!.AsObject();

            var inventoryBase = _saveData["inventory"]!.AsArray();
            var inventoryName = _saveData["inventoryObjectNames"]!.AsArray();
            var inventoryAuxData = _saveData["inventoryAuxData"]!.AsArray();

            var limitedItems = inventoryBase
                .Zip(inventoryName, inventoryAuxData)
                .Take(LoadItemLimit)
                .Select(x => (item: x.First!, objectName: x.Second!, auxData: x.Third!));
            items = [];
            foreach (var (item, objectName, auxData) in limitedItems)
            {
                var itemBase = new ItemBase(
                    objectID: item["objectID"]!.GetValue<int>(),
                    amount: item["amount"]!.GetValue<int>(),
                    variation: item["variation"]!.GetValue<int>(),
                    variationUpdateCount: item["variationUpdateCount"]!.GetValue<int>());
                string objectInternalName = objectName!.GetValue<string>()!;
                var itemAux = new ItemAuxData(auxData["index"]!.GetValue<int>(), auxData["data"]!.GetValue<string>());
                items.Add((itemBase, objectInternalName, itemAux));
            }

            return _saveData;
        }

        public ItemAuxData GetAuxData(int insertIndex)
        {
            var auxData = _saveData["inventoryAuxData"]![insertIndex]!;
            return new ItemAuxData(auxData["index"]!.GetValue<int>(), auxData["data"]!.GetValue<string>());
        }

        // 補助データ込みの書き込みメソッド
        public bool WriteItemData(int insertIndex, ItemBase itemBase, string objectName, ItemAuxData? auxData = null)
        {
            auxData ??= ItemAuxData.Default;
            var success = false;
            _saveData["inventory"]![insertIndex] = JsonNode.Parse(JsonSerializer.Serialize(itemBase, StaticResource.SerializerOption));
            _saveData["inventoryObjectNames"]![insertIndex] = objectName;
            _saveData["inventoryAuxData"]![insertIndex] = JsonNode.Parse(JsonSerializer.Serialize(auxData, StaticResource.SerializerOption));

            string changedJson = JsonSerializer.Serialize(_saveData, StaticResource.SerializerOption);

#if DEBUG
            // 確認用に別名ファイルで作成
            var verifyBuilder = new StringBuilder();
            verifyBuilder.AppendLine($"insertIndex = {insertIndex}");
            verifyBuilder.AppendLine($"objectName = {objectName}");
            verifyBuilder.AppendLine($"itemBase = {JsonSerializer.Serialize(itemBase, StaticResource.SerializerOption)}");
            if (auxData != ItemAuxData.Default)
            {
                verifyBuilder.AppendLine($"auxData = {JsonSerializer.Serialize(auxData, StaticResource.SerializerOption)}");
            }
            MessageBox.Show($"{verifyBuilder}", "書き込み内容確認");
            SaveDataPath = Path.Combine(Path.GetDirectoryName(SaveDataPath)!, "debug.json");
#endif

            // 書き込む前に元jsonの構文に戻す
            changedJson = RestoreJsonString(changedJson);

            File.WriteAllText(SaveDataPath, changedJson);
            success = true;
            return success;
        }

        public bool IsClearData()
        {
            if (_saveData["hasUnlockedSouls"]?.GetValue<bool>() is true &&
                _saveData["collectedSouls"]?.AsArray().Count is 6 &&
                _saveData["hasPlayedOutro"]?.GetValue<bool>() is true)
            {
                return true;
            }
            return false;
        }

        // IncreasedMaxHealthPermanentの増分検知
        public bool HasOveredHealth(out int increasedHealth)
        {
            int? increasedHealthNullable = _saveData["conditionsList"]?.AsArray()
                .Select(x => JsonSerializer.Deserialize<Condition>(x, StaticResource.SerializerOption))
                .SingleOrDefault(c => c?.Id == 16)?.Value;
            if (increasedHealthNullable is null)
            {
                increasedHealth = 0;
                return false;
            }
            increasedHealth = increasedHealthNullable.Value;
            if (increasedHealth < 2000) return false;

            return true;
        }

        public void UnlockAllRecipe()
        {
            MessageBox.Show("未作成の料理の組み合わせを出力します。");

            var allCookedCategoryId = StaticResource.AllCookedBaseCategories
                .SelectMany(c => new[] { c.objectID, c.objectID + (int)CookRarity.Rare })
                .OrderBy(id => id)
                .ToList();
            List<int> allFoodID = StaticResource.AllFoodMaterials.Select(c => c.objectID).ToList();
            List<int> allVariations = allFoodID
                .SelectMany((ID, index) => allFoodID.Skip(index), (IdA, IdB) => Form1.CalculateVariation(IdA, IdB))
                .ToList();

            var allDiscoverdVariation = _saveData["discoveredObjects2"]!.AsArray()
                .Select(obj => JsonSerializer.Deserialize<DiscoveredObjects>(obj)!)
                .Where(obj => allCookedCategoryId.Contains(obj.objectID))
                .Select(c => c!.variation)
                .Distinct()
                .ToList();
            var intersectRecipeVariation = allVariations.Intersect(allDiscoverdVariation).ToList();
            double cookRate = (double)intersectRecipeVariation.Count / allVariations.Count;
            var outputResult = MessageBox.Show($"現在の正規レシピ網羅率は {cookRate:P2} %です。" +
                $"（{intersectRecipeVariation.Count} / {allVariations.Count}）\n" +
                $"※ゲーム内レシピブックとカウントが異なる場合があります。\n\n" +
                $"一度も調理していない組み合わせを出力しますか？", "", MessageBoxButtons.YesNo);
            if (outputResult is DialogResult.Yes)
            {
                var exceptRecipe = allVariations.Except(allDiscoverdVariation).ToArray();
                var foodBuilder = new StringBuilder();
                foreach (var variation in exceptRecipe)
                {
                    Form1.ReverseCalcurateVariation(variation, out int materialIdA, out int materialIdB);
                    string FoodA = StaticResource.AllFoodMaterials.Single(f => f.objectID == materialIdA).DisplayName;
                    string FoodB = StaticResource.AllFoodMaterials.Single(f => f.objectID == materialIdB).DisplayName;
                    foodBuilder.AppendLine(FoodA + " + " + FoodB);
                }
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"UncreatedRecipe.txt");
                File.WriteAllText(path, foodBuilder.ToString());
                MessageBox.Show($"{exceptRecipe.Length} 通りのレシピを出力しました。\n{path}");

            }

            //todo 全レシピ追加
            //全組み合わせの勝敗表からカテゴリを自動で決定させる:要食材勝敗テーブルの解明
        }

        public void DeleteAllRecipe()
        {
            var allCookedCategoryId = StaticResource.AllCookedBaseCategories
                .SelectMany(c => new[] { c.objectID, c.objectID + (int)CookRarity.Rare + (int)CookRarity.Epic })
                .OrderBy(id => id)
                .ToList();
            var discoveredObjectWithoutRecipe = _saveData["discoveredObjects2"]!.AsArray()
                .Select(obj => JsonSerializer.Deserialize<DiscoveredObjects>(obj)!)
                .Where(obj => !allCookedCategoryId.Contains(obj.objectID))
                .ToList();
            _saveData["discoveredObjects2"] = JsonNode.Parse(JsonSerializer.Serialize(discoveredObjectWithoutRecipe, StaticResource.SerializerOption));
            // データ書き込み
            string changedJson = JsonSerializer.Serialize(_saveData, StaticResource.SerializerOption);
            changedJson = RestoreJsonString(changedJson);
            File.WriteAllText(SaveDataPath, changedJson);
        }

        public List<Condition> GetConditions()
        {
            var conditions = _saveData["conditionsList"]?.AsArray()
                .Select(c => JsonSerializer.Deserialize<Condition>(c, StaticResource.SerializerOption)!)
                .ToList()!;
            return conditions;
        }

        public void BackUpConditions(IEnumerable<Condition> conditions, string filePath)
        {
            conditions = conditions.OrderBy(c => c.Id).ToList();
            //hack 例外処理

            var conditionsNode = JsonNode.Parse(JsonSerializer.Serialize(conditions, StaticResource.SerializerOption));
            string changedJson = JsonSerializer.Serialize(conditionsNode, StaticResource.SerializerOption);
            changedJson = RestoreJsonString(changedJson);
            File.WriteAllText(filePath, changedJson);
        }

        public List<Condition> LoadConditions(string fileName)
        {
            //hack 例外処理
            string jsonString = SanitizeJsonString(File.ReadAllText(fileName));
            var conditions = JsonSerializer.Deserialize<List<Condition>>(jsonString, StaticResource.SerializerOption)?
                .OrderBy(c => c.Id)
                .ToList();
            return conditions ??= [];
        }

        public void OverrideConditions(IEnumerable<Condition> conditions)
        {
            _saveData["conditionsList"] = JsonNode.Parse(JsonSerializer.Serialize(conditions, StaticResource.SerializerOption));
            string changedJson = JsonSerializer.Serialize(_saveData, StaticResource.SerializerOption);
            changedJson = RestoreJsonString(changedJson);
            File.WriteAllText(SaveDataPath, changedJson);
        }
    }
}
