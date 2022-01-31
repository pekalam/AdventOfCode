#!/bin/bash

INSERT_INPUT=$(cat ./input2_2021.txt | sed "s/ /,/" | sed -r "s/(.*)/(\1),/" | sed "$ s/),/)/" | sed -r "s/(\w+)/'\1'/")

echo "
drop table if exists #day2;
create table #day2(dir varchar(10),  units int);
insert into #day2 values
$INSERT_INPUT;
select sum(x.unit)*(select sum(units) from #day2 where dir='forward') as 'result' from
(
select dir,
case
	when dir = 'up' then -units
	when dir = 'down' then units
end as unit
from #day2 where not dir = 'forward'
) as x
" > ./part1.sql


echo "
drop table if exists #day2;
create table #day2(dir varchar(10),  units int);
insert into #day2 values
$INSERT_INPUT;
with norder as (
select ROW_NUMBER() OVER (ORDER BY (SELECT 1)) as 'n', dir, 
case 
	when dir = 'forward' then units
	when dir = 'up' then -units
	when dir = 'down' then units
end as units
from #day2
)
, c1 as (
select n, fn, units as 'funits' from (select n, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) as 'fn', units from norder where dir='forward') as x
)
select depth*horizontal from (
select sum(d.depth) as 'depth', (select sum(c1.funits) from c1) as 'horizontal' from 
(select sum(norder.units) as 'aim', sum(norder.units)*(select y.funits from c1 as y where c1.fn = y.fn) as 'depth' from norder, c1 where norder.n < c1.n and not dir='forward' group by c1.fn) as d
) as result
" > ./part2.sql