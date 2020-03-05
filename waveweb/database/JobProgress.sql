create table JobProgress
(
JobId uniqueidentifier not null,
Progress float not null,
DateLastUpdated datetime not null,
constraint pk_JobProgress_JobId primary key clustered (JobId)
)
