using Hera.Data.Entity;
using Hera.Mobile.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    [Authorize]
    public class BlogController : BaseController
    {
        public Response<List<Models.Blog.BlogItem>> BlogList(Models.Blog.BlogListRequest model)
        {
            var response = new Response<List<Models.Blog.BlogItem>>()
            {
                Result = new List<Models.Blog.BlogItem>()
            };

            var langId = this.GetLanguageId(model.Lang);
            var blogList = unitOfWork.Repository<Data.Entity.CMS_Content>().GetAll().OrderByDescending(x => x.CreatedAt).Take(20).ToList();
            foreach (var item in blogList)
            {
                var detail = item.CMS_Content_Detail.Where(x => x.LanguageId == langId).FirstOrDefault();
                if (detail != null)
                {
                    response.Result.Add(new Models.Blog.BlogItem
                    {
                        Abstract = detail.Abstract,
                        Content = detail.ContentHtml,
                        Date = item.CreatedAt.ToShortDateString(),
                        Picture = item.Picture.FromManagement(),
                        Title = detail.Name,
                        Id = detail.Id
                    });
                }
            }
            return response;
        }

        public Response<string> Log(Models.Blog.BlogLogRequest model)
        {
            var response = new Response<string>();

            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId)
                .FirstOrDefault();
            if (member != null)
            {
                if (model.Type == "Open")
                {
                    var content = unitOfWork.Repository<Data.Entity.CMS_Content_Detail>()
                        .GetBy(x => x.Id == model.ContentId).FirstOrDefault();
                    if (content != null)
                    {
                        var log = unitOfWork.Repository<Data.Entity.CMS_Content_Detail_Log>().Add(new CMS_Content_Detail_Log
                        {
                            ContentDetailId = content.Id,
                            ContentId = content.ContentId,
                            MemberId = member.Id,
                            OpenTime = DateTime.Now
                        });
                        unitOfWork.Commit();
                        response.Result = log.Id.ToString();
                    }
                }
                else
                {
                    var log = unitOfWork.Repository<Data.Entity.CMS_Content_Detail_Log>()
                        .GetBy(x => x.Id == model.LogId).FirstOrDefault();
                    if (log != null)
                    {
                        log.CloseTime = DateTime.Now;
                        unitOfWork.Repository<Data.Entity.CMS_Content_Detail_Log>().Update(log);
                        unitOfWork.Commit();
                    }
                }
            }
            return response;
        }
    }
}
