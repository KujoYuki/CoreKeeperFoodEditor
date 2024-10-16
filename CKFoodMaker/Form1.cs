using System.Diagnostics;
using System.Text.RegularExpressions;
using CKFoodMaker.Model;
using CKFoodMaker.Model.ItemAux;
using CKFoodMaker.Resource;

namespace CKFoodMaker
{
    public partial class Form1 : Form
    {

        static readonly string _errorLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"ErrorStackTrace.txt");

        private SaveDataManager _saveDataManager = SaveDataManager.GetInstance();
        private List<InternalItemInfo> _materialCategories = [];
        private List<InternalItemInfo> _cookedCategories = [];

        private string _saveDataFolderPath = string.Empty;

        public string SaveDataFolderPath
        {
            get { return _saveDataFolderPath; }
            set
            {
                savePathTextBox.Text = value;
                _saveDataFolderPath = value;
                if (Directory.Exists(_saveDataFolderPath))
                {
                    EnabeleUI();
                }
                else
                {
                    DisabeleUI();
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            Initialize();
            if (Program.IsDeveloper)
            {
                Text += "(Dev)";
                variationUpdateCountTextBox.ReadOnly = false;
                auxDataTextBox.ReadOnly = false;
                toMinusOneButton.Visible = true;
            }
        }

        public void Initialize()
        {
            try
            {
                SetMaterialCategory();
                SetCookedCategory();
                SetPetTalentCategory();
                rarityComboBox.SelectedIndex = 0;

                InitilizeFolderPath();
                if (Directory.Exists(SaveDataFolderPath))
                {
                    LoadSlots();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "初期化処理に失敗しました。");
                File.AppendAllText(_errorLogFilePath, DateTime.Now + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        private void SetMaterialCategory()
        {
            // hack StaticResource.AllFoodMaterialsからの読み込みを使い、csvからは本来調理しない追加料理だけを読み込むようにする
            string materialCatergoryDefineFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resource", "MaterialCategory.csv")
                ?? throw new FileNotFoundException($"MaterialCategory.csvが見つかりません。");
            var materialCategories = File.ReadAllLines(materialCatergoryDefineFilePath).ToArray();
            foreach (var line in materialCategories)
            {
                string[] words = line.Split(',');
                if (!Enum.TryParse(words[3], out MaterialSubCategory subCategory))
                {
                    throw new ArgumentException($"無効なMaterialSubCategory:{words[3]}");
                }

                _materialCategories.Add(new(objectID: int.Parse(words[0]),
                                          internalName: words[1],
                                          displayName: words[2],
                                          subCategory: subCategory));
            }

            //hack 視認性向上のサブカテゴリネームを追加するか検討
            var sortedMaterialCategories = _materialCategories.OrderBy(c => c.SubCategory).Select(c => c.DisplayName).ToArray();
            materialComboBoxA.Items.AddRange(sortedMaterialCategories);
            materialComboBoxB.Items.AddRange(sortedMaterialCategories);

            materialComboBoxA.SelectedIndex = 0;
            materialComboBoxB.SelectedIndex = 0;
        }

        private void SetCookedCategory()
        {
            foreach (var category in StaticResource.AllCookedBaseCategories)
            {
                _cookedCategories.Add(new(category.objectID, category.InternalName, category.JapaneseResource));
                cookedCategoryComboBox.Items.Add(category.JapaneseResource);
            }
            cookedCategoryComboBox.SelectedIndex = 0;
        }

        private void SetPetTalentCategory()
        {
            var petKinds = Enum.GetNames<PetType>();
            petKindComboBox.Items.AddRange(petKinds);
            var petColors = Enum.GetNames<PetColor>();
            petColorComboBox.Items.AddRange(petColors);

            //hack StaticResource化
            string TalentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resource", "PetTalent.csv");
            var TalentIds = File.ReadAllLines(TalentFilePath)
                .Select(line => line.Split(",").Take(2))
                .Select(info => string.Join(":", info))
                .ToArray();    // Idと効果確認だけの暫定処理
            petTalent1ComboBox.Items.AddRange(TalentIds);
            petTalent2ComboBox.Items.AddRange(TalentIds);
            petTalent3ComboBox.Items.AddRange(TalentIds);
            petTalent4ComboBox.Items.AddRange(TalentIds);
            petTalent5ComboBox.Items.AddRange(TalentIds);
            petTalent6ComboBox.Items.AddRange(TalentIds);
            petTalent7ComboBox.Items.AddRange(TalentIds);
            petTalent8ComboBox.Items.AddRange(TalentIds);
            petTalent9ComboBox.Items.AddRange(TalentIds);
        }

        private void InitilizeFolderPath()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastSaveFolderPath))
            {
                SaveDataFolderPath = Properties.Settings.Default.LastSaveFolderPath;
            }
            else
            {
                string appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))!.FullName;
                string generalPath = Path.Combine(appDataPath, @"LocalLow\Pugstorm\Core Keeper\Steam");
                if (Directory.GetDirectories(generalPath).Length is 0)
                {
                    SaveDataFolderPath = generalPath;
                    return;
                }
                SaveDataFolderPath = Path.Combine(Directory.GetDirectories(generalPath).SingleOrDefault()!, "saves");
                if (!Directory.Exists(SaveDataFolderPath))
                {
                    SaveDataFolderPath = appDataPath;
                }
            }
        }

