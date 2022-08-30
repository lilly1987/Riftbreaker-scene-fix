// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static List<string> files= new List<string>();

        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("Hello World!");
                Console.WriteLine("===  ===");
                foreach (var arg in args)
                {
                    Console.WriteLine($"arg : {arg}");

                    FileAttributes chkAtt = File.GetAttributes(arg);
                    if ((chkAtt & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        // 디렉토리일 경우
                        //Console.WriteLine("folder");
                        files.AddRange(Directory.GetFiles(arg, "*.scene", SearchOption.AllDirectories));
                    }
                    else
                    {
                        // 파일 일 경우
                        //Console.WriteLine("file");
                        //Console.WriteLine(Path.GetExtension(arg));
                        if (Path.GetExtension(arg) == ".scene")
                        {
                            files.Add(arg);
                        }
                    }
                }
                Console.WriteLine("===  ===");
                string text, result;
                string[] arr;

                StringBuilder sb = new StringBuilder();
                foreach (var file in files)
                {
                    sb.Clear();
                    Console.WriteLine($"file : {file}");
                    text = File.ReadAllText(file);
                    //Console.WriteLine(text);
                    result = text;

                    result = Regex.Replace(result, "\"\"","\"");
                    result = Regex.Replace(result, "\"\\s+\"", "\n");
                    result = Regex.Replace(result, "(\\A\\s*\"|\"\\s*\\z)", string.Empty);
                    result = Regex.Replace(result, "entity_count\\s+\"\\d+\"\\s+", string.Empty);
                    result = Regex.Replace(result, "//editor \"index\\(\\d+\\)\"\\s+", string.Empty);
                    //Console.WriteLine("=== result ===");
                    //Console.WriteLine(result);
                    //Console.WriteLine("=== result ===");

                    arr = Regex.Split(result, "EntityTemplate\\s+");
                    for (int i = 1; i < arr.Length; i++)
                    {
                        //Console.WriteLine("---------");
                        arr[i] = $"//editor \"index({i})\"\nEntityTemplate\n" + arr[i];
                        //Console.WriteLine(arr[i]);
                    }
                    //Console.WriteLine("---------");
                    arr[0] = $"entity_count \"{arr.Length - 1}\"\n" + arr[0];
                    //Console.WriteLine("---------");

                    sb.AppendJoin(string.Empty, arr);
                    //Console.WriteLine("=== result ===");
                    //Console.WriteLine(sb);
                    //Console.WriteLine("=== result ===");

                    File.Move(file, file + ".bak", true);
                    File.WriteAllText(file, sb.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Write("any key end:");
            Console.ReadLine();
        }
    }
}
