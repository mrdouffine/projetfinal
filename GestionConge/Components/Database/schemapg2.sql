ALTER TABLE utilisateurs ADD COLUMN motdepasse TEXT;
ALTER TABLE utilisateurs ADD COLUMN superieurid INT;
ALTER TABLE utilisateurs ADD CONSTRAINT fk_superieur FOREIGN KEY (superieurid) REFERENCES utilisateurs(id);

ALTER TABLE utilisateurs
ALTER COLUMN superieurid DROP NOT NULL;

-- 👥 Utilisateurs
INSERT INTO utilisateurs (Id, Nom, Email, MotDePasse, Role, SuperieurId)
VALUES 
  (1, 'Alice Dupont', 'alice@example.com', 'hashed_pw1', 'Employe', 2),
  (2, 'Bob Martin', 'bob@example.com', 'hashed_pw2', 'RH', 4),
  (3, 'Claire Durand', 'claire@example.com', 'hashed_pw3', 'Employe', 2),
  (4, 'David DOT', 'david@example.com', 'hashed_pw4', 'DOT', NULL),
  (5, 'Admin Root', 'admin@example.com', 'hashed_pw5', 'Admin', NULL);

-- 📆 Demandes de congé
INSERT INTO demandes_conge (Id, UtilisateurId, Date_Debut, Date_Fin, Motif, Statut, Date_Soumission)
VALUES 
  (1, 1, '2025-08-20', '2025-08-25', 'Vacances', 'Validé', '2025-08-01'),
  (2, 3, '2025-08-15', '2025-08-18', 'Mariage', 'En attente', '2025-08-03');

-- 🧾 Validations
INSERT INTO validations (Id, DemandeCongeId, ValideurId, Statut, Commentaire, DateValidation)
VALUES 
  (1, 1, 2, 'Validé', 'Bonnes vacances', '2025-08-02'),
  (2, 1, 4, 'Validé', 'OK pour DOT', '2025-08-03'),
  (3, 2, 2, 'En attente', NULL, '2025-08-03');

-- 📋 Plannings
INSERT INTO plannings_conge (Id, UtilisateurId, Date_Debut, Date_Fin, Motif, Date_Creation)
VALUES 
  (1, 1, '2025-08-20', '2025-08-25', 'Vacances', NOW()),
  (2, 3, '2025-08-15', '2025-08-18', 'Mariage', NOW());

-- 🔔 Rappels
INSERT INTO rappels (Id, Message, Date_Rappel, UtilisateurId)
VALUES 
  (1, 'Préparez votre retour de congé le 25 août', '2025-08-22', 1),
  (2, 'Pensez à finaliser votre demande de congé', '2025-08-04', 3);


INSERT INTO utilisateurs (Id, Nom, Email, MotDePasse, Role, SuperieurId)
VALUES 
  (6, 'Boblee Martinique', 'kojjey2@gmail.com', 'hashed_pw2', 'RH', 4)

  INSERT INTO validations (Id, DemandeCongeId, ValideurId, Statut, Commentaire, DateValidation)
VALUES 
  (4, 1, 4, 'Validé', 'OK pour Sup', '2025-08-04')

  INSERT INTO demandes_conge (Id, UtilisateurId, Date_Debut, Date_Fin, Motif, Statut, Date_Soumission)
VALUES 
  (3, 6, '2025-08-22', '2025-08-29', 'Vacances', 'Validé', '2025-08-03')
  INSERT INTO validations (Id, DemandeCongeId, ValideurId, Statut, Commentaire, DateValidation)
VALUES 
  (5, 3, 4, 'Validé', 'OK pour Sup', '2025-08-04')