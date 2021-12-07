﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Deltas;
using knowledgebuilderapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace knowledgebuilderapi.Controllers
{
    [Authorize]
    public class UserHabitRecordViewsController : ODataController
    {
        private readonly kbdataContext _context;

        public UserHabitRecordViewsController(kbdataContext context)
        {
            _context = context;
        }

        /// GET: /UserHabitRecordViews
        /// <summary>
        /// Adds support for getting User Habit Rules, for example:
        /// 
        /// GET /UserHabitRecordViews
        /// GET /UserHabitRecordViews?$filter=Name eq 'Windows 95'
        /// GET /UserHabitRecordViews?
        /// 
        /// <remarks>
        [EnableQuery]
        public IQueryable<UserHabitRecordView> Get()
        {
            String usrId = ControllerUtil.GetUserID(this);
            if (String.IsNullOrEmpty(usrId))
                throw new Exception("Failed ID");

            var results = from record in _context.UserHabitRecords
                          join habit in _context.UserHabits
                            on record.HabitID equals habit.ID
                          join auser in _context.AwardUsers
                            on habit.TargetUser equals auser.TargetUser
                          join rule in _context.UserHabitRules
                             on new { HabitID = record.HabitID, RuleID = record.RuleID.GetValueOrDefault() } equals new { HabitID = rule.HabitID, RuleID = rule.RuleID }
                          where auser.TargetUser != null
                          select new UserHabitRecordView
                          {
                              HabitID = record.HabitID,
                              Comment = record.Comment,
                              CompleteFact = record.CompleteFact,
                              RecordDate = record.RecordDate,
                              ContinuousCount = record.ContinuousCount,
                              RuleID = record.RuleID,
                              SubID = record.SubID,
                              TargetUser = habit.TargetUser,
                              HabitName = habit.Name,
                              HabitValidFrom = habit.ValidFrom,
                              HabitValidTo = habit.ValidTo,
                              RuleDaysFrom = rule.ContinuousRecordFrom,
                              RuleDaysTo = rule.ContinuousRecordTo,
                              RulePoint = rule.Point
                          };

            return results;
        }
    }
}
