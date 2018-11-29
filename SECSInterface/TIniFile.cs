using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Collections; //ArrayList

public class TIniFile
{
    private string m_Path;
    private TStringList TmpList;

    // IniFile Constructor.
    public TIniFile(string strIniPath)
    {
        m_Path = strIniPath;

        TmpList = new TStringList();
        TmpList.Clear();
        if (File.Exists(m_Path)) TmpList.LoadFromFile(m_Path);
    }

    //~TIniFile()
    public void Close()
    {
        TmpList.SaveToFile(m_Path);
    }


    public string ReadWriteString(string str_sec, string str_key, string str_default)
    {
        string r = ReadString(str_sec, str_key, str_default);
        WriteString(str_sec, str_key, r);
        return r;
    }
    public int ReadWriteInteger(string str_sec, string str_key, int int_default)
    {
        string rr = ReadString(str_sec, str_key, int_default.ToString());
        int r = Convert.ToInt32(rr);  //或 r = int32.Parse(rr);
        WriteString(str_sec, str_key, r.ToString());
        return r;
    }
    public bool ReadWriteBool(string str_sec, string str_key, bool bool_default)
    {
        string rr;
        bool r = false;
        if (bool_default)
            rr = ReadString(str_sec, str_key, "1");
        else
            rr = ReadString(str_sec, str_key, "0");

        if (rr.Trim() == "1") r = true;

        if (r)
        {
            WriteString(str_sec, str_key, "1");
        }
        else
        {
            WriteString(str_sec, str_key, "0");
        }

        return r;
    }


    public void UpdateFile()
    {
        TmpList.SaveToFile(m_Path);
    }

    public void DeleteKey(string str_sec, string str_key)
    {
        int dest_idx;
        dest_idx = FindSec(str_sec);
        if (dest_idx >= 0)
        {
            dest_idx = FindKey(str_key, dest_idx);
            if (dest_idx >= 0) //有 section 有 key
            {
                TmpList.RemoveAt(dest_idx);
            }
        }

    }

    public void EraseSection(string str_sec)
    {
        int from_idx;
        int dest_idx;
        from_idx = FindSec(str_sec);
        dest_idx = FindSecLastIdx(str_sec);

        for (int i = from_idx; i <= dest_idx; i++)
        {
            TmpList.RemoveAt(from_idx);
        }
    }

    public void WriteInteger(string str_sec, string str_key, int int_value)
    {
        WriteString(str_sec, str_key, int_value.ToString());
    }
    public int ReadInteger(string str_sec, string str_key, int int_default)
    {
        string rr = ReadString(str_sec, str_key, int_default.ToString());
        int r = Convert.ToInt32(rr);  //或 r = int32.Parse(rr);
        return r;

    }

    public void WriteBool(string str_sec, string str_key, bool bool_value)
    {
        if (bool_value)
        {
            WriteString(str_sec, str_key, "1");
        }
        else
        {
            WriteString(str_sec, str_key, "0");
        }
    }
    public bool ReadBool(string str_sec, string str_key, bool bool_default)
    {
        string rr;
        bool r = false;
        if (bool_default)
            rr = ReadString(str_sec, str_key, "1");
        else
            rr = ReadString(str_sec, str_key, "0");

        if (rr.Trim() == "1") r = true;

        return r;
    }


    public void WriteString(string str_sec, string str_key, string str_value)
    {
        int dest_idx;
        int tmp_idx;
        dest_idx = FindSec(str_sec);
        if (dest_idx >= 0)
        {
            dest_idx = FindKey(str_key, dest_idx);
            if (dest_idx >= 0) //有 section 有 key
            {
                TmpList[dest_idx] = str_key + "=" + str_value;
            }
            else //有 section 沒 key
            {
                tmp_idx = FindSecLastIdx(str_sec);
                //TmpList.Insert(tmp_idx,str_key+"="+str_value);

                if (tmp_idx != TmpList.Count - 1)
                {
                    TmpList.Insert(tmp_idx + 1, str_key + "=" + str_value);
                }
                else
                {
                    TmpList.Add(str_key + "=" + str_value);
                }
            }
        }
        else //沒 section 沒 key
        {
            TmpList.Add("[" + str_sec + "]");
            TmpList.Add(str_key + "=" + str_value);
        }
    }


