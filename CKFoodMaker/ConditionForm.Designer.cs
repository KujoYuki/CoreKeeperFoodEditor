namespace CKFoodMaker
{
    partial class ConditionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView = new DataGridView();
            label1 = new Label();
            backUpConditionsButton = new Button();
            overrideConditionsButton = new Button();
            loadConditionsButton = new Button();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            conditionIdLinkLabel = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(12, 27);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(469, 359);
            dataGridView.TabIndex = 0;
            dataGridView.CellValidating += dataGridView_CellValidating;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(407, 15);
            label1.TabIndex = 1;
            label1.Text = "全てのスキル/バフ・デバフ/食事効果/装備効果はConditionListで管理されています。";
            // 
            // backUpConditionsButton
            // 
            backUpConditionsButton.Location = new Point(519, 86);
            backUpConditionsButton.Name = "backUpConditionsButton";
            backUpConditionsButton.Size = new Size(196, 23);
            backUpConditionsButton.TabIndex = 2;
            backUpConditionsButton.Text = "コンディション値をバックアップを作成";
            backUpConditionsButton.UseVisualStyleBackColor = true;
            backUpConditionsButton.Click += backUpConditionsButton_Click;
            // 
            // overrideConditionsButton
            // 
            overrideConditionsButton.Location = new Point(519, 166);
            overrideConditionsButton.Name = "overrideConditionsButton";
            overrideConditionsButton.Size = new Size(75, 23);
            overrideConditionsButton.TabIndex = 3;
            overrideConditionsButton.Text = "上書き";
            overrideConditionsButton.UseVisualStyleBackColor = true;
            overrideConditionsButton.Click += overrideConditionsButton_Click;
            // 
            // loadConditionsButton
            // 
            loadConditionsButton.Location = new Point(519, 41);
            loadConditionsButton.Name = "loadConditionsButton";
            loadConditionsButton.Size = new Size(196, 23);
            loadConditionsButton.TabIndex = 4;
            loadConditionsButton.Text = "コンディション値を読み込む";
            loadConditionsButton.UseVisualStyleBackColor = true;
            loadConditionsButton.Click += loadConditionsButton_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Title = "開くファイルを選択してください。";
            // 
            // saveFileDialog
            // 
            saveFileDialog.FileName = "conditionBackup";
            saveFileDialog.Filter = "\"jsonファイル\"|*.json";
            saveFileDialog.Title = "保存先を選択してください。";
            // 
            // conditionIdLinkLabel
            // 
            conditionIdLinkLabel.AutoSize = true;
            conditionIdLinkLabel.Location = new Point(519, 124);
            conditionIdLinkLabel.Name = "conditionIdLinkLabel";
            conditionIdLinkLabel.Size = new Size(74, 15);
            conditionIdLinkLabel.TabIndex = 5;
            conditionIdLinkLabel.TabStop = true;
            conditionIdLinkLabel.Text = "ConditionIds";
            conditionIdLinkLabel.LinkClicked += conditionIdLinkLabel_LinkClicked;
            // 
            // ConditionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(conditionIdLinkLabel);
            Controls.Add(loadConditionsButton);
            Controls.Add(overrideConditionsButton);
            Controls.Add(backUpConditionsButton);
            Controls.Add(label1);
            Controls.Add(dataGridView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ConditionForm";
            Text = "ConditionForm";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView;
        private Label label1;
        private Button backUpConditionsButton;
        private Button overrideConditionsButton;
        private Button loadConditionsButton;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private LinkLabel conditionIdLinkLabel;
    }
}