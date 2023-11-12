using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BNRNew_API.utils
{
    public class JWTHelper
    {
        private static JsonSerializerOptions serializeOption = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string generateJwtToken(string secret,object claimdata,int expiredMinute)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            Dictionary<string, object> claims = new Dictionary<string, object>();
            Type myType = claimdata.GetType();
            List<PropertyInfo> myField = new List<PropertyInfo>(myType.GetTypeInfo().DeclaredProperties);
            myField.ForEach(e => {
                if (e.GetValue(claimdata) != null)
                {
                    var fieldType = e.PropertyType;
                    if (fieldType.IsGenericType && fieldType.Name.Contains("Nullable"))
                    {
                        fieldType = fieldType.GenericTypeArguments[0];
                    }

                    if (fieldType == typeof(DateTime))
                    {
                        var unixTimestamp = (((DateTime)e.GetValue(claimdata)).ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        claims.Add(e.Name, Convert.ToInt64(Math.Floor(unixTimestamp)));
                    }
                    else
                    {
                        claims.Add(e.Name, e.GetValue(claimdata));
                    }
                }
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[] { new Claim("iss", "123") }),
                Expires = DateTime.UtcNow.AddMinutes(expiredMinute),
                NotBefore = DateTime.UtcNow.AddSeconds(-60),
                Claims = claims,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
        public static string generateJwtToken(string secret, object claimdata, DateTime expiredAt)
        {
            return generateJwtToken(secret, claimdata, expiredAt,DateTime.UtcNow);
        }

        public static string generateJwtToken(string secret, object claimdata, DateTime expiredAt, DateTime notBefore)
        {
            var key = Encoding.ASCII.GetBytes(secret);

            Dictionary<string, object> claims = new Dictionary<string, object>();

            Type myType = claimdata.GetType();
            List<PropertyInfo> myField = new List<PropertyInfo>(myType.GetTypeInfo().DeclaredProperties);
            myField.ForEach(e => {
                if (e.GetValue(claimdata) != null)
                {
                    var fieldType = e.PropertyType;
                    if (fieldType.IsGenericType && fieldType.Name.Contains("Nullable"))
                    {
                        fieldType = fieldType.GenericTypeArguments[0];
                    }

                    if (fieldType == typeof(DateTime))
                    {
                        var unixTimestamp = (((DateTime)e.GetValue(claimdata)).ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        claims.Add(e.Name, Convert.ToInt64(Math.Floor(unixTimestamp)));
                    }
                    else
                    {
                        claims.Add(e.Name, e.GetValue(claimdata));
                    }
                }
            });


            return generateJwtTokenRaw(new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature), null, expiredAt, notBefore);
        }


        public static string generateJwtTokenRaw(SigningCredentials signingCredentials, Dictionary<string, object> claims, DateTime expiredAt, DateTime notBefore)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[] { new Claim("iss", "123") }),
                NotBefore = notBefore,
                Expires = expiredAt,
                Claims = claims,
                SigningCredentials = signingCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        ///  Fungsi ini di gunakan untuk melakukan validatasi Token JWT. pada method ini , default nya akan di lakukan validasi dengan
        ///  Validate Audience => false
        ///  ValidateIssuerSigningKey => true
        /// </summary>
        /// <param name="securityKey">securityKey untuk enkripsi JWT token yg akan di lakukan validasi. </param>
        /// <param name="token">JWT Token yg akan di validasi. </param>
        /// <param name="validateLifetime">apakah token JWT perlu di validasi lifetimenya </param>
        /// <param name="validateIssuer">apakah token JWT perlu di validasi issuernya </param>
        /// <param name="skewTime">Nilai skewtime dari token JWT, Skewtime di gunakankan untuk mentolerir validasi pada lifetime dimana jika 2 server berbeda waktunya maka akan di tolerir sebanyak nilai menit yg ada di parameter ini </param>
        /// 
        private static T validateTokenRaw<T>(SecurityKey securityKey, string token, bool validateLifetime, bool validateIssuer, int skewTime) where T : class, new()
        {
            if (token.IsNullOrEmpty())
                return null;

            var dateEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                token = token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = validateIssuer,
                    ValidateAudience = false,
                    ValidateLifetime = validateLifetime,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.FromMinutes(skewTime)
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var result = JsonSerializer.Deserialize<T>(jwtToken.Payload.SerializeToJson(), serializeOption);

                return result;
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        ///  Fungsi ini di gunakan untuk melakukan validatasi Token JWT. pada method ini , default nya akan di lakukan validasi dengan
        ///  Validasi lifetime => true 
        ///  validasi Issuer => false 
        ///  Validate Audience => false
        ///  ValidateIssuerSigningKey => true
        ///  skew time => 5 menit
        /// </summary>
        /// <param name="secret">Secret/Password untuk enkripsi JWT token yg akan di lakukan validasi. </param>
        /// <param name="token">JWT Token yg akan di validasi. </param>
        /// 
        public static T validateToken<T>(string secret, string token) where T : class, new()
        {
            var key = Encoding.ASCII.GetBytes(secret);
            return validateTokenRaw<T>(new SymmetricSecurityKey(key), token, true, false,5);
        }

        /// <summary>
        ///  Fungsi ini di gunakan untuk melakukan validatasi Token JWT. pada method ini , default nya akan di lakukan validasi dengan
        ///  Validate Audience => false
        ///  ValidateIssuerSigningKey => true
        /// </summary>
        /// <param name="secret">secret untuk enkripsi JWT token yg akan di lakukan validasi. </param>
        /// <param name="token">JWT Token yg akan di validasi. </param>
        /// <param name="validateLifetime">apakah token JWT perlu di validasi lifetimenya </param>
        /// <param name="validateIssuer">apakah token JWT perlu di validasi issuernya </param>
        /// <param name="skewTime">Nilai skewtime dari token JWT, Skewtime di gunakankan untuk mentolerir validasi pada lifetime dimana jika 2 server berbeda waktunya maka akan di tolerir sebanyak nilai menit yg ada di parameter ini </param>
        /// 
        private static T validateToken<T>(string secret, string token, bool validateLifetime, bool validateIssuer, int skewTime) where T : class, new()
        {
            var key = Encoding.ASCII.GetBytes(secret);
            return validateTokenRaw<T>(new SymmetricSecurityKey(key), token, validateLifetime, validateIssuer, skewTime);
        }

        /// <summary>
        ///  Fungsi ini di gunakan untuk melakukan validatasi Token JWT yg di signing menggunakan asymetric key. pada method ini , default nya akan di lakukan validasi dengan
        ///  Validate Audience => false
        ///  ValidateIssuerSigningKey => true
        /// </summary>
        /// <param name="publicKey">publicKey untuk decrypt JWT token yg akan di lakukan validasi. </param>
        /// <param name="token">JWT Token yg akan di validasi. </param>
        /// <param name="validateLifetime">apakah token JWT perlu di validasi lifetimenya </param>
        /// <param name="validateIssuer">apakah token JWT perlu di validasi issuernya </param>
        /// <param name="skewTime">Nilai skewtime dari token JWT, Skewtime di gunakankan untuk mentolerir validasi pada lifetime dimana jika 2 server berbeda waktunya maka akan di tolerir sebanyak nilai menit yg ada di parameter ini </param>
        /// 
        public static T validateTokenAsymetric<T>(string publicKey, string token, bool validateLifetime, bool validateIssuer, int skewTime) where T : class, new()
        {
            byte[] data = Convert.FromBase64String(publicKey);

            string jwt = System.Text.Encoding.ASCII.GetString(data);

            var rsaPublicKey = JsonSerializer.Deserialize<RSAParameters>(jwt,serializeOption);

            return validateTokenRaw<T>(new RsaSecurityKey(rsaPublicKey), token, validateLifetime, validateIssuer, skewTime);
        }

        public static T decodeContent<T>(string token) where T : class, new()
        {
            var result = new T();
            token = token.Replace("Bearer", "").Trim();

            Regex r = new Regex(@"(\w*).(\w*).(\w*)", RegexOptions.IgnoreCase);
            Match m = r.Match(token);

            var token2 = m.Groups[2].Value.Replace('_', '/').Replace('-', '+');
            switch (token2.Length % 4)
            {
                case 2: token2 += "=="; break;
                case 3: token2 += "="; break;
            }

            var base64EncodedBytes = System.Convert.FromBase64String(token2);
            var jsonStr = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            T satdikProfile = JsonSerializer.Deserialize<T>(jsonStr,serializeOption);
            return satdikProfile;

        }
    }



}
