select OrderId,
	min(WhenReceived) as small,
	max(whenReceived) as large,
	datediff(s, min(WhenReceived), max(WhenReceived)) as cycletime,
	count(*)
from app.log_entries
group by OrderId
order by cycletime desc;