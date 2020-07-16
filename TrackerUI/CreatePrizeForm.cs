using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        private TextBox placeNumberText;
        private Label placeNumberLabel;
        private Label placeNameLabel;
        private Label prizeAmountLabel;
        private TextBox placeNameText;
        private TextBox prizeAmountText;
        private Label orLabel;
        private Label prizePercentageLabel;
        private TextBox prizePercentageText;
        private Button createPrizeButton;
        private Label percentSignLabel;
        private Label headerLabel;

        IPrizeRequestor callingForm;
        public CreatePrizeForm(IPrizeRequestor caller)
        {
            InitializeComponent();
            callingForm = caller;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatePrizeForm));
            this.headerLabel = new System.Windows.Forms.Label();
            this.placeNumberText = new System.Windows.Forms.TextBox();
            this.placeNumberLabel = new System.Windows.Forms.Label();
            this.placeNameLabel = new System.Windows.Forms.Label();
            this.prizeAmountLabel = new System.Windows.Forms.Label();
            this.placeNameText = new System.Windows.Forms.TextBox();
            this.prizeAmountText = new System.Windows.Forms.TextBox();
            this.orLabel = new System.Windows.Forms.Label();
            this.prizePercentageLabel = new System.Windows.Forms.Label();
            this.prizePercentageText = new System.Windows.Forms.TextBox();
            this.createPrizeButton = new System.Windows.Forms.Button();
            this.percentSignLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI Light", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(131)))), ((int)(((byte)(255)))));
            this.headerLabel.Location = new System.Drawing.Point(14, 9);
            this.headerLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(209, 50);
            this.headerLabel.TabIndex = 13;
            this.headerLabel.Text = "Create Prize";
            // 
            // placeNumberText
            // 
            this.placeNumberText.Location = new System.Drawing.Point(285, 87);
            this.placeNumberText.Name = "placeNumberText";
            this.placeNumberText.Size = new System.Drawing.Size(230, 35);
            this.placeNumberText.TabIndex = 27;
            // 
            // placeNumberLabel
            // 
            this.placeNumberLabel.AutoSize = true;
            this.placeNumberLabel.Location = new System.Drawing.Point(67, 87);
            this.placeNumberLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.placeNumberLabel.Name = "placeNumberLabel";
            this.placeNumberLabel.Size = new System.Drawing.Size(144, 30);
            this.placeNumberLabel.TabIndex = 26;
            this.placeNumberLabel.Text = "Place Number";
            // 
            // placeNameLabel
            // 
            this.placeNameLabel.AutoSize = true;
            this.placeNameLabel.Location = new System.Drawing.Point(67, 158);
            this.placeNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.placeNameLabel.Name = "placeNameLabel";
            this.placeNameLabel.Size = new System.Drawing.Size(124, 30);
            this.placeNameLabel.TabIndex = 28;
            this.placeNameLabel.Text = "Place Name";
            this.placeNameLabel.Click += new System.EventHandler(this.placeNameLabel_Click);
            // 
            // prizeAmountLabel
            // 
            this.prizeAmountLabel.AutoSize = true;
            this.prizeAmountLabel.Location = new System.Drawing.Point(67, 227);
            this.prizeAmountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.prizeAmountLabel.Name = "prizeAmountLabel";
            this.prizeAmountLabel.Size = new System.Drawing.Size(139, 30);
            this.prizeAmountLabel.TabIndex = 29;
            this.prizeAmountLabel.Text = "Prize Amount";
            // 
            // placeNameText
            // 
            this.placeNameText.Location = new System.Drawing.Point(285, 158);
            this.placeNameText.Name = "placeNameText";
            this.placeNameText.Size = new System.Drawing.Size(230, 35);
            this.placeNameText.TabIndex = 30;
            // 
            // prizeAmountText
            // 
            this.prizeAmountText.Location = new System.Drawing.Point(285, 227);
            this.prizeAmountText.Name = "prizeAmountText";
            this.prizeAmountText.Size = new System.Drawing.Size(230, 35);
            this.prizeAmountText.TabIndex = 31;
            this.prizeAmountText.Text = "0";
            // 
            // orLabel
            // 
            this.orLabel.AutoSize = true;
            this.orLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orLabel.Location = new System.Drawing.Point(239, 282);
            this.orLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.orLabel.Name = "orLabel";
            this.orLabel.Size = new System.Drawing.Size(70, 30);
            this.orLabel.TabIndex = 32;
            this.orLabel.Text = "- OR -";
            // 
            // prizePercentageLabel
            // 
            this.prizePercentageLabel.AutoSize = true;
            this.prizePercentageLabel.Location = new System.Drawing.Point(67, 334);
            this.prizePercentageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.prizePercentageLabel.Name = "prizePercentageLabel";
            this.prizePercentageLabel.Size = new System.Drawing.Size(167, 30);
            this.prizePercentageLabel.TabIndex = 33;
            this.prizePercentageLabel.Text = "Prize Percentage";
            // 
            // prizePercentageText
            // 
            this.prizePercentageText.Location = new System.Drawing.Point(285, 334);
            this.prizePercentageText.Name = "prizePercentageText";
            this.prizePercentageText.Size = new System.Drawing.Size(230, 35);
            this.prizePercentageText.TabIndex = 34;
            this.prizePercentageText.Text = "0";
            // 
            // createPrizeButton
            // 
            this.createPrizeButton.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.createPrizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.createPrizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.createPrizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createPrizeButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createPrizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.createPrizeButton.Location = new System.Drawing.Point(194, 412);
            this.createPrizeButton.Name = "createPrizeButton";
            this.createPrizeButton.Size = new System.Drawing.Size(197, 53);
            this.createPrizeButton.TabIndex = 35;
            this.createPrizeButton.Text = "Create Prize";
            this.createPrizeButton.UseVisualStyleBackColor = true;
            this.createPrizeButton.Click += new System.EventHandler(this.createPrizeButton_Click);
            // 
            // percentSignLabel
            // 
            this.percentSignLabel.AutoSize = true;
            this.percentSignLabel.Location = new System.Drawing.Point(521, 337);
            this.percentSignLabel.Name = "percentSignLabel";
            this.percentSignLabel.Size = new System.Drawing.Size(30, 30);
            this.percentSignLabel.TabIndex = 36;
            this.percentSignLabel.Text = "%";
            // 
            // CreatePrizeForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(602, 513);
            this.Controls.Add(this.percentSignLabel);
            this.Controls.Add(this.createPrizeButton);
            this.Controls.Add(this.prizePercentageText);
            this.Controls.Add(this.prizePercentageLabel);
            this.Controls.Add(this.orLabel);
            this.Controls.Add(this.prizeAmountText);
            this.Controls.Add(this.placeNameText);
            this.Controls.Add(this.prizeAmountLabel);
            this.Controls.Add(this.placeNameLabel);
            this.Controls.Add(this.placeNumberText);
            this.Controls.Add(this.placeNumberLabel);
            this.Controls.Add(this.headerLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreatePrizeForm";
            this.Text = "CreatePrize";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void placeNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                // create the model
                PrizeModel model = new PrizeModel(placeNameText.Text,
                                                  placeNumberText.Text, 
                                                  prizeAmountText.Text, 
                                                  prizePercentageText.Text);
                // save the model
                GlobalConfig.Connection.CreatePrize(model);

                callingForm.prizeComplete(model); //im done and heres that model you asked for.

                this.Close();
                // clear the form (now done by close())
                //placeNameText.Text = "";
                //placeNumberText.Text = "";
                //prizeAmountText.Text = "0";
                //prizePercentageText.Text = "0";
            } 
            else
            {
                MessageBox.Show("this form has invalid information, please verify and try again");
            }
        }

        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValid = int.TryParse(placeNumberText.Text,out placeNumber);

            if (placeNumberValid == false)
            {
                output = false;
            }
            if (placeNumber < 1)
            {
                output = false;
            }
            if (placeNameText.Text.Length == 0)
            {
                output = false;
            }
            decimal prizeAmount = 0;
            double prizePercentage = 0;
            bool prizeAmountValid = decimal.TryParse(prizeAmountText.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageText.Text, out prizePercentage);
            if ( prizeAmountValid == false || prizePercentageValid == false) 
            {
                output = false;
            }
            if (prizeAmount <= 0 & prizePercentage <= 0)
            {
                output = false;
            }
            if (prizePercentage <= 0 & prizePercentage > 100)
            {
                output = false;

            }

            return output;
        }
    }
}
