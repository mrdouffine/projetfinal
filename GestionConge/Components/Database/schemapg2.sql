--ALTER TABLE utilisateurs ADD COLUMN motdepasse TEXT;
--ALTER TABLE utilisateurs ADD COLUMN superieurid INT;
ALTER TABLE utilisateurs ADD CONSTRAINT fk_superieur FOREIGN KEY (superieurid) REFERENCES utilisateurs(id);