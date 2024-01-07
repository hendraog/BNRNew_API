namespace PrintClient
{
    public class PrintData
    {
        public  PrintData(string type, string data, string align)
        {
            this.type = type;
            this.data = data;
            this.align = align;
        }

        public PrintData(string type)
        {
            this.type = type;
        }

        public string type { get; set; }
        public string data { get; set; }
        public string align{ get; set; }

    }
}
