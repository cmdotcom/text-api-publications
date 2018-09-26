using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmsSample
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                MainAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex}");
            }
            finally
            {
                Console.WriteLine("Press any key");
                Console.ReadKey(true);
            }
        }

        private static async Task MainAsync()
        {
            var request = new {
                messages = new { 
				    // プロダクトトークンはここから確認する: https://gateway.cmtelecom.com
                    authentication = new {producttoken = new Guid("ここにプロダクトトークンを入力")},
                    msg = new[] { new {
						// 送信者名に関しては下記＊１を確認
                        from = "送信者名をここに入力",
                        to = new[] { new {number = "+8180<ここに電話番号>"}},
                        body = new { type = "AUTO", content = "これはテストSMSです！"}
                    }}
                }
            };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://gw.cmtelecom.com/v1.0/message", content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStreamAsync());
            }
        }
    }
}
