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
        public static object SaveFile(IFormFile formFile, string FolderName)
        {
            try
            {

                var file = formFile;
                var folderName = Path.Combine("UploadFiles", FolderName);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                if (file.Length > 0)
                {


                    var _imgname = Guid.NewGuid().ToString();
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var _ext = Path.GetExtension(file.FileName);
                    //  var fullPath = Path.Combine(pathToSave, _imgname +_ext);
                    var filepath = Path.Combine(folderName, _imgname + _ext);

                    using (var stream = new FileStream(filepath, FileMode.Create))

                    {
                        file.CopyTo(stream);
                    }
                    var dbPath = "/UploadFiles/" + FolderName + "/" + _imgname + _ext;
                    return new { dbPath };

                }
                else
                {
                    return "BadRequest";
                }
            }
            catch (Exception ex)
            {
                return "Internal server error" + ex;
            }
        }
    }
}
