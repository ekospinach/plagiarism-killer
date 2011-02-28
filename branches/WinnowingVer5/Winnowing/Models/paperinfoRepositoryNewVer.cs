using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Winnowing.Models
{
    public class paperinfoRepositoryNewVer
    {
        private DBMappingDataContext db = new DBMappingDataContext();
        public List<info> FindAllInfo()
        {
            List<PaperInfo> temp = new List<PaperInfo>();
            temp = (from zz in db.PaperInfo select zz).ToList();
            List<info> result = new List<info>();
            foreach (PaperInfo item in temp){
                info newItem = new info();
                newItem.title = item.Title;
                string[] strList = item.FingerPrint.Split(' ');
                foreach (string x in strList){
                    int value = Convert.ToInt32(x);
                    newItem.fingerPrint.Add(value);
                    
                }
                result.Add(newItem); 
            }

            return result;

        }
    }
}
