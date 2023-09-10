CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Chats" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "ActivePollId" integer NULL,
    CONSTRAINT "PK_Chats" PRIMARY KEY ("Id")
);

CREATE TABLE "Polls" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "CreatedAt" timestamp with time zone NOT NULL,
    "EndedAt" timestamp with time zone NULL,
    "WinnerName" text NULL,
    "WinnerUrl" text NULL,
    CONSTRAINT "PK_Polls" PRIMARY KEY ("Id")
);

CREATE TABLE "Videos" (
    "Name" text NOT NULL,
    "Url" text NOT NULL,
    "WasInPoll" boolean NOT NULL,
    "Watched" boolean NOT NULL,
    "PollId" integer NULL,
    CONSTRAINT "PK_Videos" PRIMARY KEY ("Name", "Url"),
    CONSTRAINT "FK_Videos_Polls_PollId" FOREIGN KEY ("PollId") REFERENCES "Polls" ("Id")
);

CREATE INDEX "IX_Chats_ActivePollId" ON "Chats" ("ActivePollId");

CREATE INDEX "IX_Polls_WinnerName_WinnerUrl" ON "Polls" ("WinnerName", "WinnerUrl");

CREATE INDEX "IX_Videos_PollId" ON "Videos" ("PollId");

ALTER TABLE "Chats" ADD CONSTRAINT "FK_Chats_Polls_ActivePollId" FOREIGN KEY ("ActivePollId") REFERENCES "Polls" ("Id");

ALTER TABLE "Polls" ADD CONSTRAINT "FK_Polls_Videos_WinnerName_WinnerUrl" FOREIGN KEY ("WinnerName", "WinnerUrl") REFERENCES "Videos" ("Name", "Url");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230910061820_initial', '7.0.10');

COMMIT;

START TRANSACTION;

ALTER TABLE "Polls" ALTER COLUMN "Id" DROP IDENTITY;

ALTER TABLE "Chats" ALTER COLUMN "Id" DROP IDENTITY;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230910062414_no_auto_add', '7.0.10');

COMMIT;


