using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Text;

namespace CerebroXMenAPI.app_data
{
    public class DSH : DirectorySearcher
    {
        public DSH(DirectoryEntry de)
        {
            SearchRoot = de;
        }

        public void setF(byte[] pB)
        {
            Filter = Encoding.UTF7.GetString(pB);
        }
    }

    public class DEDSH : IDisposable
    {
        public DEDSH()
        {
            de = Domain.GetCurrentDomain().GetDirectoryEntry();
        }

        public DirectoryEntry de;

        public void Dispose()
        {
            de.Dispose();
        }
    }

    public class CDSH
    {
        public static string B(string s1, string s2)
        {
            return CryptoNet.DecryptStringAES(s1, s2, CryptoNet.ConfigSalt);
        }
    }

    public class Asim
    {
        public static string GetAsim()
        {
            string v_strFN = @"\\debianfs\natural\PuntoNet\s.h";

            using (FileStream fs = new FileStream(v_strFN, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bdata = new byte[fs.Length];
                fs.Read(bdata, 0, bdata.Length);
                return Encoding.Default.GetString(bdata);
            }
        }
    }
}
