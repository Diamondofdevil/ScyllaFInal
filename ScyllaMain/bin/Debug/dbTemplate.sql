-- Lists table, used for identifing the lists used for the brute force attacks, ID refers to the type of list (not documented already) but it could be user, pass, user-list, user list or user/list. Path is the path where the list is.
CREATE TABLE Lists ( ID INT NOT NULL, PATH NVARCHAR(200) NOT NULL, PRIMARY KEY (ID, PATH) );
-- This table contains the messages of each module. 
--ID = id of the Session; 
--Module = id of the Module who stores the message; 
--host_ = the attacked host; 
--Type = the type of the message, defined by the module. the only not permitted codes are: 
--			NoMessage = -1
--          USER_PASSWORD_FOUND = 0
--          ERROR_MESSAGE = 1
--Here is the OracleModule codes (used for an example)
--public enum OracleMessageType
--        {
--            //Defaults
--            NoMessage = -1,
--            USER_PASSWORD_FOUND = 0,
--            ERROR_MESSAGE = 1,
--            //Oracle
--            //username:created_time
--            USER = 2,
--            //version
--            VERSION = 3,
--            //sids
--            SID = 4,
--            //just role name
--            ROLE = 5,
--            //Profile:pwdPolicy
--            PASSWORD_POLICY = 6,
--            //username:status:profile
--            STATUS = 7,
--            //User:role
--            USER_ROLE = 8,
--            //Tables user can see
--            TABLES = 9,
--            //username:hash
--            HASHES = 10,
--            LINKS = 11,
--            //username:schemaname:osuser:machine:terminal:program:module:logon_time
--            AUDIT_INFO = 12
--        }
--UserName = the user who launches the attack (if it's a pre-hack it could be "");
CREATE TABLE Messages ( ID INT NOT NULL, Module INT NOT NULL, host_ NVARCHAR(300), Type INT, Message NVARCHAR(2048), UserName NVARCHAR(300), PRIMARY KEY (username,ID,Module,Type,Message) );
-- If a module is going to be attacked, it must me added to this table (ID = Session ID)
CREATE TABLE ModuleFound ( ID INT NOT NULL, Module INT NOT NULL, PRIMARY KEY (ID, Module) );
-- Sessions, ID = session ID, Comments = comments for this session, Name = name of the session, ActualModule = indicates which module was the user attacking after he quits
CREATE TABLE Session ( ID INT NOT NULL IDENTITY(1,1), Comments NVARCHAR(1000), Name NVARCHAR(100) NOT NULL, ActualModule INT, PRIMARY KEY (ID) );
-- Info about the thread state before the user quits. ThreadPos indicates the index of the list at which this thread was
CREATE TABLE ThreadInfo ( ID INT NOT NULL, ThreadPos INT NOT NULL, PRIMARY KEY (ID, ThreadPos) );
-- table that identifies the users (username) and passwords (pass) for the session ID, the module Module and the host host_
CREATE TABLE USER_PASS ( ID INT NOT NULL, Module INT NOT NULL, UserName NVARCHAR(70), Pass NVARCHAR(70) NOT NULL, host_ NVARCHAR(300), PRIMARY KEY (ID, Module, UserName, Pass, host_) );
-- table to store lists information, ID the id of the list type, data, the text you wanna store
CREATE TABLE TextData ( ID INT NOT NULL, DATA NVARCHAR(200) NOT NULL, PRIMARY KEY (ID, DATA));

ALTER TABLE Lists ADD CONSTRAINT fk2 FOREIGN KEY (ID) REFERENCES Session (ID);
ALTER TABLE Messages ADD CONSTRAINT fk7 FOREIGN KEY (ID, Module) REFERENCES ModuleFound (ID, Module);
ALTER TABLE ModuleFound ADD CONSTRAINT fk3 FOREIGN KEY (ID) REFERENCES Session (ID);
ALTER TABLE ThreadInfo ADD CONSTRAINT fk1 FOREIGN KEY (ID) REFERENCES Session (ID);
ALTER TABLE USER_PASS ADD CONSTRAINT fk5 FOREIGN KEY (ID, Module) REFERENCES ModuleFound (ID, Module);
-- session tiene moduleFound, moduleFound tiene hosts y hosts tiene Messages

INSERT INTO TextData (ID,DATA) VALUES (1, 'user');
INSERT INTO TextData (ID,DATA) VALUES (1, 'usuario');
INSERT INTO TextData (ID,DATA) VALUES (1, 'name');
INSERT INTO TextData (ID,DATA) VALUES (1, 'pass');
INSERT INTO TextData (ID,DATA) VALUES (1, 'usr');
INSERT INTO TextData (ID,DATA) VALUES (1, 'contra');
INSERT INTO TextData (ID,DATA) VALUES (1, 'hash');
INSERT INTO TextData (ID,DATA) VALUES (1, 'pwd');
INSERT INTO TextData (ID,DATA) VALUES (1, 'secret');
INSERT INTO TextData (ID,DATA) VALUES (1, 'login');
INSERT INTO TextData (ID,DATA) VALUES (1, 'auth');