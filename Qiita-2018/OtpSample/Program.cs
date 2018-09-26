using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OtpSample
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
            using (var client = new HttpClient())
            {
                // プロダクトトークンはここから確認する: https://gateway.cmtelecom.com
                client.DefaultRequestHeaders.Add("X-CM-ProductToken", "ここにプロダクトトークンを入力");

                var request = JsonConvert.SerializeObject(new {
                        length = 5,
                        expiry = 120,
						// 送信者名に関しては下記＊１を確認
                        sender = "送信者名をここに入力",
                        recipient = "+8180<ここに電話番号>",
						message = "パスコードは {code} です。",
                    }
                );

                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.cmtelecom.com/v1.0/otp/generate", content);
                response.EnsureSuccessStatusCode();
                var generateOtpResponse =
                    JsonConvert.DeserializeObject<GenerateOtpResponse>(await response.Content.ReadAsStringAsync());

                Console.WriteLine($"Generated OTP ID: {generateOtpResponse.id}");

                for(var verified = false;!verified;)
                {
                    Console.Write("Please enter the received code: ");
                    var code = Console.ReadLine();

                    request = JsonConvert.SerializeObject(new {generateOtpResponse.id, code});
                    content = new StringContent(request, Encoding.UTF8, "application/json");
                    response = await client.PostAsync("https://api.cmtelecom.com/v1.0/otp/verify", content);
                    response.EnsureSuccessStatusCode();

                    var verifyOtpResponse = JsonConvert.DeserializeObject<VerifyOtpResponse>(await response.Content.ReadAsStringAsync());
                    if (verifyOtpResponse.valid)
                    {
                        verified = true;
                        Console.WriteLine("Code is OK.");
                    }
                    else
                    {
                        Console.WriteLine("Code is invalid!");
                    }
                }
            }
        }
    }
}