CREATE DATABASE MunicipalityManagement;
GO

USE MunicipalityManagement;
GO

CREATE TABLE Citizens (
    CitizenID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(255) NOT NULL,
    Address VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(10) NOT NULL,
    Email VARCHAR(255) UNIQUE,
    DateOfBirth DATETIME,
    RegistrationDate DATETIME DEFAULT GETDATE(),
 
);
GO


CREATE TABLE ServiceRequests (
    RequestID INT PRIMARY KEY IDENTITY(1,1),
    CitizenID INT,
    ServiceType VARCHAR(255) NOT NULL,
    RequestDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (CitizenID) REFERENCES Citizens(CitizenID)
);
GO

CREATE TABLE Staff (
    StaffID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(255) NOT NULL,
    Position VARCHAR(255) NOT NULL,
    Department VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(10) NOt NULL,
    HireDate DATETIME
);
GO

CREATE TABLE Reports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    CitizenID INT,
    ReportType VARCHAR(255) NOT NULL,
    Details VARCHAR(500) NOT NULL,
    SubmissionDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(50) DEFAULT 'Under Review',
    FOREIGN KEY (CitizenID) REFERENCES Citizens(CitizenID)
);
GO
