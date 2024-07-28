namespace MysteryCompanion
{
    public static class Guest
    {
        public static void Run()
        {
            Program.WriteLine("MysteryCompanion - As Guest - Version 1.0.0\n");

            System.Net.HttpListener listener = new System.Net.HttpListener();
            listener.Prefixes.Add(Program.PostBinding);
            listener.Start();

            Program.WriteLine($"Bound to \"{Program.PostBinding}\" and waiting for execute signal.\n");

            while (true)
            {
                try
                {
                    System.Net.HttpListenerContext context = listener.GetContext();
                    System.Net.HttpListenerRequest request = context.Request;
                    System.Net.HttpListenerResponse response = context.Response;

                    if (request.HttpMethod != "POST")
                    {
                        continue;
                    }

                    System.IO.StreamReader reader = new System.IO.StreamReader(request.InputStream);
                    string command = reader.ReadToEnd();
                    reader.Dispose();

                    Program.WriteLine("Received signal.");

                    byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes("Received");
                    response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                    response.Close();

                    Program.WriteLine("Acknowledged signal.");

                    if (command == "MysteryGo")
                    {
                        Program.WriteLine("Downloading file...");
                        System.Net.WebClient webClient = new System.Net.WebClient();
                        byte[] payload = webClient.DownloadData(Program.GetBinding);
                        webClient.Dispose();
                        Program.WriteLine("Sucessfully downloaded file.");

                        Program.WriteLine("Writing payload to file...");
                        System.IO.File.WriteAllBytes(Program.FileDestinationPath, payload);
                        Program.WriteLine("Successfully wrote payload to file.");

                        Program.WriteLine("Executing file...");
                        System.Diagnostics.Process.Start(Program.FileDestinationPath);
                        Program.WriteLine("Successfully executed file.");
                    }
                    else
                    {
                        throw new System.Exception($"Invalid command \"{command}\".");
                    }

                    System.Console.WriteLine();
                }
                catch (System.Exception ex)
                {
                    Program.WriteError(ex);
                }
            }
        }
    }
}