ALTER TABLE utilisateurs
ADD COLUMN refreshtoken VARCHAR(200),
ADD COLUMN refreshtokenexpirydate TIMESTAMP;
