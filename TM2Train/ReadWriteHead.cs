// André Betz
// http://www.andrebetz.de
using System;

namespace TM2Train
{
	/// <summary>
	/// Summary description for ReadWriteHead.
	/// </summary>
	public class ReadWriteHead
	{
		private RailParts m_rp = null;
		private static int m_XRailsCnt = 4;
		private static int m_YRailsCnt = 5;
		private static int m_XReadInput = 3;
		private static int m_YWriteInput = 2;
		private static int m_YWriteOutput = 2;
		private static int m_XReadOutputTrue = 1;
		private static int m_XReadOutputFalse = 3;
		private MyPGM m_ReadWriteHead = null;
		private MyPGM Sprung1 = null;
		private MyPGM Sprung2 = null;
		private MyPGM FlipFlop = null;
		private MyPGM Lazy = null;
		private MyPGM Straight1 = null;
		private MyPGM Straight2 = null;
		private MyPGM Cross = null;
		private MyPGM Curve1 = null;
		private MyPGM Curve2 = null;
		private MyPGM Curve3 = null;

		public MyPGM RWHead
		{
			get
			{
				return m_ReadWriteHead;
			}
		}
		public static int XRailsCnt
		{
			get
			{
				return m_XRailsCnt;
			}
		}
		public static int YRailsCnt
		{
			get
			{
				return m_YRailsCnt;
			}
		}
		public static int XReadInput
		{
			get
			{
				return m_XReadInput;
			}
		}
		public static int YWriteInput
		{
			get
			{
				return m_YWriteInput;
			}
		}
		public static int YWriteOutput
		{
			get
			{
				return m_YWriteOutput;
			}
		}
		public static int XReadOutputTrue
		{
			get
			{
				return m_XReadOutputTrue;
			}
		}
		public static int XReadOutputFalse
		{
			get
			{
				return m_XReadOutputFalse;
			}
		}
		private void GenerateNeededParts(bool Set)
		{
			Sprung1 = m_rp.GetSprung(false);
			Sprung1.Rotate270();
			Sprung2 = m_rp.GetSprung(false);
			Sprung2.MirrorX();
			Sprung2.Rotate90();
			Straight1 = m_rp.GetStraight(0);
			Straight2 = m_rp.GetStraight(1);
			Cross = m_rp.GetCrossing();
			FlipFlop = m_rp.GetFlipFlop(!Set);
			Lazy = m_rp.GetLazy(!Set);
			Lazy.Rotate270();
			Curve1 = m_rp.GetCurve(0);
			Curve2 = m_rp.GetCurve(1);
			Curve3 = m_rp.GetCurve(2);
		}
		private void SetToPosition(MyPGM Part, int x,int y)
		{
			m_ReadWriteHead.CopyAtPos(Part,x*RailParts.Size,y*RailParts.Size);
		}
		public ReadWriteHead(bool Set)
		{
			m_rp = new RailParts();
			GenerateNeededParts(Set);
			m_ReadWriteHead = new MyPGM(m_XRailsCnt*RailParts.Size,m_YRailsCnt*RailParts.Size);
			SetToPosition(FlipFlop,2,0);
			SetToPosition(Curve3,3,0);
			SetToPosition(Sprung1,2,1);
			SetToPosition(Cross,3,1);
			SetToPosition(Curve2,1,1);
			SetToPosition(Straight1,1,2);
			SetToPosition(Straight1,3,2);
			SetToPosition(Sprung2,0,3);
			SetToPosition(Lazy,1,3);
			SetToPosition(Straight2,2,3);
			SetToPosition(Sprung1,3,3);
			SetToPosition(Curve1,0,4);
			SetToPosition(Straight2,1,4);
			SetToPosition(Curve3,2,4);
		}
	}
}
