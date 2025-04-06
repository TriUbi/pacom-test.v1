-- Skapa databas
CREATE DATABASE IF NOT EXISTS device_db;
USE device_db;

-- Skapa tabellen "Status" för enheterna
CREATE TABLE IF NOT EXISTS Status (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    IsOn BOOLEAN NOT NULL,
    CoilAddress INT NOT NULL
);

-- (EXEMPEL DATA, VALFRITTTT) 
INSERT INTO Status (Name, IsOn, CoilAddress) VALUES
('Entrékamera', 0, 0),
('Robotkamera A-16', 1, 1),
('Dörrsensor F-34', 1, 2);
