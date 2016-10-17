// André Betz
// http://www.andrebetz.de
using System;

namespace TM2Train
{
	/// <summary>
	/// Summary description for Core.
	/// </summary>
	public class Core
	{
		private TMState m_TMStates = null;
		private int m_XRailsCnt = 0;
		private int m_YRailsCnt = 0;
		private MyPGM m_CorePgm = null;
		private RailParts m_rp = null;
		private byte[] m_Connected = null;
		private int m_StateCnt;
		
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
		public MyPGM CorePgm
		{
			get
			{
				return m_CorePgm;
			}
		}
		public byte[] ConnectOut
		{
			get
			{
				return m_Connected;
			}
		}
		public int BeginRailsOuts
		{
			get
			{
				return ReadWriteHead.XRailsCnt+1;
			}
		}
		public int BeginRailsIn
		{
			get
			{
				return ReadWriteHead.XReadInput+1;
			}
		}
		public int States
		{
			get
			{
				return m_StateCnt;
			}
		}
		public Core(TMState ts,bool Set)
		{
			m_TMStates = ts;
			m_rp = new RailParts();
			m_StateCnt = TMLoader.CountStates(m_TMStates)/2;
			m_XRailsCnt = ReadWriteHead.XRailsCnt + m_StateCnt*2 + 1;
			m_YRailsCnt = ReadWriteHead.YRailsCnt * m_StateCnt   + 2;
			int XSize = m_XRailsCnt * RailParts.Size;
			int YSize = m_YRailsCnt * RailParts.Size;
			m_CorePgm = new MyPGM(XSize,YSize);
			SetRWHeads(m_StateCnt,Set);
			SetInputs(m_StateCnt);
			SetSubroutine(m_StateCnt);
		}
		private void SetField(MyPGM pgm,int X,int Y)
		{
			m_CorePgm.CopyAtPos(pgm,X*RailParts.Size,Y*RailParts.Size);
		}
		private void SetRWHeads(int States, bool Set)
		{
			ReadWriteHead rwh = new ReadWriteHead(Set);
			for(int i=0;i<States;i++)
			{
				int StartXPos = 1; 
				int StartYPos = 1 + i*ReadWriteHead.YRailsCnt; 
				SetField(rwh.RWHead,StartXPos,StartYPos);
			}
		}
		private void SetInputs(int States)
		{
			RailParts rp = new RailParts();
			SetField(rp.GetCurve(1),0,0);
			SetField(rp.GetStraight(1),1,0);
			SetField(rp.GetStraight(1),2,0);
			SetField(rp.GetStraight(1),4,0);
			MyPGM Sprung = rp.GetSprung(true);
			Sprung.Rotate90();
			SetField(Sprung,ReadWriteHead.YWriteInput+1,0);
			for(int i=0;i<States;i++)
			{
				int StartYPos = 1 + i*ReadWriteHead.YRailsCnt; 
				SetField(rp.GetStraight(0),0,StartYPos+0);
				SetField(rp.GetStraight(0),0,StartYPos+1);
				SetField(rp.GetStraight(0),0,StartYPos+2);
				SetField(rp.GetCrossing() ,0,StartYPos+3);
				SetField(rp.GetStraight(0),0,StartYPos+4);
			}
			int EndPos = 1 + States*ReadWriteHead.YRailsCnt;
			SetField(rp.GetCurve(0)   ,0,EndPos);
			SetField(rp.GetStraight(1),1,EndPos);
			SetField(rp.GetStraight(1),2,EndPos);
			SetField(rp.GetCurve(3)   ,ReadWriteHead.YWriteOutput+1,EndPos);
		}
		/// <summary>
		/// returns the Position in the Subroutine Matrix for next State
		/// </summary>
		/// <param name="States">Number of states</param>
		/// <param name="NextStateNr">Next State</param>
		/// <param name="Dir">Direction: true: Right, false: Left</param>
		/// <returns>State Out Position Number</returns>
		private int GetSubOutPos(int States,int NextStateNr, bool Dir)
		{
			int PosOut = 0;
			if(Dir)
			{
				PosOut = States + (States - NextStateNr -1);
			}
			else
			{
				PosOut = NextStateNr;				
			}
			return PosOut;
		}
		private MyPGM GetSubroutineConn(int States,ref int[] SubConn)
		{
			MyPGM ConnMatrix = new MyPGM(States*2,States*ReadWriteHead.YRailsCnt+1);
			TMState tmpState = m_TMStates;
			SubConn = new int[ConnMatrix.XSize];
			int[] ConnY = new int[ConnMatrix.YSize];
			m_Connected = new byte[ConnMatrix.XSize];
			int ActStateNr = 0;
			for(int i=0;i<SubConn.Length;i++)
			{
				SubConn[i] = 0;
			}
			for(int i=0;i<ConnY.Length;i++)
			{
				ConnY[i] = 0;
			}
			while(tmpState!=null)
			{
				int StateNr = TMLoader.FindStateNr(m_TMStates,tmpState.GetStateN());
				int YPos = (ActStateNr/2) * ReadWriteHead.YRailsCnt;
				if(StateNr>=0)
				{
					if((ActStateNr%2)==0)
					{
						YPos += ReadWriteHead.XReadOutputTrue;
					}
					else
					{
						YPos += ReadWriteHead.XReadOutputFalse;
					}
					bool Dir = false;
					if(tmpState.GetMove().Equals("R"))
					{
						Dir = true;
					}
					int SubPos = GetSubOutPos(States,StateNr,Dir);
					if(tmpState.GetRead().Equals(tmpState.GetWrite()))
					{
						if(SubConn[SubPos]==0)
						{
							SubConn[SubPos] = 1;
						}
						ConnMatrix.SetValue(SubPos,YPos,1);
					}
					else
					{
						SubConn[SubPos] = 2;
						ConnMatrix.SetValue(SubPos,YPos,2);
					}
					ConnY[YPos] = SubPos+1;
				}
				ActStateNr++;
				tmpState = tmpState.GetNext();
			}
			FillConnMatrix(ConnMatrix,SubConn,ConnY);
			return ConnMatrix;
		}
		private void FillConnMatrix(MyPGM ConnMat,int[] SubConn,int[] ConnY)
		{
			int XStartPos = ReadWriteHead.XRailsCnt+1;
			for(int j=0;j<ConnMat.XSize;j++)
			{
				if(SubConn[j]==2)
				{
					ConnMat.SetValue(j,0,1);
				}
			}
			for(int i=1;i<ConnMat.YSize;i++)
			{
				for(int j=0;j<ConnMat.XSize;j++)
				{
					int ValBefore = ConnMat.GetValue(j,i-1);
					int ValActual = ConnMat.GetValue(j,i);
					int Conn      = ConnY[i];
					int Sub       = SubConn[j];
					
					if(ValActual==2)
					{
						ConnMat.SetValue(j,i,2);
					}
					else if(ValActual==1)
					{
						if(ValBefore==0)
						{
							ConnMat.SetValue(j,i,3);
						}
						else					
						{
							ConnMat.SetValue(j,i,5);
						}
					}
					else
					{
						if(Conn==0)
						{						
							if(ValBefore==0)
							{
							}
							else if(ValBefore==6)
							{
							}
							else
							{
								ConnMat.SetValue(j,i,1);
							}
						}
						else
						{
							if(ValBefore==0)
							{
								if(Conn>j+1)
								{
									ConnMat.SetValue(j,i,6);
								}
							}
							else					
							{
								if(Conn>j+1)
								{
									ConnMat.SetValue(j,i,4);
								}
								else
								{
									ConnMat.SetValue(j,i,1);
								}
							}
						}
					}
				}
			}
		}
		private void SetSubroutine(int States)
		{
			RailParts rp = new RailParts();
			MyPGM Lazy = rp.GetLazy(false);
			Lazy.MirrorX();
			Lazy.Rotate90();
			MyPGM Sprung1 = rp.GetSprung(false);
			Sprung1.MirrorY();
			MyPGM Sprung2 = rp.GetSprung(false);
			Sprung2.Rotate180();
			int[] SubConn = null;

			MyPGM Begins = GetSubroutineConn(States,ref SubConn);

			bool OneCurveSet = false;
			// Paint Subroutine
			for(int i=SubConn.Length;i>0;i--)
			{
				if(SubConn[i-1]>1)
				{
					if(OneCurveSet)
					{
						SetField(Lazy,1+ReadWriteHead.XRailsCnt+i-1,0);
					}
					else
					{
						SetField(rp.GetCurve(2),1+ReadWriteHead.XRailsCnt+i-1,0);
					}
					OneCurveSet = true;
				}
				else
				{
					if(OneCurveSet)
					{
						SetField(rp.GetStraight(1),1+ReadWriteHead.XRailsCnt+i-1,0);
					}
				}
			}
			// Paint Connections
			int XStartPos = ReadWriteHead.XRailsCnt+1;
			int YStartPos = 1;
			for(int i=0;i<Begins.YSize;i++)
			{
				for(int j=0;j<Begins.XSize;j++)
				{
					int Val = Begins.GetValue(j,i);
					if(Val==1)
					{
						SetField(rp.GetStraight(0),XStartPos+j,YStartPos+i);
					}
					else if(Val==2)
					{
						SetField(Sprung1,XStartPos+j,YStartPos+i);
					}
					else if(Val==3)
					{
						SetField(rp.GetCurve(2),XStartPos+j,YStartPos+i);
					}
					else if(Val==4)
					{
						SetField(rp.GetCrossing(),XStartPos+j,YStartPos+i);
					}
					else if(Val==5)
					{
						SetField(Sprung2,XStartPos+j,YStartPos+i);
					}
					else if(Val==6)
					{
						SetField(rp.GetStraight(1),XStartPos+j,YStartPos+i);
					}
				}
			}
			int LastPos = Begins.YSize-1;
			for(int i=0;i<Begins.XSize;i++)
			{

				if(Begins.GetValue(i,LastPos)>0)
				{
					m_Connected[i] = 1;
				}
				else
				{
					m_Connected[i] = 0;
				}
			}
		}
	}
}
