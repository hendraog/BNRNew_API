namespace my_api.Controllers
{
    public class Dao :Idao
    {
        public string generateRadom()
        {
            return "ss";
        }
    }

    public interface Idao
    {
        public string generateRadom();
    }
}
