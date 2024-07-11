## Useful commands in .net console CLI

Install tooling

~~~bash
dotnet tool update -g dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator 
~~~

## EF Core migrations

Run from solution folder

~~~bash
dotnet ef migrations --project App.DAL.EF --startup-project WebApp add XXX
dotnet ef database   --project App.DAL.EF --startup-project WebApp update
dotnet ef database   --project App.DAL.EF --startup-project WebApp drop
~~~


## MVC controllers

Install from nuget:
- Microsoft.VisualStudio.Web.CodeGeneration.Design
- Microsoft.EntityFrameworkCore.SqlServer


Run from WebApp folder!

~~~bash
cd WebApp

dotnet aspnet-codegenerator controller -name AppRolesController        -actions -m  App.Domain.Identity.AppRole        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name AppUserRolesController        -actions -m  App.Domain.Identity.AppUserRole        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name AppUsersController        -actions -m  App.Domain.Identity.AppUser        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f


# use area

dotnet aspnet-codegenerator controller -name CompaniesController        -actions -m  App.Domain.Company        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name UserCompaniesController        -actions -m  App.Domain.UserCompany        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name CompanyRolesController        -actions -m  App.Domain.CompanyRole        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name ContractsController        -actions -m  App.Domain.Contract        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name LoansController        -actions -m  App.Domain.Loan        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name LoanOffersController        -actions -m  App.Domain.LoanOffer        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name LoanRequestsController        -actions -m  App.Domain.LoanRequest        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name TransactionsController        -actions -m  App.Domain.Transaction        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name UploadsController        -actions -m  App.Domain.Upload        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f


cd ..
~~~

Api controllers
~~~bash
dotnet aspnet-codegenerator controller -name CompaniesController  -m  App.Domain.Company        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name CompanyRolesController  -m  App.Domain.CompanyRole        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name LoansController  -m  App.Domain.Loan        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name LoanOffersController  -m  App.Domain.LoanOffer        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name LoanRequestsController  -m  App.Domain.LoanRequest        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name TransactionsController  -m  App.Domain.Transaction        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name UserCompaniesController  -m  App.Domain.UserCompany        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f


~~~

To convert dt to universal time for postgres: dt.ToUniversalTime()
and back to local in ui: dt.ToLocalTime()


### Generate Identity custom UI
~~~bash
dotnet aspnet-codegenerator identity -f --userClass=App.Domain.Identity.AppUser
~~~


docker buildx build --progress=plain --force-rm --push -t akaver/webapp:latest .

# multiplatform build on apple silicon
# https://docs.docker.com/build/building/multi-platform/
docker build -t webapp:latest
docker buildx create --name mybuilder --bootstrap --use
docker buildx build --platform linux/amd64 -t robertkin/webapp:latest --push .

