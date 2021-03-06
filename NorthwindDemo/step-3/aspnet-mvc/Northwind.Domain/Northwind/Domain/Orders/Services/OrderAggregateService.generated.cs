﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#region Copyright
// -----------------------------------------------------------------------
// <copyright company="cdmdotnet Limited">
//     Copyright cdmdotnet Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
#endregion
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Cqrs.Commands;
using Cqrs.Authentication;
using cdmdotnet.Logging;
using Cqrs.Repositories.Queries;
using Cqrs.Services;
using Northwind.Domain.Orders.Commands;
using Northwind.Domain.Orders.Repositories;

namespace Northwind.Domain.Orders.Services
{
	[GeneratedCode("CQRS UML Code Generator", "1.500.0.1")]
	[DataContract(Namespace="https://cqrs/Domain/Orders/1001/")]
	public partial class OrderService : IOrderService
	{
		protected ICommandSender<Cqrs.Authentication.ISingleSignOnToken> CommandSender { get; private set; }

		protected IUnitOfWorkService UnitOfWorkService { get; private set; }

		protected IOrderRepository OrderRepository { get; private set; }

		protected IQueryFactory QueryFactory { get; private set; }

		protected IAuthenticationTokenHelper<Cqrs.Authentication.ISingleSignOnToken> AuthenticationTokenHelper { get; set; }

		protected ICorrelationIdHelper CorrelationIdHelper { get; set; }

		protected ILogger Logger { get; private set; }

		public OrderService(ICommandSender<Cqrs.Authentication.ISingleSignOnToken> commandSender, IUnitOfWorkService unitOfWorkService, IQueryFactory queryFactory, IAuthenticationTokenHelper<Cqrs.Authentication.ISingleSignOnToken> authenticationTokenHelper, ICorrelationIdHelper correlationIdHelper, IOrderRepository orderRepository, ILogger logger)
		{
			CommandSender = commandSender;
			UnitOfWorkService = unitOfWorkService;
			QueryFactory = queryFactory;
			AuthenticationTokenHelper = authenticationTokenHelper;
			CorrelationIdHelper = correlationIdHelper;
			OrderRepository = orderRepository;
			Logger = logger;
		}

