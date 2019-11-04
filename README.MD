# Web SPA file analyzer (test project)

This is a simple web spa application. The user can browse, read and modify text files located on the server. Text files store tabular data using various column delimiters. This solution does not use SQL or NoSQL.

Used frameworks:
- ASP.NET MVC 5
- Web API 2
- Angular 8

Used dev tools:
- Visual Studio 2019
- Visual Studio Code
- Node.js

How to install and run:
- resolve .net dependencies
- resolve angular dependencies (npm i)
- build Angular (npm run build)
- run web app (F5).

Angular app placed in "WebServer\ClientApp", outputPath is "../Scripts/Angular".

For test project simplicity the following things not implemented/ignored:
- No tests (FE, BE), no validation. (part of it implementing in "add-core-unit-tests" branch)
- The files catalog is given whole at once, without dynamic loading.
- Reading and writing files is performed without concurrent access locks.
- Reading the file directory and file data are synchronous.
- If the file is parsed so that the number of columns in different lines is different, then alignment is not performed (so that a rectangular array is obtained).
- In order not to put the name or path to the file in the URL, the API for parsing and writing files is done through POST (although this is CRUD). If the file was stored in the database (SQL, NoSQL), then "id" would look standard and operations would be conducted through GET and PUT.
