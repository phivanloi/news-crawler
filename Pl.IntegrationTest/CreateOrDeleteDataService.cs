using Newtonsoft.Json;
using Pl.Crawler.Core;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Pl.IntegrationTest
{
    public class CreateOrDeleteDataService
    {
        private readonly IWebpageData webpageData;
        private readonly IWebsiteData websiteData;
        private readonly ICrawlConfigData crawlConfigData;
        private readonly IPaserFieldData paserFieldData;
        private readonly ISelectRuleData selectRuleData;
        private readonly IReplateRuleData replateRuleData;
        private readonly IDataExportData dataExportData;
        private readonly ISitemapData sitemapData;
        private readonly IWebpageContentData webpageContentData;
        private readonly ICacheService cacheService;

        public CreateOrDeleteDataService(
            ICacheService _cacheService,
            IWebpageContentData _webpageContentData,
            ISitemapData _sitemapData,
            IDataExportData _dataExportData,
            IReplateRuleData _replateRuleData,
            ISelectRuleData _selectRuleData,
            IPaserFieldData _paserFieldData,
            ICrawlConfigData _crawlConfigData,
            IWebsiteData _websiteData,
            IWebpageData _webpageData)
        {
            webpageData = _webpageData;
            websiteData = _websiteData;
            crawlConfigData = _crawlConfigData;
            paserFieldData = _paserFieldData;
            selectRuleData = _selectRuleData;
            replateRuleData = _replateRuleData;
            dataExportData = _dataExportData;
            sitemapData = _sitemapData;
            webpageContentData = _webpageContentData;
            cacheService = _cacheService;
        }


        public void CreateData()
        {
            webpageData.CreateIndexForUrlMd5();
            webpageData.CreateIndexForNextDownloadTime();
            webpageData.CreateIndexForWebsiteId();
            webpageData.CreateIndexForDownloadRank();
            webpageContentData.CreateIndexWebpageId();
            dataExportData.CreateIndexForExistKey();

            CreateDataCafef();
            cacheService.Clear();

        }

        public void CreateDataCafef()
        {
            var website = new Website()
            {
                Domain = "https://cafef.vn/",
                Name = "Kênh thông tin kinh tế - tài chính Việt Nam",
                Rank = 9,
                FindLinkOnlySiteMap = true,
            };
            var checkInsert = websiteData.Insert(website);
            if (checkInsert)
            {
                var config = new CrawlConfig()
                {
                    AutoExport = true,
                    ExportApiUrl = "http://api.ritc.truongdientu.vn/api/News?ApiSecurityCode=8bb5b1f158835d1f436640e91f1b39e1",
                    PageTypeName = "Chi tiết tin bài",
                    UrlPattern = "https://vov.vn/(.*?)/(.*?)-(\\d+).vov",
                    WebsiteId = website.Id
                };
                var checkInsertConfig = crawlConfigData.Insert(config);
                if (checkInsertConfig)
                {
                    var paserTitle = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "Title"
                    };
                    var checkInsertTitle = paserFieldData.Insert(paserTitle);
                    if (checkInsertTitle)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserTitle.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//h2[@class=\"article__title\"]/span",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserTitle.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//h1[@class='cms-title']",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserTitle.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//h2[@class='cms-title']",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserTitle.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='wrap']/h4[@class='heading']",
                            ParserType = 0,
                            IsHtml = false
                        });
                    }

                    var paserDescription = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "Description"
                    };
                    var checkInsertDescription = paserFieldData.Insert(paserDescription);
                    if (checkInsertDescription)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserDescription.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class=\"article__head\"]/h4/div",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserDescription.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='article__head']",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserDescription.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='article__sapo']",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserDescription.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='article__sapo cms-desc']",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserDescription.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='wrap']/p[@class='desc']",
                            ParserType = 0,
                            IsHtml = false
                        });
                    }

                    var paserContent = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "Content"
                    };
                    var checkInsertContent = paserFieldData.Insert(paserContent);
                    if (checkInsertContent)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserContent.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class=\"article__body\"]",
                            ParserType = 0,
                            IsHtml = true
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserContent.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@id='article-body']",
                            ParserType = 0,
                            IsHtml = true
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserContent.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@id='cms-video']",
                            ParserType = 0,
                            IsHtml = true
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserContent.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@id='article__infographic']",
                            ParserType = 0,
                            IsHtml = true
                        });

                        replateRuleData.Insert(new ReplateRule()
                        {
                            ParserType = 0,
                            ParseFieldId = paserContent.Id,
                            ReplaceData = "",
                            SelectKey = "//div[@class='position-code']",
                        });
                    }

                    var paserTagKeywords = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "TagKeywords"
                    };
                    var checkInsertTagKeywords = paserFieldData.Insert(paserTagKeywords);
                    if (checkInsertTagKeywords)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserTagKeywords.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class=\"tags\"]/a",
                            ParserType = 0,
                            IsHtml = false,
                            SelectMultiple = true
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserTagKeywords.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='article__tag']/div[@class='box-content']/a[@class='tag-item']",
                            ParserType = 0,
                            IsHtml = false,
                            SelectMultiple = true
                        });
                    }

                    var paserCategoryName = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "CategoryName"
                    };
                    var checkInsertCategoryName = paserFieldData.Insert(paserCategoryName);
                    if (checkInsertCategoryName)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserCategoryName.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class=\"breadcrumb__link\"]",
                            ParserType = 0,
                            IsHtml = false
                        });

                        replateRuleData.Insert(new ReplateRule()
                        {
                            ParserType = 1,
                            ParseFieldId = paserCategoryName.Id,
                            ReplaceData = "",
                            SelectKey = "<meta property=\"article:section\" content=\"",
                        });

                        replateRuleData.Insert(new ReplateRule()
                        {
                            ParserType = 1,
                            ParseFieldId = paserCategoryName.Id,
                            ReplaceData = "",
                            SelectKey = "\" />",
                        });
                    }

                    var paserPublishTime = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "PublishTime"
                    };
                    var checkInsertPublishTime = paserFieldData.Insert(paserPublishTime);
                    if (checkInsertPublishTime)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserPublishTime.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='article__time']/a",
                            ParserType = 0,
                            IsHtml = false
                        });

                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserPublishTime.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "//div[@class='article__meta']/time",
                            ParserType = 0,
                            IsHtml = false
                        });

                        replateRuleData.Insert(new ReplateRule()
                        {
                            ParserType = 1,
                            ParseFieldId = paserPublishTime.Id,
                            ReplaceData = "$4:$3:$5 $1:$2",
                            SelectKey = ".*?, (\\d{2}):(\\d{2}), (\\d{2})/(\\d{2})/(\\d{4})",
                        });
                    }

                    var paserImage = new ParseField()
                    {
                        CrawlConfigId = config.Id,
                        FieldName = "Image"
                    };
                    var checkInsertImage = paserFieldData.Insert(paserImage);
                    if (checkInsertImage)
                    {
                        selectRuleData.Insert(new SelectRule()
                        {
                            ParseFieldId = paserImage.Id,
                            IsDefault = false,
                            DefaultData = string.Empty,
                            SelectKey = "<meta property=\"og:image\" content=\".*?\" />",
                            ParserType = 1,
                            IsHtml = false
                        });

                        replateRuleData.Insert(new ReplateRule()
                        {
                            ParserType = 1,
                            ParseFieldId = paserImage.Id,
                            ReplaceData = "",
                            SelectKey = "<meta property=\"og:image\" content=\"",
                        });

                        replateRuleData.Insert(new ReplateRule()
                        {
                            ParserType = 1,
                            ParseFieldId = paserImage.Id,
                            ReplaceData = "",
                            SelectKey = "\" />",
                        });
                    }
                }

                using (var md5Creater = MD5.Create())
                {
                    var webpages = new List<Webpage>();
                    var sitemaps = JsonConvert.DeserializeObject<List<Sitemap>>(File.ReadAllText($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\vovsitemap.json"));
                    sitemaps.ForEach(q =>
                    {
                        q.WebsiteId = website.Id;
                        webpages.Add(new Webpage()
                        {
                            Url = q.Url,
                            UrlMd5 = Utility.GetMd5Hash(md5Creater, q.Url),
                            WebsiteId = website.Id,
                            NextDownloadTime = DateTime.Now,
                            DownloadRank = q.DownloadRank,
                            IsSiteMapPage = true,
                        });
                    });

                    sitemapData.Insert(sitemaps);
                    webpageData.Insert(webpages);
                }
            }

        }

        /// <summary>
        /// Xóa hết dữ liệu
        /// </summary>
        public void DeleteData()
        {
            cacheService.Clear();
            websiteData.Truncate();
            crawlConfigData.Truncate();
            paserFieldData.Truncate();
            selectRuleData.Truncate();
            replateRuleData.Truncate();
            sitemapData.Truncate();
            webpageData.Truncate();
            dataExportData.Truncate();
            webpageContentData.Truncate();
        }
    }
}
