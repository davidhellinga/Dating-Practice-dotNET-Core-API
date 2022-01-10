all : clean restore build

clean:
	dotnet clean

restore:
	dotnet restore

build: 
	dotnet build

up:
	dotnet watch run

migration: $(migration)
	dotnet ef migrations add $(m)

dbupdate:
	 dotnet ef database update
 
dbreset:
	dotnet ef database drop
	make db-update