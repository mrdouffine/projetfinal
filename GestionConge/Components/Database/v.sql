-- Ajouter la colonne OrdreValidation à la table Validation
ALTER TABLE "validations" 
ADD COLUMN "ordre_validation" INTEGER NOT NULL DEFAULT 1;

-- Optionnel : Ajouter un commentaire sur la colonne pour la documentation
COMMENT ON COLUMN "validations"."ordre_validation" IS 'Ordre de validation dans le workflow (1 = premier niveau, 2 = second niveau, etc.)';