// André Betz
// http://www.andrebetz.de
using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

// Converts a Turing machine description into a Traincircuit
// The Turing machine should have a alphabet with 2 Symbols (true=='t'/false=='f')
// each state has two transitions for each Symbol
// (State-Actual,false,State-Next,Write-Sym,Direction)(State-Actual,true,State-Next,Write-Sym,Direction)
namespace TM2Train
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TM2Train : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button LoadTM;
		private System.Windows.Forms.Button HelpButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TM2Train()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LoadTM = new System.Windows.Forms.Button();
			this.HelpButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// LoadTM
			// 
			this.LoadTM.Location = new System.Drawing.Point(8, 8);
			this.LoadTM.Name = "LoadTM";
			this.LoadTM.Size = new System.Drawing.Size(112, 23);
			this.LoadTM.TabIndex = 0;
			this.LoadTM.Text = "Convert TM";
			this.LoadTM.Click += new System.EventHandler(this.LoadTM_Click);
			// 
			// HelpButton
			// 
			this.HelpButton.Location = new System.Drawing.Point(136, 8);
			this.HelpButton.Name = "HelpButton";
			this.HelpButton.Size = new System.Drawing.Size(104, 23);
			this.HelpButton.TabIndex = 1;
			this.HelpButton.Text = "Show Help";
			this.HelpButton.Click += new System.EventHandler(this.Help_Click);
			// 
			// TM2Train
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(258, 47);
			this.Controls.Add(this.HelpButton);
			this.Controls.Add(this.LoadTM);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TM2Train";
			this.Text = "TM2Train (C) www.andrebetz.de";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new TM2Train());
		}

		private void LoadTM_Click(object sender, System.EventArgs e)
		{

			OpenFileDialog opfd = new OpenFileDialog();
			opfd.Filter = "XML File (*.tm)|*.tm" ;
			opfd.FilterIndex = 1 ;
			opfd.RestoreDirectory = true ;
			opfd.Title = "Load Turing Machine";
			DialogResult res = opfd.ShowDialog(this);
			string TMName = opfd.FileName;

			if(res== DialogResult.OK)
			{
				string PgmName = Path.ChangeExtension(TMName,"pgm");
				TMLoader TM = new TMLoader();
				if(TM.Load(TMName))
				{
					TMState ts = TM.GetStates;
					Tape tp = TM.GetTape;
					Circuit circ = new Circuit(ts,tp,TMLoader.FindStateNr(ts,TM.StartState),TM.StartTapePos);
					circ.CircuitPgm.Write(PgmName);
				}
			}
		}

		private void Help_Click(object sender, System.EventArgs e)
		{
			Help.ShowHelp(this,"..\\..\\TM2TrainHelp.htm");
		}
	}
}
