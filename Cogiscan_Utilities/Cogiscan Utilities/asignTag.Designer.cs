namespace Cogiscan_Utilities
{
    partial class asignTag
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
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblToDo = new System.Windows.Forms.Label();
            this.txtContenedor = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtBarcode
            // 
            this.txtBarcode.BackColor = System.Drawing.Color.White;
            this.txtBarcode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBarcode.Enabled = false;
            this.txtBarcode.Location = new System.Drawing.Point(17, 73);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(126, 20);
            this.txtBarcode.TabIndex = 1;
            this.txtBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarcode_KeyDown);
            // 
            // lblToDo
            // 
            this.lblToDo.BackColor = System.Drawing.Color.DodgerBlue;
            this.lblToDo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblToDo.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToDo.ForeColor = System.Drawing.Color.AliceBlue;
            this.lblToDo.Location = new System.Drawing.Point(0, 0);
            this.lblToDo.Name = "lblToDo";
            this.lblToDo.Size = new System.Drawing.Size(151, 19);
            this.lblToDo.TabIndex = 2;
            this.lblToDo.Text = "Escanee un Código";
            this.lblToDo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtContenedor
            // 
            this.txtContenedor.BackColor = System.Drawing.Color.White;
            this.txtContenedor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtContenedor.Location = new System.Drawing.Point(17, 36);
            this.txtContenedor.Name = "txtContenedor";
            this.txtContenedor.Size = new System.Drawing.Size(126, 20);
            this.txtContenedor.TabIndex = 3;
            this.txtContenedor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContenedor_KeyDown);
            // 
            // asignTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(151, 101);
            this.ControlBox = false;
            this.Controls.Add(this.txtContenedor);
            this.Controls.Add(this.lblToDo);
            this.Controls.Add(this.txtBarcode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "asignTag";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.asignTag_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBarcode;
        public System.Windows.Forms.Label lblToDo;
        private System.Windows.Forms.TextBox txtContenedor;
    }
}