// André Betz
// http://www.andrebetz.de
using System;
using System.IO;

namespace TM2Train
{
	/// <summary>
	/// Summary description for TMLoader.
	/// </summary>
	public class TMLoader
	{
		private TMState Start;
		private TMState Last;
		private string m_actState;
		private Tape Pos;
		private Tape OldPos;
		private Tape Begin;
		private int m_StartPos;

		public TMLoader()
		{
			Pos = null;
			Begin = null;
			Tape.m_PosNumCnt = 0;
		}
		public Tape GetTape
		{
			get
			{
				return Begin;
			}
		}
		public TMState GetStates
		{
			get
			{
				return Start;
			}
		}
		public int StartTapePos
		{
			get
			{
				return m_StartPos;
			}
		}
		public string StartState
		{
			get
			{
				return m_actState;
			}
		}
		public static int CountStates(TMState ts)
		{
			TMState tmpState = ts;
			int cnt = 0;
			while(tmpState!=null)
			{
				cnt++;
				tmpState = tmpState.GetNext();
			}
			return cnt;
		}
		public static int CountTape(Tape tp)
		{
			Tape tmpTape = tp;
			int cnt = 0;
			while(tmpTape!=null)
			{
				cnt++;
				tmpTape = tmpTape.GetNext();
			}
			return cnt;
		}
		public static int FindStateNr(TMState ts,string StateName)
		{
			TMState tmpState = ts;
			int StateNr = 0;
			bool Found = false;
			while(tmpState!=null)
			{
				if(StateName.Equals(tmpState.GetStateF()))
				{
					Found = true;
					break;
				}
				StateNr++;
				tmpState = tmpState.GetNext();
			}
			if(Found)
			{
				return StateNr/2;
			}
			else
			{
				return -1;
			}
		}

		public  bool Load(string FileName) 
		{
			string readinput = "";
			char[] ReadData = null;
			BinaryReader br = null;

			if(FileName==null)
			{
				return false;
			}
			try
			{
				FileStream fs = new FileStream(FileName,FileMode.Open,FileAccess.Read);					
				br = new BinaryReader(fs);
				ReadData = br.ReadChars((int)fs.Length);
			}
			catch
			{
				return false;
			}
			finally
			{
				if(br!=null)
				{
					br.Close();
				}
			}
			System.Text.StringBuilder ChrToStr = new System.Text.StringBuilder();
			ChrToStr.Append(ReadData);
			readinput = ChrToStr.ToString();
			return ParseInputString(readinput);  	
		}
		private void Init(string state,int TpPos) 
		{
			m_actState = state;
			m_StartPos = TpPos;
			OldPos = Pos;
		}
		private Tape AddTape(string sign) 
		{
			if(Pos == null) 
			{
				Pos = new Tape(sign);
				Begin = Pos;
			} 
			else 
			{
				Pos.SetNext(new Tape(sign));
				Pos = Pos.GetNext();
			}
			return Pos;
		}
		private void LoadTape(string Tape) 
		{
			Tape next;
			int len = Tape.Length;
			for(int i=0;i<len;i++) 
			{
				string Value = "";
				Value += Tape.ToCharArray()[i];
				next = AddTape(Value);
				if(i==0) 
				{
					Begin = next;
				}
			}
		}
		private int DelNoSigns(string Input, int spos) 
		{
			int newpos = spos;
			int slen = Input.Length;
			if(newpos<slen) 
			{
				char sign = Input.ToCharArray()[newpos];
				while((sign==' '||sign=='\n'||sign=='\r'||sign=='\t')&&(newpos<slen)) 
				{
					newpos++;
					if(newpos<slen) 
					{
						sign = Input.ToCharArray()[newpos];
					}
				}
			}
			return newpos;
		}
		private int DelComment(string Input, int spos) 
		{
			int newpos = spos;
			int slen = Input.Length;
			char sign = Input.ToCharArray()[newpos];
			while((sign!='\n')&&(newpos<slen)) 
			{
				newpos++;
				if(newpos<slen) 
				{
					sign = Input.ToCharArray()[newpos];
				}
			}
			return newpos;  		
		}
		private TMState AddState(string state1, string read, string state2, string write, string move) 
		{
			if(Start == null) 
			{
				Start = new TMState(state1,read,state2,write,move);
				Last = Start;
			} 
			else 
			{
				Last.SetNext(new TMState(state1,read,state2,write,move));
				Last = Last.GetNext();
			}
			return Last;
		}
		private bool ParseInputString(string Input) 
		{
			int automstate = 0;
			int len = Input.Length;
			int spos = 0;
			char sign;
			bool failure = true;
			string InputRead1 = "";
			string InputRead2 = "";
			string InputRead3 = "";
			string InputRead4 = "";
			string InputRead5 = "";
  	
			while(spos<len) 
			{
				spos = DelNoSigns(Input,spos);
				if(spos<len) 
				{
					sign = Input.ToCharArray()[spos];	
					switch(automstate) 
					{
						case 0:
							if (sign=='(') 
							{
								automstate = 1;
							}
							else if (sign=='#')
							{
								spos = DelComment(Input,spos);
							}
							else 
							{
								automstate = 20;
								break;
							}
							break;
						case 1:
							if(sign==',') 
							{
								automstate = 2;
							}
							else
							{
								InputRead1 += sign; 
							}  	    	
							break;
						case 2:
							if(sign==')') 
							{
								automstate = 3;
								int Value = 0;
								try
								{
									Value = Convert.ToInt32(InputRead2,10);
								}
								catch
								{
									automstate = 20;
								}
								Init(InputRead1,Value);
								InputRead1 = "";
								InputRead2 = "";
							}
							else
							{
								InputRead2 += sign; 
							}  	    	
							break;
						case 3:
							if (sign=='(') 
							{
								automstate = 4;
							}
							else if (sign=='#')
							{
								spos = DelComment(Input,spos);
							}
							else 
							{
								automstate = 20;
							}
							break;
						case 4:
							if(sign==')') 
							{
								automstate = 5;
								LoadTape(InputRead1);
								InputRead1 = "";
							}
							else
							{
								InputRead1 += sign; 
							}  	    	
							break;
						case 5:
							if (sign=='(') 
							{
								automstate = 6;
							}
							else if (sign=='#')
							{
								spos = DelComment(Input,spos);
							}
							else 
							{
								automstate = 20;
							}
							break;
						case 6:
							if(sign==',') 
							{
								automstate = 7;
							}
							else
							{
								InputRead1 += sign; 
							}  	    	
							break;
						case 7:
							if(sign==',') 
							{
								automstate = 8;
							}
							else
							{
								InputRead2 += sign; 
							}  	    	
							break;
						case 8:
							if(sign==',') 
							{
								automstate = 9;
							}
							else
							{
								InputRead3 += sign; 
							}  	    	
							break;
						case 9:
							if(sign==',') 
							{
								automstate = 10;
							}
							else
							{
								InputRead4 += sign; 
							}  	    	
							break;
						case 10:
							if(sign==')') 
							{
								automstate = 5;
								AddState(InputRead1,InputRead2,InputRead3,InputRead4,InputRead5);
								InputRead1 = "";
								InputRead2 = "";
								InputRead3 = "";
								InputRead4 = "";
								InputRead5 = "";
							}
							else
							{
								InputRead5 += sign; 
							}  	    	
							break;

						default:
							failure = false;
							break;
					}
					spos++;
				}
			}
			return failure;  		
		}
	}
}
