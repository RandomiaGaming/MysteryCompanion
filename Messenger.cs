namespace MysteryCompanion
{
    public static class Messenger
    {
        public static void Run()
        {
            Program.WriteLine("MysteryCompanion - As Messenger - Version 1.0.0\n");

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.Timeout = new System.TimeSpan(0, 0, 10);

            Program.WriteLine($"Bound to \"{Program.PostBinding}\" and sending execute signal.");

            System.Net.Http.StringContent content = new System.Net.Http.StringContent("MysteryGo");
            System.Net.Http.HttpResponseMessage response = client.PostAsync(Program.PostBinding, content).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (responseContent == "Received")
                {
                    Program.WriteLine("Client executing. Process closing...");
                }
                else
                {
                    throw new System.Exception($"Invalid response \"{responseContent}\"");
                }
            }
            else
            {
                throw new System.Exception($"HTTP code {response.StatusCode}");
            }
        }
    }
}