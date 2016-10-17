// André Betz
// http://www.andrebetz.de
using System;
using System.Collections;

namespace TM2Train
{
	/// <summary>
	/// RailParts contains some Parts of basic rails
	/// Straight is a single line connection between two points
	/// Cross is a crossing of two directions lika a bridge
	/// Curve is a direction changing
	/// switch change the direction depending on its direction value there are 3 
	/// different switches:
	/// Lazy:
	/// if the train come from upper it leaves right or goes down, if it comes from
	/// down or right it changes the switch to its direction where it comes from
	/// Sprung:
	/// The direction for a train which comes from upper is fixed to right or down
	/// if it comes from right or down the stored direction is not changed
	/// FlipFlop
	/// if a train passes from Upper/down/right the direction will change after leaving
	/// </summary>
	public class RailParts
	{
		/// <summary>
		/// the type of the RailPart is stored in the first Position of the picture
		/// </summary>
		public enum RailType
		{
			STRAIGHT = 20,
			CROSS	 = RailType.STRAIGHT*2,
			CURVE    = RailType.STRAIGHT*3,
			FLIPFLOP = RailType.STRAIGHT*4,
			LAZY     = RailType.STRAIGHT*5,
			SPRUNG   = RailType.STRAIGHT*6,
			START    = RailType.STRAIGHT*7
	};
		public static int Size
		{
			get
			{
				return 7;
			}
		}
		private byte[] m_Straight = { 0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0};

		private byte[] m_Cross = {    0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  255,255,255,255,255,255,255,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0};

		private byte[] m_Curve = {	0,0,0,255,0,0,0,
								    0,0,0,255,0,0,0,
									0,0,0,0,255,0,0,
									0,0,0,0,0,255,255,
									0,0,0,0,0,0,0,
									0,0,0,0,0,0,0,
									0,0,0,0,0,0,0};

		private byte[] m_Switch =   { 0,0,0,255,0,0,0,
									  0,0,0,255,255,0,0,
									  0,0,0,255,0,255,0,
									  0,0,0,255,0,0,255,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0,
									  0,0,0,255,0,0,0};

		private byte[] m_Start =   {    0,0,0,255,0,0,0,
										0,0,255,255,255,0,0,
										0,255,0,255,0,255,0,
										255,0,0,255,0,0,255,
										0,0,0,255,0,0,0,
										0,0,0,255,0,0,0,
										0,0,0,255,0,0,0};

		private MyPGM GetSwitch(RailType rt,bool Right)
		{
			MyPGM FlipFlopSwitch = new MyPGM(Size,Size,m_Switch);

			FlipFlopSwitch.SetValue(0,0,(byte)rt);
			if(Right)
				FlipFlopSwitch.SetValue(5,1,128);
			else
				FlipFlopSwitch.SetValue(2,1,128);
			
			return FlipFlopSwitch;
		}
		/// <summary>
		/// FlipFlop
		/// If a Train comes from above it goes to direction where it is set (Right or down)
		/// If a Train comes from right the switch is set to reight and train goes up
		/// If a Train comes from down the switch is set to down and train goes up
		/// </summary>
		/// <param name="Right">direction changes to direction where trains come (Right/Down)</param>
		/// <returns></returns>
		public MyPGM GetLazy(bool Right)
		{
			return GetSwitch(RailType.LAZY,Right);
		}
		/// <summary>
		/// Lazy
		/// this switch will ever change the direction after a Train is passing
		/// </summary>
		/// <param name="Right">direction changes after every passing</param>
		/// <returns></returns>
		public MyPGM GetFlipFlop(bool Right)
		{
			return GetSwitch(RailType.FLIPFLOP,Right);
		}
		/// <summary>
		/// If a Train comes from above it goes to direction where it is set (Right or down)
		/// If a Train comes 
		/// </summary>
		/// <param name="Right">direction set once never changed</param>
		/// <returns></returns>
		public MyPGM GetSprung(bool Right)
		{
			return GetSwitch(RailType.SPRUNG,Right);
		}
		/// <summary>
		/// a Connection straight rail
		/// </summary>
		/// <param name="Rot">up/down or left/right</param>
		/// <returns></returns>
		public MyPGM GetStraight(int Rot)
		{
			MyPGM StraightRail = new MyPGM(Size,Size,m_Straight);
			StraightRail.SetValue(0,0,(byte)RailType.STRAIGHT);
			if(Rot>0)
			{
				StraightRail.Rotate90();
			}
			return StraightRail;
		}
		/// <summary>
		/// rails are crossing like tunnel or bridge over another rail
		/// </summary>
		/// <returns></returns>
		public MyPGM GetCrossing()
		{
			MyPGM Crossing = new MyPGM(Size,Size,m_Cross);
			Crossing.SetValue(0,0,(byte)RailType.CROSS);
			return Crossing;
		}
		/// <summary>
		/// RailCurve Change direction
		/// </summary>
		/// <returns></returns>
		public MyPGM GetCurve(int Rot)
		{
			MyPGM Curve = new MyPGM(Size,Size,m_Curve);
			Curve.SetValue(0,0,(byte)RailType.CURVE);
			for(int i=0;i<Rot;i++)
			{
				Curve.Rotate90();
			}
			return Curve;
		}
		/// <summary>
		/// Starter
		/// </summary>
		/// <returns></returns>
		public MyPGM GetStart()
		{
			MyPGM Starter = new MyPGM(Size,Size,m_Start);
			Starter.SetValue(0,0,(byte)RailType.START);
			Starter.Rotate90();
			return Starter;
		}
		/// <summary>
		/// Copy a ARil at a Destination
		/// </summary>
		/// <param name="Dest">Destination Picture</param>
		/// <param name="Src">Pic to copy in</param>
		/// <param name="X">at X Pos</param>
		/// <param name="Y">at Y Pos</param>
		public static void SetField(MyPGM Dest,MyPGM Src,int X,int Y)
		{
			Dest.CopyAtPos(Src,X*RailParts.Size,Y*RailParts.Size);
		}
		public RailParts()
		{
		}
	}
}
