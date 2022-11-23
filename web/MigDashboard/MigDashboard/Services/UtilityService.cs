using System.Text;

namespace MigDashboard.Services
{
    public class UtilityService
    {
        public static List<List<string>> LogFileContent(string logFilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var contents = new List<List<string>>();
            try
            {
                var logs = File.ReadAllText(logFilePath, 
                    Encoding.GetEncoding("gb2312")).Split("\n");                

                var segment = new List<string>();
                foreach (var row in logs)
                {
                    if (row.Contains("//////////////////////////////"))
                    {
                        if (segment.Count > 0) contents.Add(segment);
                        segment = new List<string>();
                    }
                    else
                    {
                        segment.Add(row);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The file cant to be read");
                Console.WriteLine(e.Message);
            }

            contents.Reverse();
            return contents;

        }


    }
}
