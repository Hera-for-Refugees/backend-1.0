using Hera.Mobile.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Hera.Mobile.Api.Controllers
{
    [Authorize]
    public class QuestionController : BaseController
    {
        public Response<Models.Question.QuestionResponse> QuestionList(Models.Question.QuestionRequest model)
        {
            var response = new Response<Models.Question.QuestionResponse>();
            var questionList = unitOfWork.Repository<Data.Entity.Category_Question>().GetBy(x => x.CategoryId == model.CategoryId).OrderBy(x => x.OrderNo);
            var langId = this.GetLanguageId(model.Lang);
            var category = unitOfWork.Repository<Data.Entity.Translation>().GetBy(x => x.RecordId == model.CategoryId && x.LanguageId == langId && x.TableId == 1).FirstOrDefault();
            response.Result = new Models.Question.QuestionResponse
            {
                CategoryId = model.CategoryId,
                Description = "",
                Title = category == null ? "" : category.Translation1,
                QuestionList = new List<Models.Question.QuestionItem>()
            };
            var memberOldData = unitOfWork.Repository<Data.Entity.Member_Question_Answer>().GetBy(x => x.CategoryId == model.CategoryId && x.MemberId == model.MemberId);
            foreach (var item in questionList)
            {
                var oldAnswer = memberOldData.Where(x => x.QuestionId == item.Id);
                var translation = unitOfWork.Repository<Data.Entity.Translation>().GetBy(x => x.TableId == 3 && x.LanguageId == langId && x.RecordId == item.Id).FirstOrDefault();
                var question = new Models.Question.QuestionItem
                {
                    AnswerList = new List<Models.Question.AnswerItem>(),
                    AnswerTypeId = item.AnswerTypeId,
                    AnswerDataTypeId = item.AnswerDataTypeId ?? 0,
                    ConnectedAnswerId = item.ConnectedAnswerId ?? 0,
                    ConnectedQuestionId = item.ConnectedQuestionId ?? 0,
                    IsConnected = item.IsConnected,
                    Question = translation.Translation1,
                    QuestionId = item.Id
                };
                if (oldAnswer.Count() > 0)
                {
                    question.Answer = oldAnswer.Count() == 1 ? oldAnswer.FirstOrDefault().Answer : string.Join(",", oldAnswer.Select(x => x.AnswerId));
                    if (item.AnswerTypeId == 6 && item.AnswerDataTypeId == 3)
                    {
                        var dateList = question.Answer.Split(',');
                        foreach (var controlDate in dateList)
                        {
                            if (!string.IsNullOrEmpty(controlDate))
                            {
                                question.AnswerList.Add(new Models.Question.AnswerItem
                                {
                                    Answer = controlDate,
                                    AnswerId = 0,
                                    DataTypeId = 3,
                                    IsDefault = false
                                });
                            }
                        }
                        //question.AnswerList.Add(new Models.Question.AnswerItem { AnswerId = 0, Answer = "", DataTypeId = 3, IsDefault = false });
                    }
                }
                foreach (var ans in item.Category_Question_Answer)
                {
                    translation = unitOfWork.Repository<Data.Entity.Translation>().GetBy(x => x.TableId == 4 && x.LanguageId == langId && x.RecordId == ans.Id).FirstOrDefault();
                    var answerItem = new Models.Question.AnswerItem
                    {
                        Answer = translation == null ? "" : translation.Translation1,
                        AnswerId = ans.Id,
                        DataTypeId = ans.DataTypeId,
                        IsDefault = ans.IsDefault
                    };
                    if (answerItem.IsDefault && string.IsNullOrEmpty(question.Answer))
                    {
                        question.Answer = answerItem.AnswerId.ToString();
                    }
                    question.AnswerList.Add(answerItem);
                }
                response.Result.QuestionList.Add(question);
            }
            if (model.ChildId > 0)
            {
                var child = unitOfWork.Repository<Data.Entity.Member_Child>().GetBy(x => x.Id == model.ChildId).FirstOrDefault();
                if (child != null)
                {
                    foreach (var item in response.Result.QuestionList)
                    {
                        switch (item.QuestionId)
                        {
                            case 74:
                                item.Answer = child.NameSurname; break;
                            case 75:
                                item.Answer = child.Gender == "Erkek" ? "113" : "112"; break;
                            case 76:
                                item.Answer = child.Birthdate.ToString("yyyy-MM-dd"); break;
                            case 77:
                                var nameList = child.Vaccine.Split(',');
                                var answerList = unitOfWork.Repository<Data.Entity.Category_Question_Answer>().GetBy(x => x.QuestionId == 77);
                                var idList = new List<string>();
                                foreach (var name in nameList)
                                {
                                    var ans = answerList.Where(x => x.Answer == name).FirstOrDefault();
                                    if (ans != null)
                                    {
                                        idList.Add(ans.Id.ToString());
                                    }
                                }
                                item.Answer = string.Join(",", idList);
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
            return response;
        }

        public Response<string> SaveAnswer(Models.Question.SaveAnswerRequest model)
        {
            var member = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Id == model.MemberId).FirstOrDefault();
            if (member == null)
            {
                return new Response<string>();
            }
            else
            {
                var category = unitOfWork.Repository<Data.Entity.Category>().GetBy(x => x.Id == model.CategoryId).FirstOrDefault();
                if (category == null)
                {
                    return new Response<string>();
                }
                else
                {
                    if (model.ChildId > -1)
                    {
                        var child = unitOfWork.Repository<Data.Entity.Member_Child>().GetBy(x => x.MemberId == member.Id && x.Id == model.ChildId).FirstOrDefault();
                        if (child == null)
                        {
                            child = new Data.Entity.Member_Child
                            {
                                MemberId = member.Id,
                                NameSurname = model.QuestionList.Where(x => x.QuestionId == 74).FirstOrDefault().AnswerList.First().Answer.IsEmpty(" "),
                                Gender = model.QuestionList.Where(x => x.QuestionId == 75).FirstOrDefault().AnswerList.First().Answer == "113" ? "Erkek" : "Kız",
                                Birthdate = model.QuestionList.Where(x => x.QuestionId == 76).FirstOrDefault().AnswerList.First().Answer.ToDateConvert(),
                                Vaccine = string.Join(",", model.QuestionList.Where(x => x.QuestionId == 77).FirstOrDefault().AnswerList.Select(x => x.Answer))
                            };
                            child = unitOfWork.Repository<Data.Entity.Member_Child>().Add(child);
                            unitOfWork.Commit();

                            unitOfWork.Repository<Data.Entity.Member_Child>().RunQuery("EXEC sp_Set_Child_Vaccinate @ChildId", new object[]
                            {
                                new SqlParameter("@ChildId",child.Id)
                            });
                            unitOfWork.Commit();

                        }
                        else
                        {
                            var childOldName = child.NameSurname;
                            var childName = model.QuestionList.Where(x => x.QuestionId == 74).FirstOrDefault().AnswerList.First().Answer.IsEmpty(" ");
                            var childNameIsChanged = false;
                            if (childName.Trim().Length > 0)
                            {
                                childNameIsChanged = childName != child.NameSurname;
                                child.NameSurname = childName;
                            }

                            var birthdateIsChanged = false;
                            var birthdate = model.QuestionList.Where(x => x.QuestionId == 76).FirstOrDefault().AnswerList.First().Answer.ToDateConvert();
                            if (birthdate != child.Birthdate)
                            {
                                birthdateIsChanged = true;
                                child.Birthdate = birthdate;
                            }

                            if (childNameIsChanged || birthdateIsChanged)
                            {
                                unitOfWork.Repository<Data.Entity.Member_Child>().RunQuery("EXEC sp_Update_Child_Vaccinate @ChildId,@OldName,@NewName,@Birthdate,@NameIsChanged,@BirthdateIsChanged", new object[]
                                {
                                    new SqlParameter("@ChildId",child.Id),
                                    new SqlParameter("@OldName",childOldName),
                                    new SqlParameter("@NewName",child.NameSurname),
                                    new SqlParameter("@Birthdate",child.Birthdate),
                                    new SqlParameter("@NameIsChanged",childNameIsChanged),
                                    new SqlParameter("@BirthdateIsChanged",birthdateIsChanged),
                                });
                                unitOfWork.Commit();
                            }

                            child.Gender = model.QuestionList.Where(x => x.QuestionId == 75).FirstOrDefault().AnswerList.First().Answer;
                            child.Vaccine = string.Join(",", model.QuestionList.Where(x => x.QuestionId == 77).FirstOrDefault().AnswerList.Select(x => x.Answer));
                            unitOfWork.Repository<Data.Entity.Member_Child>().Update(child);
                            unitOfWork.Commit();
                        }
                    }
                    else
                    {
                        var questionList = unitOfWork.Repository<Data.Entity.Category_Question>().GetBy(x => x.CategoryId == model.CategoryId);
                        var memberOldData = unitOfWork.Repository<Data.Entity.Member_Question_Answer>().GetBy(x => x.MemberId == model.MemberId && x.CategoryId == model.CategoryId);
                        foreach (var item in model.QuestionList)
                        {
                            var question = questionList.Where(x => x.Id == item.QuestionId).FirstOrDefault();
                            if (question.AnswerTypeId == 4 || question.AnswerTypeId == 3)
                            {
                                unitOfWork.Repository<Data.Entity.Member_Question_Answer>().Delete(x => x.MemberId == member.Id && x.CategoryId == category.Id && x.QuestionId == question.Id);
                                unitOfWork.Commit();
                            }
                            foreach (var ans in item.AnswerList)
                            {
                                var oldData = memberOldData.Where(x => x.QuestionId == item.QuestionId && x.AnswerId == ans.AnswerId).FirstOrDefault();
                                if (oldData == null)
                                {
                                    unitOfWork.Repository<Data.Entity.Member_Question_Answer>().Add(new Data.Entity.Member_Question_Answer
                                    {
                                        MemberId = member.Id,
                                        CategoryId = category.Id,
                                        QuestionId = item.QuestionId,
                                        AnswerId = ans.AnswerId,
                                        Answer = ans.Answer,
                                        CreatedAt = DateTime.Now
                                    });
                                }
                                else
                                {
                                    oldData.Answer = ans.Answer;
                                    oldData.UpdatedAt = DateTime.Now;
                                    unitOfWork.Repository<Data.Entity.Member_Question_Answer>().Update(oldData);
                                }
                                unitOfWork.Commit();
                                if (item.QuestionId == 72 && ans.Answer.IsDate())
                                {
                                    unitOfWork.Repository<Data.Entity.Member_Pregnancy>()
                                        .RunQuery("EXEC sp_Set_Member_Pregnancy @MemberId,@LastDate", new object[]
                                        {
                                            new SqlParameter("@MemberId",model.MemberId),
                                            new SqlParameter("@LastDate",ans.Answer)
                                        });
                                    unitOfWork.Commit();
                                }
                            }
                        }
                    }
                    var text = "Kayıt Yapıldı...";
                    if (model.Lang == "en")
                    {
                        text = "Information is updated";
                    }
                    else if (model.Lang == "ar")
                    {
                        text = "يتم تحديث المعلومات";
                    }
                    return new Response<string> { Result = text };
                }
            }

        }
    }
}
