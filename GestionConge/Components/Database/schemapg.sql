-- Supprimer les tables dans l'ordre des dépendances (enfants → parents)
DROP TABLE IF EXISTS validations;
DROP TABLE IF EXISTS rappels;
DROP TABLE IF EXISTS plannings_conge;
DROP TABLE IF EXISTS demandes_conge;
DROP TABLE IF EXISTS utilisateurs;

-- Table Utilisateur
CREATE TABLE utilisateurs (
    id SERIAL PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    role VARCHAR(50) NOT NULL
);

-- Table DemandeConge
CREATE TABLE demandes_conge (
    id SERIAL PRIMARY KEY,
    utilisateurid INT NOT NULL,
    date_debut TIMESTAMP NOT NULL,
    date_fin TIMESTAMP NOT NULL,
    motif TEXT NOT NULL,
    statut VARCHAR(50) NOT NULL DEFAULT 'En attente',
    date_soumission TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_utilisateur_demande
        FOREIGN KEY (utilisateurid)
        REFERENCES utilisateurs(id)
        ON DELETE CASCADE
);

-- Table Validation
CREATE TABLE validations (
    id SERIAL PRIMARY KEY,
    demandecongeid INT NOT NULL,
    valideurid INT NOT NULL,
    statut VARCHAR(50) NOT NULL DEFAULT 'En attente',
    commentaire TEXT,
    datevalidation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_demande_validation
        FOREIGN KEY (demandecongeid)
        REFERENCES demandes_conge(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_valideur_validation
        FOREIGN KEY (valideurid)
        REFERENCES utilisateurs(id)
        ON DELETE CASCADE
);

-- Table PlanningConge
CREATE TABLE plannings_conge (
    id SERIAL PRIMARY KEY,
    utilisateurid INT NOT NULL,
    date_debut TIMESTAMP NOT NULL,
    date_fin TIMESTAMP NOT NULL,
    motif TEXT NOT NULL,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_utilisateur_planning
        FOREIGN KEY (utilisateurid)
        REFERENCES utilisateurs(id)
        ON DELETE CASCADE
);

-- Table Rappel
CREATE TABLE rappels (
    id SERIAL PRIMARY KEY,
    message TEXT NOT NULL,
    date_rappel TIMESTAMP NOT NULL,
    utilisateurid INT NOT NULL,

    CONSTRAINT fk_utilisateur_rappel
        FOREIGN KEY (utilisateurid)
        REFERENCES utilisateurs(id)
        ON DELETE CASCADE
);