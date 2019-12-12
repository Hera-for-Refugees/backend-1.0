using System;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class StringExtensions
{
    public static bool ContainsWithSensitive(this string text, string value,
        StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
        return text.IndexOf(value, stringComparison) >= 0;
    }
    public static string ToTitleCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        TextInfo textInfo = new CultureInfo("tr-TR", false).TextInfo;
        return textInfo.ToTitleCase(text.ToLower()).Replace("Empty", "");
    }

    public static string ReplaceTurkishChar(this string text, bool isUpper)
    {
        if (isUpper)
            return text.ToUpper().Replace("Ç", "C").Replace("Ğ", "G").Replace("İ", "I").Replace("Ö", "O").Replace("Ü", "U").Replace("Ş", "S");
        else
            return text.Replace("Ç", "C").Replace("Ğ", "G").Replace("İ", "I").Replace("Ö", "O").Replace("Ü", "U").Replace("Ş", "S")
                .Replace("Ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ü", "u").Replace("ş", "s");
    }

    public static bool IsDate(this string text)
    {
        if (DateTime.TryParse(text, out DateTime dt))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public static string ToAbsoluteUrl(this string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return relativeUrl;

        if (HttpContext.Current == null)
            return relativeUrl;

        if (relativeUrl.StartsWith("/"))
            relativeUrl = relativeUrl.Insert(0, "~");
        if (!relativeUrl.StartsWith("~/"))
            relativeUrl = relativeUrl.Insert(0, "~/");

        var url = HttpContext.Current.Request.Url;
        var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

        return String.Format("{0}://{1}{2}{3}",
                             url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
    }

    public static DateTime ToDate(this string dateStr)
    {
        try
        {
            return DateTime.ParseExact(dateStr, Hera.Core.Cache.Keys.DEFAULT_FORMAT, CultureInfo.InvariantCulture);
        }
        catch
        {
            return Convert.ToDateTime("01/01/1001");
        }
    }
    public static DateTime ToDateConvert(this string dateStr)
    {
        try
        {
            return Convert.ToDateTime(dateStr);
        }
        catch
        {
            return Convert.ToDateTime("01/01/1001");
        }
    }
    public static bool ContainsRegex(this string text, string search)
    {
        return Regex.IsMatch(text, search, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// String ifadeye belirtilen formattaki parametreleri set eder
    /// </summary>
    /// <param name="value">Merhaba {0} {1},</param>
    /// <param name="args">new []{"Ali","Veli"}</param>
    /// <returns></returns>
    public static string Format(this string value, params object[] args)
    {
        return string.Format(value, args);
    }

    /// <summary>
    /// String ifadenin maskelenmiş halini return eder. "05544997363".FormatWithMask(####-###-##-##)
    /// </summary>
    /// <param name="mask">örn : tel => ####-###-##-##</param>
    /// <returns></returns>
    public static string FormatWithMask(this string input, string mask)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var output = string.Empty;
        var index = 0;
        foreach (var m in mask)
        {
            if (m == '#')
            {
                if (index < input.Length)
                {
                    output += input[index];
                    index++;
                }
            }
            else
                output += m;
        }
        return output;
    }

    /// <summary>
    /// String ifadeyi belirtilen key değerine göre RSA ile şifreler
    /// </summary>    
    /// <param name="key">Key değeri, Decrypt işleminde gerekli olacağından stabil yada kayıt altında olan bir değer belirtiniz. (Row Id, Data Id, PrimaryKey gibi) </param>
    /// <returns></returns>
    public static string Encrypt(this string stringToEncrypt, string key)
    {
        if (string.IsNullOrEmpty(stringToEncrypt))
        {
            throw new ArgumentException("An empty string value cannot be encrypted.");
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Cannot encrypt using an empty key. Please supply an encryption key.");
        }
        CspParameters cspp = new CspParameters();
        cspp.KeyContainerName = key;
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
        rsa.PersistKeyInCsp = true;
        byte[] bytes = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(stringToEncrypt), true);
        return BitConverter.ToString(bytes);
    }
    /// <summary>
    /// RSA ile şifrelenmiş değeri Encrypt eder. Şifreleme esnasında kullanılan key değerini parametre olarak göndermeniz gerekli.
    /// </summary>
    /// <param name="key">Encrypt edilirken kullanılan key değeri</param>
    /// <returns></returns>
    public static string Decrypt(this string stringToDecrypt, string key)
    {
        string result = null;

        if (string.IsNullOrEmpty(stringToDecrypt))
        {
            throw new ArgumentException("An empty string value cannot be encrypted.");
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Cannot decrypt using an empty key. Please supply a decryption key.");
        }
        try
        {
            CspParameters cspp = new CspParameters();
            cspp.KeyContainerName = key;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            string[] decryptArray = stringToDecrypt.Split(new string[] { "-" }, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray, (s => Convert.ToByte(byte.Parse(s, System.Globalization.NumberStyles.HexNumber))));
            byte[] bytes = rsa.Decrypt(decryptByteArray, true);
            result = System.Text.UTF8Encoding.UTF8.GetString(bytes);
        }
        finally
        {
            // no need for further processing
        }
        return result;
    }

    public static bool IsNumeric(this string theValue)
    {
        long retNum;
        return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
    }

    #region HashExtension
    public enum eHashType
    {
        HMAC, HMACMD5, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512,
        MACTripleDES, MD5, RIPEMD160, SHA1, SHA256, SHA384, SHA512
    }

    private static byte[] GetHash(string input, eHashType hash)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);

        switch (hash)
        {
            case eHashType.HMAC:
                return HMAC.Create().ComputeHash(inputBytes);
            case eHashType.HMACMD5:
                return HMACMD5.Create().ComputeHash(inputBytes);
            case eHashType.HMACSHA1:
                return HMACSHA1.Create().ComputeHash(inputBytes);
            case eHashType.HMACSHA256:
                return HMACSHA256.Create().ComputeHash(inputBytes);
            case eHashType.HMACSHA384:
                return HMACSHA384.Create().ComputeHash(inputBytes);
            case eHashType.HMACSHA512:
                return HMACSHA512.Create().ComputeHash(inputBytes);
            case eHashType.MACTripleDES:
                return MACTripleDES.Create().ComputeHash(inputBytes);
            case eHashType.MD5:
                return MD5.Create().ComputeHash(inputBytes);
            case eHashType.RIPEMD160:
                return RIPEMD160.Create().ComputeHash(inputBytes);
            case eHashType.SHA1:
                return SHA1.Create().ComputeHash(inputBytes);
            case eHashType.SHA256:
                return SHA256.Create().ComputeHash(inputBytes);
            case eHashType.SHA384:
                return SHA384.Create().ComputeHash(inputBytes);
            case eHashType.SHA512:
                return SHA512.Create().ComputeHash(inputBytes);
            default:
                return inputBytes;
        }
    }
    /// <summary>
    /// Seçilen hash algoritmasına göre string ifadeyi hashler
    /// </summary>
    /// <param name="input">Hashlenecek olan string ifade</param>
    /// <param name="hashType">Hash algoritması</param>
    /// <returns>Result olarak hashlenmiş değer döner, hata var ise string.Empty döner</returns>
    public static string ComputeHash(this string input, eHashType hashType)
    {
        try
        {
            byte[] hash = GetHash(input, hashType);
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                ret.Append(hash[i].ToString("x2"));
            return ret.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }
    #endregion

    public static string ToPassword(this string pass)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] btr = Encoding.UTF8.GetBytes(pass);
        btr = md5.ComputeHash(btr);
        StringBuilder sb = new StringBuilder();
        foreach (byte ba in btr)
        {
            sb.Append(ba.ToString("x2").ToLower());
        }
        return sb.ToString().ToUpper();
    }
    public static string ToHide(this string text, int length)
    {
        var result = "";
        for (int i = 0; i < length; i++)
        {
            result += "*";
        }
        return result;
    }

    public static string FromManagement(this string text)
    {
        return ConfigurationManager.AppSettings["manageUrl"] + text;
    }
    public static string FromApi(this string text)
    {
        return ConfigurationManager.AppSettings["apiUrl"] + text.Replace("~", "");
    }


    public static string ToSlug(this string text)
    {
        string str = text.RemoveAccent().ToLower();
        // invalid chars           
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        // convert multiple spaces into one space   
        str = Regex.Replace(str, @"\s+", " ").Trim();
        // cut and trim 
        //str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        str = Regex.Replace(str, @"\s", "-"); // hyphens   
        return str;
    }

    static string RemoveAccent(this string txt)
    {
        byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

    public static Boolean IsBase64String(this String base64String)
    {
        if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0 || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            return false;
        try
        {
            var bytes = Convert.FromBase64String(base64String);
            return true;
        }
        catch (Exception)
        {
        }
        return false;
    }

    public static string IsEmpty(this String text, string defaultValue)
    {
        return string.IsNullOrEmpty(text) ? defaultValue : text;
    }
    public static string Check(this String text, string defaultValue)
    {
        return text.Contains(defaultValue) ? "" : text;
    }

}