        private void LoadSlots()
        {
            saveSlotNoComboBox.Items.Clear();
            // セーブデータ一覧の取得
            Regex regex = new(@"^\d{1,2}|debug$", RegexOptions.Compiled);
            string[] savePaths = new DirectoryInfo(SaveDataFolderPath).GetFiles(@"*.json")
                .Where(file => regex.IsMatch(Path.GetFileNameWithoutExtension(file.Name)))
                .Select(fileInfo => fileInfo.FullName).ToArray();
            if (savePaths.Length is 0)
            {
                DisabeleUI();
                return;
            }
            else
            {
                EnabeleUI();
            }

            foreach (string savePath in savePaths)
            {
                var saveNo = Path.GetFileNameWithoutExtension(savePath);
                saveSlotNoComboBox.Items.Add(saveNo);
            }
            if (saveSlotNoComboBox.Items.Count > 0)
            {
                saveSlotNoComboBox.SelectedItem = saveSlotNoComboBox.Items[0];
            }

            LoadItems();
        }

        private void LoadItems()
        {
            // リロード時のindex保持とクリア
            int selectedInventoryIndex = 0;
            if (inventoryIndexComboBox.Items.Count > 0)
            {
                selectedInventoryIndex = inventoryIndexComboBox.SelectedIndex;
            }
            inventoryIndexComboBox.Items.Clear();

            // 選択されたセーブデータのファイルのアイテム読み込み
            string selecetedSaveDataPath = Path.Combine(SaveDataFolderPath, saveSlotNoComboBox.SelectedItem?.ToString() + ".json");
            _saveDataManager.SaveDataPath = selecetedSaveDataPath;

            // 選択中のセーブデータのアイテム情報をinventoryIndexComboBoxに反映する
            int indexNo = 1;
            foreach (var (item, objectName, auxData) in _saveDataManager.Items)
            {
                if (item == ItemBase.Default)
                {
                    inventoryIndexComboBox.Items.Add($"{indexNo,2} : ----");
                }
                else
                {
                    inventoryIndexComboBox.Items.Add($"{indexNo,2} : {objectName}");
                }
                indexNo++;
            }
            inventoryIndexComboBox.SelectedIndex = selectedInventoryIndex;

            LoadPanel();
        }

        private void LoadPanel()
        {
            var selectedItem = _saveDataManager.Items[inventoryIndexComboBox.SelectedIndex];
            int selectedObjectID = selectedItem.item.objectID;
            int variation = selectedItem.item.variation;

            objectIdTextBox.Text = selectedObjectID.ToString();
            amoutTextBox.Text = selectedItem.item.amount.ToString();
            variationTextBox.Text = variation.ToString();
            variationUpdateCountTextBox.Text = selectedItem.item.variationUpdateCount.ToString();
            objectNameTextBox.Text = selectedItem.objectName;
            auxIndexTextBox.Text = selectedItem.auxData.index.ToString();
            auxDataTextBox.Text = selectedItem.auxData.data;

            // 料理の場合は料理情報をセットする
            if (IsCookedItem(selectedObjectID, out var rarity, out var indexBaseOffset))
            {
                ReverseCalcurateVariation(variation, out var materialIDA, out var materialIDB);
                materialComboBoxA.SelectedItem = _materialCategories.SingleOrDefault(c => c.ObjectID == materialIDA)?.DisplayName;
                materialComboBoxB.SelectedItem = _materialCategories.SingleOrDefault(c => c.ObjectID == materialIDB)?.DisplayName;

                cookedCategoryComboBox.SelectedIndex = indexBaseOffset;
                rarityComboBox.SelectedIndex = rarity switch
                {
                    CookRarity.Common => 0,
                    CookRarity.Rare => 1,
                    CookRarity.Epic => 2,
                    _ => throw new NotImplementedException(),
                };
                createdNumericNo.Value = selectedItem.item.amount;
            }
            else
            {
                ResetFoodTab();
            }

            IEnumerable<int> allPetIds = Enum.GetValues(typeof(PetType)).Cast<int>();
            if (allPetIds.Contains(selectedItem.item.objectID))
            {
                // ペットの場合はAuxDataをセットする
                InitLoadedPetTab(selectedItem.auxData, selectedItem.item);
            }
            else
            {
                ResetPetTab();
            }
        }

