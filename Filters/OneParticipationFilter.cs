using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Promotion.Models;
using Promotion.Interfaces;
using System.Security.Claims;

namespace Promotion.Filters
{
    public class OneParticipationFilter : ActionFilterAttribute
    {
        private readonly IParticipationRepository _participationRepository;

        public OneParticipationFilter(IParticipationRepository participationRepository)
        {
            _participationRepository = participationRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int participantId = Int32.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            int participatioCount = _participationRepository.CountParticipations(participantId);

            if (participatioCount > 0)
            {
                context.Result = new RedirectResult(context.HttpContext.Request.Headers["Referer"].ToString());
            }
        }
    }
}