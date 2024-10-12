using CKFoodMaker.Model;
using CKFoodMaker.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CKFoodMaker
{
    public partial class ConditionForm : Form
    {
        SaveDataManager _saveDataManager = SaveDataManager.GetInstance();

        private BindingList<Condition> Conditions = [];

        public ConditionForm()
        {
            InitializeComponent();

            Conditions = new(_saveDataManager.GetConditions().OrderBy(c => c.Id).ToList());
            dataGridView.DataSource = Conditions;
            InitDataGridView();
        }

        private void InitDataGridView()
        {
            //todo チェックボックスの追加とか新規の空行追加とか
            //throw new NotImplementedException();
        }

        private void conditionIdLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://core-keeper.fandom.com/wiki/Condition_IDs");
        }

        private void backUpConditionsButton_Click(object sender, EventArgs e)
        {
            var result = saveFileDialog.ShowDialog();
            if (result is DialogResult.OK)
            {
                _saveDataManager.BackUpConditions(Conditions, saveFileDialog.FileName);
            }
        }

        private void loadConditionsButton_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result is DialogResult.OK)
            {
                var condtions = _saveDataManager.LoadConditions(saveFileDialog.FileName);
                Conditions = new(condtions);
                dataGridView.Refresh();
            }
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //新規追加されたConditonのバリデーション
            if (dataGridView.Rows[e.RowIndex].IsNewRow &&
                !string.IsNullOrEmpty(e.FormattedValue?.ToString()))
            {
                var condition = new Condition(0, 0, 0, 0);  //hack 仮
                Conditions.Add(condition);
            }
        }

        private void overrideConditionsButton_Click(object sender, EventArgs e)
        {
            //todo 上書き実行
            //DataGridViewから編集、取得した値をListでまとめる
            //_saveDataManager.OverrideConditions();
        }
    }
}
