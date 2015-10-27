namespace PegaBandeira
{
    partial class MenuInicial
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
            this.btn_VeriJog = new System.Windows.Forms.Button();
            this.ltb_JogOn = new System.Windows.Forms.ListBox();
            this.btn_Convida = new System.Windows.Forms.Button();
            this.btn_IniPartida = new System.Windows.Forms.Button();
            this.lbl_Nome = new System.Windows.Forms.Label();
            this.lbl_Apelido = new System.Windows.Forms.Label();
            this.txb_Nome = new System.Windows.Forms.TextBox();
            this.txb_Apelido = new System.Windows.Forms.TextBox();
            this.lbl_Convidou = new System.Windows.Forms.Label();
            this.gb_Convite = new System.Windows.Forms.GroupBox();
            this.gb_Config = new System.Windows.Forms.GroupBox();
            this.lbl_Espera = new System.Windows.Forms.Label();
            this.tm_verJogOn = new System.Windows.Forms.Timer(this.components);
            this.gb_ConviteRecv = new System.Windows.Forms.GroupBox();
            this.lbl_NomJogConv = new System.Windows.Forms.Label();
            this.btn_RecusConv = new System.Windows.Forms.Button();
            this.btn_AceitoConv = new System.Windows.Forms.Button();
            this.gb_Convite.SuspendLayout();
            this.gb_Config.SuspendLayout();
            this.gb_ConviteRecv.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_VeriJog
            // 
            this.btn_VeriJog.Enabled = false;
            this.btn_VeriJog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_VeriJog.Location = new System.Drawing.Point(399, 50);
            this.btn_VeriJog.Name = "btn_VeriJog";
            this.btn_VeriJog.Size = new System.Drawing.Size(113, 44);
            this.btn_VeriJog.TabIndex = 0;
            this.btn_VeriJog.Text = "Verificar Jogadores Online";
            this.btn_VeriJog.UseVisualStyleBackColor = true;
            this.btn_VeriJog.Click += new System.EventHandler(this.btn_VeriJog_Click);
            // 
            // ltb_JogOn
            // 
            this.ltb_JogOn.FormattingEnabled = true;
            this.ltb_JogOn.Location = new System.Drawing.Point(12, 146);
            this.ltb_JogOn.Name = "ltb_JogOn";
            this.ltb_JogOn.Size = new System.Drawing.Size(291, 355);
            this.ltb_JogOn.TabIndex = 1;
            // 
            // btn_Convida
            // 
            this.btn_Convida.Location = new System.Drawing.Point(20, 123);
            this.btn_Convida.Name = "btn_Convida";
            this.btn_Convida.Size = new System.Drawing.Size(123, 34);
            this.btn_Convida.TabIndex = 2;
            this.btn_Convida.Text = "Convidar ";
            this.btn_Convida.UseVisualStyleBackColor = true;
            this.btn_Convida.Click += new System.EventHandler(this.btn_Convida_Click);
            // 
            // btn_IniPartida
            // 
            this.btn_IniPartida.Location = new System.Drawing.Point(12, 537);
            this.btn_IniPartida.Name = "btn_IniPartida";
            this.btn_IniPartida.Size = new System.Drawing.Size(123, 58);
            this.btn_IniPartida.TabIndex = 3;
            this.btn_IniPartida.Text = "Pronto! Começa a partida.";
            this.btn_IniPartida.UseVisualStyleBackColor = true;
            this.btn_IniPartida.Visible = false;
            this.btn_IniPartida.Click += new System.EventHandler(this.btn_IniPartida_Click);
            // 
            // lbl_Nome
            // 
            this.lbl_Nome.AutoSize = true;
            this.lbl_Nome.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Nome.Location = new System.Drawing.Point(9, 38);
            this.lbl_Nome.Name = "lbl_Nome";
            this.lbl_Nome.Size = new System.Drawing.Size(58, 20);
            this.lbl_Nome.TabIndex = 6;
            this.lbl_Nome.Text = "Nome:";
            // 
            // lbl_Apelido
            // 
            this.lbl_Apelido.AutoSize = true;
            this.lbl_Apelido.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Apelido.Location = new System.Drawing.Point(9, 83);
            this.lbl_Apelido.Name = "lbl_Apelido";
            this.lbl_Apelido.Size = new System.Drawing.Size(69, 20);
            this.lbl_Apelido.TabIndex = 7;
            this.lbl_Apelido.Text = "Apelido:";
            // 
            // txb_Nome
            // 
            this.txb_Nome.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txb_Nome.Location = new System.Drawing.Point(81, 35);
            this.txb_Nome.MaxLength = 60;
            this.txb_Nome.Name = "txb_Nome";
            this.txb_Nome.Size = new System.Drawing.Size(209, 26);
            this.txb_Nome.TabIndex = 8;
            this.txb_Nome.Text = "Jefferson PC 1";
            this.txb_Nome.TextChanged += new System.EventHandler(this.AlteraNomeTxb);
            // 
            // txb_Apelido
            // 
            this.txb_Apelido.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txb_Apelido.Location = new System.Drawing.Point(84, 80);
            this.txb_Apelido.MaxLength = 15;
            this.txb_Apelido.Name = "txb_Apelido";
            this.txb_Apelido.Size = new System.Drawing.Size(206, 26);
            this.txb_Apelido.TabIndex = 9;
            this.txb_Apelido.Text = "JCS PC 1";
            this.txb_Apelido.TextChanged += new System.EventHandler(this.AlteraNomeTxb);
            // 
            // lbl_Convidou
            // 
            this.lbl_Convidou.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Convidou.Location = new System.Drawing.Point(17, 32);
            this.lbl_Convidou.Name = "lbl_Convidou";
            this.lbl_Convidou.Size = new System.Drawing.Size(279, 88);
            this.lbl_Convidou.TabIndex = 5;
            this.lbl_Convidou.Text = "Convite enviado para .... Esperando Resposta.";
            // 
            // gb_Convite
            // 
            this.gb_Convite.Controls.Add(this.btn_Convida);
            this.gb_Convite.Controls.Add(this.lbl_Convidou);
            this.gb_Convite.Enabled = false;
            this.gb_Convite.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_Convite.Location = new System.Drawing.Point(336, 146);
            this.gb_Convite.Name = "gb_Convite";
            this.gb_Convite.Size = new System.Drawing.Size(312, 163);
            this.gb_Convite.TabIndex = 13;
            this.gb_Convite.TabStop = false;
            this.gb_Convite.Text = "Envia Convite";
            this.gb_Convite.Visible = false;
            // 
            // gb_Config
            // 
            this.gb_Config.Controls.Add(this.txb_Nome);
            this.gb_Config.Controls.Add(this.btn_VeriJog);
            this.gb_Config.Controls.Add(this.lbl_Nome);
            this.gb_Config.Controls.Add(this.lbl_Apelido);
            this.gb_Config.Controls.Add(this.txb_Apelido);
            this.gb_Config.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_Config.Location = new System.Drawing.Point(12, 2);
            this.gb_Config.Name = "gb_Config";
            this.gb_Config.Size = new System.Drawing.Size(636, 138);
            this.gb_Config.TabIndex = 15;
            this.gb_Config.TabStop = false;
            this.gb_Config.Text = "Meus Dados";
            // 
            // lbl_Espera
            // 
            this.lbl_Espera.AutoSize = true;
            this.lbl_Espera.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Espera.Location = new System.Drawing.Point(332, 501);
            this.lbl_Espera.Name = "lbl_Espera";
            this.lbl_Espera.Size = new System.Drawing.Size(304, 20);
            this.lbl_Espera.TabIndex = 16;
            this.lbl_Espera.Text = "Esperando outro jogador para começar.";
            this.lbl_Espera.Visible = false;
            // 
            // tm_verJogOn
            // 
            this.tm_verJogOn.Interval = 5000;
            this.tm_verJogOn.Tick += new System.EventHandler(this.verJogOn_Tick);
            // 
            // gb_ConviteRecv
            // 
            this.gb_ConviteRecv.Controls.Add(this.lbl_NomJogConv);
            this.gb_ConviteRecv.Controls.Add(this.btn_RecusConv);
            this.gb_ConviteRecv.Controls.Add(this.btn_AceitoConv);
            this.gb_ConviteRecv.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_ConviteRecv.Location = new System.Drawing.Point(336, 330);
            this.gb_ConviteRecv.Name = "gb_ConviteRecv";
            this.gb_ConviteRecv.Size = new System.Drawing.Size(312, 168);
            this.gb_ConviteRecv.TabIndex = 17;
            this.gb_ConviteRecv.TabStop = false;
            this.gb_ConviteRecv.Text = "Convite Recebido";
            this.gb_ConviteRecv.Visible = false;
            // 
            // lbl_NomJogConv
            // 
            this.lbl_NomJogConv.Location = new System.Drawing.Point(16, 34);
            this.lbl_NomJogConv.Name = "lbl_NomJogConv";
            this.lbl_NomJogConv.Size = new System.Drawing.Size(280, 88);
            this.lbl_NomJogConv.TabIndex = 2;
            this.lbl_NomJogConv.Text = "O jogador ... te convidou para jogar. Deseja aceitar?";
            // 
            // btn_RecusConv
            // 
            this.btn_RecusConv.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RecusConv.Location = new System.Drawing.Point(225, 125);
            this.btn_RecusConv.Name = "btn_RecusConv";
            this.btn_RecusConv.Size = new System.Drawing.Size(75, 37);
            this.btn_RecusConv.TabIndex = 1;
            this.btn_RecusConv.Text = "Recuso";
            this.btn_RecusConv.UseVisualStyleBackColor = true;
            this.btn_RecusConv.Click += new System.EventHandler(this.btn_RecusConv_Click);
            // 
            // btn_AceitoConv
            // 
            this.btn_AceitoConv.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AceitoConv.Location = new System.Drawing.Point(20, 125);
            this.btn_AceitoConv.Name = "btn_AceitoConv";
            this.btn_AceitoConv.Size = new System.Drawing.Size(75, 37);
            this.btn_AceitoConv.TabIndex = 0;
            this.btn_AceitoConv.Text = "Aceito";
            this.btn_AceitoConv.UseVisualStyleBackColor = true;
            this.btn_AceitoConv.Click += new System.EventHandler(this.btn_AceitoConv_Click);
            // 
            // MenuInicial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 617);
            this.Controls.Add(this.gb_ConviteRecv);
            this.Controls.Add(this.lbl_Espera);
            this.Controls.Add(this.gb_Config);
            this.Controls.Add(this.gb_Convite);
            this.Controls.Add(this.btn_IniPartida);
            this.Controls.Add(this.ltb_JogOn);
            this.Name = "MenuInicial";
            this.Text = "Menu Inicial";
            this.gb_Convite.ResumeLayout(false);
            this.gb_Config.ResumeLayout(false);
            this.gb_Config.PerformLayout();
            this.gb_ConviteRecv.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_VeriJog;
        private System.Windows.Forms.ListBox ltb_JogOn;
        private System.Windows.Forms.Button btn_Convida;
        private System.Windows.Forms.Button btn_IniPartida;
        private System.Windows.Forms.Label lbl_Nome;
        private System.Windows.Forms.Label lbl_Apelido;
        private System.Windows.Forms.TextBox txb_Nome;
        private System.Windows.Forms.TextBox txb_Apelido;
        private System.Windows.Forms.Label lbl_Convidou;
        private System.Windows.Forms.GroupBox gb_Convite;
        private System.Windows.Forms.GroupBox gb_Config;
        private System.Windows.Forms.Label lbl_Espera;
        private System.Windows.Forms.Timer tm_verJogOn;
        private System.Windows.Forms.GroupBox gb_ConviteRecv;
        private System.Windows.Forms.Label lbl_NomJogConv;
        private System.Windows.Forms.Button btn_RecusConv;
        private System.Windows.Forms.Button btn_AceitoConv;
    }
}

