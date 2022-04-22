﻿using DeliveryOriginal.Admin.Core.Extensions;
using DeliveryOriginal.Admin.Core.Identity;
using DeliveryOriginal.Admin.Core.Interfaces;
using DeliveryOriginal.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeliveryOriginal.Admin.Controllers
{
    [CustomAuthorize(Role = RoleGroup.Regulars)]
    public class OrderController : Controller
    {
        protected readonly IUnitOfWork UnitOfWork;
        public OrderController()
        {
            UnitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> OrderDashboard(int? defaultOrderId, OrderOrderBy orderBy = OrderOrderBy.OrderNumberDesc)
        {
            var orders = await UnitOfWork.OrderRepository.GetAll();

            var dashboardOrders = GetDashboardOrders(orders);

            dashboardOrders = SortDashboardOrder(dashboardOrders, orderBy);

            var model = new OrderDashboardVM
            {
                Orders = dashboardOrders,
                SelectedOrder = defaultOrderId.HasValue ? dashboardOrders?.Where(order => order.Id == defaultOrderId)?.FirstOrDefault() : null,
                OrderOrderBy = orderBy
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> RenderDashboardOrderDetailsPartial(int? orderId = null)
        {
            if (orderId.HasValue)
            {
                var order = await UnitOfWork.OrderRepository.Get(orderId.Value);

                if (order != null)
                {
                    var model = GetDashboardOrder(order);
                    return PartialView("_OrderDetails", model);
                }
            }

            return PartialView("_OrderDetails", null);
        }

        [HttpGet]
        public async Task<string> AcceptOrder(int orderId)
        {
            var order = await UnitOfWork.OrderRepository.Get(orderId);

            if (order != null)
            {
                order.Status = OrderStatus.ReadyForCooking;
                await UnitOfWork.OrderRepository.Update(order);
                return order.Status.GetDisplayName();
            }

            return null;
        }

        [HttpGet]
        public async Task<string> DeclineOrder(int orderId)
        {
            var order = await UnitOfWork.OrderRepository.Get(orderId);

            if (order != null)
            {
                order.Status = OrderStatus.Declined;
                await UnitOfWork.OrderRepository.Update(order);
                return order.Status.GetDisplayName();
            }

            return null;
        }

        [HttpGet]
        public async Task<ActionResult> RenderOrdersListWithSort(OrderOrderBy orderBy)
        {
            var orders = await UnitOfWork.OrderRepository.GetAll();

            var dashboardOrders = GetDashboardOrders(orders);

            dashboardOrders = SortDashboardOrder(dashboardOrders, orderBy);

            return PartialView("_OrderList", dashboardOrders);
        }

        private List<OrderDetailsVM> SortDashboardOrder(List<OrderDetailsVM> dashboardOrders, OrderOrderBy orderBy)
        {
            switch (orderBy)
            {
                case OrderOrderBy.OrderNumberAsc:
                    dashboardOrders = dashboardOrders.OrderBy(order => order.Id).ToList();
                    break;
                case OrderOrderBy.OrderStatusAsc:
                    dashboardOrders = dashboardOrders.OrderBy(order => order.Status).ToList();
                    break;
                case OrderOrderBy.OrderStatusDesc:
                    dashboardOrders = dashboardOrders.OrderByDescending(order => order.Status).ToList();
                    break;
                case OrderOrderBy.OrderNumberDesc:
                default:
                    dashboardOrders = dashboardOrders.OrderByDescending(order => order.Id).ToList();
                    break;
            }
            return dashboardOrders;
        }

        private OrderDetailsVM GetDashboardOrder(Order order)
        {
            var dashboardOrder = new OrderDetailsVM
            {
                Id = order.Id,
                Address = order.Address,
                CurrentEmployee = order.CurrentEmployee,
                Customer = order.Customer,
                Dishes = order.Dishes,
                Status = order.Status,
                SubmittedAt = order.SubmittedAt
            };
            if (dashboardOrder?.Dishes != null)
            {
                foreach (var dish in dashboardOrder?.Dishes)
                {
                    dashboardOrder.TotalCost += dish.Cost;
                }
            }

            return dashboardOrder;
        }

        private List<OrderDetailsVM> GetDashboardOrders(List<Order> orders)
        {
            var dashboardOrders = new List<OrderDetailsVM>();
            foreach (var order in orders)
            {
                var dashboardOrder = new OrderDetailsVM
                {
                    Id = order.Id,
                    Address = order.Address,
                    CurrentEmployee = order.CurrentEmployee,
                    Customer = order.Customer,
                    Dishes = order.Dishes,
                    Status = order.Status,
                    SubmittedAt = order.SubmittedAt
                };
                if (dashboardOrder?.Dishes != null)
                {
                    foreach (var dish in dashboardOrder?.Dishes)
                    {
                        dashboardOrder.TotalCost += dish.Cost;
                    }
                }

                dashboardOrders.Add(dashboardOrder);
            }

            return dashboardOrders;
        }
    }
}