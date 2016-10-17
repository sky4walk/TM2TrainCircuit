// André Betz
// http://www.andrebetz.de
using System;

namespace TM2Train
{
	/// <summary>
	/// Summary description for Tape.
	/// </summary>
	public class Tape 
	{
		string m_Sign;
		Tape m_Next;
		Tape m_Last;
		int m_PosNum;
		public static int  m_PosNumCnt = 0;

		public Tape(int Pos) 
		{
			m_Next = null;
			m_Last = null;
			m_PosNum = Pos;
		}

		public int GetPosNum() 
		{
			return m_PosNum;
		}

		public Tape(string Sign) 
		{
			SetSign(Sign);
			m_Next = null;
			m_Last = null;
			m_PosNum = m_PosNumCnt;
			m_PosNumCnt++;
		}

		public void SetNext(Tape next) 
		{
			m_Next = next;
			if(next!=null)
			{
				m_Next.SetLast(this);
			}
		}

		public void SetLast(Tape last) 
		{
			m_Last = last;
		}

		public Tape GetNext() 
		{
			return m_Next;
		}

		public Tape GetLast() 
		{
			return m_Last;
		}

		public void SetSign(string sign) 
		{
			m_Sign = sign;
		}

		public string GetSign() 
		{
			return m_Sign;
		}
  
		public void ResetPos()
		{
			m_PosNumCnt = 1;
			m_PosNum = 0;
		}
	}
}
