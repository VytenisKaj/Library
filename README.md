## Library web application using .NET6 and ASP.NET

Requirements:

	● As a reader be able to find book by title, author, publisher, publishing date, genre, ISBN code and book status (Is it available)
	● As a reader be able to borrow or reserve a book if it is not reserved or borrowed yet
	● As a library manager be able to register returned books

○ The first requirement is accomplished by "Search book" navigation item.
List of books can be filtered by inputting parameters to search(leaving a field empty will not filter by it, exception "Availability", it has to be chosen between "All", "Available" or "Not available", default "All")

○ The second requirement is accomplished in "Books" navigation item. They can be found next to each book
Reserving a book will automatically reserve it for 1 calendar day and borrowing a book can be inputted manually for up to 3 months from actual date
Each of these actions require you to be logged in as user or admin.

○ The third requirement is accomplished in "Books" navigation item. It can be done by pressing "Returned book".
This will set the book available again. This action require you to be logged on as admin (as well as many other features, like adding or editing books)

For testing 2 accounts are already available:

	● Regular user:
		○ email: user1@library.com
		○ password: 123456
	● Admin user:
		○ email: admin@library.com
		○ password: admin1

You can create new accounts if you wish. To create an admin account, you need to be logged on as admin first and the select "Admin" check box
Email information will not be used, emails can even be fake, they just need to follow standart email rules.
This authentification and authorization method is not final and may or may not be updated later.

NOTE: I know this application is not secure, however I cannot run a server on my side and some sensitive code needs to be in this repository.
However, this is not a production application and no real sensitive user information will be stored.

How to run:

	● Most convenient way is to open this solution in Visual Studio 2022 (older versions should work, but it is not tested)
	and launch using "IIS Express". Make sure you have required modules for ASP.NET.
NOTE: Internet connection is required to run this application!
