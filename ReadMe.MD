# Lab#1
## Topics Involved
- N Layered Architecture (N=3)
  - Presentation Layer
    - API Endpoints using Minimal API
  - Business Logic Layer
    - Services
  - Data Access Layer
    - Repositories
- Database
- ORM
  - EF Core
  - Migrations
- DTO
- Infrastructure As a Code
  - Docker (docker-compose)

## Task
1. Create and apply migration adding VinCode to Car entity
2. Hide VinCode from `/cars` API endpoint
3. Fix Bugs:
   1. Color is not saved
   2. Price is not caved
4. Update `POST /car` endpoint to update existing car if the car with the same VinCode already exists
5. Mark `VinCode` as unique in database, add and apply migration. [Doc](https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=data-annotations#index-uniqueness)


## Time Limit
45 minutes

## Grading
- 1 point - Task 1
- 1 point - Task 2
- 1 point - Task 3
- 1 point - Task 4
- 1 point - Task 5

## How to submit
1. Fork this repository (https://github.com/podkolzzzin/SDPLabs)
2. Create a new branch with your name
3. Commit your changes to the new branch
4. Create a pull request to the `main` branch of this repository

----

# Lab#2
## Topics Involved
- EventBus.
  - RabbitMQ
    - Queue
    - Exchange
- ORM
  - EF Core
    - Migrations
    - Navigation Properties

## Task
1. Add model that describes Mileage(Distance, DateTime) of the car
   1. Add and apply migration
   2. Using `POST /car` create a couple of cars
2. Start listening to the `CarMovedEvent`
   1. Insert new Mileage record when CarMovedEvent is received
3. Update `GET /cars` endpoint to return total Mileage of each car
4. Publish events
   1. `CarReceivedDangerousMileage` when Mileage is more than 100_000
   2. `CarReceivedCriticalMileage` when Mileage is more than 200_000
5. Create new simple console application `SDPLabs.Alerts` that listens to `CarReceivedDangerousMileage` and `CarReceivedCriticalMileage`.
   1. Application should print(Mark, Model, Id, TotalMileage) using yellow color when `CarReceivedDangerousMileage` is received
   2. Application should print(Mark, Model, Id, TotalMileage) using red color when `CarReceivedCriticalMileage` is received

## Time Limit
75 minutes

## Grading
- 1 point - Task 1
- 1 point - Task 2
- 1 point - Task 3
- 1 point - Task 4
- 1 point - Task 5

## How to submit
1. Fork this repository (https://github.com/podkolzzzin/SDPLabs)
   1. Checkout `lab2-branch`
2. Create a new branch with your name
3. Commit your changes to the new branch
4. Create a pull request to the `lab2-branch` branch of this repository

## Recommendations
- Use `docker-compose up` to start RabbitMQ
- Look how `SDPLabs.RandomPublisher` works, launch it. It will publish `CarMovedEvent` to random cars
- There is class in `SDPLabs` `RabbitMqListenerService` that must be used to listen to the events
- Start with listening to `CarMovedEvent`. Print something in console, check how it works
- `SDPLabs.RandomPublisher` project to publish test events. To Launch it click Right Mouse Button on the project -> Debug -> Start new instance
- Handle `CarMovedEvent`: Make it append data to the database
- Publish `CarReceivedDangerousMileage` and `CarReceivedCriticalMileage` events
- Create new project in `Apps` solution folder called `SDPLabs.Alerts`
- Add reference to `SDPLabs.Common.Events` project
- Subscribe to `CarReceivedDangerousMileage` and `CarReceivedCriticalMileage` events
- Print Alerts to the console

## After Lab. Interesting Moments:
- Please, pay attention that the common thing for multiple apps is moved to a separate project and it is referenced to multiple `Apps`