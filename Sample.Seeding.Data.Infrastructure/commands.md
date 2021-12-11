﻿## Install EF Core migrations global tool
```
dotnet tool install dotnet-ef --global --ignore-failed-sources
```

## Update EF Core migrations global tool
```
dotnet tool update dotnet-ef --global --ignore-failed-sources
```

## Add migration
```
dotnet ef migrations add Add_Seeding_Tracking -o Migrations -c EmployeesDatabaseContext
```

## Update database
```
dotnet ef database update -c EmployeesDatabaseContext
```