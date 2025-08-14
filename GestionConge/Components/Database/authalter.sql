ALTER TABLE utilisateurs
ADD RefreshToken NVARCHAR(200) NULL,
RefreshTokenExpiryDate DATETIME NULL;
