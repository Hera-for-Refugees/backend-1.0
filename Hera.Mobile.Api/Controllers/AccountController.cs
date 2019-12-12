using Hera.Mobile.Api.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    public class AccountController : BaseController
    {

        public Response<Models.Account.RegisterResponse> Register(Models.Account.RegisterRequest model)
        {
            var response = new Response<Models.Account.RegisterResponse>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Mobile == model.Mobile).FirstOrDefault();
            if (member == null)
            {
                var birthdate = DateTime.Now;
                DateTime.TryParse(model.Birthdate, out birthdate);
                member = new Data.Entity.Member
                {
                    Firstname = model.Firstname.ToTitleCase(),
                    Lastname = model.Lastname.ToTitleCase(),
                    Gender = model.Gender.IsEmpty("genderEMPTY"),
                    Mobile = model.Mobile,
                    Email = model.Email.IsEmpty("emailEMPTY"),
                    Password = model.Password.ToPassword(),
                    Birthdate = birthdate,
                    Job = model.Job.ToTitleCase().IsEmpty("jobEMPTY"),
                    Photo = model.Photo.IsBase64String() ? Core.Helper.Media.Picture.CreateFromBase64(model.Photo, Core.Helper.Media.PictureType.Member_Profile) : "~/Media/Member/memberDefault.png",
                    Address = model.Address.IsEmpty("addressEMPTY"),
                    IsApproved = true,
                    ApproveCode = Guid.NewGuid(),
                    RegisterDate = DateTime.Now,
                    ApproveDate = DateTime.Now,
                    Platform = model.Platform
                };
                member = unitOfWork.Repository<Data.Entity.Member>().Add(member);
                unitOfWork.Commit();
                response.Result = new Models.Account.RegisterResponse
                {
                    Firstname = member.Firstname,
                    MemberId = member.Id,
                    Lastname = member.Lastname
                };
            }
            else
            {
                response.HasError = true;
                response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Register", "errPhoneNumberAlreadyUse", model.Lang));
            }
            return response;
        }

        public Response<Models.Account.LoginResponse> Login(Models.Account.LoginRequest model)
        {
            var response = new Response<Models.Account.LoginResponse>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Mobile == model.Mobile).FirstOrDefault();
            if (member == null)
            {
                response.HasError = true;
                response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Login", "errMobilePhone", model.Lang));
            }
            else
            {
                if (member.Password == model.Password.ToPassword() || member.ForgotPassword == model.Password.ToPassword())
                {
                    if (member.IsApproved)
                    {
                        member.Password = model.Password.ToPassword();
                        member.ForgotPassword = Core.Helper.Security.Password.GenerateMobileApprove(6).ToPassword();
                        unitOfWork.Repository<Data.Entity.Member>().Update(member);
                        unitOfWork.Commit();
                        response.Result = new Models.Account.LoginResponse
                        {
                            Firstname = member.Firstname,
                            Lastname = member.Lastname,
                            MemberId = member.Id
                        };
                    }
                    else
                    {
                        response.HasError = true;
                        response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Login", "errLogin", model.Lang));
                    }
                }
                else
                {
                    response.HasError = true;
                    response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Login", "errLogin", model.Lang));
                }
            }
            return response;
        }

        public Response<string> ForgotPassword(Models.Account.ForgotPasswordRequest model)
        {
            var response = new Response<string>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Mobile == model.Mobile)
                .FirstOrDefault();
            if (member == null)
            {
                response.HasError = true;
                response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Forgot Password", "errMobilePhoneNotFound", model.Lang));
            }
            else
            {
                var newPassword = Core.Helper.Security.Password.GenerateMobileApprove(6);
                member.ForgotPassword = newPassword.ToPassword();
                unitOfWork.Repository<Data.Entity.Member>().Update(member);
                unitOfWork.Commit();

                Regex digitsOnly = new Regex(@"[^\d]");
                var targetNumber = digitsOnly.Replace(member.Mobile, "");

                var message = messageSource.GetServiceMessage("Forgot Password", "lblNewPassSMS", model.Lang) + " " + newPassword;
                Hera.Core.SMS.Mobile.SendSms(targetNumber, message);
                response.Result = messageSource.GetServiceMessage("Forgot Password", "lblNewPassCode", model.Lang);// + " " + message;
            }
            return response;
        }

        [Authorize]
        public Response<string> ChangePassword(Models.Account.ChangePasswordRequest model)
        {
            var response = new Response<string>();
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Mobile == model.Mobile)
                .FirstOrDefault();
            if (member == null)
            {
                response.HasError = true;
                response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Forgot Password", "errMobilePhoneNotFound", model.Lang));
            }
            else
            {
                if (member.ForgotPassword == model.SmsCode.ToPassword())
                {
                    member.ForgotPassword = Core.Helper.Security.Password.GenerateMobileApprove(6);
                    member.Password = model.NewPassword.ToPassword();
                    unitOfWork.Repository<Data.Entity.Member>().Update(member);
                    unitOfWork.Commit();
                    response.Result = "Ok";
                }
                else
                {
                    response.HasError = true;
                    response.Error = new Error(this.GetErrorTitle(model.Lang), messageSource.GetServiceMessage("Forgot Password", "errSmsCode", model.Lang));
                }
            }
            return response;
        }
    }
}
