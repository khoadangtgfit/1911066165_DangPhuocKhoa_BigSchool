﻿using _1911066165_DangPhuocKhoa_BigSchool.DTOs;
using _1911066165_DangPhuocKhoa_BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace _1911066165_DangPhuocKhoa_BigSchool.Controllers
{
    public class FollowingsController : ApiController
    {
        // GET: Followings
        private readonly ApplicationDbContext _DbContext;
        public FollowingsController()
        {
            _DbContext = new ApplicationDbContext();
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult Follow(FollowingDto followingDto)
        {
            var userId = User.Identity.GetUserId();
            if (_DbContext.Followings.Any(f => f.FollowerId == userId && f.FolloweeId == followingDto.FolloweeId))
                return BadRequest("Following Already Axists !");
                

            var folowing = new Following
            {
                FollowerId = userId,
                FolloweeId = followingDto.FolloweeId
            };

            _DbContext.Followings.Add(folowing);
            _DbContext.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        public IHttpActionResult UnFollow(string followeeId, string followerId)
        {
            var follow = _DbContext.Followings
                .Where(x => x.FolloweeId == followeeId && x.FollowerId == followerId)
                .Include(x => x.Followee)
                .Include(x => x.Follower).SingleOrDefault();

            var followingNotification = new FollowingNotification()
            {
                Id = 0,
                Logger = follow.Follower.Name + " unfollow " + follow.Followee.Name
            };

            _DbContext.FollowingNotifications.Add(followingNotification);

            _DbContext.Followings.Remove(follow);
            _DbContext.SaveChanges();
            return Ok();
        }
        }
}