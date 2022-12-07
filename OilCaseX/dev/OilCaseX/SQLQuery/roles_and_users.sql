SELECT UserName, Name 
FROM AspNetUsers 
    inner join aspnetuserroles on AspNetUsers.id = aspnetuserroles.userid 
    inner join aspnetroles on aspnetuserroles.roleid = aspnetroles.id order by UserName;
