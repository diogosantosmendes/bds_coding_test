![integration tests badge](https://github.com/diogosantosmendes/bds_coding_test/actions/workflows/integration_tests.yml/badge.svg)
![unit tests badge](https://github.com/diogosantosmendes/bds_coding_test/actions/workflows/unit_tests.yml/badge.svg)

### Content
1. [Decisions and any assumptions](#Decisions-and-assumptions)
2. [Entities Class Diagram](#Entities-Class-Diagram)
3. [Services Diagram](#Services-Diagram)
4. [Solution projects dependencies](#Solution-projects-dependencies)

## Decisions and assumptions 
- I developed a solution following the principles of a Clean Architecture, allowing the benefits:
	- Separation of Concerns by system division into layers, each with a specific responsibility, making the code easier to understand, navigate and maintain.
	- Testability by isolating business logic from external dependencies, it becomes easier to write unit tests.
	- Flexibility as the architecture allows system changes without affecting other parts, making it easier to adapt to new requirements.
	- Reusability as components can be reused across different parts of the application or even in different projects.
- Since WebUI was not a requirement I'm presenting a simple API that calls directly the Application layer, ignoring Authentication, Authorization, Model validations, Anti-Forgery token validations, Telemetry,  etc;
- Since the scope of the project is a simple sytstem that manages auctions, I'm ignoring the user management domain, and so the bids will only receive a hipotetical userId;
- I added the *Manufacturer* and *Model* entities to demonstrate how I would integrate with the *Vehicle*, but then I didn't develop any service to provide CRUD operations on that;
- As the DB management was not a requirement I created a dummy in-memory data system, similar to a DbContext, that is injected as a Singleton (to keep the data during runtime accross requests), this way we just need to run the Api and start calling the endpoints, Also the code doesn't deal with transactions, so I assume every transaction will apply only a single write operation;
- As it was requested to focus on Unit testing, I created unit tests for the conditions defined only on the *Application* level, and also a simple Integration tests stack for all the endpoints (testing all the flow throught the architecture layers);
- Added 2 github actions to run both (Unit and Integration) Tests stack on every commit on *main* branch, assigning the status of the last run to the badges on the top of this doc.

## Entities Class Diagram

```mermaid
classDiagram
	Model o-- Manufacturer

	IVehicle *-- Manufacturer
	IVehicle *-- Model
	IVehicle *-- VehicleType

	IVehicle <|.. Hatchback
	IVehicle <|.. Sudan
	IVehicle <|.. Suv
	IVehicle <|.. Truck

    Auction *-- IVehicle
    Auction *-- AuctionState

    Bid *-- Auction

    class Manufacturer{
        +string Id
        +string Name
        +List< Model > Models
    }
    class Model{
        +string Id
        +string Name
    }
    class VehicleType{
        <<Enumeration>>
        HATCHBACK
        SUDAN
        SUV
        TRUCK
    }
    class IVehicle{
	<<interface>>
        +string Id
        +VehicleType Type
        +string ManufacturerId
        +string ModelId
        +int Year
        +float StartingBid
    }
    class Hatchback{
        +int Doors
    }
    class Sudan{
        +int Doors
    }
    class Suv{
        +int Seats
    }
    class Truck{
        +int LoadCapacity
    }
    class AuctionState{
        <<Enumeration>>
        STARTED
        CLOSED
    }
    class Auction{
        +string Id
        +string VehicleId
        +AuctionState State
        +float HighestBidValue
    }
    class Bid{
        +string Id
        +string AuctionId
        +string UserId
        +float Value
    }
```

## Services Diagram

```mermaid
classDiagram
    class VehiclesService{
        +addVehicle(IVehicle vehicle) Result
        +searchVehicles(Page page, Filter? filters, Sort? sort) PageResult< IVehicle >
    }

    class AuctionsService{
        +start(string vehicleId) DataResponse< Auction >
        +bid(string userId, string auctionId, float value) Response
        +close(string auctionId) Response
    }
```

## Solution projects dependencies
```mermaid
flowchart TB
Application-->p
Infrastructure-->p
subgraph Presentation
	p["Pesentation.WebApi"]
end
da-->aa
di-->ai
ds-->as
subgraph Application
	aa["Application.Auctions"]
	ai["Application.Inventory"]
	as["Application.Shared"]
	as-->ai
	as-->aa
	ai-->aa
end
subgraph Domain
	da["Domain.Auctions"]
	di["Domain.Inventory"]
	ds["Domain.Shared"]
	ds-->di
	ds-->da
	di-->da
end
aa-->ia
ai-->ii
as-->is
subgraph Infrastructure
	ia["Infrastructure.Auctions"]
	ii["Infrastructure.Inventory"]
	is["Infrastructure.Shared"]
	is-->ii
	is-->ia
end
Application-->tu
Presentation-->ti
subgraph Tests
	tu["Tests.Unit"]
	ti["Tests.Integration"]
end
```
