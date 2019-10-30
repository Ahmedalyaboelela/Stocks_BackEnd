using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace BAL.Helper
{
    public static class UploadHelper
    {
        // Upload Image,File To Existing Folder 
        public static Dictionary<string, string> SaveFile(IFormFile formFile, string FolderName)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            try
            {
                var file = formFile;
                var folderName = Path.Combine("UploadFiles", FolderName);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                if (file.Length > 0)
                {


                    var _imgname = Guid.NewGuid().ToString();
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    // Updated To GetFileName By Elgendy
                    var _ext = Path.GetFileName(file.FileName);

                    //  var fullPath = Path.Combine(pathToSave, _imgname +_ext);
                    var filepath = Path.Combine(folderName, _imgname + _ext);
                   
                    using (var stream = new FileStream(filepath, FileMode.Create))

                    {
                        file.CopyTo(stream);
                    }
                    string dbPath = "/UploadFiles/" + FolderName + "/" + _imgname + _ext;
                    temp.Add("dbPath", dbPath);
                    temp.Add("_ext", _ext);
                    temp.Add("stat", "done");
                    return temp;

                }
                else
                {
                    temp.Add("dbPath", "");
                    temp.Add("_ext", "");
                    temp.Add("stat", "BadRequest");
                    return temp;
                }
            }
            catch (Exception ex)
            {
                temp.Add("dbPath", "");
                temp.Add("_ext", "");
                temp.Add("stat", "Internal server error" + ex.ToString());
                return temp;
            }
        }
    }
}
