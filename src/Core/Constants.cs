using System;
using System.Collections.Generic;

namespace eda.core
{

	public static class AppConstants
	{
		public const string EXCHANGE_NAME = "retail_system";
		public const string DATA_SERVICE_QUEUE_NAME = "data_q";
		public const string LOGGING_QUEUE_NAME = "logger_q";
		public const string INVOICING_QUEUE_NAME = "invoicer_q";
		public const string SHIPPING_QUEUE_NAME = "shipping_q";
		public const string WAREHOUSE_QUEUE_NAME = "warehouse_q";
		public const string MASTER_CUSTOMER_QUEUE_NAME = "customer_q";

		public const string ORDER_ACCEPTED_EVENT = "order_accepted";
		public const string CUSTOMER_BILLED_EVENT = "customer_billed";
		public const string READY_FOR_SHIPMENT_EVENT = "order_ready_to_ship";
		public const string SHIPPED_EVENT = "order_shipped";
		public const string ORDER_FULLFILLED_EVENT = "order_fulfilled";
		public const string NEW_CUSTOMER_EVENT = "new_customer_submitted";
		public const string CUSTOMER_CREATED_EVENT = "customer_created";

		public static IEnumerable<string> EventCollection => new List<string>()
				{
						ORDER_ACCEPTED_EVENT,
						CUSTOMER_BILLED_EVENT,
						READY_FOR_SHIPMENT_EVENT,
						SHIPPED_EVENT,
						ORDER_FULLFILLED_EVENT,
						NEW_CUSTOMER_EVENT,
						CUSTOMER_CREATED_EVENT
				};
	}
}
