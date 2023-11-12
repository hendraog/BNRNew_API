namespace BNRNew_API.CustomeAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JWTRightAttribute : Attribute
    {
        public JWTRightAttribute()
        {
        }
    }
}
