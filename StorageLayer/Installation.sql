BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Grades";
CREATE TABLE IF NOT EXISTS "Grades" (
	"Id"	INTEGER NOT NULL,
	"Type"	TEXT NOT NULL,
	"Nom"	TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Specialisations";
CREATE TABLE IF NOT EXISTS "Specialisations" (
	"Id"	INTEGER NOT NULL,
	"Type"	TEXT NOT NULL,
	"Nom"	TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Unites";
CREATE TABLE IF NOT EXISTS "Unites" (
	"Id"	INTEGER NOT NULL,
	"Type"	TEXT NOT NULL,
	"Nom"	TEXT NOT NULL,
	"UnitSpe"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Infractions";
CREATE TABLE IF NOT EXISTS "Infractions" (
	"Id"	INTEGER NOT NULL,
	"Type"	TEXT NOT NULL,
	"Nom"	TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Vehicules";
CREATE TABLE IF NOT EXISTS "Vehicules" (
	"Id"	INTEGER NOT NULL,
	"VehSpe"	TEXT NOT NULL,
	"Nom"	TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Actions";
CREATE TABLE IF NOT EXISTS "Actions" (
	"Id"	INTEGER NOT NULL,
	"Nom"	TEXT NOT NULL,
	"ActInfraction"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Effectifs";
CREATE TABLE IF NOT EXISTS "Effectifs" (
	"Id"	INTEGER NOT NULL,
	"IdDiscord"	TEXT NOT NULL,
	"Nom"	TEXT NOT NULL,
	"Prenom"	TEXT NOT NULL,
	"EffGrade"	TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (1,'CommissaireGeneral','Commissaire Général');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (2,'CommissaireDivisionnaire','Commissaire Divisionnaire');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (3,'Commissaire','Commissaire');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (4,'CommandantDivisionnaire','Commandant Divisionnaire');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (5,'Commandant','Commandant');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (6,'Capitaine','Capitaine');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (7,'Lieutenant','Lieutenant');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (8,'LieutenantStagiaire','Lieutenant Stagiaire');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (9,'MajorRULP','Major RULP');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (10,'MajorEEX','Major à Echelon Exceptionnel');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (11,'Major','Major');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (12,'BrigadierChef','Brigadier Chef');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (14,'GardienDeLaPaix','Gardien de la Paix');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (15,'GardienDeLaPaixStagiaire','Gardien de la Paix Stagiaire');
INSERT INTO "Grades" ("Id","Type","Nom") VALUES (16,'PolicierAdj','Policier Adjoint');
INSERT INTO "Specialisations" ("Id","Type","Nom") VALUES (1,'PS','Police Secours');
INSERT INTO "Specialisations" ("Id","Type","Nom") VALUES (2,'CRS','Compagnie Républicaine de Sécurité');
INSERT INTO "Specialisations" ("Id","Type","Nom") VALUES (3,'SDSS','Sous-Direction des Services Spécialisés');
INSERT INTO "Specialisations" ("Id","Type","Nom") VALUES (4,'DRPJ','Direction Régionale de la Police Judiciaire');
INSERT INTO "Specialisations" ("Id","Type","Nom") VALUES (5,'OPJ','Officier de Police Judiciaire');
INSERT INTO "Specialisations" ("Id","Type","Nom") VALUES (6,'BRI','Brigade de Recherche et d''Intervention');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (1,'TN750','TN 750','PS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (2,'PS120A','PS 120 A','PS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (3,'PS120B','PS 120 B','PS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (4,'PS120C','PS 120 C','PS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (5,'TM120A','TM 120 A','PS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (6,'TM120B','TM 120 B','PS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (7,'MIKE1','MIKE 1','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (8,'MIKE2','MIKE 2','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (9,'MIKE3','MIKE 3','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (10,'MIKE4','MIKE 4','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (11,'PEGASE1','PEGASE 1','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (12,'PEGASE2','PEGASE 2','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (13,'A52A1','52 A1','SDSS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (14,'A52A11','52 A11','SDSS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (15,'A52D2','52 D2','SDSS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (16,'A52D21','52 D21','SDSS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (17,'TM51A','TM 51 A','SDSS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (18,'TM51B','TM 51 B','SDSS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (19,'MONFORT40','MONFORT 40','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (20,'MONFORT41','MONFORT 41','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (21,'MONFORT42','MONFORT 42','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (22,'MONFORT70','MONFORT 70','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (23,'MONFORT71','MONFORT 71','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (24,'MONFORT72','MONFORT 72','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (25,'MONFORT80','MONFORT 80','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (26,'MONFORT81','MONFORT 81','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (27,'MONFORT82','MONFORT 82','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (28,'MONFORT85','MONFORT 85','CRS');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (29,'DRPJ','DRPJ','DRPJ');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (30,'PJUNITE','PJ Unite','OPJ');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (31,'PJALPHA','PJ Alpha','OPJ');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (32,'PJBRAVO','PJ Bravo','OPJ');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (33,'TOPAZ1','TOPAZ 1','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (34,'TOPAZ2','TOPAZ 2','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (35,'TOPAZ3','TOPAZ 3','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (36,'TOPAZ4','TOPAZ 4','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (37,'TOPAZ5','TOPAZ 5','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (38,'TOPAZ6','TOPAZ 6','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (39,'TOPAZ7','TOPAZ 7','BRI');
INSERT INTO "Unites" ("Id","Type","Nom","UnitSpe") VALUES (40,'TOPAZ8','TOPAZ 8','BRI');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (1,'Vitesse','Vitesse');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (2,'Stationnement','Stationnement');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (3,'Autre','Autre');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (4,'Circulation','Circulation');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (5,'Criminalite','Criminalité');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (6,'Drogue','Drogue');
INSERT INTO "Infractions" ("Id","Type","Nom") VALUES (7,'Violence','Violence');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (4,'PS','Peugeot 3008 II');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (5,'PS','Peugeot 5008 II');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (6,'PS','Megane III Estate');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (7,'PS','Megane IV Estate');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (8,'PS','Dacia Duster (ancienne sérigraphie)');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (9,'PS','Dacia Duster (nouvelle sérigraphie)');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (10,'PS','Skoda Octavia III');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (11,'PS','Skoda Octavia III Combi');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (12,'PS','Ford Focus III Break');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (13,'PS','Renault Senic IV');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (14,'PS','Peugeot Rifter');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (15,'PS','Renault Trafic III');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (16,'PS','Renault Master II');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (17,'PS','Peugeot Expert III PMV');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (18,'PS','Volkswagen Passat B8 Break Préfecture de Police');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (19,'PS','Volkswagen ID3 Préfecture de Police');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (20,'PS','Volkswagen Sharan II Préfecture de Police');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (21,'PS','Yamaha FJR 1300');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (22,'PS','Yamaha Tracer 900');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (23,'PS','BMW 1250 RT');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (24,'PS','Yamaha XTZ 1200 Ténéré');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (25,'SDSS','Yamaha FJR 1300');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (26,'SDSS','Yamaha Tracer 900');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (27,'SDSS','BMW 1250 RT');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (28,'SDSS','Yamaha XTZ 1200 Ténéré');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (29,'SDSS','Yamaha MT09 banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (30,'SDSS','Yamaha FJR 1300 banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (31,'SDSS','Yamaha XTZ 1200 Ténéré banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (32,'SDSS','Ford Galaxy basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (33,'SDSS','Peugeot 508 I basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (34,'SDSS','Peugeot 508 I SW basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (35,'SDSS','Peugeot 508 II basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (36,'SDSS','Ford Mondeo IV Break basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (37,'SDSS','Skoda Octavia III basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (38,'SDSS','Skoda Octavia III Combi basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (39,'SDSS','Volkswagen Passat B8 basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (40,'SDSS','Volkswagen Passat B8 Break basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (41,'SDSS','Renault Talisman Estate basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (42,'SDSS','Volkswagen Sharan II basse visibilité');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (43,'SDSS','Volkswagen Sharan II banalisé');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (44,'SDSS','Megane IV banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (45,'SDSS','Megane IV Estate banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (46,'SDSS','Peugeot 3008 II banalisé');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (47,'SDSS','Peugeot 5008 II banalisé');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (48,'SDSS','Peugeot 308 III banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (49,'SDSS','Skoda Octavia III Combi banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (50,'SDSS','DS7 banalisé');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (51,'SDSS','Seat Leon IV FR banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (52,'SDSS','Volkswagen Passat B8 banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (53,'SDSS','Volkswagen Passat B8 Break banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (54,'SDSS','Peugeot 508 I banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (55,'SDSS','Renault Talisman Estate banalisée');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (56,'CRS','Renault K380 lanceur d''eau');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (57,'CRS','Megane IV Estate CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (58,'CRS','Peugeot Expert III PMV CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (59,'CRS','Peugeot Rifter CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (60,'CRS','Renault Trafic III CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (61,'CRS','Renault Trafic III PMV CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (62,'CRS','Renault Master III CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (63,'CRS','Renault Master III PC CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (64,'CRS','Yamaha FJR 1300 CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (65,'CRS','Yamaha Tracer 900 CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (66,'CRS','BMW 1250 RT CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (67,'CRS','BMW 1250 RT CRS 1');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (68,'CRS','Fiat Ducato III CRS');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (69,'BRI','Volkswagen Amarok');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (70,'BRI','Volkswagen Transporter T6');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (71,'BRI','Panhard PVP');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (72,'BRI','Audi RS3');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (73,'BRI','Audi A3 Berline');
INSERT INTO "Vehicules" ("Id","VehSpe","Nom") VALUES (74,'BRI','Mercedes Sprinter III DOPC');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (13,'Circulation avec des plaques sales, illisibles ou non conformes','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (14,'Circulation dans une voie de bus','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (15,'Circulation d’un véhicule en marche normale à une vitesse anormalement réduite','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (16,'Circulation sur la voie du milieu ou sur la gauche sur l’autoroute','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (17,'Circuler à moins de 80 km/h sur la voie de gauche sur l’autoroute','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (18,'Conduite sans le signe A pour un conducteur novice','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (19,'Contrôle technique hors délai','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (20,'Défaut d’assurance','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (21,'Défaut de carte grise','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (22,'Défaut de gilet et de triangle','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (23,'Fumer au volant','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (24,'Infractions liées au stationnement payant (non-paiement ou temps dépassé)','Stationnement');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (25,'Non changement de propriétaire sur la carte grise','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (26,'Non changement d’adresse de la carte grise','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (27,'Non désignation d’un conducteur auteur d’une infraction','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (28,'Non-port du casque à vélo pour les enfants de moins de 12 ans','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (29,'Non-port de la ceinture par un passager majeur','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (30,'Non présentation de la carte grise','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (31,'Non présentation de l’attestation assurance','Autre');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (32,'Non réalisation d''un stage de sensibilisation à la sécurité routière obligatoire en permis probatoire','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (33,'Non-respect du feu orange','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (34,'Pneus usés','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (35,'Refus d’acquittement du péage','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (36,'Stationnement gênant ou abusif ou sur une place « handicapé »','Stationnement');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (37,'Stationnement et l’arrêt sur la bande d’arrêt d’urgence','Stationnement');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (38,'Transport d''un enfant sans dispositif de retenue adapté à son âge','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (39,'Usage abusif des feux de route','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (40,'Usage abusif du klaxon','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (41,'Vitesse excessive eu égard aux circonstances','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (42,'Chevauchement de la ligne continue','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (43,'Excès de vitesse de 20 km/h hors agglomération','Vitesse');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (44,'Franchissement ou chevauchement d’une ligne délimitant une bande d’arrêt d’urgence','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (45,'Non-respect du port des gants à moto','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (46,'Accélération lors d’un dépassement','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (47,'Circulation ou stationnement sur le terre-plein central de l’autoroute','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (48,'Excès de vitesse compris entre 20 et 29 km/h','Vitesse');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (49,'Arrêt ou stationnement dangereux','Stationnement');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (50,'Changement de direction sans clignotant','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (51,'Circulation sans motif sur la partie gauche de la chaussée','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (52,'Circulation sur la bande d’arrêt d’urgence','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (53,'Conduire un véhicule sans respecter les conditions de validité ou les restrictions d’usage du permis de conduire','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (54,'Dépassement dangereux','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (55,'Dépassement par la droite','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (56,'Excès de vitesse compris entre 30 et 39 km/h','Vitesse');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (57,'Franchissement de la ligne continue','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (58,'Non-port de la ceinture de sécurité','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (59,'Non-port du casque','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (60,'Non-respect des distances de sécurité','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (61,'Port d’oreillettes, d’écouteurs et kits mains-libres pendant la conduite','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (62,'Présence dans le champ de vision du conducteur d’un écran','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (63,'Stationnement sur la chaussée la nuit ou par temps de brouillard, en un lieu dépourvu d’éclairage public, d’un véhicule sans éclairage ni signalisation','Stationnement');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (64,'Téléphoner au volant','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (65,'Vitres teintées avec une teinte de moins de 30%','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (66,'Circulation de nuit ou par temps de brouillard en un lieu dépourvu d’éclairage public, d’un véhicule sans éclairage ni signalisation','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (67,'Circulation en sens interdit','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (68,'Excès de vitesse compris entre 40 et 49 km/h','Vitesse');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (69,'Marche arrière ou demi-tour sur autoroute','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (70,'Non-respect de l’arrêt au feu','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (71,'Non-respect de l’arrêt au stop','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (72,'Non-respect de priorité d’un véhicule prioritaire','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (73,'Non-respect du cédez-le-passage','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (74,'Refus de priorité','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (75,'Franchissement d’un passage à niveau','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (76,'Circulation sur une barrière de dégel','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (77,'Conduite après usage de stupéfiants','Drogue');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (78,'Conduite en état alcoolique (taux d’alcoolémie supérieur ou égal à 0,5g/l et inférieur à 0,8g/l)','Drogue');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (79,'Conduite en état d’ivresse manifeste','Drogue');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (80,'Conduite malgré un retrait de permis (rétention, suspension, annulation, invalidation du permis de conduire)','Drogue');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (81,'Délit de fuite','Violence');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (82,'Excès de vitesse supérieur à 50 km/h par rapport à la vitesse maximale autorisée','Vitesse');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (83,'Gêne ou entrave à la circulation','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (84,'Homicide ou blessures involontaires entraînant une incapacité totale de travail','Violence');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (85,'Non-respect du cédez-le-passage à un piéton sur un passage clouté','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (86,'Refus d’obtempérer','Circulation');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (87,'Refus de se soumettre à un test de dépistage de stupéfiants','Drogue');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (88,'Refus de se soumettre à une vérification de présence d’alcool dans le sang','Drogue');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (89,'Usage volontaire de fausses plaques d’immatriculation, défaut volontaire de plaques et fausses déclarations','Criminalité');
INSERT INTO "Actions" ("Id","Nom","ActInfraction") VALUES (90,'Utilisation d''un détecteur de radar, d''un avertisseur ou d''un système antiradar','Circulation');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (1,'294160938968416256','Leroy','Clément','MajorEEX');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (2,'303278510338867200','Paoli','Marcel','Major');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (3,'304573573568397313','Hanks','Eric','Capitaine');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (4,'328947555771613184','Leclerc','Maxime','CommissaireGeneral');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (5,'344974672552787968','Bendone','Yass','CommissaireDivisionnaire');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (6,'351098208488783883','Rieper','Tobias','GardienDeLaPaix');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (7,'351649555244384256','Duke','Numa','Commissaire');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (8,'380844293822349312','Gasly','Joris','Lieutenant');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (9,'411613836353994765','Caprichi','Julien','Commandant');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (10,'430725823570378752','Bogeat','Mickael','CommandantDivisionnaire');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (11,'477511559736852511','Levis','Thomas','Major');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (12,'501080728558370816','Ghost','Wassim','GardienDeLaPaix');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (13,'502146299173273600','Ferrandi','Fabien','Capitaine');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (14,'532575903855804416','Walker','Steven','BrigadierChef');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (15,'572786378593665026','Tyson','Alex','Lieutenant');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (16,'619276382471585792','Dubois','Victor','BrigadierChef');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (17,'647450460160262144','Legrade','Jacques','Major');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (18,'655153366757801996','Dubois','Romain','GardienDeLaPaixStagiaire');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (19,'665642430472781854','Pacotier','Jason','GardienDeLaPaix');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (20,'677154388065910822','Doutier','Benjamin','GardienDeLaPaixStagiaire');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (21,'692082582862430258','Lemieux','Léo','GardienDeLaPaixStagiaire');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (22,'694806576162275369','Stone','Peter','BrigadierChef');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (23,'781163941669634069','Leroy','Augustin','PolicierAdj');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (24,'791067475251822592','Davies','Marcus','Capitaine');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (25,'886334076859068426','Sokolov','Rico','GardienDeLaPaix');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (26,'897182221910286337','Tron','Gael','CommissaireGeneral');
INSERT INTO "Effectifs" ("Id","IdDiscord","Nom","Prenom","EffGrade") VALUES (27,'930428278970384444','Turner','Marc','GardienDeLaPaixStagiaire');
COMMIT;
