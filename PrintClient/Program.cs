using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PrintClient;
using System.Drawing.Printing;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader()
                                                  .AllowAnyMethod();
                      });
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);



app.MapPost("/print", ([FromBody] List<PrintData> data) =>
{
    var printerName = (string) builder.Configuration.GetValue(typeof(string),"printerName1");
    if(printerName == null)
    {
        PrinterSettings settings = new PrinterSettings();
        printerName = settings.PrinterName;
    }

    Printer printer = new Printer(printerName, "utf-8");

    Thread.Sleep(100);
    foreach (var item in data)
    {
        if (item.type.Equals("qr") || item.type.Equals("data"))
        {
            printer.AlignLeft();
            if (item.align.Equals("center"))
                printer.AlignCenter();
            if (item.align.Equals("right"))
                printer.AlignRight();
        }

        if (item.type.Equals("qr"))
            printer.QrCode(item.data, QrCodeSize.Size2);
        else if (item.type.Equals("data"))
            printer.Append(item.data);
        else if (item.type.Equals("cut"))
            printer.FullPaperCut();
        else if (item.type.Equals("newline"))
            printer.NewLine();
    }
    printer.FullPaperCut();
    printer.PrintDocument();
    return "";
});

app.Run();