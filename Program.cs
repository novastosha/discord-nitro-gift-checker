using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace Discord_Nitro_Checker
{
    class Program
    {
        static void Main(string[] args)
        {

            Checker checker = new Checker();

            Console.WriteLine("Discord Nitro Gift Checker \n");
            if(args.Length == 0) {

                Console.WriteLine("Please type a nitro gift code!");

                string code = Console.ReadLine();


                if(code.Length == 0) {
                    Console.WriteLine("Code cannot be empty!");
                    return;
                }

                writeCodeState(checker.check(code));

            }else if(args.Length == 1) {
                writeCodeState(checker.check(args[0]));
            }else if(args.Length == 2) {
                string pathRead = args[0];
                string pathWrite = args[1];

                if(FileManager.check(pathRead) && FileManager.check(pathWrite)){
                    
                    string[] readLines = File.ReadAllLines(pathRead);
                    string[] workingCodes = new string[readLines.Length+1];
                    int index = 1;
                    int indexDone = 1;

                    foreach (string line in readLines) {
                        if(indexDone == 5){
                            Thread.Sleep(59000);
                            indexDone = 1;
                        }
                        if(checker.check(line)){
                            workingCodes[index-1] = line;
                            Console.WriteLine("Code: '"+line+"' @ line "+index+" is a valid code!");
                        }else{
                            Console.WriteLine("Code: '"+line+"' @ "+index+" does not work!");
                        }

                        
                        index++;
                        indexDone++;
                    }
                    File.WriteAllLines(pathWrite,workingCodes);
                }
            }
        }

        static void writeCodeState(bool b) {
             switch (b) {
                    case true:
                        Console.WriteLine("Working code!");
                        break;
                    case false:
                        Console.WriteLine("Invalid Gift code!");
                        break;    
            }
        }
    }


    class FileManager
    {
        public static bool check(string path) {
            FileAttributes attr = File.GetAttributes(path);

            if(File.Exists(path) && path.EndsWith(".txt") && !attr.HasFlag(FileAttributes.Directory)) {
                return true;
            }else{
                return false;
            }
        }
    }

    class Checker
    {
        public bool check(string code) {
            bool valid = false;
            using(HttpClient httpClient = new HttpClient()) {
                string codeUrl = "https://discord.com/api/v8/entitlements/gift-codes/" + code + "?with_application=true&with_subscription_plan=true";

                Task<HttpResponseMessage> response = httpClient.GetAsync(codeUrl);
                
                Task<string> responseBody = response.Result.Content.ReadAsStringAsync();

                bool fl = response.Result.StatusCode != HttpStatusCode.NotFound;

                if(fl) {
                    valid = true;
                }else{
                    valid = false;
                }

            }

            return valid;
            
        }
    }
}