        private void saveSlotNoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadItems();
        }

        private void toMaxButton_Click(object sender, EventArgs e)
        {
            createdNumericNo.Value = 9999;
        }

        private void toMinusOneButton_Click(object sender, EventArgs e)
        {
            createdNumericNo.Value = -1;
        }

        private void openSevePathDialogButton_Click(object sender, EventArgs e)
        {
            string appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))!.FullName;
            string generalPath = Path.Combine(appDataPath, @"LocalLow\Pugstorm\Core Keeper\Steam");
            if (Directory.GetDirectories(generalPath).Length is 0)
            {
                // ユーザーIDフォルダ見つからない場合はSteamフォルダで留め置く
                saveFolderBrowserDialog.SelectedPath = generalPath;
            }
            else
            {
                saveFolderBrowserDialog.SelectedPath = Path.Combine(Directory.GetDirectories(generalPath).FirstOrDefault()!, "saves");
            }

            var result = saveFolderBrowserDialog.ShowDialog();
            if (result is DialogResult.OK)
            {
                SaveDataFolderPath = saveFolderBrowserDialog.SelectedPath;
            }
            if (Directory.Exists(SaveDataFolderPath))
            {
                LoadSlots();
            }
        }

        private void InitLoadedPetTab(ItemAuxData auxData, ItemBase item)
        {
            auxData.GetPetData(out var name, out var color, out var talents);
            petKindComboBox.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(PetType)).Cast<int>().ToArray(), item.objectID);
            petColorComboBox.SelectedIndex = color;
            petExpNumeric.Value = item.amount;
            petNameTextBox.Text = name;

            InitLoadedPetTalents(talents);
        }

        private void ResetPetTab()
        {
            petColorComboBox.SelectedIndex = -1;
            petExpNumeric.Value = 0;
            petNameTextBox.Text = string.Empty;
            petTalent1ComboBox.SelectedIndex = -1;
            petTalent2ComboBox.SelectedIndex = -1;
            petTalent3ComboBox.SelectedIndex = -1;
            petTalent4ComboBox.SelectedIndex = -1;
            petTalent5ComboBox.SelectedIndex = -1;
            petTalent6ComboBox.SelectedIndex = -1;
            petTalent7ComboBox.SelectedIndex = -1;
            petTalent8ComboBox.SelectedIndex = -1;
            petTalent9ComboBox.SelectedIndex = -1;
            petTalent1ValidCheckBox.Checked = false;
            petTalent2ValidCheckBox.Checked = false;
            petTalent3ValidCheckBox.Checked = false;
            petTalent4ValidCheckBox.Checked = false;
            petTalent5ValidCheckBox.Checked = false;
            petTalent6ValidCheckBox.Checked = false;
            petTalent7ValidCheckBox.Checked = false;
            petTalent8ValidCheckBox.Checked = false;
            petTalent9ValidCheckBox.Checked = false;
        }

        private void ResetFoodTab()
        {
            materialComboBoxA.SelectedIndex = -1;
            materialComboBoxB.SelectedIndex = -1;
            cookedCategoryComboBox.SelectedIndex = -1;
            rarityComboBox.SelectedIndex = -1;
        }

        private void InitLoadedPetTalents(List<PetTalent> talents)
        {
            petTalent1ValidCheckBox.Checked = talents[0].Points == 1;
            petTalent1ComboBox.SelectedIndex = talents[0].Talent;
            petTalent2ValidCheckBox.Checked = talents[1].Points == 1;
            petTalent2ComboBox.SelectedIndex = talents[1].Talent;
            petTalent3ValidCheckBox.Checked = talents[2].Points == 1;
            petTalent3ComboBox.SelectedIndex = talents[2].Talent;
            petTalent4ValidCheckBox.Checked = talents[3].Points == 1;
            petTalent4ComboBox.SelectedIndex = talents[3].Talent;
            petTalent5ValidCheckBox.Checked = talents[4].Points == 1;
            petTalent5ComboBox.SelectedIndex = talents[4].Talent;
            petTalent6ValidCheckBox.Checked = talents[5].Points == 1;
            petTalent6ComboBox.SelectedIndex = talents[5].Talent;
            petTalent7ValidCheckBox.Checked = talents[6].Points == 1;
            petTalent7ComboBox.SelectedIndex = talents[6].Talent;
            petTalent8ValidCheckBox.Checked = talents[7].Points == 1;
            petTalent8ComboBox.SelectedIndex = talents[7].Talent;
            petTalent9ValidCheckBox.Checked = talents[8].Points == 1;
            petTalent9ComboBox.SelectedIndex = talents[8].Talent;
        }

        private void inventoryIndexComboBox_TextChanged(object sender, EventArgs e)
        {
            LoadPanel();
        }

        private void objectIdsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://core-keeper.fandom.com/wiki/Object_IDs");
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (!Program.IsDeveloper)
            {
                if (!_saveDataManager.IsClearData())
                {
                    MessageBox.Show("クリア済みでない場合は機能を制限します。\n通常クリア後にお楽しみください。");
                    return;
                }
                if (_saveDataManager.HasOveredHealth(out int overedHealth))
                {
                    MessageBox.Show($"Code : {overedHealth}", "体力過剰", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            bool result = false;
            ItemBase item;
            string objectName;
            try
            {
                switch (itemEditTabControl.SelectedTab?.Name)
                {
                    case "foodTab":
                        int materialAId = _materialCategories
                        .Single(c => c.DisplayName == materialComboBoxA
                        .SelectedItem?.ToString()).ObjectID;
                        int materialBId = _materialCategories
                            .Single(c => c.DisplayName == materialComboBoxB
                            .SelectedItem?.ToString()).ObjectID;
                        int calculatedVariation = CalculateVariation(materialAId, materialBId);

                        // レア度反映
                        int baseObjectId = _cookedCategories.Single(c => c.DisplayName == cookedCategoryComboBox.SelectedItem!.ToString()).ObjectID;
                        DetermineCookedAttributes(baseObjectId, rarityComboBox.SelectedItem?.ToString()!, out int fixedObjectId, out objectName);
                        item = new(objectID: fixedObjectId,
                                   amount: Convert.ToInt32(createdNumericNo.Value),
                                   variation: calculatedVariation);
                        _saveDataManager.WriteItemData(inventoryIndexComboBox.SelectedIndex, item, objectName);
                        result = true;
                        break;

                    case "petTab":
                        if (!Enum.GetValues(typeof(PetType)).Cast<int>().Contains(int.Parse(objectIdTextBox.Text)))
                        {
                            MessageBox.Show("選択中のアイテムがペットではありません。\nインベントリ枠でペットアイテムを選択して編集してください。");
                            return;
                        }
                        var allPetTypes = (PetType[])Enum.GetValues(typeof(PetType));
                        var allPetColors = (PetColor[])Enum.GetValues(typeof(PetColor));
                        var auxData = _saveDataManager.GetAuxData(inventoryIndexComboBox.SelectedIndex);
                        auxData.AuxPrefabManager?.UpdatePet(
                            petNameTextBox.Text,
                            allPetColors[petColorComboBox.SelectedIndex],
                            GeneratePetTalentLists());
                        item = new(objectID: (int)allPetTypes[petKindComboBox.SelectedIndex],
                            amount: (int)petExpNumeric.Value,
                            variation: 0);
                        objectName = Enum.GetNames(typeof(PetType))[petKindComboBox.SelectedIndex];

                        // ItemAuxDataを込みで書きこむ
                        result = _saveDataManager.WriteItemData(inventoryIndexComboBox.SelectedIndex, item, objectName, auxData);
                        break;

                    case "advancedTab":
                        item = GenerateItemBase();
                        objectName = objectNameTextBox.Text;
                        result = _saveDataManager.WriteItemData(inventoryIndexComboBox.SelectedIndex, item, objectName);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                if (result)
                {
                    EnableResultMessage($"{objectName}を作成しました。");
                }
                // 書き換え後の再読み込み
                LoadItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "書き込み処理に失敗しました。");
                File.AppendAllText(_errorLogFilePath, DateTime.Now + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        private ItemBase GenerateItemBase()
        {
            if (amountConstCheckBox.Checked)
            {
                amoutTextBox.Text = amountConst.Value.ToString();
            }
            return new(objectID: objectIdTextBox.Text, amount: amoutTextBox.Text, variation: variationTextBox.Text);
        }

        private List<PetTalent> GeneratePetTalentLists()
        {
            List<PetTalent> talentList =
            [
                new(petTalent1ComboBox.SelectedIndex, petTalent1ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent2ComboBox.SelectedIndex, petTalent2ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent3ComboBox.SelectedIndex, petTalent3ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent4ComboBox.SelectedIndex, petTalent4ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent5ComboBox.SelectedIndex, petTalent5ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent6ComboBox.SelectedIndex, petTalent6ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent7ComboBox.SelectedIndex, petTalent7ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent8ComboBox.SelectedIndex, petTalent8ValidCheckBox.Checked is true ? 1 : 0),
                new(petTalent9ComboBox.SelectedIndex, petTalent9ValidCheckBox.Checked is true ? 1 : 0),
            ];

            return talentList;
        }

        private async void EnableResultMessage(string message)
        {
            resultLabel.Text = message;
            resultLabel.Visible = true;
            await Task.Delay(3000);
            resultLabel.Visible = false;
        }

        private static void DetermineCookedAttributes(int baseObjectId, string rarity, out int fixedObjectId, out string fixedInternalName)
        {
            fixedObjectId = rarity switch
            {
                "コモン" => baseObjectId,
                "レア" => baseObjectId + (int)CookRarity.Rare,
                "エピック" => baseObjectId + (int)CookRarity.Epic,
                _ => throw new ArgumentException(null, nameof(rarity)),
            };
            string baseInternalName = StaticResource.AllCookedBaseCategories.Single(c => c.objectID == baseObjectId).InternalName;
            fixedInternalName = rarity switch
            {
                "コモン" => baseInternalName,
                "レア" => baseInternalName + "Rare",
                "エピック" => baseInternalName + "Epic",
                _ => throw new ArgumentException(null, nameof(rarity))
            };
        }

        /// <summary>
        /// 決まった食材Idから合成後の料理のvariation値を計算する。
        /// </summary>
        /// <param name="IdA">1つめの食材のId(dec)</param>
        /// <param name="IdB">2つめの食材のId(dec)</param>
        /// <returns></returns>
        public static int CalculateVariation(int IdA, int IdB)
        {
            // ゲーム内動作に合わせて降順に入れ替え
            if (IdA < IdB) (IdA, IdB) = (IdB, IdA);
            // 各IDを16進数にし、1つの文字列としてつなげる。
            string combinedHex = IdA.ToString("X4") + IdB.ToString("X4");
            // 10進数に戻す
            var combinedDecimal = Convert.ToInt32(combinedHex, 16);
            return combinedDecimal;
        }

        private void SetDefaultButton_Click(object sender, EventArgs e)
        {
            objectIdTextBox.Text = ItemBase.Default.objectID.ToString();
            variationTextBox.Text = ItemBase.Default.variation.ToString();
            objectNameTextBox.Text = string.Empty;
            amoutTextBox.Text = ItemBase.Default.amount.ToString();
            variationUpdateCountTextBox.Text = ItemBase.Default.variationUpdateCount.ToString();
            auxIndexTextBox.Text = ItemAuxData.Default.index.ToString();
            auxDataTextBox.Text = ItemAuxData.Default.data.ToString();
        }

        /// <summary>
        /// variationからIdへの逆算
        /// </summary>
        /// <param name="variation"></param>
        /// <param name="materialA">材料の食材A</param>
        /// <param name="materialB">材料の食材B</param>
        public static void ReverseCalcurateVariation(int variation, out int materialA, out int materialB)
        {
            string combinedHex = variation.ToString("X8");
            string strA = combinedHex[..4];
            string strB = combinedHex[4..];
            materialA = Convert.ToInt32(strA, 16);
            materialB = Convert.ToInt32(strB, 16);
        }

        private static bool IsCookedItem(int objectID, out CookRarity rarity, out int indexBaseOffset)
        {
            int[] cookedCategoryAllIds = StaticResource.AllCookedBaseCategories
                .Select(c => c.objectID)
                .SelectMany(id => new[] { id, id + (int)CookRarity.Rare, id + (int)CookRarity.Epic })
                .OrderBy(id => id)
                .ToArray();
            int categorySize = StaticResource.AllCookedBaseCategories.Count;
            var commonIDs = cookedCategoryAllIds.Take(categorySize).ToArray();
            var rareIDs = cookedCategoryAllIds.Skip(categorySize).Take(categorySize).ToArray();
            if (commonIDs.Contains(objectID))
            {
                rarity = CookRarity.Common;
                indexBaseOffset = Array.IndexOf(commonIDs, objectID);
            }
            else if (rareIDs.Contains(objectID))
            {
                rarity = CookRarity.Rare;
                indexBaseOffset = Array.IndexOf(rareIDs, objectID);
            }
            else
            {
                rarity = CookRarity.Epic;
                var epicIDs = cookedCategoryAllIds.Skip(categorySize * 2).ToArray();
                indexBaseOffset = Array.IndexOf(epicIDs, objectID);
            }
            return cookedCategoryAllIds.Contains(objectID);
        }

        private void EnabeleUI()
        {
            saveSlotNoComboBox.Enabled = true;
            inventoryIndexComboBox.Enabled = true;
            createButton.Enabled = true;
            previousItemButton.Enabled = true;
            nextItemButton.Enabled = true;
            unlockAllRecipeButton.Enabled = true;
        }
        private void DisabeleUI()
        {
            saveSlotNoComboBox.Enabled = false;
            inventoryIndexComboBox.Enabled = false;
            createButton.Enabled = false;
            previousItemButton.Enabled = false;
            nextItemButton.Enabled = false;
            unlockAllRecipeButton.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastSaveFolderPath = SaveDataFolderPath;
            Properties.Settings.Default.Save();
        }

        private void savePathTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveDataFolderPath = savePathTextBox.Text;
        }
        private void previousItemButton_Click(object sender, EventArgs e)
        {
            if (inventoryIndexComboBox.SelectedIndex > 0)
            {
                inventoryIndexComboBox.SelectedIndex--;
            }
        }

        private void nextItemButton_Click(object sender, EventArgs e)
        {
            if (inventoryIndexComboBox.SelectedIndex < SaveDataManager.LoadItemLimit - 1)
            {
                inventoryIndexComboBox.SelectedIndex++;
            }
        }

        private void unlockAllRecipeButton_Click(object sender, EventArgs e)
        {
            _saveDataManager.UnlockAllRecipe();

            // todo レシピ全追加が実現したらこちらを有効化する。
            //var result = MessageBox.Show("この操作は元に戻せません。実行しますか？", "確認", MessageBoxButtons.YesNo);
            //if (result is DialogResult.Yes)
            //{
            //    _saveDataManager.UnlockAllResipe();
            //}
        }

        private void petKindComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            petColorComboBox.Items.Clear();

            var allPetType = Enum.GetValues<PetType>();
            var petTabDic = Enumerable.Range(0, allPetType.Length - 1)
                .Select(i => (i, allPetType[i]))
                .ToDictionary();
            var colorDic = Enumerable.Range(0, Enum.GetValues<PetColor>().Length - 1)
                .Select(i => (i, Enum.GetValues<PetColor>()[i]))
                .ToDictionary();
            //多色ペット判定
            if (new int[] { 0, 1, 2, 4, 9, 10 }.Contains(petKindComboBox.SelectedIndex))
            {
                foreach (var color in Enum.GetValues<PetColor>())
                {
                    petColorComboBox.Items.Add(StaticResource.PetColorDict[(allPetType[petKindComboBox.SelectedIndex], color)]);
                }
            }
            else
            {
                // スライム系
                petColorComboBox.Items.Add(StaticResource.PetColorDict[(allPetType[petKindComboBox.SelectedIndex], PetColor.Color_1)]);
            }
            petColorComboBox.SelectedIndex = 0;
        }

        private void openConditionsButton_Click(object sender, EventArgs e)
        {
            var conditionForm = new ConditionForm();
            conditionForm.ShowDialog();
        }
    }
}
