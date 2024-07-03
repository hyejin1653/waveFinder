
using System.Net.Sockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
///waveFinderWeb.waveFinder.strData = "$WFWAV,1,2022-04-28 13:20:00,1,900,300,1.7,6.4,245\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,2,900,330,1.7,6.5,355\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,3,900,0,1.8,6.3,20\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,4,900,30,2.2,6.4,65\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,5,1200,300,1.7,6.7,320\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,6,1200,330,1.7,7.0,245\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,7,1200,0,1.8,6.6,20\r\n$WFWAV,1,2022 - 04 - 28 13:20:00,8,1200,30,2.2,6.4,45";
//try
//{
//    TcpClient tc = new TcpClient("127.0.0.1", 8080);
//    NetworkStream stream = tc.GetStream();

//    while (true)
//    {
//        byte[] outbuf = new byte[stream.Length];
//        int nbytes = stream.Read(outbuf, 0, outbuf.Length);
//        string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);
//        waveFinderWeb.waveFinder.strData = output;
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//    Console.ReadLine();
//}

try
{
    string strRecvMsg = "";
    waveFinderWeb.waveFinder.strData = "";

    TcpClient sockClient = new TcpClient("127.0.0.1", 9999);
    NetworkStream ns = sockClient.GetStream();
    StreamReader sr = new StreamReader(ns);
    StreamWriter sw = new StreamWriter(ns);

    while (true)
    {
        strRecvMsg = sr.ReadLine() + "\r\n";
        if (strRecvMsg == null || strRecvMsg == "\r\n")
        {
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(x =>
            {
                x.AllowAnyHeader().
                AllowAnyMethod().
                AllowAnyOrigin().
                SetIsOriginAllowed(origin => true).
                WithOrigins("http://*").
                WithOrigins("https://*").
                AllowCredentials();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();



            app.Run();
        }
        waveFinderWeb.waveFinder.strData += strRecvMsg;
        Console.WriteLine(strRecvMsg);
    }

    sr.Close();
    sw.Close();
    ns.Close();
    sockClient.Close();

    //Console.WriteLine("접속 종료");
    Console.ReadLine();
}
catch (SocketException e)
{
    Console.WriteLine(e.ToString());
}












