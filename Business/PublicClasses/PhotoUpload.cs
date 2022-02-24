using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.PublicClasses
{
    public class PhotoUpload
    {
        public static string CreateImg(IFormFile file)
        {
            try
            {
                string imgname = GenerateCode.GuidCode() + Path.GetExtension(file.FileName);
                string ImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", imgname);

                using (var stream = new FileStream(ImgPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return imgname;
            }
            catch (Exception)
            {
                return "false";
            }

        }

        public static bool DeleteImg(string imgname)
        {
            try
            {
                string Fullpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/" + imgname);
                if (File.Exists(Fullpath))
                {
                    File.Delete(Fullpath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }

    public class GenerateCode
    {
        public static string ActiveCode()
        {
            Random random = new Random();

            return random.Next(100000, 999000).ToString();
        }

        public static string GuidCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
