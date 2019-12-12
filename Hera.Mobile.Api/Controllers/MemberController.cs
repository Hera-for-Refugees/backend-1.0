using Hera.Data.Entity;
using Hera.Mobile.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public Response<Models.Member.ProfileResponse> ProfileInfo(Models.Member.ProfileRequest model)
        {
            var response = new Response<Models.Member.ProfileResponse>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId).FirstOrDefault();
            if (member == null)
            {
                response.HasError = true;
                response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Login", "errMemberNotFound", model.Lang));
            }
            else
            {
                response.Result = new Models.Member.ProfileResponse
                {
                    Address = member.Address.Check("EMPTY"),
                    Birthdate = member.Birthdate.ToString("yyyy-MM-dd"),
                    Email = member.Email.Check("EMPTY"),
                    Firstname = member.Firstname,
                    Gender = member.Gender.Check("EMPTY"),
                    Job = member.Job.Check("EMPTY"),
                    Lastname = member.Lastname,
                    MemberId = member.Id,
                    Mobile = member.Mobile,
                    ProfilePhoto = member.Photo.FromApi()
                };
            }
            return response;
        }

        public Response<bool> ProfileInfoUpdate(Models.Member.ProfileResponse model)
        {
            var response = new Response<bool>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId).FirstOrDefault();
            if (member != null)
            {
                member.Address = model.Address;
                if (DateTime.TryParse(model.Birthdate, out DateTime dt))
                {
                    member.Birthdate = dt;
                }
                member.Email = model.Email;
                member.Firstname = model.Firstname;
                member.Gender = model.Gender;
                member.Job = model.Job;
                member.Lastname = model.Lastname;
                member.Mobile = model.Mobile;
                if (member.Photo != model.ProfilePhoto && !model.ProfilePhoto.Contains("http://"))
                {
                    member.Photo = Core.Helper.Media.Picture.CreateFromBase64(model.ProfilePhoto, Core.Helper.Media.PictureType.Member_Profile);
                }
                unitOfWork.Repository<Data.Entity.Member>().Update(member);
                unitOfWork.Commit();
            }
            return response;
        }

        public Response<List<Models.Member.ChildItem>> ChildList(Models.Member.ChildRequest model)
        {
            var response = new Response<List<Models.Member.ChildItem>>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId).FirstOrDefault();
            if (member == null)
            {
                response.HasError = true;
                response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Login", "errMemberNotFound", model.Lang));
            }
            else
            {
                response.Result = new List<Models.Member.ChildItem>();
                var childList = unitOfWork.Repository<Data.Entity.Member_Child>().GetBy(x => x.MemberId == member.Id).ToList();
                foreach (var item in childList)
                {
                    response.Result.Add(new Models.Member.ChildItem { Id = item.Id, NameSurname = item.NameSurname });
                }
            }
            return response;
        }

        public Response<List<Models.Member.HealthRecordItem>> HealthRecordList(Models.Member.HealthRecordRequest model)
        {
            var response = new Response<List<Models.Member.HealthRecordItem>>
            {
                Result = new List<Models.Member.HealthRecordItem>()
            };
            var dataList = unitOfWork.Repository<Data.Entity.Member_Health_Record>().GetBy(x => x.MemberId == model.MemberId).OrderByDescending(x => x.Date);
            foreach (var item in dataList)
            {
                response.Result.Add(new Models.Member.HealthRecordItem
                {
                    Date = item.Date.ToShortDateString(),
                    Id = item.Id,
                    Name = item.Name,
                    Photo = item.Photo.FromApi()
                });
            }
            return response;
        }

        public Response<string> SaveRecord(Models.Member.HealthRecordItem model)
        {
            var record = unitOfWork.Repository<Data.Entity.Member_Health_Record>().GetBy(x => x.Id == model.Id && x.MemberId == model.MemberId).FirstOrDefault();
            if (record == null)
            {
                record = new Data.Entity.Member_Health_Record
                {
                    Date = DateTime.Now,
                    MemberId = model.MemberId,
                    Name = model.Name,
                    Photo = Core.Helper.Media.Picture.CreateFromBase64(model.Photo, Core.Helper.Media.PictureType.Member_Health_Record)
                };

                unitOfWork.Repository<Data.Entity.Member_Health_Record>().Add(record);
                unitOfWork.Commit();
            }
            return new Response<string> { Result = "Kayıt yapıldı" };
        }

        public Response<string> SaveDevice(Models.Member.SaveDeviceRequest model)
        {
            var response = new Response<string>();

            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId).FirstOrDefault();
            if (member != null)
            {
                member.DeviceToken = model.DeviceToken;
                member.Platform = model.Platform;
                member.NotificationUserId = model.NotificationUserId;
                member.Language = model.Lang.ToLower();
                unitOfWork.Repository<Data.Entity.Member>().Update(member);
                unitOfWork.Commit();

                unitOfWork.Repository<Data.Entity.Member>().RunQuery("EXEC sp_Mobile_Member_Language_Change @MemberId",new object[]
                {
                    new SqlParameter("@MemberId",member.Id)
                });
                unitOfWork.Commit();

            }
            return response;
        }

        public Response<List<Models.Member.NotificationItem>> NotificationList(Models.Member.NotificationListRequest model)
        {
            var response = new Response<List<Models.Member.NotificationItem>>
            {
                Result = new List<Models.Member.NotificationItem>()
            };
            var messageList = unitOfWork.Repository<Data.Entity.Member_Notification>().GetBy(x => x.MemberId == model.MemberId && x.IsDelete == false).OrderByDescending(x => x.Id).ToList();
            var langId = this.GetLanguageId(model.Lang);
            foreach (var item in messageList)
            {
                var msgDetail = unitOfWork.Repository<Data.Entity.Notification_Sent_Detail>().GetBy(x => x.SentId == item.NotificationSentId && x.LanguageId == langId).ToList();
                foreach (var msg in msgDetail)
                {
                    response.Result.Add(new Models.Member.NotificationItem
                    {
                        Id = item.Id,
                        Date = item.Notification_Sent.Date.ToShortDateString(),
                        IsRead = item.IsRead,
                        Message = msg.Message
                    });
                }

            }
            return response;
        }

        public Response<string> NotificationSave(Models.Member.NotificationSaveRequest model)
        {
            var response = new Response<string>();

            var message = unitOfWork.Repository<Data.Entity.Member_Notification>().GetBy(x => x.MemberId == model.MemberId && x.Id == model.MessageId).FirstOrDefault();
            var langId = this.GetLanguageId(model.Lang);
            if (message != null)
            {
                if (model.Type == "Read" && message.IsRead == false)
                {
                    message.IsRead = true;
                    message.ReadDate = DateTime.Now;
                    unitOfWork.Repository<Data.Entity.Member_Notification>().Update(message);
                    unitOfWork.Commit();
                }
                else if (model.Type == "Delete")
                {
                    message.IsDelete = true;
                    message.DeleteDate = DateTime.Now;
                    unitOfWork.Repository<Data.Entity.Member_Notification>().Update(message);
                    unitOfWork.Commit();
                }
            }
            return response;
        }

        public Response<List<Data.Entity.sp_Get_Member_Calendar_Result>> MemberCalendar(Models.Member.CalendarRequest model)
        {
            var response = new Response<List<Data.Entity.sp_Get_Member_Calendar_Result>>
            {
                Result = new List<sp_Get_Member_Calendar_Result>()
            };

            response.Result = unitOfWork.Repository<Data.Entity.sp_Get_Member_Calendar_Result>()
                .ExecWtihSP("EXEC sp_Get_Member_Calendar @MemberId", new object[]
                {
                    new SqlParameter("@MemberId", model.MemberId)
                }).ToList();

            return response;
        }

        public Response<string> UpdateCalendar(Models.Member.UpdateCalendarRequest model)
        {
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId).FirstOrDefault();
            if (member != null)
            {
                if (model.Type == "Vaccinate")
                {
                    var vaccinate = unitOfWork.Repository<Data.Entity.Member_Child_Vaccinate>()
                        .GetBy(x => x.MemberId == member.Id && x.Id == model.RecordId).FirstOrDefault();
                    if (vaccinate != null && !vaccinate.IsCompleted)
                    {
                        vaccinate.IsCompleted = true;
                        vaccinate.CompletedDate=DateTime.Now;
                        unitOfWork.Repository<Data.Entity.Member_Child_Vaccinate>().Update(vaccinate);
                        unitOfWork.Commit();
                    }
                }
                else
                {
                    var pregnancy = unitOfWork.Repository<Data.Entity.Member_Pregnancy>()
                        .GetBy(x => x.MemberId == member.Id && x.Id == model.RecordId).FirstOrDefault();
                    if (pregnancy != null && !pregnancy.IsCompleted)
                    {
                        pregnancy.IsCompleted = true;
                        pregnancy.CompletedDate=DateTime.Now;
                        unitOfWork.Repository<Data.Entity.Member_Pregnancy>().Update(pregnancy);
                        unitOfWork.Commit();
                    }
                }
            }
            return new Response<string>();
        }
    }
}