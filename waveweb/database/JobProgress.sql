create table JobProgress
(
JobId uniqueidentifier not null,
Progress float not null,
IsComplete bit not null constraint JobProgress_IsComplete_Default default(0),
DateLastUpdated datetime not null,
constraint pk_JobProgress_JobId primary key clustered (JobId)
)
