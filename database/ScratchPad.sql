use tenmo;

select *
from users;

select *
from users
join accounts on users.user_id = accounts.user_id;
 