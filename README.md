# $/ABAC System

## Overview
This system is responsible for managing the ABAC (Attribute Based Access Control) system for cloud resources. 
It is responsible for managing the ABAC policies, and for enforcing them. This is done by several components: 
1. UserManagement Service - responsible for managing the users and their attributes.
2. PolicyManagement Service - responsible for managing the cloud resource policies and their corresponding conditions.
3. AttributesManagement Service - responsible for managing the attributes of the users and the policies.
4. ResourceManagement Service - responsible for managing the resources and their corresponding policies.
5. PreAuthorizationEngine Service - an engine responsible for the pre authorization of the users based on previous user's policy approvals.
6. AuthorizationEngine Service - an engine responsible for the authorization of the users based on the user's attributes and the resource's policies.
7. AuthorizationManagement Service - responsible for managing the user's authorization requests by using PreAuthorization & Authorization engines.

This solution contains only three test projects in order to demonstrate the system's functionality.
These projects are meant to cover the most important scenarios of the system, since the rest of the system's logic is pretty straight forward.

## How to Run the Solution
In order to run the solution you need to have the following installed:
1. Visual Studio 2022
2. Azure Cosmos DB Emulator
3. Azure Functions Extension for Visual Studio

Each service is configured to run locally on a different port. Therefore, in order to run the solution you need to configure VS 
to run all services at once. This can be done by right clicking on the solution and selecting "Properties". Then, select 
"Multiple startup projects" and set all services to "Start" (you can ignore the test projects and Common project).
In order to run the solution, press F5 or the 'Start' button.

Please note that sometimes the Cosmos emulator fails to create the required databases and collections. This should be resolved 
by re-running the services.

Please note that running the solution locally as a containerized solution is problematic. 
The CosmosDB emulator does not integrate well with the containerized services, and therefore the services fail to connect to the DB.
This issue only affects local testing and does not occur in a cloud environment.
Docker files are provided for each service anyway.

## User-facing API Schemas
1. Get User:
* Request Example:
GET http://localhost:7071/api/users/tomg

* Response Example:
```json
{
    "userId": "tomg",
    "attributes": {
        "role": "admin",
        "level": 10,
        "rank": 4
    }
}
```

2. Create User:
* Request Example:
POST http://localhost:7071/api/users
Body:
```json
{
	"userId": "tomg",
	"attributes": {
		"role": "admin",
		"level": 10,
		"rank": 4
	}
}
```
* Response Example:
```json
{
    "userId": "tomg",
    "attributes": {
        "role": "admin",
        "level": 10,
        "rank": 4
    }
}
```

3. Update User Attributes:
PUT  http://localhost:7071/api/users/tomg
Body:
```json
{
    "startsWith": "user",
    "rank": 15
}
```
* Response Example:
```json
{
	"userId": "tomg",
    "attributes": {
        "startsWith": "user",
        "rank": 15
    }
}
```

4. Update User Attribute:
PATCH http://localhost:7071/api/users/tomg/attributes/isNotAdmin
Body:
```json
{
   "value": true
}
```
* Response Example:
```json
{
	"userId": "tomg",
    "attributes": {
        "startsWith": "user",
        "rank": 15,
        "isNotAdmin": true
    }
}
```

5. Delete User Attribute:
DELETE http://localhost:7071/api/users/tomg/attributes/isNotAdmin

* Response Example:
```json
{
	"userId": "tomg",
    "attributes": {
        "startsWith": "user",
        "rank": 15,
    }
}
```

6. Get Attribute:
* Request Example:
GET http://localhost:7079/api/attributes/role

* Response Example:
```json
{
    "attributeName": "role",
    "attributeType": "string"
}
```

7. Create Attribute:
* Request Example:
POST http://localhost:7079/api/attributes
Body:
```json
{
    "attributeName": "IsSuperAdmin",
    "attributeType": "boolean"
}
```
* Response Example:
```json
{
    "attributeName": "IsSuperAdmin",
    "attributeType": "boolean"
}
```

8. Get Policy:
* Request Example:
* GET http://localhost:7077/api/policies/3

* Response Example:
```json
{
    "policyId": "3",
    "conditions": [
        {
            "attributeName": "rank",
            "operator": ">",
            "value": 3
        },
        {
            "attributeName": "role",
            "operator": "startsWith",
            "value": "admin"
        }
    ]
}
```

9. Create Policy:
* Request Example:
POST http://localhost:7077/api/policies
Body:
```json
{
    "policyId": "4",
    "conditions": [
        {
            "attributeName": "level",
            "operator": "=",
            "value": 2
        }
    ]
}
```
* Response Example:
```json
{
    "policyId": "4",
    "conditions": [
        {
            "attributeName": "level",
            "operator": "=",
            "value": 2
        }
    ]
}
```

10. Update Policy's Conditions:
PUT http://localhost:7077/api/policies/4
Body:
```json
{
    "value": [
        {
            "attributeName": "rank",
            "operator": ">",
            "value": 3
        },
        {
            "attributeName": "role",
            "operator": "startsWith",
            "value": "admin"
        }
    ]
}
```
* Response Example:
```json
{
    "policyId": "4",
    "conditions": [
        {
            "attributeName": "rank",
            "operator": ">",
            "value": 3
        },
        {
            "attributeName": "role",
            "operator": "startsWith",
            "value": "admin"
        }
    ]
}
```

11. Get Resource:
* Request Example:
* GET  http://localhost:7075/api/resources/keyvault

* Response Example:
```json
{
    "resourceId": "keyvault",
    "policyIds": [
        "1",
        "2",
        "3"
    ]
}
```

12. Create Resource:
* Request Example:
POST http://localhost:7075/api/resources
Body:
```json
{
    "resourceId": "CosmosDB",
    "policyIds": ["10", "20", "30"]
}
```
* Response Example:
```json
{
    "resourceId": "CosmosDB",
    "policyIds": [
        "10",
        "20",
        "30"
    ]
}
```

13. Update Resource's Policies:
PUT http://localhost:7075/api/resources/CosmosDB
Body:
```json
{
    "value": ["1", "2", "3"]
}
```
* Response Example:
```json
{
    "resourceId": "CosmosDB",
    "policyIds": [
        "1",
        "2",
        "3"
    ]
}
```

14. Get Authorization:
* Request Example:
* GET  http://localhost:7073/api/isAuthorized?userId=tomg&resourceId=keyvault

* Response Example:
```json
{
    "isAuthorized": true
}
```

## Non-user-facing API Schemas
15. Create Authorization Request:
* Request Example:
POST http://localhost:5860/api/authorizationEngine
Body:
```json
{
    "policyIds": ["1", "2", "3"],
    "attributes":
    {
        "rank": 6,
        "role": "admin"
    }
}
```
* Response Example:
```json
{
    "containsFulfilledPolicy": true,
    "policyFulfilledId": "3"
}
```

16. Create Pre-Authorization Request:
* Request Example:
POST http://localhost:7151/api/preAuthorizationEngine
Body:
```json
{
    "policyIds": ["1", "50"],
    "userLastUpdated": "2023-11-22T09:00:00.594Z",
    "userApprovedPolicies":
        {
            "1": "2023-11-20T09:00:00.594Z",
            "2": "2023-11-20T09:00:00.594Z"
        }
}
```
* Response Example:
```json
{
    "fulfilledPolicie": [],
    "unFulfilledPolicies": [
        "1"
    ]
}
```