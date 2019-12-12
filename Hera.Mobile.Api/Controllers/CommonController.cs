using Hera.Mobile.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    [Authorize]
    public class CommonController : BaseController
    {
        public Response<bool> CheckInfo(Models.Common.CheckInfoRequest model)
        {
            var response = new Response<bool>();
            switch (model.Type)
            {
                case "MemberMobile":
                    response = this.CheckMemberMobile(model.Value);
                    if (response.HasError)
                    {
                        response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Register", "errPhoneNumberAlreadyUse", model.Lang));
                    }
                    break;
                case "MemberEmail":
                    response = this.CheckMemberEmail(model.Value);
                    if (response.HasError)
                    {
                        response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Register", "errEmailInUse", model.Lang));
                    }
                    break;
                default:
                    break;
            }
            return response;
        }

        Response<bool> CheckMemberMobile(string mobile)
        {
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Mobile == mobile).FirstOrDefault();
            return new Response<bool>
            {
                HasError = member != null
            };
        }
        Response<bool> CheckMemberEmail(string email)
        {
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Email == email).FirstOrDefault();
            return new Response<bool>
            {
                HasError = false// member != null
            };
        }

        public Response<List<Models.Common.HealthCenter>> HealthCenterList(Models.Common.HealthCenterRequest model)
        {
            var response = new Response<List<Models.Common.HealthCenter>>
            {
                Result = new List<Models.Common.HealthCenter>()
            };

            var dataList = unitOfWork.Repository<Data.Entity.sp_Mobile_NearByHealthCenter_Result>().ExecWtihSP("EXEC sp_Mobile_NearByHealthCenter @Latitude,@Longitude",
                new SqlParameter("@Latitude", model.Latitude),
                new SqlParameter("@Longitude", model.Longitude)).ToList();

            foreach (var item in dataList)
            {
                var distance = item.Distance.Value < 1 ? (Math.Round(item.Distance.Value, 2) * 100) + " m" : Math.Round(item.Distance.Value, 2) + " km";
                response.Result.Add(new Models.Common.HealthCenter
                {
                    Address = item.Address.ToTitleCase(),
                    Distance = distance,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Name = item.Name,
                    Phone = item.Phone
                });
            }

            return response;
        }


    }
}
