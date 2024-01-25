namespace MysteryCompanion
{
    public static class Host
    {
        public static void Run()
        {
            Program.WriteLine("MysteryCompanion - As Host - Version 1.0.0\n");

            System.Net.HttpListener listener = new System.Net.HttpListener();
            listener.Prefixes.Add(Program.GetBinding);
            listener.Start();

            Program.WriteLine($"Bound to \"{Program.GetBinding}\" and serving file.");

            while (true)
            {
                try
                {
                    System.Net.HttpListenerContext context = listener.GetContext();

                    Program.WriteLine("Received upload request...");

                    System.Net.HttpListenerResponse response = context.Response;

                    System.IO.FileStream fileStream = System.IO.File.Open(Program.FileSourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                    response.ContentLength64 = fileStream.Length;
                    response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    response.AddHeader("Content-Disposition", $"attachment; filename=\"{System.IO.Path.GetFileName(Program.FileSourcePath)}\"");

                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        response.OutputStream.Write(buffer, 0, bytesRead);
                    }

                    fileStream.Close();
                    fileStream.Dispose();

                    response.Close();

                    Program.WriteLine("Successfully uploaded file.");
                }
                catch (System.Exception ex)
                {
                    Program.WriteError(ex);
                }
            }
        }
    }
}