using CascBasic.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CascBasic.Classes
{
    public class FileService
    {
        private ApplicationDbContext _db;

        public FileService(ApplicationDbContext context)
        {
            _db = context;
        }
               
        public byte[] GetImageFromDataBase(int id)
        {
            var inst = _db.Institutions.Find(id);
            if (inst == null)
                return null;
            byte[] cover = inst.CollegeCrest;
            return cover;
        }

        #region helpers
        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        #endregion

    }
}