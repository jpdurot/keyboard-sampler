drop table `SoundInfo`;
CREATE TABLE `SoundInfo` (
	`Id`	INTEGER NOT NULL UNIQUE,
	`Name`	TEXT NOT NULL UNIQUE,
	`Uri`	TEXT NOT NULL,
	`Horodate`	bigint NOT NULL,
	`ImageUri`	TEXT,
	PRIMARY KEY(Id)
);