﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using Cqrs.Authentication;
using Cqrs.Events;
using Cqrs.Services;
using Northwind.Domain.Orders.Entities;
using Northwind.Domain.Orders.Events;
using Northwind.Domain.Orders.Services;
using Northwind.Domain.Orders.Services.ServiceHost.Ninject.ServiceChannelFactories;
using Northwind.Web.Dashboard.Models;

namespace Northwind.Web.Dashboard.Controllers
{
	public class OrdersController : Controller
	{
		public ActionResult Orders_Read([DataSourceRequest] DataSourceRequest request)
		{
			return Json(GetOrders().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Orders_Create([DataSourceRequest]DataSourceRequest request, OrderViewModel order)
		{
			if (ModelState.IsValid)
			{
				order.OrderID = new Random(Guid.NewGuid().GetHashCode()).Next();
				var orderServiceFactory = new HttpOrderServiceChannelFactory();

				IOrderService service = orderServiceFactory.CreateChannel();
				var request2 = new ServiceRequestWithData<ISingleSignOnToken, OrderEntity>
				{
					AuthenticationToken = new SingleSignOnToken(),
					CorrelationId = Guid.NewGuid(),
					Data = new OrderEntity
					{
						OrderId = order.OrderID,
						CustomerId = order.CustomerID,
						EmployeeId = order.EmployeeID,
						OrderDate = order.OrderDate,
						ShipCountry = order.ShipCountry,
						ShipViaId = order.ShipVia,
						ShippedDate = order.ShippedDate,
						ShipName = order.ShipName,
						ShipAddress = order.ShipAddress,
						ShipCity = order.ShipCity,
						ShipPostalCode = order.ShipPostalCode
					}
				};

				// Act
				IServiceResponseWithResultData<OrderEntity> response2 = service.CreateOrder(request2);
				Guid correlationId = response2.CorrelationId;

				var request3 = new ServiceRequestWithData<ISingleSignOnToken, Guid>
				{
					AuthenticationToken = new SingleSignOnToken(),
					Data = correlationId
				};

				IEnumerable<EventData> result;
				int loopCount = 0;

				// We're using an in-process bus so this isn't actually necassary, but it demonstrates how to wait for the command and event bus to complete their processes
				do
				{
					// Wait 0.5 of a second and ask again
					if (loopCount > 0)
						System.Threading.Thread.Sleep(500);

					IServiceResponseWithResultData<IEnumerable<EventData>> response3 = service.GetEventData(request3);
					result = response3.ResultData;
					result = result.Where(x => x.EventType == typeof (OrderCreated).AssemblyQualifiedName);
				} while (!result.Any() && loopCount++ < 10);

			}
			return Json(new[] { order }.ToDataSourceResult(request, ModelState));
		}

		public ActionResult Orders_Update([DataSourceRequest]DataSourceRequest request, OrderViewModel order)
		{
			if (ModelState.IsValid)
			{
				using (var northwind = new NorthwindEntities())
				{
					var entity = northwind.Orders.FirstOrDefault(o => o.OrderID == order.OrderID);
					if (entity == null)
					{
						string errorMessage = string.Format("Cannot update record with OrderID:{0} as it's not available.", order.OrderID);
						ModelState.AddModelError("", errorMessage);
					}
					else
					{
						entity.CustomerID = order.CustomerID;
						entity.EmployeeID = order.EmployeeID;
						entity.OrderDate = order.OrderDate;
						entity.ShipCountry = order.ShipCountry;
						entity.ShipVia = order.ShipVia;
						entity.ShippedDate = order.ShippedDate;
						entity.ShipName = order.ShipName;
						entity.ShipAddress = order.ShipAddress;
						entity.ShipCity = order.ShipCity;
						entity.ShipPostalCode = order.ShipPostalCode;

						northwind.Orders.Attach(entity);
						northwind.Entry(entity).State = EntityState.Modified;
						northwind.SaveChanges();
					}
				}
			}
			return Json(new[] { order }.ToDataSourceResult(request, ModelState));
		}

		public ActionResult Orders_Destroy([DataSourceRequest]DataSourceRequest request, OrderViewModel order)
		{
			if (ModelState.IsValid)
			{
				using (var northwind = new NorthwindEntities())
				{
					List<Order_Detail> details = northwind.Order_Details.Where(od => od.OrderID == order.OrderID).ToList();

					foreach (var orderDetail in details)
					{
						northwind.Order_Details.Remove(orderDetail);
					}

					var entity = new Order
					{
						CustomerID = order.CustomerID,
						OrderID = order.OrderID,
						EmployeeID = order.EmployeeID,
						OrderDate = order.OrderDate,
						ShipCountry = order.ShipCountry,
						ShipVia = order.ShipVia,
						ShippedDate = order.ShippedDate,
						ShipName = order.ShipName,
						ShipAddress = order.ShipAddress,
						ShipCity = order.ShipCity,
						ShipPostalCode = order.ShipPostalCode
					};
					northwind.Orders.Attach(entity);
					northwind.Orders.Remove(entity);
					northwind.SaveChanges();
				}
			}
			return Json(new[] { order }.ToDataSourceResult(request, ModelState));
		}

		public ActionResult Countries_Read()
		{
			var countries = GetOrders().GroupBy(o => o.ShipCountry).Select(group => new
			{
				Country = group.Key == null ? " Other" : group.Key
			}).OrderBy(c => c.Country).ToList();

			return Json(countries, JsonRequestBehavior.AllowGet);
		}

		private static IQueryable<OrderViewModel> GetOrders()
		{
			var orderServiceFactory = new HttpOrderServiceChannelFactory();

			IOrderService service = orderServiceFactory.CreateChannel();
			var request2 = new ServiceRequestWithData<ISingleSignOnToken, OrderServiceGetAllOrdersParameters>
			{
				AuthenticationToken = new SingleSignOnToken(),
				CorrelationId = Guid.NewGuid(),
				Data = new OrderServiceGetAllOrdersParameters()
			};

			// Act
			IServiceResponseWithResultData<IEnumerable<OrderEntity>> response = service.GetAllOrders(request2);

			IQueryable<OrderViewModel> orders = response.ResultData.Select(order => new OrderViewModel
			{
				CustomerID = order.CustomerId,
				OrderID = order.OrderId,
				EmployeeID = order.EmployeeId,
				OrderDate = order.OrderDate,
				ShipCountry = order.ShipCountry,
				ShipVia = order.ShipViaId,
				ShippedDate = order.ShippedDate,
				ShipName = order.ShipName,
				ShipAddress = order.ShipAddress,
				ShipCity = order.ShipCity,
				ShipPostalCode = order.ShipPostalCode

			}).AsQueryable();

			return orders;
		}

	}
}