		/// <summary>
		/// Create a new instance of the <see cref="Entities.OrderEntity"/>
		/// </summary>
		public IServiceResponseWithResultData<Entities.OrderEntity> CreateOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest)
		{
			AuthenticationTokenHelper.SetAuthenticationToken(serviceRequest.AuthenticationToken);
			CorrelationIdHelper.SetCorrelationId(serviceRequest.CorrelationId);
			UnitOfWorkService.SetCommitter(this);
			Entities.OrderEntity item = serviceRequest.Data;
			if (item.Rsn == Guid.Empty)
				item.Rsn = Guid.NewGuid();

			var command = new CreateOrderCommand(item.Rsn, item.OrderId, item.CustomerId, item.EmployeeId, item.OrderDate, item.RequiredDate, item.ShippedDate, item.ShipViaId, item.Freight, item.ShipName, item.ShipAddress, item.ShipCity, item.ShipRegion, item.ShipPostalCode, item.ShipCountry);
			OnCreateOrder(serviceRequest, command);
			CommandSender.Send(command);
			OnCreatedOrder(serviceRequest, command);

			UnitOfWorkService.Commit(this);
			return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity>(item));
		}

		partial void OnCreateOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest, CreateOrderCommand command);

		partial void OnCreatedOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest, CreateOrderCommand command);

		/// <summary>
		/// Update an existing instance of the <see cref="Entities.OrderEntity"/>
		/// </summary>
		public IServiceResponseWithResultData<Entities.OrderEntity> UpdateOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest)
		{
			AuthenticationTokenHelper.SetAuthenticationToken(serviceRequest.AuthenticationToken);
			CorrelationIdHelper.SetCorrelationId(serviceRequest.CorrelationId);
			UnitOfWorkService.SetCommitter(this);
			Entities.OrderEntity item = serviceRequest.Data;

			var locatedItem = OrderRepository.Load(item.Rsn);
			if (locatedItem == null)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = ServiceResponseStateType.FailedValidation });

			var command = new UpdateOrderCommand(item.Rsn, item.OrderId, item.CustomerId, item.EmployeeId, item.OrderDate, item.RequiredDate, item.ShippedDate, item.ShipViaId, item.Freight, item.ShipName, item.ShipAddress, item.ShipCity, item.ShipRegion, item.ShipPostalCode, item.ShipCountry);
			ServiceResponseStateType? serviceResponseStateType = null;
			OnUpdateOrder(serviceRequest, ref command, locatedItem, ref serviceResponseStateType);
			if (serviceResponseStateType != null && serviceResponseStateType != ServiceResponseStateType.Succeeded)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = serviceResponseStateType.Value });

			CommandSender.Send(command);
			OnUpdatedOrder(serviceRequest, ref command, locatedItem, ref serviceResponseStateType);
			if (serviceResponseStateType != null && serviceResponseStateType != ServiceResponseStateType.Succeeded)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = serviceResponseStateType.Value });

			UnitOfWorkService.Commit(this);
			return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity>(item));
		}

		partial void OnUpdateOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest, ref UpdateOrderCommand command, Entities.OrderEntity locatedItem, ref ServiceResponseStateType? serviceResponseStateType);

		partial void OnUpdatedOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest, ref UpdateOrderCommand command, Entities.OrderEntity locatedItem, ref ServiceResponseStateType? serviceResponseStateType);

		/// <summary>
		/// Logically delete an existing instance of the <see cref="Entities.OrderEntity"/>
		/// </summary>
		public IServiceResponse DeleteOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest)
		{
			AuthenticationTokenHelper.SetAuthenticationToken(serviceRequest.AuthenticationToken);
			CorrelationIdHelper.SetCorrelationId(serviceRequest.CorrelationId);
			UnitOfWorkService.SetCommitter(this);
			Entities.OrderEntity item = serviceRequest.Data;

			var locatedItem = OrderRepository.Load(item.Rsn, false);
			if (locatedItem == null)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = ServiceResponseStateType.FailedValidation });

			if (locatedItem.IsLogicallyDeleted)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = ServiceResponseStateType.FailedValidation });

			var command = new DeleteOrderCommand(item.Rsn);
			ServiceResponseStateType? serviceResponseStateType = null;
			OnDeleteOrder(serviceRequest, ref command, locatedItem, ref serviceResponseStateType);
			if (serviceResponseStateType != null && serviceResponseStateType != ServiceResponseStateType.Succeeded)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = serviceResponseStateType.Value });

			CommandSender.Send(command);
			OnDeletedOrder(serviceRequest, ref command, locatedItem, ref serviceResponseStateType);
			if (serviceResponseStateType != null && serviceResponseStateType != ServiceResponseStateType.Succeeded)
				return CompleteResponse(new ServiceResponseWithResultData<Entities.OrderEntity> { State = serviceResponseStateType.Value });

			UnitOfWorkService.Commit(this);
			return CompleteResponse(new ServiceResponse());
		}

		partial void OnDeleteOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest, ref DeleteOrderCommand command, Entities.OrderEntity locatedItem, ref ServiceResponseStateType? serviceResponseStateType);

		partial void OnDeletedOrder(IServiceRequestWithData<Cqrs.Authentication.ISingleSignOnToken, Entities.OrderEntity> serviceRequest, ref DeleteOrderCommand command, Entities.OrderEntity locatedItem, ref ServiceResponseStateType? serviceResponseStateType);

		protected virtual TServiceResponse CompleteResponse<TServiceResponse>(TServiceResponse serviceResponse)
			where TServiceResponse : IServiceResponse
		{
			serviceResponse.CorrelationId = CorrelationIdHelper.GetCorrelationId();
			return serviceResponse;
		}
	}
}
