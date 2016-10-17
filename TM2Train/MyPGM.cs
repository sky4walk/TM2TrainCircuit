// André Betz
// http://www.andrebetz.de
using System;
using System.IO;

namespace TM2Train
{
	/// <summary>
	/// Summary description for MyPGM.
	/// </summary>
	public class MyPGM
	{
		#region Variables
		private int m_XSize = 0;
		private int m_YSize = 0;
		private int m_Colordepth = 0;
		private byte[] m_PixelMap = null;
		private char[] m_WhiteSpaces = {' ','\n','\r','\t'};
		private string m_Advertise = "http://www.andrebetz.de";
		#endregion
		#region properties
		public int XSize
		{
			get
			{
				return m_XSize;
			}
		}
		public int YSize
		{
			get
			{
				return m_YSize;
			}
		}
		public int ColorDepth
		{
			get
			{
				return m_Colordepth;
			}
		}
		#endregion
		#region constructors
		public MyPGM()
		{
			m_XSize = 0;
			m_YSize = 0;
			m_Colordepth = 0;
			m_PixelMap = null;
		}
		public MyPGM(string FileName)
		{
			m_XSize = 0;
			m_YSize = 0;
			m_Colordepth = 0;
			m_PixelMap = null;
			Load(FileName);
		}
		public MyPGM(int x,int y)
		{
			m_XSize = x;
			m_YSize = y;
			m_Colordepth = 255;
			m_PixelMap = ClearMap(x,y);
		}
		public MyPGM(int x,int y, byte[] PixMap)
		{
			m_XSize = x;
			m_YSize = y;
			m_Colordepth = 255;
			m_PixelMap = Copy(PixMap);
		}
		public MyPGM(int x,int y, byte[] PixMap,bool bCopyData)
		{
			m_XSize = x;
			m_YSize = y;
			m_Colordepth = 255;
			if(bCopyData)
				m_PixelMap = Copy(PixMap);
			else
				m_PixelMap = PixMap;
		}
		public MyPGM(MyPGM picture)
		{
			m_XSize = picture.XSize;
			m_YSize = picture.YSize;
			m_Colordepth = picture.ColorDepth;
			m_PixelMap = Copy(picture);
		}
		#endregion
		#region LoadRoutines
		public bool Load(string FileName)
		{
			BinaryReader br = null;
			byte[] Data = null;
			try
			{
				FileStream fs = new FileStream(FileName,FileMode.Open,FileAccess.Read);					
				br = new BinaryReader(fs);
				Data = br.ReadBytes((int)fs.Length);
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

			if(!ParseFile(Data,ref m_XSize,ref m_YSize, ref m_PixelMap))
			{
				m_XSize = 0;
				m_YSize = 0;
				m_PixelMap = null;
				return false;
			}

			return true;
		}
		private string GenValue(int x,int y)
		{
			int Len = 4;
			string temp = "";
			temp += GetValue(x,y);
			string Spaces = "";
			for(int i=0;i<Len-temp.Length;i++)
			{
				Spaces += " ";
			}
			return Spaces + temp;
		}
		public bool Write(string FileName)
		{
			string P2File = "";
			P2File += "P2\r\n# "+m_Advertise+"\r\n"+m_XSize+" "+m_YSize+"\r\n"+m_Colordepth+"\r\n";
			for(int i=0;i<m_YSize;i++)
			{
				for(int j=0;j<m_XSize;j++)
				{
					P2File += GenValue(j,i);
				}
				P2File += "\r\n";
			}

			BinaryWriter br = null;
			try
			{
				if(File.Exists(FileName))
				{
					File.Delete(FileName);
				}
				FileStream fs = new FileStream(FileName,FileMode.CreateNew,FileAccess.Write);					
				br = new BinaryWriter(fs);
				br.Write(P2File.ToCharArray());
				br.Flush();
			}
			catch(Exception e)
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

			return true;
		}
		private bool IsWhiteSpace(byte Sign)
		{
			for(int i=0;i<m_WhiteSpaces.Length;i++)
			{
				if(m_WhiteSpaces[i]==Sign)
				{
					return true;
				}
			}
			return false;
		}
		private int DeleteWhiteSpaces(byte[] Data,int Pos)
		{
			int NewPos = Pos;
			while(NewPos<Data.Length && IsWhiteSpace(Data[NewPos]))
			{
				NewPos++;
			}
			return NewPos;
		}
		private string ReadTilWhiteSpaces(byte[] Data,ref int Pos)
		{
			string Value = "";
			while(Pos<Data.Length && !IsWhiteSpace(Data[Pos]))
			{
				Value += (char)Data[Pos];
				Pos++;
			}
			return Value;
		}
		private int SkipComment(byte[] Data,int Pos)
		{
			int NewPos = Pos;
			while(NewPos<Data.Length && !(Data[NewPos]=='\n' || Data[NewPos]=='\r'))
			{
				NewPos++;
			}
			return NewPos;
		}
		private bool ParseFile(byte[] Data, ref int m_XSize,ref int m_YSize, ref byte[] m_PixelMap)
		{
			int FilePos = 0;
			int FileEnd = Data.Length;
			int state = 0;
			int PixelPos = 0;
			bool Res = true;
			while(FilePos<FileEnd)
			{
				FilePos = DeleteWhiteSpaces(Data,FilePos);
				switch(state)
				{
					case 0:
					{
						string Tag = ReadTilWhiteSpaces(Data,ref FilePos);
						if(Tag.Equals("P2"))
						{
							state = 1;
						}
						else
						{
							state = 100;
						}
						break;
					}
					case 1:
					{
						string Tag = ReadTilWhiteSpaces(Data,ref FilePos);
						if(Tag.ToCharArray()[0]=='#')
						{
							FilePos = SkipComment(Data,FilePos);
							state = 1;
						}
						else
						{
							try
							{
								m_XSize = Convert.ToInt32(Tag);
								state = 2;
							}
							catch
							{
								state = 100;
							}
						}
						break;
					}
					case 2:
					{
						string Tag = ReadTilWhiteSpaces(Data,ref FilePos);
						try
						{
							m_YSize = Convert.ToInt32(Tag);
							state = 3;
						}
						catch
						{
							state = 100;
						}
						break;
					}
					case 3:
					{
						string Tag = ReadTilWhiteSpaces(Data,ref FilePos);
						try
						{
							m_Colordepth = Convert.ToInt32(Tag);
							m_PixelMap = new byte[m_XSize*m_YSize];
							state = 4;
						}
						catch
						{
							state = 100;
						}
						break;
					}
					case 4:
					{
						string Tag = ReadTilWhiteSpaces(Data,ref FilePos);
						try
						{
							int Color = Convert.ToInt32(Tag);
							m_PixelMap[PixelPos] = (byte)Color;
							PixelPos++;
							state = 4;
						}
						catch
						{
							state = 100;
						}
						break;
					}
					default:
						Res = false;
						break;
				}
			}
			return Res;
		}
		#endregion
		#region MemoryCopies
		public MyPGM GetCopy()
		{
			return new MyPGM(this);
		}
		public byte GetValue(int i)
		{
			if(i<m_PixelMap.Length)
			{
				return m_PixelMap[i];
			}
			return 0;
		}
		public byte GetValue(int x,int y)
		{
			int Pos = y*m_XSize+x;
			if(Pos<m_PixelMap.Length && x<m_XSize && y<m_YSize)
			{
				return m_PixelMap[Pos];
			}
			return 0;
		}
		public void SetValue(int x,int y, byte Value)
		{
			int Pos = y*m_XSize+x;
			if(Pos<m_PixelMap.Length && x<m_XSize && y<m_YSize)
			{
				m_PixelMap[Pos] = Value;
			}
		}
		private byte[] Copy(MyPGM Pixmap)
		{
			byte[] NewPixMap = new byte[Pixmap.XSize*Pixmap.YSize];
			for(int i=0;i<NewPixMap.Length;i++)
			{
				NewPixMap[i] = Pixmap.GetValue(i);
			}
			return NewPixMap;
		}
		private byte[] Copy(byte[] Pixmap)
		{
			byte[] NewPixMap = new byte[Pixmap.Length];
			for(int i=0;i<NewPixMap.Length;i++)
			{
				NewPixMap[i] = Pixmap[i];
			}
			return NewPixMap;
		}
		private byte[] ClearMap(int x, int y)
		{
			byte[] NewPixMap = new byte[x*y];
			for(int i=0;i<NewPixMap.Length;i++)
			{
				NewPixMap[i] = 0;
			}
			return NewPixMap;
		}
		public void CopyAtPos(MyPGM pgm,int x, int y)
		{
			for(int i=0;i<pgm.YSize;i++)
			{
				for(int j=0;j<pgm.XSize;j++)
				{
					this.SetValue(x+j,y+i,pgm.GetValue(j,i));
				}
			}
		}
		#endregion
		#region Transforms
		public void MirrorX()
		{
			for(int i=0;i<(m_YSize/2);i++)
			{
				for(int j=0;j<m_XSize;j++)
				{
					byte temp = GetValue(j,i);
					SetValue(j,i,GetValue(j,m_YSize-i-1));
					SetValue(j,m_YSize-i-1,temp);
				}
			}
		}
		public void MirrorY()
		{
			this.Rotate90();
			this.MirrorX();
			this.Rotate270();
		}
		public void Rotate90()
		{
			MyPGM temp = new MyPGM(m_XSize,m_YSize,m_PixelMap,false);
			
			m_XSize = temp.YSize;
			m_YSize = temp.XSize;
			m_PixelMap = new byte[m_XSize*m_YSize];

			for(int i=0;i<temp.YSize;i++)
			{
				for(int j=0;j<temp.XSize;j++)
				{
					SetValue(m_XSize-i-1,j,temp.GetValue(j,i));
				}
			}
		}
		public void Rotate180()
		{
			this.Rotate90();
			this.Rotate90();
		}
		public void Rotate270()
		{
			this.Rotate180();
			this.Rotate90();
		}
		#endregion
	}
}
