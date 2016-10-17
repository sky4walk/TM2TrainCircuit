//written by André Betz 
//http://www.andrebetz.de
using System;

namespace TM2Train
{
	/// <summary>
	/// Summary description for TMState.
	/// </summary>
	public class TMState 
	{
		TMState m_Next;
		string  m_StateN;
		string  m_Read;
		string  m_StateF;
		string  m_Write;
		string  m_Move;

		public TMState(string StateF,string Read, string StateN, string Write, string Move) 
		{
			m_StateF = StateF;
			m_Read   = Read;
			m_StateN = StateN;
			m_Write  = Write;
			m_Move   = Move;
			m_Next   = null;
		}
		public TMState GetNext() 
		{
			return m_Next;
		}

		public void SetNext(TMState next) 
		{
			m_Next = next;
		}

		public string GetStateN() 
		{
			return m_StateN;
		}

		public string GetRead() 
		{
			return m_Read;
		}

		public string GetStateF()
		{
			return m_StateF;
		}

		public string GetWrite() 
		{
			return m_Write;
		}

		public string GetMove() 
		{
			return m_Move;
		}

	}
}