    public string ReadString(string str_sec, string str_key, string str_default)
    {
        string r = str_default;
        int dest_idx;
        dest_idx = FindSec(str_sec);
        if (dest_idx >= 0)
        {
            dest_idx = FindKey(str_key, dest_idx);
            if (dest_idx >= 0) //有 section 有 key
            {
                string[] tmp = TmpList[dest_idx].ToString().Split('=');
                r = tmp[1];
            }
        }

        return r;

    }

    private int FindSec(string str_sec)
    {
        int r = -1;
        string str_sec_label = "[" + str_sec.ToUpper() + "]";
        for (int i = 0; i <= TmpList.Count - 1; i++)
        {
            if (TmpList[i].ToString().ToUpper() == str_sec_label)
            {
                r = i;
                break;
            }
        }
        return r;
    }

    private int FindKey(string str_key, int from_idx)
    {
        int r = -1;   //bruce0211
        for (int i = from_idx + 1; i <= TmpList.Count - 1; i++)
        {
            if (TmpList[i].ToString() != "")
            {
                if (TmpList[i].ToString().Substring(0, 1) == "[") //遇到下一個節區
                {
                    break;
                }
            }

            string[] tmp = TmpList[i].ToString().Split('=');
            if (tmp[0].Trim().ToUpper() == str_key.Trim().ToUpper())
            {
                r = i;
                break;
            }
        }
        return r;
    }

    private int FindSecLastIdx(string str_sec) //找到某一 section 區最後位置
    {
        int r = -1;  //bruce0211
        int dest_idx = FindSec(str_sec);
        if (dest_idx >= 0)
        {
            for (int i = dest_idx + 1; i <= TmpList.Count - 1; i++)
            {
                if (TmpList[i].ToString().Substring(0, 1) == "[") //遇到下一個節區
                {
                    r = i - 1;
                    break;
                }
            }

            if (r == -1) //沒遇到下一個節區
            {
                r = TmpList.Count - 1;
            }

        }
        else
        {
            r = TmpList.Count - 1;
        }

        return r;

    }


}


//-------------------------------------------------
//TStringList
//-------------------------------------------------
//使用例
//TStringList TmpList = new TStringList();
//TmpList.Add("bruce");
//TmpList.Add("bruce 0211");
//TmpList.SaveToFile("bruce_0211.txt");  
//
//TmpList.LoadFromFile("bruce_0211.txt");  
//for (int i=0; i<=TmpList.Count-1; i++)
//{
//  MessageBox.Show(TmpList[i]); 
//}
//-------------------------------------------------
public class TStringList : ArrayList
{
    public void SaveToFile(string fn)
    {
        //StreamWriter sw = File.CreateText(fn);
        StreamWriter sw = new StreamWriter(fn, false, System.Text.Encoding.Default); //這樣才能讀入中文字元 System.Text.Encoding.GetEncoding(950)

        for (int i = 0; i != this.Count; i++)
        {
            sw.WriteLine(this[i]);
        }

        sw.Close();
    }

    public void LoadFromFile(string fn)
    {
        /*
         using (StreamReader sr = new StreamReader("TestFile.txt")) 
        {
         string line;
         // Read and display lines from the file until the end of 
         // the file is reached.
         while ((line = sr.ReadLine()) != null) 
         {
          Console.WriteLine(line);
         }
        }
        */

        this.Clear();
        /*
        StreamReader sr = new StreamReader(File.OpenRead(fn));
        //或 StreamReader sr = new StreamReader(File.Open(fn, FileMode.Open));
        */

        //這樣才能讀入中文字元 System.Text.Encoding.GetEncoding(950)
        StreamReader sr = new StreamReader(fn, System.Text.Encoding.Default); //或 System.Text.Encoding.Default


        while (sr.Peek() >= 0)
        {
            this.Add(sr.ReadLine());
        }

        sr.Close();
    }

}

