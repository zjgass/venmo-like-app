use tenmo;

select *
from users;

select *
from users
join accounts on users.user_id = accounts.user_id;

select *
from transfer_statuses;

-- Add some transfers and test.
begin transaction;

insert into users (username, password_hash, salt)
values ('test1', 'something messy', 'salt');

insert into users (username, password_hash, salt)
values ('test2', 'something messy', 'salt');

insert into accounts (user_id, balance)
values
(
	(select user_id
	 from users
	 where username = 'test1'),
	1000
);

insert into accounts (user_id, balance)
values
(
	(select user_id
	 from users
	 where username = 'test2'),
	1000
);

insert into transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount)
values
(
	(select transfer_type_id
	 from transfer_types
	 where transfer_type_desc = 'request'),
	(select transfer_status_id
	 from transfer_statuses
	 where transfer_status_desc = 'approved'),
	(select account_id
	 from accounts
	 where user_id = (select user_id from users
		where username = 'test1')),
	(select account_id
	 from accounts
	 where user_id = (select user_id from users
		where username = 'test2')),
	50
);

select transfer_id, type.transfer_type_desc, status.transfer_status_desc,
	userfrom.username, userto.username, amount
from transfers
join transfer_types as type on transfers.transfer_type_id = type.transfer_type_id
join transfer_statuses as status on transfers.transfer_status_id = status.transfer_status_id
join accounts as accfrom on transfers.account_from = accfrom.account_id
join accounts as accto on transfers.account_to = accto.account_id
join users as userfrom on accfrom.user_id = userfrom.user_id
join users as userto on accto.user_id = userto.user_id
where userfrom.user_id = (select user_id from users where username = 'test1');

rollback transaction; 

-- Withdraw and deposit
begin transaction;

insert into users (username, password_hash, salt)
values ('test1', 'something messy', 'salt');

insert into users (username, password_hash, salt)
values ('test2', 'something messy', 'salt');

insert into accounts (user_id, balance)
values
(
	(select user_id
	 from users
	 where username = 'test1'),
	1000
);

insert into accounts (user_id, balance)
values
(
	(select user_id
	 from users
	 where username = 'test2'),
	1000
);

update accounts
set balance = (
	select balance
	from accounts where user_id = (
		select user_id from users where username = 'test1'
	)
)	+ 100
where user_id = (select user_id from users where username = 'test1');

select *
from accounts
where user_id = (select user_id from users where username = 'test1');

rollback transaction;