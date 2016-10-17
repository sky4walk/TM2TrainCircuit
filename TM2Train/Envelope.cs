// André Betz
// http://www.andrebetz.de
using System;

namespace TM2Train
{
	/// <summary>
	/// Summary description for Envelope.
	/// </summary>
	public class Envelope
	{
		private Core m_core = null;
		private MyPGM m_EnvPgm = null;
		private int m_XRailsCnt;
		private int m_YRailsCnt;
		private int m_States = 0;
		public MyPGM EnvelopePgm
		{
			get
			{
				return m_EnvPgm;
			}
		}
		public int XRailsCnt
		{
			get
			{
				return m_XRailsCnt;
			}
		}
		public int YRailsCnt
		{
			get
			{
				return m_YRailsCnt;
			}
		}
		public int FirstInOutput
		{
			get
			{
				return m_States+ReadWriteHead.XReadInput;
			}
		}
		private void SetField(MyPGM pgm,int X,int Y)
		{
			RailParts.SetField(m_EnvPgm,pgm,X,Y);
		}
		public Envelope(TMState ts,bool Set)
		{
			m_core = new Core(ts,Set);
			m_States = m_core.States;
			m_XRailsCnt = (m_core.XRailsCnt  + m_States*4) ;
			m_YRailsCnt = (m_core.YRailsCnt  + m_States*2) ;
			int XSize = m_XRailsCnt * RailParts.Size;
			int YSize = m_YRailsCnt * RailParts.Size;
			m_EnvPgm = new MyPGM(XSize,YSize);
			m_EnvPgm.CopyAtPos(m_core.CorePgm,m_States*2*RailParts.Size,m_States*RailParts.Size);
			SetConn();
		}
		private void SetConn()
		{
			RailParts rp = new RailParts();
			int OutYPos = m_core.YRailsCnt      + m_States;
			int OutXPos = m_core.BeginRailsOuts + m_States*2;
			MyPGM Sprung1 = rp.GetSprung(false);
			Sprung1.Rotate90();
			Sprung1.MirrorY();
			MyPGM Sprung2 = rp.GetSprung(false);
			Sprung2.Rotate270();
			Sprung2.MirrorY();
			MyPGM Sprung3 = rp.GetSprung(true);
			Sprung3.Rotate270();
			Sprung3.MirrorY();
			for(int i=0;i<m_States;i++)
			{
				for(int j=0;j<m_States;j++)
				{
					if(j==i)
					{
						SetField(Sprung1,m_States*2-j-1,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(Sprung2,j,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetCurve(1),m_States*4+ReadWriteHead.XRailsCnt+i+1,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(Sprung3,m_States*5+ReadWriteHead.XRailsCnt+m_States-i,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
					}
					else if(j<i)
					{
						SetField(rp.GetCrossing(),m_States*2-j-1,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetStraight(1),j,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetStraight(1),m_States*4+ReadWriteHead.XRailsCnt+i+1,ReadWriteHead.YRailsCnt*j+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetStraight(1),m_States*5+ReadWriteHead.XRailsCnt+m_States-j,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
					}
					else
					{
						SetField(rp.GetStraight(1),m_States*2-j-1,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetCrossing(),j,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetStraight(0),m_States*4+ReadWriteHead.XRailsCnt+i+1,ReadWriteHead.YRailsCnt*j+ReadWriteHead.XReadInput+m_States+1);
						SetField(rp.GetCrossing(),m_States*5+ReadWriteHead.XRailsCnt+m_States-j,ReadWriteHead.YRailsCnt*i+ReadWriteHead.XReadInput+m_States+1);
					}
				}
				for(int j=i;j<m_States-1;j++)
				{
					for(int z=1;z<ReadWriteHead.YRailsCnt;z++)
					{
						SetField(rp.GetStraight(0),m_States*2-i-1,ReadWriteHead.YRailsCnt*j+ReadWriteHead.XReadInput+m_States+1+z);
						SetField(rp.GetStraight(0),m_States*4+ReadWriteHead.XRailsCnt+i+1,ReadWriteHead.YRailsCnt*j+ReadWriteHead.XReadInput+m_States+1+z);
					}
				}
				for(int j=0;j<i;j++)
				{
					for(int z=1;z<ReadWriteHead.YRailsCnt;z++)
					{
						SetField(rp.GetStraight(0),i,ReadWriteHead.YRailsCnt*j+ReadWriteHead.XReadInput+m_States+1+z);
						SetField(rp.GetStraight(0),m_States*5+ReadWriteHead.XRailsCnt+(m_States-i-1)+1,ReadWriteHead.YRailsCnt*j+ReadWriteHead.XReadInput+m_States+1+z);
					}
				}
				for(int j=0;j<(ReadWriteHead.YRailsCnt-ReadWriteHead.XReadInput);j++)
				{
					SetField(rp.GetStraight(0),m_States+i,ReadWriteHead.YRailsCnt*m_States+m_States+j);
					SetField(rp.GetStraight(0),m_States*4+ReadWriteHead.XRailsCnt+1+i,ReadWriteHead.YRailsCnt*m_States+m_States+j);
				}
				for(int j=m_States*2;j<OutXPos;j++)
				{
					SetField(rp.GetStraight(1),j,OutYPos+i);
				}
				for(int j=m_States;j<m_States*3+m_core.XRailsCnt;j++)
				{
					SetField(rp.GetStraight(1),j,i);
				}
				for(int j=0;j<ReadWriteHead.XReadInput+1;j++)
				{
					SetField(rp.GetStraight(0),i,j+m_States);
					SetField(rp.GetStraight(0),m_States*3+m_core.XRailsCnt+i,j+m_States);
				}
			}
			SetRound(m_States,OutXPos,OutYPos,2);
			SetRound(m_States,m_States,OutYPos,3);
			SetRound(m_States,OutXPos+m_States,OutYPos,3);
			SetRound(m_States,OutXPos+m_States+m_States,OutYPos,2);
			SetRound(m_States,0,0,0);
			SetRound(m_States,m_States*3+m_core.XRailsCnt,0,1);
		}
		private void SetRound(int States,int x, int y,int Rot)
		{
			RailParts rp = new RailParts();
			MyPGM BigCurve = new MyPGM(States*RailParts.Size,States*RailParts.Size);
			for(int i=0;i<States;i++)
			{
				for(int j=0;j<States;j++)
				{
					if(i==j)
					{
						RailParts.SetField(BigCurve,rp.GetCurve(1),i,j);
					}
					else if(j<i)
					{
						RailParts.SetField(BigCurve,rp.GetStraight(1),i,j);						
					}
					else
					{
						RailParts.SetField(BigCurve,rp.GetStraight(0),i,j);						
					}
				}
			}
			for(int i=0;i<Rot;i++)
			{
				BigCurve.Rotate90();
			}
			SetField(BigCurve,x,y);
		}
	}
}
