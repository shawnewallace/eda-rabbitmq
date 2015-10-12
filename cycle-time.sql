select OrderId,
	min(WhenReceived) as small,
	max(whenReceived) as large,
	datediff(s, min(WhenReceived), max(WhenReceived)) as cycletime,
	count(*)
from app.log_entries
group by OrderId
order by cycletime desc;


select * from app.log_entries
where orderid = '2D4BE6EB-6274-4074-8690-C014D3FCA645'
order by whenReceived asc;