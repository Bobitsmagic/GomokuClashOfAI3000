﻿namespace Gomoku
{
	partial class Screen
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.BEngine1 = new System.Windows.Forms.Button();
			this.BReset = new System.Windows.Forms.Button();
			this.BStatistics = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// BEngine1
			// 
			this.BEngine1.Location = new System.Drawing.Point(483, 12);
			this.BEngine1.Name = "BEngine1";
			this.BEngine1.Size = new System.Drawing.Size(161, 99);
			this.BEngine1.TabIndex = 0;
			this.BEngine1.Text = "DoEngineMove!";
			this.BEngine1.UseVisualStyleBackColor = true;
			this.BEngine1.Click += new System.EventHandler(this.BEngine1_Click);
			// 
			// BReset
			// 
			this.BReset.Location = new System.Drawing.Point(605, 409);
			this.BReset.Name = "BReset";
			this.BReset.Size = new System.Drawing.Size(45, 23);
			this.BReset.TabIndex = 1;
			this.BReset.Text = "Reset";
			this.BReset.UseVisualStyleBackColor = true;
			this.BReset.Click += new System.EventHandler(this.BReset_Click);
			// 
			// BStatistics
			// 
			this.BStatistics.Location = new System.Drawing.Point(483, 136);
			this.BStatistics.Name = "BStatistics";
			this.BStatistics.Size = new System.Drawing.Size(161, 30);
			this.BStatistics.TabIndex = 2;
			this.BStatistics.Text = "DataZeugs";
			this.BStatistics.UseVisualStyleBackColor = true;
			this.BStatistics.Click += new System.EventHandler(this.BStatistics_Click);
			// 
			// Screen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(662, 444);
			this.Controls.Add(this.BStatistics);
			this.Controls.Add(this.BReset);
			this.Controls.Add(this.BEngine1);
			this.Name = "Screen";
			this.Text = "BestGameEu";
			this.Load += new System.EventHandler(this.Screen_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Screen_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Screen_MouseClick);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button BEngine1;
		private System.Windows.Forms.Button BReset;
		private System.Windows.Forms.Button BStatistics;
	}
}

