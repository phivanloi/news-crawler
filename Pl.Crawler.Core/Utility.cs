using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Pl.Crawler.Core.Constants;
using Pl.Crawler.Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pl.Crawler.Core
{
    public static class Utility
    {
        private static readonly object lockerWriteLog = new object();

        #region Web Helper

        /// <summary>
        /// Kiểm tra xem yêu cầu của client gửi lên có phải là mobile hay không
        /// </summary>
        /// <param name="httpContext">Nội dung yêu cầu từ client</param>
        /// <returns>bool</returns>
        public static bool IsMobileRequest(this HttpContext httpContext)
        {
            string userAgent = httpContext?.Request?.Headers["User-Agent"];
            if (!string.IsNullOrEmpty(userAgent))
            {
                Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                return b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4));
            }
            return false;
        }

        /// <summary>
        /// hỗ trợ Call đến một restfull api
        /// </summary>
        /// <param name="url">Url call method</param>
        /// <param name="method">Thương thức có thể dùng Microsoft.AspNetCore.Http.HttpMethods</param>
        /// <param name="dataToSend">Chỗi string gửi đi</param>
        /// <param name="contentType">Media type</param>
        /// <param name="headers">Thông tin header nếu có</param>
        /// <returns>Task string</returns>
        public static async Task<string> CallRestfulUrlAsync(string url, string method = "GET", string dataToSend = "", string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            try
            {
                using (StringContent stringContent = new StringContent(dataToSend, Encoding.UTF8, contentType))
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", CommonConstants.RequestUserAgent);
                        if (headers?.Count > 0)
                        {
                            foreach (var item in headers)
                            {
                                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                            }
                        }
                        HttpResponseMessage httpResponseMessage = null;
                        if (StringComparer.OrdinalIgnoreCase.Equals(HttpMethods.Get, method))
                        {
                            httpResponseMessage = await httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                        }
                        else if (StringComparer.OrdinalIgnoreCase.Equals(HttpMethods.Post, method))
                        {
                            httpResponseMessage = await httpClient.PostAsync(url, stringContent);
                        }
                        else if (StringComparer.OrdinalIgnoreCase.Equals(HttpMethods.Put, method))
                        {
                            httpResponseMessage = await httpClient.PutAsync(url, stringContent);
                        }
                        else if (StringComparer.OrdinalIgnoreCase.Equals(HttpMethods.Delete, method))
                        {
                            httpResponseMessage = await httpClient.DeleteAsync(url);
                        }
                        else if (StringComparer.OrdinalIgnoreCase.Equals(HttpMethods.Patch, method))
                        {
                            httpResponseMessage = await httpClient.PatchAsync(url, stringContent);
                        }
                        string resultData = httpResponseMessage != null ? Encoding.UTF8.GetString(await httpResponseMessage.Content.ReadAsByteArrayAsync()) : string.Empty;
                        httpResponseMessage?.Dispose();
                        return resultData;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy thông tin trình duyệt của client
        /// </summary>
        /// <param name="httpContext">Nội dung yêu cầu từ client</param>
        /// <returns>string</returns>
        public static string ClientBrowser(this HttpContext httpContext)
        {
            if (httpContext?.Request?.Headers == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            StringValues userAgent = httpContext.Request.Headers["User-Agent"];

            Dictionary<string, string> _versionMap = new Dictionary<string, string> { { "/8", "1.0" }, { "/1", "1.2" }, { "/3", "1.3" }, { "/412", "2.0" }, { "/416", "2.0.2" }, { "/417", "2.0.3" }, { "/419", "2.0.4" }, { "?", "/" } };
            List<MatchExpression> _matchs = new List<MatchExpression> {
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(opera\smini)\/([\w\.-]+)",RegexOptions.IgnoreCase),// Opera Mini
                        new Regex(@"(opera\s[mobiletab]+).+version\/([\w\.-]+)",RegexOptions.IgnoreCase),// Opera Mobi/Tablet
                        new Regex(@"(opera).+version\/([\w\.]+)",RegexOptions.IgnoreCase),// Opera > 9.80
                        new Regex(@"(opera)[\/\s]+([\w\.]+)",RegexOptions.IgnoreCase)// Opera < 9.80
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(opios)[\/\s]+([\w\.]+)",RegexOptions.IgnoreCase)// Opera mini on iphone >= 8.0
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Opera Mini {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"\s(opr)\/([\w\.]+)",RegexOptions.IgnoreCase)// Opera Webkit
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Opera Mini {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(kindle)\/([\w\.]+)",RegexOptions.IgnoreCase),// Kindle
                        new Regex(@"(lunascape|maxthon|netfront|jasmine|blazer)[\/\s]?([\w\.]+)*",RegexOptions.IgnoreCase),// Lunascape/Maxthon/Netfront/Jasmine/Blazer

                        new Regex(@"(avant\s|iemobile|slim|baidu)(?:browser)?[\/\s]?([\w\.]*)",RegexOptions.IgnoreCase), // Avant/IEMobile/SlimBrowser/Baidu
                        new Regex(@"(?:ms|\()(ie)\s([\w\.]+)",RegexOptions.IgnoreCase),// Internet Explorer

                        new Regex(@"(rekonq)\/([\w\.]+)*",RegexOptions.IgnoreCase),// Rekonq
                        new Regex(@"(chromium|flock|rockmelt|midori|epiphany|silk|skyfire|ovibrowser|bolt|iron|vivaldi|iridium|phantomjs)\/([\w\.-]+)",RegexOptions.IgnoreCase), // Chromium/Flock/RockMelt/Midori/Epiphany/Silk/Skyfire/Bolt/Iron/Iridium/PhantomJS
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(trident).+rv[:\s]([\w\.]+).+like\sgecko",RegexOptions.IgnoreCase)// IE11
                    },
                    Action = (Match match) => {
                        string nameAndVersion = match.Value;
                        return $"Ie 11 {nameAndVersion}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(edge)\/((\d+)?[\w\.]+)",RegexOptions.IgnoreCase),// Microsoft Edge
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(yabrowser)\/([\w\.]+)",RegexOptions.IgnoreCase)// Yandex
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Yandex {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(comodo_dragon)\/([\w\.]+)",RegexOptions.IgnoreCase)// Comodo Dragon
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"{nameAndVersion[0].Replace('_',' ')} {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(micromessenger)\/([\w\.]+)",RegexOptions.IgnoreCase)// WeChat
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"WeChat {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"xiaomi\/miuibrowser\/([\w\.]+)",RegexOptions.IgnoreCase)// MIUI Browser
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"MIUI {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"\swv\).+(chrome)\/([\w\.]+)",RegexOptions.IgnoreCase)// Chrome WebView
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"{new Regex("(.+)").Replace(nameAndVersion[0],"$1 WebView")} {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"android.+samsungbrowser\/([\w\.]+)",RegexOptions.IgnoreCase),
                        new Regex(@"android.+version\/([\w\.]+)\s+(?:mobile\s?safari|safari)*",RegexOptions.IgnoreCase)// Android Browser
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Android Browser {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(chrome|omniweb|arora|[tizenoka]{5}\s?browser)\/v?([\w\.]+)",RegexOptions.IgnoreCase),// Chrome/OmniWeb/Arora/Tizen/Nokia
                        new Regex(@"(qqbrowser)[\/\s]?([\w\.]+)",RegexOptions.IgnoreCase)// QQBrowser
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(uc\s?browser)[\/\s]?([\w\.]+)",RegexOptions.IgnoreCase),
                        new Regex(@"ucweb.+(ucbrowser)[\/\s]?([\w\.]+)",RegexOptions.IgnoreCase),
                        new Regex(@"juc.+(ucweb)[\/\s]?([\w\.]+)",RegexOptions.IgnoreCase),// UCBrowser
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Android Browser {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(dolfin)\/([\w\.]+)",RegexOptions.IgnoreCase)// Dolphin
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Dolphin {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"((?:android.+)crmo|crios)\/([\w\.]+)",RegexOptions.IgnoreCase)// Chrome for Android/iOS
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Chrome {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@";fbav\/([\w\.]+);",RegexOptions.IgnoreCase)// Facebook App for iOS
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Facebook {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"fxios\/([\w\.-]+)",RegexOptions.IgnoreCase)// Firefox for iOS
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Firefox {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"version\/([\w\.]+).+?mobile\/\w+\s(safari)",RegexOptions.IgnoreCase)// Mobile Safari
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Mobile Safari {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"version\/([\w\.]+).+?(mobile\s?safari|safari)",RegexOptions.IgnoreCase)// Safari & Safari Mobile
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"{nameAndVersion[1]} {nameAndVersion[0]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"webkit.+?(mobile\s?safari|safari)(\/[\w\.]+)",RegexOptions.IgnoreCase)// Safari < 3.0
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        string version = nameAndVersion[1];
                        return $"{nameAndVersion[0]} {(_versionMap.Keys.Any(m=>m==version) ? _versionMap[version] : version)}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(konqueror)\/([\w\.]+)",RegexOptions.IgnoreCase),// Konqueror
                        new Regex(@"(webkit|khtml)\/([\w\.]+)",RegexOptions.IgnoreCase)
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(navigator|netscape)\/([\w\.-]+)",RegexOptions.IgnoreCase)// Netscape
                    },
                    Action = (Match match) =>{
                        string[] nameAndVersion = match.Value.Split('/');
                        return $"Netscape {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex("(swiftfox)",RegexOptions.IgnoreCase),// Swiftfox
                        new Regex(@"(icedragon|iceweasel|camino|chimera|fennec|maemo\sbrowser|minimo|conkeror)[\/\s]?([\w\.\+]+)",RegexOptions.IgnoreCase),// IceDragon/Iceweasel/Camino/Chimera/Fennec/Maemo/Minimo/Conkeror
                        new Regex(@"(firefox|seamonkey|k-meleon|icecat|iceape|firebird|phoenix)\/([\w\.-]+)",RegexOptions.IgnoreCase),// Firefox/SeaMonkey/K-Meleon/IceCat/IceApe/Firebird/Phoenix
                        new Regex(@"(mozilla)\/([\w\.]+).+rv\:.+gecko\/\d+",RegexOptions.IgnoreCase),// Mozilla
                        new Regex(@"(polaris|lynx|dillo|icab|doris|amaya|w3m|netsurf|sleipnir)[\/\s]?([\w\.]+)",RegexOptions.IgnoreCase),// Polaris/Lynx/Dillo/iCab/Doris/Amaya/w3m/NetSurf/Sleipnir
                        new Regex(@"(links)\s\(([\w\.]+)",RegexOptions.IgnoreCase),// Links
                        new Regex(@"(gobrowser)\/?([\w\.]+)*",RegexOptions.IgnoreCase),// GoBrowser
                        new Regex(@"(ice\s?browser)\/v?([\w\._]+)",RegexOptions.IgnoreCase),// ICE Browser
                        new Regex(@"(mosaic)[\/\s]([\w\.]+)",RegexOptions.IgnoreCase)// Mosaic
                    },
                    Action = NameVersionAction
                },
            };

            foreach (MatchExpression matchItem in _matchs)
            {
                foreach (Regex regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(userAgent))
                    {
                        Match match = regexItem.Match(userAgent);
                        return matchItem.Action(match);
                    }
                }
            }

            return string.Empty;

            string NameVersionAction(Match match)
            {
                string name = new Regex("^[a-zA-Z]+", RegexOptions.IgnoreCase).Match(match.Value).Value;
                if (match.Value.Length > name.Length)
                {
                    string version = match.Value.Substring(name.Length + 1);
                    return $"{name} {version}";
                }
                return name;
            }
        }

        /// <summary>
        /// Lấy thông tin hệ điều hành của client
        /// </summary>
        /// <param name="httpContext">Nội dung yêu cầu từ client</param>
        /// <returns>string</returns>
        public static string ClientOs(this HttpContext httpContext)
        {
            Dictionary<string, string> _versionMap = new Dictionary<string, string> { { "4.90", "ME" }, { "NT3.51", "NT 3.11" }, { "NT4.0", "NT 4.0" }, { "NT 5.0", "2000" }, { "NT 5.1", "XP" }, { "NT 5.2", "XP" }, { "NT 6.0", "Vista" }, { "NT 6.1", "7" }, { "NT 6.2", "8" }, { "NT 6.3", "8.1" }, { "NT 6.4", "10" }, { "NT 10.0", "10" }, { "ARM", "RT" } };
            StringValues userAgent = httpContext.Request.Headers["User-Agent"];
            List<MatchExpression> _matchs = new List<MatchExpression> {
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"microsoft\s(windows)\s(vista|xp)",RegexOptions.IgnoreCase),// Windows (iTunes)
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(windows)\snt\s6\.2;\s(arm)",RegexOptions.IgnoreCase),// Windows RT
                        new Regex(@"(windows\sphone(?:\sos)*)[\s\/]?([\d\.\s]+\w)*",RegexOptions.IgnoreCase),// Windows Phone
                    },
                    Action = (Match match) => {
                        string name = new Regex(@"(^[a-zA-Z]+\s[a-zA-Z]+)",RegexOptions.IgnoreCase).Match(match.Value).Value;
                        if(name.Length<match.Value.Length)
                        {
                            string version = match.Value.Substring(name.Length+1);
                            return $"{name} {(_versionMap.Keys.Any(m=>m==version)? _versionMap[version]:version)}";
                        }
                        return name;
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(windows\smobile|windows)[\s\/]?([ntce\d\.\s]+\w)",RegexOptions.IgnoreCase)
                    },
                    Action = (Match match)=>{
                        string name = new Regex("(^[a-zA-Z]+)",RegexOptions.IgnoreCase).Match(match.Value).Value;
                        if(name.Length<match.Value.Length)
                        {
                            string version = match.Value.Substring(name.Length + 1);
                            return $"{name} {(_versionMap.Keys.Any(m=>m==version)? _versionMap[version]:version)}";
                        }
                        return name;
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(win(?=3|9|n)|win\s9x\s)([nt\d\.]+)",RegexOptions.IgnoreCase)
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        string version = nameAndVersion[1];
                        return $"Windows {(_versionMap.Keys.Any(m=>m==version)? _versionMap[version]:version)}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"\((bb)(10);",RegexOptions.IgnoreCase)// BlackBerry 10
                    },
                    Action = (Match match)=> $"BlackBerry BB10 {match.Value}"
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(blackberry)\w*\/?([\w\.]+)*",RegexOptions.IgnoreCase),// Blackberry
                        new Regex(@"(tizen)[\/\s]([\w\.]+)",RegexOptions.IgnoreCase),// Tizen
                        new Regex(@"(android|webos|palm\sos|qnx|bada|rim\stablet\sos|meego|contiki)[\/\s-]?([\w\.]+)*",RegexOptions.IgnoreCase),// Android/WebOS/Palm/QNX/Bada/RIM/MeeGo/Contiki
                        new Regex("linux;.+(sailfish);",RegexOptions.IgnoreCase)// Sailfish OS
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(symbian\s?os|symbos|s60(?=;))[\/\s-]?([\w\.]+)*",RegexOptions.IgnoreCase)// Symbian
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        return $"Symbian {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"\((series40);",RegexOptions.IgnoreCase)// Series 40
                    },
                    Action = (Match match) => match.Value
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"mozilla.+\(mobile;.+gecko.+firefox",RegexOptions.IgnoreCase)// Firefox OS
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        return $"Firefox OS {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(nintendo|playstation)\s([wids34portablevu]+)",RegexOptions.IgnoreCase),// Nintendo/Playstation
                        new Regex(@"(mint)[\/\s\(]?(\w+)*",RegexOptions.IgnoreCase),// Mint
                        new Regex(@"(mageia|vectorlinux)[;\s]",RegexOptions.IgnoreCase),// Mageia/VectorLinux
                        new Regex(@"(joli|[kxln]?ubuntu|debian|[open]*suse|gentoo|(?=\s)arch|slackware|fedora|mandriva|centos|pclinuxos|redhat|zenwalk|linpus)[\/\s-]?(?!chrom)([\w\.-]+)*",RegexOptions.IgnoreCase),// Joli/Ubuntu/Debian/SUSE/Gentoo/Arch/Slackware
                        new Regex(@"(hurd|linux)\s?([\w\.]+)*",RegexOptions.IgnoreCase),// Hurd/Linux
                        new Regex(@"(gnu)\s?([\w\.]+)*",RegexOptions.IgnoreCase)// GNU
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(cros)\s[\w]+\s([\w\.]+\w)",RegexOptions.IgnoreCase)// Chromium OS
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        return $"Chromium OS {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(sunos)\s?([\w\.]+\d)*",RegexOptions.IgnoreCase)// Solaris
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        return $"Solaris {nameAndVersion[1]}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"\s([frentopc-]{0,4}bsd|dragonfly)\s?([\w\.]+)*",RegexOptions.IgnoreCase),// FreeBSD/NetBSD/OpenBSD/PC-BSD/DragonFly
                        new Regex(@"(haiku)\s(\w+)",RegexOptions.IgnoreCase)
                    },
                    Action = NameVersionAction
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(ip[honead]+)(?:.*os\s([\w]+)*\slike\smac|;\sopera)",RegexOptions.IgnoreCase)// iOS
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        return $"iOS {new Regex(@"\d+(?:\.\d+)*").Match(nameAndVersion[1].Replace("_",".")).Value}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"(mac\sos\sx)\s?([\w\s\.]+\w)*",RegexOptions.IgnoreCase),
                        new Regex(@"(macintosh|mac(?=_powerpc)\s)",RegexOptions.IgnoreCase)// Mac OS
                    },
                    Action = (Match match)=>{
                        string[] nameAndVersion = new string[]{match.Value.Substring(0,match.Value.IndexOf(" ")),match.Value.Substring(match.Value.IndexOf(" ")+1) };
                        return $"Mac OS {nameAndVersion[1].Replace('_','.')}";
                    }
                },
                new MatchExpression{
                    Regexes = new List<Regex>{
                        new Regex(@"((?:open)?solaris)[\/\s-]?([\w\.]+)*",RegexOptions.IgnoreCase),// Solaris
                        new Regex(@"(aix)\s((\d)(?=\.|\)|\s)[\w\.]*)*",RegexOptions.IgnoreCase),// AIX
                        new Regex(@"(plan\s9|minix|beos|os\/2|amigaos|morphos|risc\sos|openvms)",RegexOptions.IgnoreCase),// Plan9/Minix/BeOS/OS2/AmigaOS/MorphOS/RISCOS/OpenVMS
                        new Regex(@"(unix)\s?([\w\.]+)*",RegexOptions.IgnoreCase)// UNIX
                    },
                    Action = NameVersionAction
                }
            };

            foreach (MatchExpression matchItem in _matchs)
            {
                foreach (Regex regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(userAgent))
                    {
                        Match match = regexItem.Match(userAgent);
                        return matchItem.Action(match);
                    }
                }
            }

            return string.Empty;

            string NameVersionAction(Match match)
            {
                string name = new Regex("^[a-zA-Z]+", RegexOptions.IgnoreCase).Match(match.Value).Value;
                return match.Value.Length > name.Length ? $"{name} {match.Value.Substring(name.Length + 1)}" : name;
            }
        }

        /// <summary>
        /// Hàm cho biết request này có phải ajax hay không
        /// </summary>
        /// <param name="httpContext">http context</param>
        /// <returns>bool</returns>
        public static bool IsAjaxRequest(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            return httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        /// <summary>
        /// Lấy đường dẫn liên quan của một Request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>URL liên quan</returns>
        public static string GetUrlReferrer(this HttpContext httpContext)
        {
            try
            {
                string urlReferrer = "";
                if (httpContext?.Request != null && httpContext.Request.Headers != null)
                {
                    urlReferrer = httpContext.Request.Headers["Referer"].ToString();
                }
                return urlReferrer.ToLowerInvariant();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy IP của máy client gửi yêu cầu đến máy chủ
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>Id của máy khách</returns>
        public static string GetCurrentIpAddress(this HttpContext httpContext)
        {
            return httpContext?.Connection != null ? httpContext.Connection.RemoteIpAddress.ToString() : string.Empty;
        }

        /// <summary>
        /// Lấy danh sách ip address theo host name
        /// </summary>
        /// <returns>IPAddress Array</returns>
        public static IPAddress[] GetListIpServer()
        {
            return Dns.GetHostAddresses(Dns.GetHostName());
        }

        /// <summary>
        /// Lấy đường dẫn hiện tại của request
        /// ex: http://ims.quangich.com/issues/291
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="includeQueryString">Có kèm theo các query string không</param>
        /// <returns>Đường dẫn web</returns>
        public static string GetThisPageUrl(this HttpContext httpContext, bool includeQueryString = true)
        {
            try
            {
                string url = "";
                if (httpContext?.Request != null)
                {
                    if (includeQueryString)
                    {
                        url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
                    }
                    else
                    {
                        url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";
                    }
                }
                return url.ToLowerInvariant();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy trạng thái bảo mật của yêu cầu
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>bool</returns>
        public static bool IsCurrentConnectionSecured(this HttpContext httpContext)
        {
            bool useSsl = false;
            if (httpContext?.Request != null)
            {
                useSsl = httpContext.Request.IsHttps;
            }
            return useSsl;
        }

        /// <summary>
        /// Trả về true nếu url yêu cầu là đến một tài nguyên tĩnh không cần phải webserver xử lý
        /// Nguồn https://github.com/aspnet/StaticFiles/blob/dev/src/Microsoft.AspNetCore.StaticFiles/FileExtensionContentTypeProvider.cs
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>bool</returns>
        public static bool IsStaticResource(this HttpContext httpContext)
        {
            if (httpContext?.Request != null)
            {
                string path = httpContext.Request.Path;
                FileExtensionContentTypeProvider contentTypeProvider = new FileExtensionContentTypeProvider();
                return contentTypeProvider.TryGetContentType(path, out _);
            }
            return false;
        }

        /// <summary>
        /// Lấy domain của website
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>domain hệ thống</returns>
        public static string GetWebDomain(this HttpContext httpContext)
        {
            return httpContext?.Request != null ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}".ToLowerInvariant() : string.Empty;
        }

        /// <summary>
        /// Lấy giá trị của QueryString
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="queryString">Tên của query string cần lấy</param>
        /// <param name="url">Url chưa query string, Nếu để null thì lấy url hiện tại</param>
        /// <returns>Giá trị string</returns>
        public static string GetQueryString(this HttpContext httpContext, string queryString, string url = "")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                url = httpContext.GetThisPageUrl();
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            Uri uri = new Uri(url);
            QueryHelpers.ParseQuery(uri.Query).TryGetValue(queryString, out StringValues value);
            return value;
        }

        /// <summary>
        /// Làm mới một giá trị query string trên url
        /// </summary>
        /// <param name="url">Một url cần làm mới</param>
        /// <param name="queryStringModification">Tên query string</param>
        /// <param name="anchor">Giá trị làm mới</param>
        /// <returns>Đường dẫn url mới</returns>
        public static string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            if (url == null)
            {
                url = string.Empty;
            }

            url = url.ToLowerInvariant();

            if (queryStringModification == null)
            {
                queryStringModification = string.Empty;
            }

            queryStringModification = queryStringModification.ToLowerInvariant();

            if (anchor == null)
            {
                anchor = string.Empty;
            }

            anchor = anchor.ToLowerInvariant();

            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        /// <summary>
        /// Gỡ bỏ một query string trên url
        /// </summary>
        /// <param name="url">Đường dẫn url</param>
        /// <param name="queryString">Tên query string cần loại bỏ</param>
        /// <returns>Đường dẫn url mới</returns>
        public static string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
            {
                url = string.Empty;
            }

            if (queryString == null)
            {
                queryString = string.Empty;
            }

            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    StringBuilder builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return url + (string.IsNullOrEmpty(str) ? "" : ("?" + str));
        }

        /// <summary>
        /// Hàm lấy một htmlDocument theo url
        /// </summary>
        /// <param name="strLink">Url cần lấy</param>
        /// <returns>htmlDocument</returns>
        public static async Task<HtmlDocument> GetHtmlDocumentByLink(string strLink)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", CommonConstants.RequestUserAgent);
                    HtmlDocument document = new HtmlDocument();
                    document.Load(await httpClient.GetStreamAsync(strLink), Encoding.UTF8);
                    document.OptionOutputAsXml = true;
                    document.OptionAutoCloseOnEnd = true;
                    document.OptionFixNestedTags = true;
                    return document;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm kiểm tra xem url hiện tại có đang trong trạng thái cần kiếm tra hay không
        /// </summary>
        /// <param name="url">Url cần kiếm tra</param>
        /// <param name="status">Trạng thái cần so sánh</param>
        /// <returns>bool</returns>
        public static async Task<bool> CheckHttpStatus(string url, HttpStatusCode status = HttpStatusCode.OK)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", CommonConstants.RequestUserAgent);
                    using (HttpResponseMessage webResponse = await client.GetAsync(url))
                    {
                        return webResponse.StatusCode == status;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy giá trị cookie bằng key
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key">Key</param>
        /// <returns>Trả về dạng string</returns>
        public static string GetCookieByKey(this HttpContext httpContext, string key)
        {
            if (httpContext?.Request != null && httpContext.Request.Cookies != null)
            {
                string cookie = httpContext.Request.Cookies[key];
                if (cookie != null)
                {
                    return cookie;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Sét giá trị vào cookies
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key">Key</param>
        /// <param name="value">Giá trị</param>
        /// <param name="expiresTime">Thời gian hết hạn tính bằng giây</param>
        /// <param name="path">Đường dẫn</param>
        public static void SetCookieByKey(this HttpContext httpContext, string key, string value, long? expiresTime = null, string path = "")
        {
            if (httpContext?.Response != null && httpContext.Response.Cookies != null)
            {
                CookieOptions option = new CookieOptions()
                {
                    HttpOnly = false
                };
                if (expiresTime.HasValue)
                {
                    option.Expires = DateTime.Now.AddSeconds(expiresTime.Value);
                }

                if (string.IsNullOrEmpty(path))
                {
                    option.Path = path;
                }

                httpContext.Response.Cookies.Delete(key);
                httpContext.Response.Cookies.Append(key, value, option);
            }
        }

        /// <summary>
        /// Lấy giá trị sesstion bằng key
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key">Key</param>
        /// <returns>Trả về dạng string</returns>
        public static string GetSessionKeyToString(this HttpContext httpContext, string key)
        {
            return httpContext?.Session != null ? httpContext.Session.GetString(key) : string.Empty;
        }

        /// <summary>
        /// Sét giá trị vào sesstion
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key">Key</param>
        /// <param name="value">Giá trị</param>
        public static void SetSessionKey(this HttpContext httpContext, string key, string value)
        {
            if (httpContext?.Session != null)
            {
                httpContext.Session.SetString(key, value);
            }
        }

        /// <summary>
        /// Bỏ giá trị của sesstion
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key">Key</param>
        public static void RemoveSessionKey(this HttpContext httpContext, string key)
        {
            if (httpContext?.Response != null)
            {
                httpContext.Session.Remove(key);
            }
        }

        /// <summary>
        /// Làm đường dẫn upload file và tạo thư mục sẵn sàng upload
        /// Hàm này có sửa phải đồng bộ với workService.cs
        /// </summary>
        /// <param name="rootPath">Thư mục upload gốc</param>
        /// <param name="uploadDate"></param>
        /// <param name="size">Các kích thước thumb nếu có</param>
        /// <returns>string</returns>
        public static string BuildUploadPath(string rootPath, DateTime? uploadDate = null, string size = "")
        {
            if (uploadDate == null)
            {
                uploadDate = DateTime.Now;
            }

            string fullPath = rootPath + uploadDate.Value.ToString("/yyyy/MM/dd/").Replace('/', '\\');
            if (!string.IsNullOrEmpty(size))
            {
                fullPath = rootPath + size + uploadDate.Value.ToString("/yyyy/MM/dd/").Replace('/', '\\');
            }

            fullPath = Path.Combine(fullPath);
            CreateDirectory(fullPath);
            return fullPath;
        }

        /// <summary>
        /// Hàm lấy domain từ một url
        /// </summary>
        /// <param name="url">Url chuyền vào</param>
        /// <returns>Domain chính</returns>
        public static string GetDomainNameByUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            Uri uri = new Uri(url);
            return uri.Host;
        }

        /// <summary>
        /// Cố gắng ghi thử file webconfig
        /// </summary>
        /// <param name="rootPath">Đường dẫn gốc thư mục của ứng dụng</param>
        /// <returns>bool</returns>
        public static bool TryWriteWebConfig(string rootPath)
        {
            try
            {
                string path = Path.Combine(rootPath ?? string.Empty, "web.config");
                return TryWriteFile(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cố gắng ghi thử file appsetting
        /// </summary>
        /// <param name="rootPath">Đường dẫn gốc thư mục của ứng dụng</param>
        /// <returns>bool</returns>
        public static bool TryWriteAppSetting(string rootPath)
        {
            try
            {
                string path = Path.Combine(rootPath ?? string.Empty, "appsettings.json");
                return TryWriteFile(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Chuyển một html content ra string
        /// </summary>
        /// <param name="htmlContent">Khối html</param>
        /// <returns>string</returns>
        public static string ToHtmlString(this IHtmlContent htmlContent)
        {
            using (StringWriter writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        #endregion Web Helper

        #region File Helper

        /// <summary>
        /// Hàm download một file bằng web request
        /// </summary>
        /// <param name="url">Url trỏ đến file, hoặc stream của file</param>
        /// <param name="outputFilePath">Đường dẫn để lưu lại file</param>
        public static async Task<bool> DownloadFile(string url, string outputFilePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(outputFilePath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                using (HttpClient client = new HttpClient())
                {
                    File.WriteAllBytes(outputFilePath, await client.GetByteArrayAsync(url));
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Hàm ghi file theo mảng bytes
        /// </summary>
        /// <param name="bytes">Mảng byte</param>
        /// <param name="outputFilePath">Đường dẫn đề ghi vào</param>
        /// <returns>bool</returns>
        public static bool SaveFileFromBytes(byte[] bytes, string outputFilePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(outputFilePath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xóa file theo đường dẫn. Hàm không thow lỗi nếu có
        /// </summary>
        /// <param name="filePart">Đường dẫn file</param>
        /// <returns>bool</returns>
        public static bool DeleteFile(string filePart)
        {
            FileInfo _file = new FileInfo(filePart);
            if (_file.Exists)
            {
                _file.Delete();
            }

            return true;
        }

        /// <summary>
        /// Thử cố ghi vào thực mục nào đó
        /// </summary>
        /// <param name="path">Thư mục cần gi</param>
        /// <returns>bool</returns>
        public static bool TryWriteDirectory(string path)
        {
            try
            {
                Directory.SetLastWriteTimeUtc(path, DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thử cố ghi vào thực mục nào đó
        /// </summary>
        /// <param name="path">File cần gi</param>
        /// <returns>bool</returns>
        public static bool TryWriteFile(string path)
        {
            try
            {
                File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thử kiểm tra xem thư mục đã tồn tại và có thể ghi hay không
        /// </summary>
        /// <param name="path">Thư mục cần gi</param>
        /// <returns>bool</returns>
        public static bool CheckDirectoryReady(string path)
        {
            try
            {
                CreateDirectory(path);
                string testFilePath = path + "\\checkwrite.temp";
                File.WriteAllText(testFilePath, "Test write file");
                File.Delete(testFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Hàm kiểm tra và đổi tên file nếu file đã tồn tại. Hàm sẽ đổi tên file mới để cho save không bị đè.
        /// </summary>
        /// <param name="fullPath">Full đường dẫn</param>
        /// <param name="newNameFormat">Định dạng tên mới với 0 là phần tên và 1 là phần tên được thêm mới</param>
        /// <returns>Tên mới</returns>
        public static string IdentityFileName(string fullPath, string newNameFormat = "{0}({1})")
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return string.Empty;
            }

            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format(newNameFormat, fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }

        /// <summary>
        /// Check xem đã có thư mục chưa thì phải tạo thư mục.
        /// </summary>
        /// <param name="path"></param>
        public static bool CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Hàm lấy đuôi mở rộng file từ một đường dẫn thư mục hoặc url
        /// có chứa dấu .
        /// </summary>
        /// <param name="path">Đường dẫn thư mục hoặc url</param>
        /// <param name="withDot"></param>
        /// <returns>string</returns>
        public static string GetFileExtension(string path, bool withDot = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            try
            {
                string extension = Path.GetExtension(path);
                return withDot ? extension : extension.TrimStart('.');
            }
            catch (UriFormatException)
            {
                Uri uri = new Uri(path);
                string extension = Path.GetExtension(uri.LocalPath);
                return withDot ? extension : extension.TrimStart('.');
            }
        }

        /// <summary>
        /// Hàm lấy tên file từ một đường dẫn thư mục hoặc url
        /// </summary>
        /// <param name="path">Đường dẫn thư mục hoặc url</param>
        /// <returns>string</returns>
        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            try
            {
                return Path.GetFileName(path);
            }
            catch (UriFormatException)
            {
                Uri uri = new Uri(path);
                return Path.GetFileName(uri.LocalPath);
            }
        }

        /// <summary>
        /// Lấy kích thước một file dựa vào đường dẫn trực tiếp đến file
        /// </summary>
        /// <param name="path">Đường dẫn</param>
        /// <returns>1- là không có file hoặc lỗi</returns>
        public static long GetFileSize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return -1;
            }

            FileInfo file = new FileInfo(path);
            return file.Exists ? file.Length : -1;
        }

        /// <summary>
        /// Lấy danh sách tên file trong một thư mục
        /// </summary>
        /// <param name="directoryPath">Thư mục cần lấy danh sách file</param>
        /// <param name="searchPattern">Lọc file</param>
        /// <param name="topDirectoryOnly">Chỉ lấy đúng thư mục hay lấy cả các thư muc con. Mặc định là lấy đúng 1 thư mục chỉ định</param>
        /// <returns>IEnumerable string</returns>
        public static IEnumerable<string> ListFileNames(string directoryPath, string searchPattern, bool topDirectoryOnly = true)
        {
            return Directory.EnumerateFiles(directoryPath, searchPattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        #endregion File Helper

        #region Number Helper

        /// <summary>
        /// Tạo một số int ngẫu nhiên
        /// </summary>
        /// <param name="min">Giới hạn nhỏ nhất</param>
        /// <param name="max">Giới hạn lớn nhất</param>
        /// <returns>int</returns>
        public static int RandomInteger(int min = 0, int max = int.MaxValue)
        {
            Random rng = new Random(DateTime.Now.Millisecond);
            return rng.Next(min, max);
        }

        /// <summary>
        /// Sinh một chuỗi số ngẫu nhiên
        /// </summary>
        /// <param name="length">Độ dài</param>
        /// <returns>Result string</returns>
        public static string RandomDigitCode(int length)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            string str = string.Empty;
            for (int i = 0; i < length; i++)
            {
                str = string.Concat(str, random.Next(10).ToString());
            }

            return str;
        }

        /// <summary>
        /// Lấy số nhỏ nhất trong 3 số
        /// </summary>
        /// <param name="a">Số đầu tiên</param>
        /// <param name="b">Số thứ 2</param>
        /// <param name="c">Số thứ 3</param>
        /// <returns>int</returns>
        public static int Min3(int a, int b, int c)
        {
            int min;
            if (a > b)
            {
                min = b;
            }
            else
            {
                min = a;
            }

            if (min > c)
            {
                min = c;
            }

            return min;
        }

        /// <summary>
        /// Hàm cộng thêm phần trăm vào số gốc
        /// </summary>
        /// <param name="baseValue">Giá gốc</param>
        /// <param name="percent">Phần trăm</param>
        /// <returns>decimal</returns>
        public static decimal AddPercent(decimal baseValue, decimal percent = 10)
        {
            return baseValue + GetPercent(baseValue, percent);
        }

        /// <summary>
        /// Hàm tính % một giá trị
        /// </summary>
        /// <param name="baseValue">Giá trị gốc</param>
        /// <param name="percent">Phần trăm</param>
        /// <returns>decimal</returns>
        public static decimal GetPercent(decimal baseValue, decimal percent = 10)
        {
            return (baseValue * percent) / 100;
        }

        #endregion Number Helper

        #region String Helper

        /// <summary>
        /// Tạo mô tả ngắn từ một đoạn html
        /// </summary>
        /// <param name="htmlContent">Chuỗi html</param>
        /// <param name="len">Độ dài</param>
        /// <param name="expanded">Ký tự thêm vào cuối</param>
        /// <returns>string</returns>
        public static string CreateShortContent(string htmlContent, int len = 300, string expanded = "...")
        {
            string shortContent = ShortenByWord(CompressorString(ConvertHtmlToPlainText(htmlContent)), len, expanded);
            return WebUtility.HtmlDecode(shortContent);
        }

        /// <summary>
        /// Hàm cắt ngắn một chuỗi
        /// Nếu nẻ một chữ thì bỏ chữ đó cho đến dấu khoảng cách cuối cùng
        /// </summary>
        /// <param name="sentence">Chuỗi cần cắt</param>
        /// <param name="len">Độ dài</param>
        /// <param name="expanded"></param>
        /// <returns>Chuỗi cộng thêm sau khi cắt ngắn</returns>
        public static string ShortenByWord(string sentence, int len, string expanded = "...")
        {
            if (sentence == null)
            {
                return string.Empty;
            }

            len -= expanded.Length;

            if (sentence.Length > len)
            {
                sentence = sentence.Substring(0, len);
                int pos = sentence.LastIndexOf(' ');
                if (pos > 0)
                {
                    sentence = sentence.Substring(0, pos);
                }

                return sentence + expanded;
            }
            return sentence;
        }

        /// <summary>
        /// Chuyển ký tự enter thàng thẻ br trong html
        /// </summary>
        /// <param name="strContent">Chuỗi nội dung</param>
        /// <returns>string</returns>
        public static string ReplaceLineBreak(string strContent)
        {
            return !string.IsNullOrEmpty(strContent) ? Regex.Replace(strContent, @"\t|\n|\r", "<br />") : strContent;
        }

        /// <summary>
        /// Chuyển các thẻ html thành các ký tự đánh dấu
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string StripTags(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = Regex.Replace(text, @"(>)(\r|\n)*(<)", "><");
            text = Regex.Replace(text, "(<[^>]*>)([^<]*)", "$2");
            return Regex.Replace(text, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");
        }

        /// <summary>
        /// Chuyển mã html thành text bình thường
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns>text</returns>
        public static string ConvertHtmlToPlainText(string htmlString)
        {
            if (string.IsNullOrEmpty(htmlString))
            {
                return "";
            }

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);
            return htmlDocument.DocumentNode.InnerText;
        }

        /// <summary>
        /// Loại bỏ thẻ a trong một chuỗi html
        /// Ví dụ đầu vào <a href="http://example.com">Name</a> đầu ra "Name")
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns>Text string</returns>
        public static string ReplaceAnchorTags(string htmlString)
        {
            return string.IsNullOrEmpty(htmlString)
                ? string.Empty
                : Regex.Replace(htmlString, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Hàm loại bỏ các ký tự unicode sang các kí tự thường.
        /// </summary>
        /// <param name="value">Chuỗi unicode</param>
        /// <returns>string</returns>
        public static string RemoveUnicode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            string nfd = value.Normalize(NormalizationForm.FormD);
            StringBuilder retval = new StringBuilder(nfd.Length);
            foreach (char ch in nfd)
            {
                if (ch >= '\u0300' && ch <= '\u036f')
                {
                    continue;
                }

                if (ch >= '\u1dc0' && ch <= '\u1de6')
                {
                    continue;
                }

                if (ch >= '\ufe20' && ch <= '\ufe26')
                {
                    continue;
                }

                if (ch >= '\u20d0' && ch <= '\u20f0')
                {
                    continue;
                }

                retval.Append(ch);
            }
            return retval.ToString();
        }

        /// <summary>
        /// Tạo url bằng một đoạn unicode string
        /// </summary>
        /// <param name="text">Chuỗi string</param>
        /// <param name="maxLength">Độ dài url tối đa</param>
        /// <returns>string</returns>
        public static string UrlFromUnicode(string text, int maxLength = 150)
        {
            if (text == null) return "";
            var normalizedString = text
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            var stringLength = normalizedString.Length;
            var prevdash = false;
            var trueLength = 0;
            char c;
            for (int i = 0; i < stringLength; i++)
            {
                c = normalizedString[i];
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (c < 128)
                            stringBuilder.Append(c);
                        else
                            stringBuilder.Append(RemapInternationalCharToAscii(c));
                        prevdash = false;
                        trueLength = stringBuilder.Length;
                        break;
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.MathSymbol:
                        if (!prevdash)
                        {
                            stringBuilder.Append('-');
                            prevdash = true;
                            trueLength = stringBuilder.Length;
                        }
                        break;
                }
                if (maxLength > 0 && trueLength >= maxLength)
                    break;
            }
            var result = stringBuilder.ToString().Trim('-');
            return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
        }

        /// <summary>
        /// Chuyển ký tự unicode sang ascii
        /// </summary>
        /// <param name="c">Ký tự</param>
        /// <returns>string</returns>
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Bỏ các ký tự đặc biệt nhưng k bỏ unicode
        /// </summary>
        /// <param name="value">Chuỗi string</param>
        /// <param name="len">độ dài</param>
        /// <returns>string</returns>
        public static string RemoveSpecialNotUnicode(string value, int len = 150)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (len > 0)
            {
                value = ShortenByWord(value, len, "");
            }

            for (int i = 32; i < 48; i++)
            {
                value = value.Replace(((char)i).ToString(), "-");
            }

            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            value = value.Replace(".", "-");
            value = value.Replace(" ", "-");
            value = value.Replace(",", "-");
            value = value.Replace(";", "-");
            value = value.Replace(":", "-");
            value = value.Replace("?", "-");
            value = value.Replace("!", "-");
            value = value.Replace("“", "-");
            value = value.Replace("”", "-");
            value = value.Replace("]", "-");
            value = value.Replace("[", "-");
            value = value.Replace("\t", "-");
            while (value.Contains("--"))
            {
                value = value.Replace("--", "-");
            }

            if (value[value.Length - 1].Equals('-'))
            {
                value = value.Substring(0, value.Length - 1);
            }

            return value;
        }

        /// <summary>
        /// Hàm tạo tên file từ một tên unicode
        /// </summary>
        /// <param name="fileNameFull"></param>
        /// <param name="len">độ dài</param>
        /// <returns>string</returns>
        public static string FileNameFromUnicode(string fileNameFull, int len = 150)
        {
            if (string.IsNullOrEmpty(fileNameFull))
            {
                return string.Empty;
            }

            string fileNameOnly = Path.GetFileNameWithoutExtension(fileNameFull);
            string extension = Path.GetExtension(fileNameFull);
            string path = Path.GetDirectoryName(fileNameFull);

            if (len > 0)
            {
                fileNameOnly = ShortenByWord(fileNameOnly, len, "");
            }

            fileNameOnly = UrlFromUnicode(fileNameOnly);
            string newFileName = Regex.Replace(fileNameOnly, "[-]{2,}", "-");
            return Path.Combine(path, newFileName + extension);
        }

        /// <summary>
        /// Sinh chuỗi tự động với độ dài được cố định
        /// </summary>
        /// <param name="size">Độ dài</param>
        /// <returns>string</returns>
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor((26 * random.NextDouble()) + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Loại bỏ toàn bộ các ký tự không phải là ký tự sổ ra khỏi một chuỗi
        /// </summary>
        /// <param name="data">Dữ liệu ban đầu</param>
        /// <returns>string</returns>
        public static string RemoveAllNonAlphanumericCharacters(string data)
        {
            Regex rgx = new Regex("[^0-9.]");
            return rgx.Replace(data, "");
        }

        /// <summary>
        /// Loại bỏ toàn bộ các ký tự sổ ra khỏi một chuỗi
        /// </summary>
        /// <param name="data">Dữ liệu ban đầu</param>
        /// <returns>string</returns>
        public static string RemoveAllAlphanumericCharacters(string data)
        {
            Regex rgx = new Regex("[0-9]");
            return rgx.Replace(data, "");
        }

        /// <summary>
        /// Lấy đoạn string đầu hoặc cuối sau khi split
        /// </summary>
        /// <param name="data">Dữ liệu ban đầu</param>
        /// <param name="split">Ký tự chia</param>
        /// <returns>string</returns>
        public static string GetNameOnTreeName(string data, string split = ">")
        {
            if (!string.IsNullOrEmpty(data) && data.Contains(split))
            {
                string name = data.Substring(data.LastIndexOf(split) + 1);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }
            }
            return data;
        }

        /// <summary>
        /// Hàm hỗ trợ replace nội dung
        /// </summary>
        /// <param name="content">Nôi dung</param>
        /// <param name="keyAndValue">Cặp giá trị và key</param>
        /// <returns>Nội dung</returns>
        public static string ReplaceContentHelper(string content, Dictionary<string, string> keyAndValue)
        {
            foreach (KeyValuePair<string, string> item in keyAndValue)
            {
                content = content.Replace(item.Key, item.Value);
            }

            return content;
        }

        /// <summary>
        /// Hàm làm sạch nội dung lấy về tự động
        /// </summary>
        /// <param name="content">Nội dung lấy về tự động</param>
        /// <returns>string</returns>
        public static string ClearContentInCrawlImport(string content)
        {
            content = RemovePropetie(content, "font-family");
            content = RemovePropetie(content, "background-color");
            content = RemovePropetie(content, "background");
            content = RemovePropetie(content, "font-size");
            content = RemovePropetie(content, "class");
            content = RemoveTag(content, "class");
            //Loại bỏ comment html
            return Regex.Replace(content, "<!--.*?-->", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Hàm làm sạch mô tả lấy về tự động
        /// </summary>
        /// <param name="description">Nội dung lấy về tự động</param>
        /// <returns>string</returns>
        public static string ClearDescriptionInCrawlImport(string description)
        {
            return CompressorString(ConvertHtmlToPlainText(description));
        }

        /// <summary>
        /// Kiểm tra xem trong nội dung có video hay không
        /// </summary>
        /// <param name="content">Nội dung html</param>
        /// <returns>bool</returns>
        public static bool CheckContentHaveVideo(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            bool isVideo = false;
            ////Kiem tra cho the object
            const string objectMacht = "<object(.*?)</object>";
            MatchCollection listObjectData = Regex.Matches(content, objectMacht, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
            if (listObjectData.Count > 0)
            {
                isVideo = true;
            }
            //Kiem tra cho the video
            const string videoMacht = "<video(.*?)</video>";
            MatchCollection listvideoData = Regex.Matches(content, videoMacht, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
            if (listvideoData.Count > 0)
            {
                isVideo = true;
            }

            return isVideo;
        }

        /// <summary>
        /// Kiểm tra xem trong nội dung có ảnh hay không và phải lớn hơn 2 ảnh
        /// </summary>
        /// <param name="content">Nội dung html</param>
        /// <returns>bool</returns>
        public static bool CheckContentHaveImage(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            bool isImage = false;
            ////Kiem tra cho the object
            const string objectMacht = "<img(.*?)</img>";
            MatchCollection listObjectData = Regex.Matches(content, objectMacht, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
            if (listObjectData.Count > 2)
            {
                isImage = true;
            }

            return isImage;
        }

        /// <summary>
        /// Lấy danh sách thẻ trong html string
        /// </summary>
        /// <param name="str">html string</param>
        /// <param name="tagName">Tên thẻ cần lấy</param>
        /// <returns>Danh sách thẻ lấy được từ html string</returns>
        public static List<string> GetListTag(string str, string tagName = "")
        {
            if (str == null || str.Trim()?.Length == 0)
            {
                return null;
            }

            Match mt = (new Regex("'<[^<]*?>'")).Match(str);
            while (mt.Success)
            {
                str = str.Replace(mt.Value, "");
                mt = mt.NextMatch();
            }

            mt = (new Regex("(<(?<tag>[^<]*?)\">|<(?<tag>[^<]*?)>)")).Match(str);
            List<string> listTag = new List<string>();
            while (mt.Success)
            {
                if (string.IsNullOrEmpty(tagName))
                {
                    listTag.Add(mt.Value);
                }
                else
                    if (mt.Value.ToLower().StartsWith("<" + tagName))
                {
                    listTag.Add(mt.Value);
                }

                mt = mt.NextMatch();
            }

            return listTag;
        }

        /// <summary>
        /// Lấy danh sách url ảnh trong một string content html
        /// </summary>
        /// <param name="str">string content html</param>
        /// <returns>List string</returns>
        public static List<string> GetListImageUrl(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }

            List<string> imgList = new List<string>();
            List<string> listTag = GetListTag(str, "img");
            if (listTag?.Count > 0)
            {
                for (int i = 0; i < listTag.Count; i++)
                {
                    string item = listTag[i];
                    Match mt = (new Regex("src=\"(?<src>.*?)\"", RegexOptions.IgnoreCase)).Match(item);
                    string src = mt.Success ? mt.Groups["src"].Value : string.Empty;

                    if (!string.IsNullOrEmpty(src))
                    {
                        imgList.Add(src);
                    }
                }
            }

            return imgList;
        }

        /// <summary>
        /// Lấy danh sách url trong một string content html
        /// </summary>
        /// <param name="str">string content html</param>
        /// <returns>List string</returns>
        public static List<string> GetListLinkUrl(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }

            List<string> urlList = new List<string>();
            List<string> listTag = GetListTag(str, "a");
            if (listTag?.Count > 0)
            {
                for (int i = 0; i < listTag.Count; i++)
                {
                    string item = listTag[i];
                    Match mt = (new Regex("href=\"(?<href>.*?)\"", RegexOptions.IgnoreCase)).Match(item);
                    string href = mt.Success ? mt.Groups["href"].Value : string.Empty;

                    if (!string.IsNullOrEmpty(href))
                    {
                        urlList.Add(href);
                    }
                }
            }

            return urlList;
        }

        /// <summary>
        /// Hàm loại bỏ thẻ trong một html content
        /// </summary>
        /// <param name="htmlContent">Nội dung cần bỏ thẻ</param>
        /// <param name="tagName"></param>
        /// <param name="isSaveContent">Có dữ lại nội dung thẻ hay không</param>
        /// <returns>Trả về một string mới không còn thẻ cần loại bỏ</returns>
        public static string RemoveTag(string htmlContent, string tagName, bool isSaveContent = false)
        {
            try
            {
                //Trường hợp mà thẻ kết thúc bằng dấu /> thì loại vì không có nội dung
                string oneTag = "<" + tagName + "(.*?)/>";
                htmlContent = Regex.Replace(htmlContent, oneTag, "");

                //trường hợp thẻ kết thúc hợp lệ thì phức tạp hơn
                if (isSaveContent)
                {
                    string fistTag = "<" + tagName + "(.*?)>";
                    string endTag = "</" + tagName + ">";
                    htmlContent = Regex.Replace(htmlContent, fistTag, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    htmlContent = Regex.Replace(htmlContent, endTag, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                }
                else
                {
                    string matchTag = "<" + tagName + "(.*?)</" + tagName + ">";
                    htmlContent = Regex.Replace(htmlContent, matchTag, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                }

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
                HtmlNodeCollection tagList = new HtmlNodeCollection(htmlDocument.DocumentNode.ParentNode);
                tagList = htmlDocument.DocumentNode.SelectNodes("//" + tagName + "");
                if (tagList != null)
                {
                    foreach (HtmlNode item in tagList)
                    {
                        item.Remove();
                    }
                }

                return htmlDocument.DocumentNode.OuterHtml;
            }
            catch
            {
                return htmlContent;
            }
        }

        /// <summary>
        /// Hàm thay đổi tag nọ tành tág kia
        /// </summary>
        /// <param name="htmlContent">Nội dung html cần thay đổi</param>
        /// <param name="curetTagName">Tên Tag hiện tại cần thay đổi</param>
        /// <param name="newTagName">Tên Tag mới</param>
        /// <returns>Trả về string đã thay đổi.</returns>
        public static string ChangeTag(string htmlContent, string curetTagName, string newTagName)
        {
            try
            {
                string startTag = "<" + curetTagName;
                string endTag = "</" + curetTagName + ">";
                string newStartTag = "<" + newTagName;
                string newEndTag = "</" + newTagName + ">";
                htmlContent = Regex.Replace(htmlContent, startTag, newStartTag, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, endTag, newEndTag, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, startTag.ToUpper(), newStartTag, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                return Regex.Replace(htmlContent, endTag.ToUpper(), newEndTag, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            catch
            {
                return htmlContent;
            }
        }

        /// <summary>
        /// Hàm loại bỏ một thuộc tính của các element trong một html truỗi
        /// </summary>
        /// <param name="htmlContent">Nội dung html</param>
        /// <param name="propetieName">Tên thuộc tính</param>
        /// <returns>Chuỗi trả về</returns>
        public static string RemovePropetie(string htmlContent, string propetieName)
        {
            try
            {
                string notCssPropetie = propetieName + "=\"(.*?)\"";
                string notCssPropetie2 = propetieName + "=(.*?)&amp;";
                string notCssPropetie1 = propetieName + "=(.*?) ";
                string cssPropetie = propetieName + ":(.*?);";
                string cssPropetie1 = propetieName + ":(.*?)\"";
                htmlContent = Regex.Replace(htmlContent, notCssPropetie, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, notCssPropetie1, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, notCssPropetie2, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, cssPropetie1, "\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, cssPropetie, ";", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, notCssPropetie.ToUpper(), "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, notCssPropetie1.ToUpper(), "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, notCssPropetie2.ToUpper(), "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlContent = Regex.Replace(htmlContent, cssPropetie1.ToUpper(), "\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                return Regex.Replace(htmlContent, cssPropetie.ToUpper(), ";", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            catch
            {
                return htmlContent;
            }
        }

        /// <summary>
        /// Chuyển một đoạn text có nhiều dòng, lộn xộn các khoảng cách thành đoạn text chỉ có một dòng duy nhất và rất là chật tự
        /// </summary>
        /// <param name="content">Nội dung đoạn text</param>
        /// <returns>string</returns>
        public static string CompressorString(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = Regex.Replace(content, @"\t|\n|\r", " ");//Loại bỏ các kí tự xuống dòng
                content = Regex.Replace(content, "[ ]{2,}", " ");//chuyển nhiều hơn 2 ký tự khoảng trắng về làm một
                content = content.Trim();
            }
            return content;
        }

        /// <summary>
        /// So sánh độ tương đồng giữa 2 chuỗi với nhau
        /// </summary>
        /// <param name="s">Chuỗi thứ nhất</param>
        /// <param name="t">Chuỗi thứ 2</param>
        /// <returns>int</returns>
        private static int ComputeDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] distance = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; distance[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; distance[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);
                    distance[i, j] = Min3(distance[i - 1, j] + 1, distance[i, j - 1] + 1, distance[i - 1, j - 1] + cost);
                }
            }
            return distance[n, m];
        }

        /// <summary>
        /// Tính điểm tương đồng giừa hai chuỗi ký tự
        /// </summary>
        /// <param name="source">Chuỗi gốc</param>
        /// <param name="dest">Chuỗi cần so sánh</param>
        /// <returns>double</returns>
        public static double SimilarityScore(string source, string dest)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest))
            {
                return 0;
            }

            int distance = ComputeDistance(source, dest);
            return 1 - (2.0 * distance / (source.Length + dest.Length));
        }

        /// <summary>
        /// Cắt một bỏ một đoạn ở cuối một chuỗi
        /// </summary>
        /// <param name="input">Chuỗi ban đầu</param>
        /// <param name="suffixToRemove">Chuỗi cần cắt bỏ</param>
        /// <param name="comparisonType">Loại so sánh chuỗi cắt bỏ</param>
        /// <returns>string</returns>
        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else
            {
                return input;
            }
        }

        #endregion String Helper

        #region DateTime Helper

        /// <summary>
        /// Hàm chuyển thời gian thành chữ cho rễ đọc
        /// </summary>
        /// <param name="dateTime">Thời gian</param>
        /// <param name="secondsText">Mấy giây trước</param>
        /// <param name="minutesText">Mấy phút trước</param>
        /// <param name="hoursText">Mấy giờ trước</param>
        /// <param name="yesterdayText">Ngày hôm qua</param>
        /// <param name="dateTimeFormat">Định dạng ngày thánh</param>
        /// <returns>string</returns>
        public static string GetTextTime(DateTime dateTime, string secondsText = " giây trước", string minutesText = " phút trước", string hoursText = " giờ trước", string yesterdayText = "hôm qua, ", string dateTimeFormat = "")
        {
            if (dateTime > DateTime.Now)
            {
                return dateTime.ToString();
            }

            double timeDifference = (DateTime.Now - dateTime).TotalSeconds;
            if (timeDifference < 60)
            {
                return (long)timeDifference + secondsText;
            }

            if (timeDifference < 3600)
            {
                return (long)(DateTime.Now - dateTime).TotalMinutes + minutesText;
            }

            if (timeDifference < 86400)
            {
                return (long)(DateTime.Now - dateTime).TotalHours + hoursText;
            }

            if (timeDifference < 172800)
            {
                return yesterdayText + dateTime.ToString("h:mm:ss tt");
            }

            return !string.IsNullOrWhiteSpace(dateTimeFormat) ? dateTime.ToString(dateTimeFormat) : dateTime.ToString();
        }

        /// <summary>
        /// Hàm lấy danh sách các ngày trong tuần
        /// Hàm mặc định lấy vùng miên mặc định
        /// </summary>
        /// <param name="oneDayInWeek">Một ngày bất kỳ trong tuần đó, Nếu để trống là tuần hiện tại</param>
        /// <param name="startOfWeek"></param>
        /// <returns>List DateTime</returns>
        public static List<DateTime> GetWeekDay(DateTime? oneDayInWeek = null, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            DateTime tempDate = oneDayInWeek ?? DateTime.Now;
            DateTime copyDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day);
            int diff = copyDate.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            DateTime startDate = copyDate.AddDays(-diff).Date;

            //listWeekDay.Add(new DateTime(startDate.Year, startDate.Month, 7).AddMilliseconds(-1));
            return Enumerable.Range(0, 7).Select(q => startDate.AddDays(q)).ToList();
        }

        /// <summary>
        /// Hàm lấy ngày bắt đầu tuần và kết thúc tuần
        /// </summary>
        /// <param name="dayInWeek">Ngày trong tuần</param>
        /// <param name="start">Ngày bắt đầu</param>
        /// <param name="end">Ngày kết thúc</param>
        /// <param name="startOfWeek">Ngày bắt đầu tuần</param>
        public static void GetWeekStartAndEndDay(DateTime dayInWeek, out DateTime start, out DateTime end, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            List<DateTime> listDayOfWeek = GetWeekDay(dayInWeek, startOfWeek);
            start = listDayOfWeek.FirstOrDefault();
            end = listDayOfWeek.LastOrDefault();
        }

        /// <summary>
        /// Hàm lấy ngày bắt đầu ngày và kết thúc ngày
        /// </summary>
        /// <param name="timeInDay">Thời giam trong ngày</param>
        /// <param name="start">Thời gian bắt đầu</param>
        /// <param name="end">Thời gian kết thúc</param>
        public static void GetDayStartAndEndTime(DateTime timeInDay, out DateTime start, out DateTime end)
        {
            start = new DateTime(timeInDay.Year, timeInDay.Month, timeInDay.Day);
            end = start.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// Hàm lấy ngày bắt đầu tháng và kết thúc tháng
        /// </summary>
        /// <param name="dayInMonth">Ngày trong tháng</param>
        /// <param name="start">Ngày bắt đầu</param>
        /// <param name="end">Ngày kết thúc</param>
        public static void GetMonthStartAndEndDay(DateTime dayInMonth, out DateTime start, out DateTime end)
        {
            DateTime copyDate = new DateTime(dayInMonth.Year, dayInMonth.Month, dayInMonth.Day);//Chuyển về ngày không có giờ hoặc phút hay tíc tắc
            start = copyDate.AddDays(1 - copyDate.Day);
            end = start.AddMonths(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// Hàm lấy ngày bắt đầu năm và kết thúc năm
        /// </summary>
        /// <param name="dayInYear">Ngày trong năm</param>
        /// <param name="start">Ngày bắt đầu</param>
        /// <param name="end">Ngày kết thúc</param>
        public static void GetYearStartAndEndDay(DateTime dayInYear, out DateTime start, out DateTime end)
        {
            start = new DateTime(dayInYear.Year, 1, 1);
            end = start.AddYears(1).AddMilliseconds(-1);
        }

        #endregion DateTime Helper

        #region Email Helper

        /// <summary>
        /// Hàm gửi email
        /// </summary>
        /// <param name="userName">Tài khoản</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="subject">Tiêu đề</param>
        /// <param name="body">Nội dung</param>
        /// <param name="from">Gửi từ</param>
        /// <param name="from">Tên người gửi</param>
        /// <param name="fromName"></param>
        /// <param name="tos">Gửi đến</param>
        /// <param name="host">Server mail</param>
        /// <param name="port">Cổng</param>
        /// <param name="isSsl">Có sử dụng ssl hay không</param>
        /// <param name="isUseDefauletCredentials">Sử dụng tài khoản mặc định hay k</param>
        /// <param name="bcc">Danh sách email bcc</param>
        /// <param name="cc">Danh sách email cc</param>
        /// <param name="attachCollection">Đính kèm theo</param>
        public static async Task<bool> SendEmail(string userName, string password, string subject, string body, string from, string fromName, List<string> tos, string host, int port, bool isSsl, bool isUseDefauletCredentials = false, List<string> bcc = null, List<string> cc = null, AttachmentCollection attachCollection = null)
        {
            if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(from) || tos == null || tos.Count <= 0 || port <= 0 || string.IsNullOrEmpty(host))
            {
                return false;
            }

            if (!isUseDefauletCredentials)
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return false;
                }
            }

            MailMessage message = new MailMessage
            {
                From = new MailAddress(from, fromName)
            };
            foreach (string address in tos.Where(to => !string.IsNullOrWhiteSpace(to)))
            {
                message.To.Add(address.Trim());
            }

            if (bcc != null)
            {
                foreach (string address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            if (cc != null)
            {
                foreach (string address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            if (attachCollection?.Count > 0)
            {
                foreach (Attachment attach in attachCollection)
                {
                    message.Attachments.Add(attach);
                }
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Host = host;
                smtpClient.Port = port;
                smtpClient.EnableSsl = isSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (isUseDefauletCredentials)
                {
                    smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else
                {
                    smtpClient.Credentials = new NetworkCredential(userName, password);
                }

                await smtpClient.SendMailAsync(message);
            }
            return true;
        }

        #endregion Email Helper

        #region Object Helper

        /// <summary>
        /// Set một giá trị vào một thuộc tính của object
        /// </summary>
        /// <param name="instance">Object cần sét giá trị</param>
        /// <param name="propertyName">Tên thuộc tính</param>
        /// <param name="value">Giá trị cần sét</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance is null");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName is null off empty");
            }

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
            {
                throw new Exception(string.Format("No property '{0}' found on the instance of type '{1}'.", propertyName, instance.GetType()));
            }

            if (!pi.CanWrite)
            {
                throw new Exception(string.Format("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instance.GetType()));
            }

            if (pi.PropertyType.IsConstructedGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && value != null)
            {
                value = Convert.ChangeType(value, Nullable.GetUnderlyingType(pi.PropertyType));
            }

            pi.SetValue(instance, value, new object[0]);
        }

        /// <summary>
        /// Lấy giá trị của thuộc tính thuộc object
        /// </summary>
        /// <param name="instance">Object</param>
        /// <param name="propertyName">Tên thuộc tính</param>
        public static object GetProperty(object instance, string propertyName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance is null");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName is null off empty");
            }

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
            {
                throw new Exception(string.Format("No property '{0}' found on the instance of type '{1}'.", propertyName, instance.GetType()));
            }

            if (!pi.CanRead)
            {
                throw new Exception(string.Format("The property '{0}' on the instance of type '{1}' does not have a getter.", propertyName, instance.GetType()));
            }

            return pi.GetValue(instance, null);
        }

        /// <summary>
        /// Lấy giá trị của một thuộc tính trong object
        /// </summary>
        /// <typeparam name="T">Kiểu</typeparam>
        /// <param name="instance">Object chưa giá trị</param>
        /// <param name="propertyName">Tên thuộc tính cần lấy</param>
        public static T GetProperty<T>(object instance, string propertyName)
        {
            object value = GetProperty(instance, propertyName);
            return value == null ? default : (T)value;
        }

        /// <summary>
        /// Lấy danh sách các thuộc tính publish của object
        /// </summary>
        /// <param name="instance">Object</param>
        /// <returns>Danh sách thuộc tính</returns>
        public static List<PropertyInfo> GetPropertyList(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance is null");
            }

            Type instanceType = instance.GetType();
            return instanceType.GetProperties().ToList();
        }

        /// <summary>
        /// Hàn này binding dữ liệu từ objec một vào object 2
        /// </summary>
        /// <param name="readObject">Object đọc dữ liệu</param>
        /// <param name="saveObject">Object save dữ liệu</param>
        public static object BindingObjectData(object readObject, object saveObject)
        {
            if (readObject == null || saveObject == null)
            {
                return saveObject;
            }

            List<PropertyInfo> readPropertyList = GetPropertyList(readObject);
            List<PropertyInfo> savePropertyList = GetPropertyList(saveObject);
            IEnumerable<PropertyInfo> bindingPropertyList = from read in readPropertyList
                                                            join save in savePropertyList on read.Name equals save.Name
                                                            where read.GetType().Name == save.GetType().Name
                                                            where read.PropertyType.Equals(save.PropertyType)
                                                            where save.CanWrite
                                                            where read.CanRead
                                                            select read;
            foreach (PropertyInfo property in bindingPropertyList)
            {
                object value = GetProperty(readObject, property.Name);
                SetProperty(saveObject, property.Name, value);
            }
            return saveObject;
        }

        /// <summary>
        /// Hàn này binding dữ liệu từ objec một vào object 2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="readObject">Object đọc dữ liệu</param>
        /// <param name="saveObject">Object save dữ liệu</param>
        public static T BindingObjectData<T>(object readObject, object saveObject = null) where T : class
        {
            if (readObject == null)
            {
                return null;
            }

            if (saveObject == null)
            {
                saveObject = Activator.CreateInstance<T>();
            }

            return (T)BindingObjectData(readObject, saveObject);
        }

        /// <summary>
        /// Lấy thuộc tính ấn của một đối tượng
        /// </summary>
        /// <param name="target">Đối tượng</param>
        /// <param name="fieldName">Tên thộc tính</param>
        /// <returns>Object value</returns>
        public static object GetPrivateFieldValue(object target, string fieldName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "The assignment target cannot be null.");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("fieldName", "The field name cannot be null or empty.");
            }

            Type t = target.GetType();
            FieldInfo fi = null;

            while (t != null)
            {
                fi = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                if (fi != null)
                {
                    break;
                }

                t = t.BaseType;
            }

            if (fi == null)
            {
                throw new Exception($"Field '{fieldName}' not found in type hierarchy.");
            }

            return fi.GetValue(target);
        }

        #endregion Object Helper

        #region Validate Helper

        /// <summary>
        /// Chuỗi định dạng email
        /// </summary>
        public const string EmailRegexString = "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$";

        /// <summary>
        /// Chuỗi định dạng url hợp lệ khi thêm mới một chuyên mục, tin bài
        /// </summary>
        public const string BuildUrlRegexString = "[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$";

        #endregion Validate Helper

        #region Security

        /// <summary>
        /// Chuyển một đoạn text có các ký tự đặc biệt về base 64bit
        /// </summary>
        /// <param name="plainText">Text thường</param>
        /// <returns>string base64</returns>
        public static string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Chuyển một đoạn tring từ base 64 thành chuỗi thường
        /// </summary>
        /// <param name="base64EncodedData">Chuỗi base 64</param>
        /// <returns>string chuỗi thường</returns>
        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return string.Empty;
            }

            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Tạo mã md5
        /// </summary>
        /// <param name="md5Hash">Thể hiện của thuật toàn md5</param>
        /// <param name="input">Chuỗi cần tạo mã</param>
        /// <returns>String md5</returns>
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// Hàm so sánh md5
        /// </summary>
        /// <param name="md5Hash">Thể hiện của thuật toàn md5</param>
        /// <param name="input">Chuỗi cần so sánh</param>
        /// <param name="hash">Mã md5 cần so sánh</param>
        /// <returns>bool</returns>
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }

        /// <summary>
        /// Mã hóa một chuỗi TripleDES.
        /// </summary>
        /// <param name="message">Chuỗi cần mã hóa</param>
        /// <param name="passphrase">Key vecter</param>
        public static string EncryptString(string message, string passphrase = "vhasyvnemmvh")
        {
            if (string.IsNullOrEmpty(message))
            {
                return "";
            }

            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key = mD5CryptoServiceProvider.ComputeHash(uTF8Encoding.GetBytes(passphrase));
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] bytes = uTF8Encoding.GetBytes(message);
            byte[] inArray = tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            tripleDESCryptoServiceProvider.Clear();
            mD5CryptoServiceProvider.Clear();
            return Convert.ToBase64String(inArray)
                .Replace("&", "dhand")
                .Replace("=", "mdequal")
                .Replace("+", "dhplus")
                .Replace(" ", "mdsep")
                .Replace("/", "dhdiv")
                .Replace("\\", "mdmod");
        }

        /// <summary>
        /// Giải mã một chuỗi TripleDES.
        /// </summary>
        /// <param name="message">Chuỗi cần giải mã</param>
        /// <param name="passphrase">Key vecter</param>
        public static string DecryptString(string message, string passphrase = "vhasyvnemmvh")
        {
            if (string.IsNullOrEmpty(message))
            {
                return "";
            }

            message = message.Replace("dhand", "&")
                .Replace("mdequal", "=")
                .Replace("dhplus", "+")
                .Replace("mdsep", " ")
                .Replace("dhdiv", "/")
                .Replace("mdmod", "\\");
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key = mD5CryptoServiceProvider.ComputeHash(uTF8Encoding.GetBytes(passphrase));
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] array = Convert.FromBase64String(message);
            byte[] bytes = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
            tripleDESCryptoServiceProvider.Clear();
            mD5CryptoServiceProvider.Clear();
            return uTF8Encoding.GetString(bytes);
        }

        #endregion Security

        #region Tree Helper

        /// <summary>
        /// Xây dựng cây danh sách
        /// </summary>
        /// <typeparam name="T">Loại object</typeparam>
        /// <typeparam name="K">Thuộc tính object</typeparam>
        /// <param name="collection">Danh sách cần tạo tree</param>
        /// <param name="idSelector">Thuộc tính id của object</param>
        /// <param name="parentIdSelector">Thuộc tính cha id</param>
        /// <param name="orderSelector">Thuộc tính xắp xếp thứ tự</param>
        /// <param name="rootId">Giá trị gốc</param>
        public static IEnumerable<TreeItem<T>> GenerateTree<T, K>(this IEnumerable<T> collection, Func<T, K> idSelector, Func<T, K> parentIdSelector, Func<T, K> orderSelector, K rootId = default)
        {
            foreach (T c in collection.Where(c => parentIdSelector(c).Equals(rootId)).OrderBy(q => orderSelector(q)))
            {
                yield return new TreeItem<T>
                {
                    Item = c,
                    Children = collection.GenerateTree(idSelector, parentIdSelector, orderSelector, idSelector(c))
                };
            }
        }

        #endregion Tree Helper

        #region Async Funtion
        //code from: http://social.msdn.microsoft.com/Forums/en/async/thread/163ef755-ff7b-4ea5-b226-bbe8ef5f4796

        /// <summary>
        /// Chạy một hàm bất đồng đồng bộ
        /// </summary>
        /// <param name="task">Hàm cần chạy</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Chạy một hàm bất đồng đồng bộ
        /// </summary>
        /// <typeparam name="T">Kiểu trả về</typeparam>
        /// <param name="task">Hàm cần chạy</param>
        /// <returns>Kiểu dữ liệu trả về</returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default;
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public Exception InnerException { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
        #endregion

        #region Csv Converter

        /// <summary>
        /// Chuyển một Stream file csv về mảng đối tượng
        /// </summary>
        /// <typeparam name="T">Loại đối tượng</typeparam>
        /// <param name="stream">File Stream</param>
        /// <param name="skipFirstLine">Bỏ qua dòng dầu tiên</param>
        /// <param name="csvDelimiter">Ký tự phân cách</param>
        /// <returns>IList T</returns>
        public static IList<T> ReadCsvStream<T>(Stream stream, bool skipFirstLine = true, string csvDelimiter = ",") where T : new()
        {
            var records = new List<T>();
            var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(csvDelimiter.ToCharArray());
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                }
                else
                {
                    _ = records.GetType().GetTypeInfo().GenericTypeArguments[0];
                    var item = new T();
                    var properties = item.GetType().GetProperties();
                    for (int i = 0; i < values.Length; i++)
                    {
                        properties[i].SetValue(item, Convert.ChangeType(values[i], properties[i].PropertyType), null);
                    }

                    records.Add(item);
                }
            }

            return records;
        }

        /// <summary>
        /// Chuyển một mảng object ra chuỗi nội dung file csv
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="data">Mảng data</param>
        /// <param name="includeHeader">Đưa tiều đề cấu trúc vào file csv</param>
        /// <param name="csvDelimiter">Ký tự phân cách</param>
        /// <returns>string</returns>
        public static string ExportCsv<T>(IList<T> data, bool includeHeader = true, string csvDelimiter = ",") where T : new()
        {
            var type = data.GetType();
            Type itemType;

            if (type.GetGenericArguments().Length > 0)
            {
                itemType = type.GetGenericArguments()[0];
            }
            else
            {
                itemType = type.GetElementType();
            }

            var stringWriter = new StringWriter();

            if (includeHeader)
            {
                stringWriter.WriteLine(
                    string.Join<string>(
                        csvDelimiter, itemType.GetProperties().Select(x => x.Name)
                    )
                );
            }

            foreach (var obj in data)
            {
                var vals = obj.GetType().GetProperties().Select(pi => new
                {
                    Value = pi.GetValue(obj, null)
                }
                );

                string line = string.Empty;
                foreach (var val in vals)
                {
                    if (val.Value != null)
                    {
                        var escapeVal = val.Value.ToString();
                        if (escapeVal.Contains(","))
                        {
                            escapeVal = string.Concat("\"", escapeVal, "\"");
                        }

                        if (escapeVal.Contains("\r"))
                        {
                            escapeVal = escapeVal.Replace("\r", " ");
                        }

                        if (escapeVal.Contains("\n"))
                        {
                            escapeVal = escapeVal.Replace("\n", " ");
                        }

                        line = string.Concat(line, escapeVal, csvDelimiter);
                    }
                    else
                    {
                        line = string.Concat(line, string.Empty, csvDelimiter);
                    }
                }

                stringWriter.WriteLine(line.TrimEnd(csvDelimiter.ToCharArray()));
            }

            return stringWriter.ToString();
        }
        #endregion

        /// <summary>
        /// Ghi log ra màng hình console và log vào file
        /// log file theo ngày
        /// </summary>
        /// <param name="message">Nội dung log</param>
        /// <param name="color">Màu sắc ở màn hình console</param>
        public static void WriteConsole(this string message, ConsoleColor color = ConsoleColor.White)
        {
            try
            {
                lock (lockerWriteLog)
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}