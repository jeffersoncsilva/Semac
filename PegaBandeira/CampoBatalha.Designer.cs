namespace PegaBandeira
{
    partial class CampoBatalha
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
            this.components = new System.ComponentModel.Container();
            this.lbl_NomeJog = new System.Windows.Forms.Label();
            this.lbl_TempoRestante = new System.Windows.Forms.Label();
            this.lbl_Placar = new System.Windows.Forms.Label();
            this.lbl_Inativo = new System.Windows.Forms.Label();
            this.tm_UpdtTempoPartida = new System.Windows.Forms.Timer(this.components);
            this.lbl_Resultado = new System.Windows.Forms.Label();
            this.tm_Bala = new System.Windows.Forms.Timer(this.components);
            this.tm_Congelamento = new System.Windows.Forms.Timer(this.components);
            this.lbl_FinDeJogo = new System.Windows.Forms.Label();
            this.btn_RetornaMenuInicial = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_NomeJog
            // 
            this.lbl_NomeJog.Location = new System.Drawing.Point(53, 19);
            this.lbl_NomeJog.Name = "lbl_NomeJog";
            this.lbl_NomeJog.Size = new System.Drawing.Size(270, 52);
            this.lbl_NomeJog.TabIndex = 0;
            this.lbl_NomeJog.Text = "Nome Jogador:";
            // 
            // lbl_TempoRestante
            // 
            this.lbl_TempoRestante.Location = new System.Drawing.Point(30, 90);
            this.lbl_TempoRestante.Name = "lbl_TempoRestante";
            this.lbl_TempoRestante.Size = new System.Drawing.Size(270, 90);
            this.lbl_TempoRestante.TabIndex = 1;
            this.lbl_TempoRestante.Text = "label2";
            // 
            // lbl_Placar
            // 
            this.lbl_Placar.Location = new System.Drawing.Point(30, 223);
            this.lbl_Placar.Name = "lbl_Placar";
            this.lbl_Placar.Size = new System.Drawing.Size(270, 69);
            this.lbl_Placar.TabIndex = 2;
            this.lbl_Placar.Text = "label3";
            // 
            // lbl_Inativo
            // 
            this.lbl_Inativo.Location = new System.Drawing.Point(409, 41);
            this.lbl_Inativo.Name = "lbl_Inativo";
            this.lbl_Inativo.Size = new System.Drawing.Size(258, 65);
            this.lbl_Inativo.TabIndex = 3;
            this.lbl_Inativo.Text = "label1";
            // 
            // tm_UpdtTempoPartida
            // 
            this.tm_UpdtTempoPartida.Interval = 1000;
            this.tm_UpdtTempoPartida.Tick += new System.EventHandler(this.tm_UpdtTempoPartida_Tick);
            // 
            // lbl_Resultado
            // 
            this.lbl_Resultado.AutoSize = true;
            this.lbl_Resultado.Location = new System.Drawing.Point(403, 98);
            this.lbl_Resultado.Name = "lbl_Resultado";
            this.lbl_Resultado.Size = new System.Drawing.Size(35, 13);
            this.lbl_Resultado.TabIndex = 4;
            this.lbl_Resultado.Text = "label1";
            this.lbl_Resultado.Visible = false;
            // 
            // tm_Bala
            // 
            this.tm_Bala.Interval = 500;
            this.tm_Bala.Tick += new System.EventHandler(this.tm_Bala_Tick);
            // 
            // tm_Congelamento
            // 
            this.tm_Congelamento.Interval = 2000;
            this.tm_Congelamento.Tick += new System.EventHandler(this.tm_Congelamento_Tick);
            // 
            // lbl_FinDeJogo
            // 
            this.lbl_FinDeJogo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FinDeJogo.Location = new System.Drawing.Point(56, 120);
            this.lbl_FinDeJogo.Name = "lbl_FinDeJogo";
            this.lbl_FinDeJogo.Size = new System.Drawing.Size(611, 261);
            this.lbl_FinDeJogo.TabIndex = 5;
            this.lbl_FinDeJogo.Text = "label1";
            // 
            // btn_RetornaMenuInicial
            // 
            this.btn_RetornaMenuInicial.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RetornaMenuInicial.Location = new System.Drawing.Point(298, 312);
            this.btn_RetornaMenuInicial.Name = "btn_RetornaMenuInicial";
            this.btn_RetornaMenuInicial.Size = new System.Drawing.Size(183, 69);
            this.btn_RetornaMenuInicial.TabIndex = 6;
            this.btn_RetornaMenuInicial.Text = "Voltar";
            this.btn_RetornaMenuInicial.UseVisualStyleBackColor = true;
            this.btn_RetornaMenuInicial.Click += new System.EventHandler(this.btn_RetornaMenuInicial_Click);
            // 
            // CampoBatalha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 480);
            this.Controls.Add(this.btn_RetornaMenuInicial);
            this.Controls.Add(this.lbl_FinDeJogo);
            this.Controls.Add(this.lbl_Resultado);
            this.Controls.Add(this.lbl_Inativo);
            this.Controls.Add(this.lbl_Placar);
            this.Controls.Add(this.lbl_TempoRestante);
            this.Controls.Add(this.lbl_NomeJog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CampoBatalha";
            this.Text = "CampoBatalha";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CampoBatalha_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CampoBatalha_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CampoBatalha_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_NomeJog;
        private System.Windows.Forms.Label lbl_TempoRestante;
        private System.Windows.Forms.Label lbl_Placar;
        private System.Windows.Forms.Label lbl_Inativo;
        private System.Windows.Forms.Timer tm_UpdtTempoPartida;
        private System.Windows.Forms.Label lbl_Resultado;
        private System.Windows.Forms.Timer tm_Bala;
        private System.Windows.Forms.Timer tm_Congelamento;
        private System.Windows.Forms.Label lbl_FinDeJogo;
        private System.Windows.Forms.Button btn_RetornaMenuInicial;
    }
}