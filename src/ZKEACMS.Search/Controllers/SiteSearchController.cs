/*!
 * http://www.zkea.net/
 * Copyright 2017 ZKEASOFT
 * http://www.zkea.net/licenses
 */

using Easy.Constant;
using Easy.Mvc.Authorize;
using Easy.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using ZKEACMS.Article.Models;
using ZKEACMS.Article.Service;
using Easy.Extend;
using ZKEACMS.Search.Models;
using ZKEACMS.Search.Service;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace ZKEACMS.Search.Controllers
{
    [DefaultAuthorize(Policy = PermissionKeys.ManageSearch)]
    public class SiteSearchController : Controller
    {
        private readonly IWebPageService _webPageService;
        private readonly IOptions<SearchOption> _searchOption;
        public SiteSearchController(IWebPageService service, IOptions<SearchOption> searchOption)
        {
            _webPageService = service;
            _searchOption = searchOption;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Start(string path)
        {
            SpiderProcess.Start(_searchOption.Value.DotNet);
            return Json(true);
        }
        [HttpPost]
        public IActionResult Status()
        {
            return Json(new { Message = SpiderProcess.OutputMessage, Error = SpiderProcess.ErrorMessage });
        }
        [AllowAnonymous]
        public IActionResult Search(string q, int? p)
        {
            Pagin pagination = new Pagin
            {
                PageIndex = p ?? 0,
                BaseUrlFormat = Url.Action("Search", new { q = q }) + "&p={0}"
            };
            return View(new SearchResult
            {
                Query = q,
                WebPages = _webPageService.Search(q, pagination),
                Pagination = pagination
            });
        }
    }
}
