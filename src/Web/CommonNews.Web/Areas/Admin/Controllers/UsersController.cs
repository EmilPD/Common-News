﻿namespace CommonNews.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;
    using AutoMapper;
    using Bytes2you.Validation;
    using Common.Contracts;
    using Data.Models;
    using Models;
    using Services.Data.Common.Contracts;
    using ViewModels.Pagination;
    using Web.Controllers;

    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        private readonly IDataService<ApplicationUser> usersService;
        private readonly IPaginationFactory paginationFactory;

        public UsersController(
            IDataService<ApplicationUser> usersService, IPaginationFactory paginationFactory, IMapper mapper)
            : base(mapper)
        {
            Guard.WhenArgument(usersService, "UsersService").IsNull().Throw();
            Guard.WhenArgument(paginationFactory, "PaginationFactory").IsNull().Throw();

            this.usersService = usersService;
            this.paginationFactory = paginationFactory;
        }

        // GET: Admin/Users
        [HttpGet]
        [OutputCache(Duration = 60, Location = OutputCacheLocation.Client, VaryByParam = "*")]
        public ActionResult Index(int? page, PageViewModel<UsersViewModel> pageViewModel)
        {
            var users = this.usersService.GetAll().OrderBy(x => x.UserName).AsEnumerable();
            var pagination = this.paginationFactory.CreatePagination(users.Count(), page);

            pageViewModel.Items = this.Mapper.Map<IEnumerable<UsersViewModel>>(users)
                                                 .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
                                                 .Take(pagination.PageSize);
            pageViewModel.Pagination = pagination;

            return this.View("Index", pageViewModel);
        }

        [HttpGet]
        public ActionResult Delete(Guid id, ApplicationUser user)
        {
            user = this.usersService.GetById(id.ToString());
            if (user == null)
            {
                return this.HttpNotFound("Page not found!");
            }

            var userViewModel = this.Mapper.Map<UsersViewModel>(user);
            return this.View(userViewModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id, UsersViewModel userViewModel)
        {
            this.usersService.Delete(id.ToString());

            return this.RedirectToAction("Index", "Users");
        }
    }
}