// André Betz
// http://www.andrebetz.de
using System;

namespace TM2Train
{
	/// <summary>
	/// Summary description for Circuit.
	/// </summary>
	public class Circuit
	{
		MyPGM m_CircuitPgm = null;

		public MyPGM CircuitPgm
		{
			get
			{
				return m_CircuitPgm;
			}
		}

		public Circuit(TMState ts,Tape tp,int StateBegin,int TapePos)
		{
			int envXSize = 0;
			int envYInOut = 0;
			RailParts rp = new RailParts();
			MyPGM straight = rp.GetStraight(1);
			MyPGM Start = rp.GetStart();
			int TpCnt = TMLoader.CountTape(tp);
			int STCnt = TMLoader.CountStates(ts)/2;
			Tape tmpTape = tp;
			Envelope env = null;
			for(int i=0;i<TpCnt;i++)
			{
				if(tmpTape.GetSign().Equals("t"))
				{
					env = new Envelope(ts,true);
				}
				else
				{
					env = new Envelope(ts,false);
				}
				if(i==0)
				{
					envXSize = (env.XRailsCnt+1);
					envYInOut = env.FirstInOutput;
					int XSize = envXSize*TpCnt;
					int YSize = env.YRailsCnt;
					m_CircuitPgm = new MyPGM(XSize*RailParts.Size,YSize*RailParts.Size);
				}
				RailParts.SetField(m_CircuitPgm,env.EnvelopePgm,envXSize*i+1,0);

				for(int j=0;j<STCnt;j++)
				{
					if(j==StateBegin && i==TapePos)
					{
						RailParts.SetField(m_CircuitPgm,Start,envXSize*i,envYInOut+ReadWriteHead.YRailsCnt*j+1);
					}
					else
					{
						RailParts.SetField(m_CircuitPgm,straight,envXSize*i,envYInOut+ReadWriteHead.YRailsCnt*j+1);
					}
				}

				tmpTape = tmpTape.GetNext();
			}
		}
	}
}
