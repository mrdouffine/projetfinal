-- ⚠️ Assure-toi que les tables existent déjà avant d'exécuter ceci

-- 🔄 Réinitialiser (facultatif)
--DELETE FROM Validation;
--DELETE FROM DemandeConge;
--DELETE FROM PlanningConge;
--DELETE FROM Rappel;
--DELETE FROM Utilisateur;

-- 👥 Utilisateurs
INSERT INTO Utilisateur (Id, Nom, Email, MotDePasse, Role, SuperieurId)
VALUES 
  (1, 'Alice Dupont', 'alice@example.com', 'hashed_pw1', 'Employe', 2),
  (2, 'Bob Martin', 'bob@example.com', 'hashed_pw2', 'RH', 4),
  (3, 'Claire Durand', 'claire@example.com', 'hashed_pw3', 'Employe', 2),
  (4, 'David DOT', 'david@example.com', 'hashed_pw4', 'DOT', 0),
  (5, 'Admin Root', 'admin@example.com', 'hashed_pw5', 'Admin', 0);

-- 📆 Demandes de congé
INSERT INTO DemandeConge (Id, UtilisateurId, DateDebut, DateFin, Motif, Statut, DateSoumission)
VALUES 
  (1, 1, '2025-08-20', '2025-08-25', 'Vacances', 'Validé', '2025-08-01'),
  (2, 3, '2025-08-15', '2025-08-18', 'Mariage', 'En attente', '2025-08-03');

-- 🧾 Validations
INSERT INTO Validation (Id, DemandeCongeId, ValideurId, Statut, Commentaire, DateValidation)
VALUES 
  (1, 1, 2, 'Validé', 'Bonnes vacances', '2025-08-02'),
  (2, 1, 4, 'Validé', 'OK pour DOT', '2025-08-03'),
  (3, 2, 2, 'En attente', NULL, '2025-08-03');

-- 📋 Plannings
INSERT INTO PlanningConge (Id, UtilisateurId, DateDebut, DateFin, Motif, DateCreation)
VALUES 
  (1, 1, '2025-08-20', '2025-08-25', 'Vacances', GETDATE()),
  (2, 3, '2025-08-15', '2025-08-18', 'Mariage', GETDATE());

-- 🔔 Rappels
INSERT INTO Rappel (Id, Message, DateRappel, UtilisateurId)
VALUES 
  (1, 'Préparez votre retour de congé le 25 août', '2025-08-22', 1),
  (2, 'Pensez à finaliser votre demande de congé', '2025-08-04', 3);
