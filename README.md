"BalanceUP": 
BalanceUP is a mobile personal budgeting application built with C# and .NET MAUI. It helps users set a monthly budget, log daily expenses, and visualize spending through progress bars and pie charts. The app is designed especially for students to track their financial habits and make more informed spending decisions.
Note: This project was developed as part of a university course. The repository was uploaded to GitHub after the course ended, so there is no commit history or branches from the development phase.

Features:
User Authentication
Secure login and sign-up using email and password with validation.
"Remember Me" option with SecureStorage and Preferences
Password show/hide toggle
Budget Management
Set a monthly budget (one entry per month)
Visualize spending progress with a dynamic progress bar (green <80%, yellow ≥80%, red = exceeded)
Expense Tracking
Log expenses by category (food, bills, shopping, etc.)
Add expenses with category, amount, and date selection
Swipe-to-delete functionality for expense records
Data Visualization
Pie charts using Microcharts.Maui + SkiaSharp
Monthly spending breakdown with navigation between months
Profile Management
Edit username and password
Upload profile picture from camera or gallery
Logout functionality
Offline-first Design
Local storage with SQLite
Works without internet connection

Tech Stack:
Framework: .NET MAUI
Database: SQLite (via sqlite-net-pcl)
Libraries:
Microcharts.Maui
SkiaSharp
Microsoft.Maui.Controls
Microsoft.Extensions.Logging.Debug
Other Features:
SecureStorage
Preferences
SwipeView
MediaPicker
DatePicker

Screens & Flow:
Loading Screen – App logo + activity indicator
Login / Sign-Up – Secure authentication with validation
Home Page – Budget and expenses overview with progress bar + grouped expenses list
Set Budget Page – Define monthly budget
Add Expense Page – Add expenses with category, amount, and date
Chart Page – Pie chart visualization of expenses by category
Profile Page – Manage profile details and logout

Known Limitations:
Local-only authentication – No cloud sync, all data is local.
No password hashing – Passwords stored in plaintext in the local DB.
Charts – Only support monthly breakdowns, not weekly/daily.
Navigation – After setting a budget, user isn’t auto-redirected to Home.
Stability – Occasional crashes when navigating quickly between pages (likely due to unhandled exceptions in .NET MAUI).

Project Notes:
App logo created using IbisPaint
Wireframes designed in Draw.io
Icons from SVGRepo
Password regex validation adapted from Gerald Versluis (YouTube tutorial)

License:
This project was created for educational purposes as part of a university course.
