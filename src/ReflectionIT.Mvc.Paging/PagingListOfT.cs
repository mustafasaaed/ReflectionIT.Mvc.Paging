﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace ReflectionIT.Mvc.Paging {

    public class PagingList<T> : List<T>, IPagingList where T : class {

        public int PageIndex { get; }
        public int PageCount { get; }
        public string Action { get; set; }
        public string SortExpression { get; }

        public string DefaultSortExpression { get; }

        [Obsolete("Use PagingList.CreateAsync<T>() instead")] 
        public static Task<PagingList<T>> CreateAsync(IOrderedQueryable<T> qry, int pageSize, int pageIndex) {
            return PagingList.CreateAsync(qry, pageSize, pageIndex);
        }

        [Obsolete("Use PagingList.CreateAsync<T>() instead")]
        public static Task<PagingList<T>> CreateAsync(IQueryable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) {
            return PagingList.CreateAsync(qry, pageSize, pageIndex, sortExpression, defaultSortExpression);
        }

        internal PagingList(List<T> list, int pageSize, int pageIndex, int pageCount)
            : base(list) {
            this.PageIndex = pageIndex;
            this.PageCount = pageCount;
            this.Action = "Index";
        }

        internal PagingList(List<T> list, int pageSize, int pageIndex, int pageCount, string sortExpression, string defaultSortExpression)
            : this(list, pageSize, pageIndex, pageCount) {

            this.SortExpression = sortExpression;
            this.DefaultSortExpression = defaultSortExpression;
        }

        public RouteValueDictionary RouteValue { get; set; }

        public RouteValueDictionary GetRouteValueForPage(int pageIndex) {

            var dict = this.RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(this.RouteValue);

            dict["page"] = pageIndex;

            if (this.SortExpression != this.DefaultSortExpression) {
                dict["sortExpression"] = this.SortExpression;
            }

            return dict;
        }

        public RouteValueDictionary GetRouteValueForSort(string sortExpression) {

            var dict = this.RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(this.RouteValue);

            if (sortExpression == this.SortExpression) {
                sortExpression = "-" + sortExpression;
            }

            dict["sortExpression"] = sortExpression;

            return dict;
        }

        public int NumberOfPagesToShow { get; set; } = PagingOptions.Current.DefaultNumberOfPagesToShow;

        public int StartPageIndex {
            get {
                int half = (int)((NumberOfPagesToShow - 0.5) / 2);
                var start = Math.Max(1, this.PageIndex - half);
                if (start + NumberOfPagesToShow - 1 > this.PageCount) {
                    start = this.PageCount - NumberOfPagesToShow + 1;
                }
                return Math.Max(1, start);
            }
        }

        public int StopPageIndex {
            get {
                return Math.Min(this.PageCount, StartPageIndex + NumberOfPagesToShow - 1);
            }
        }

    }
}