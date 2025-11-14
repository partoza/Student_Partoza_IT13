-- ============================================
-- CREATE DATABASE
-- ============================================
CREATE DATABASE DB_Student_Partoza_IT13;
GO

USE DB_Student_Partoza_IT13;
GO

-- ============================================
-- CREATE tbEmployee TABLE
-- ============================================
CREATE TABLE tbEmployee (
    empID INT IDENTITY(1,1) PRIMARY KEY,
    empFirstName NVARCHAR(100) NOT NULL,
    empLastName NVARCHAR(100) NOT NULL,
    empUsername NVARCHAR(50) NOT NULL,
    empPassword NVARCHAR(50) NOT NULL
);
GO

-- ============================================
-- CREATE tbStudent TABLE
-- ============================================
CREATE TABLE tbStudent (
    studID INT IDENTITY(1,1) PRIMARY KEY,
    studName NVARCHAR(100) NOT NULL,
    studLastName NVARCHAR(100) NOT NULL,
    studBirthdate DATE NOT NULL,
    studYearLevel NVARCHAR(20) NOT NULL,
    studProgram NVARCHAR(50) NOT NULL,
    createdByEmployee INT NOT NULL,
    status VARCHAR(20) NOT NULL
);
GO

-- ============================================
-- ADD FOREIGN KEY (createdByEmployee → empID)
-- ============================================
ALTER TABLE tbStudent
ADD CONSTRAINT FK_tbStudent_tbEmployee
FOREIGN KEY (createdByEmployee) REFERENCES tbEmployee(empID);
GO

-- ============================================
-- INSER DUMMY DATA FOR EMPLOYEE
-- ============================================
INSERT INTO tbEmployee (empFirstName, empLastName, empUsername, empPassword)
VALUES ('John Rex', 'Partoza', 'johnrex', 'admin123');
GO

-- ============================================
-- INSER DUMMY DATA FOR STUDENTS
-- ============================================
INSERT INTO tbStudent 
(studName, studLastName, studBirthdate, studYearLevel, studProgram, createdByEmployee, status)
VALUES
('Maria', 'Lopez', '2003-05-12', '1st Year', 'BSIT', 1, 'Active'),

('Joshua', 'Cruz', '2002-11-23', '2nd Year', 'BSCS', 1, 'Active'),

('Angela', 'Santos', '2004-02-14', '1st Year', 'BSIS', 1, 'Pending'),

('Mark', 'Reyes', '2001-08-09', '3rd Year', 'BSIT', 1, 'Active'),

('Kim', 'Delos Santos', '2003-12-01', '1st Year', 'BSCS', 1, 'Dropped');
GO

