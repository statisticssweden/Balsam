namespace Balsam.Api
{
    internal static class DirectoryUtil
    {

        ////Removes all content in the directory
        //public static void EmptyDirectory(string directory)
        //{
        //    var di = new DirectoryInfo(directory);
        //    foreach (var file in di.GetFiles())
        //    {
        //        file.Delete();
        //    }
        //    foreach (var dir in di.GetDirectories())
        //    {
        //        dir.Delete(true);
        //    }
        //}


        /// <summary>
        /// Make sure that the directory exsist
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool AssureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                return true;
            }
            return false;
        }
    }
}