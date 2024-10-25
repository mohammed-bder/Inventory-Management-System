### *<h1 align="center">✨Inventory Management System (IMS)✨</h1>*
The **Inventory Management System (IMS)** is a comprehensive web-based application designed to streamline and manage inventory operations efficiently. Built using the ASP.NET MVC framework, this system enables users to track products, suppliers, and stock levels, while offering features like low-stock alerts to ensure smooth inventory management. The system is intended for businesses of all sizes, allowing them to maintain an organized stock system and prevent shortages or excess.
<hr>

![Dashboard](https://github.com/mohamed-belall/Inventory-Management-System/blob/main/ERD/Dashboard.png)
![Analytics Dashboard](https://github.com/mohamed-belall/Inventory-Management-System/blob/main/ERD/Dashboard_analysis.png)

## 🌟*Features*

- **Product Management**: Add, edit, delete, and categorize products.
- **Supplier Management**: Manage supplier details like name, contact information, and address.
- **Stock Tracking**: Monitor stock levels and receive low-stock alerts for efficient restocking.
- **Role-Based Access**: User authentication with role assignment for better control over access and permissions.
- **Employee Management**: Manage employee details and view assigned roles.
- **Advanced Filtering and Searching**: Filter products by status and search products by name.
- **CRUD Operations**: Full Create, Read, Update, and Delete operations for products, suppliers, categories, and employees.
- **Responsive Dashboard**: The system is fully responsive, offering an intuitive user experience across different devices.
- **Authentication**: Secures the system by allowing only registered users to access the application.
- **Authorization**: Ensures that users can only perform actions based on their assigned roles (e.g., Admin, Employee).
- **Change Password**: Users can securely update their password through the **Change Password** feature.
- **Forgot Password**: If a user forgets their password, they can reset it via the **Forgot Password** feature.

## 🛠️*Technologies Used*

- **ASP.NET Core MVC**: For structuring the web application.
- **Entity Framework Core**: For data management and interaction with the SQL Server database.
- **SQL Server**: For efficient data storage and retrieval.
- **Bootstrap**: For responsive and modern front-end design.
- **JavaScript**: For enhancing interactivity and client-side functionality.
- **LINQ**: For querying data.
- **ASP.NET Identity**: For user authentication and role management.
- **EPPlus Package**: For exporting data to Excel files.
- **iTextSharp Package**: For reading and manipulating PDF documents programmatically.
## 📖*Usage*
* ## 📊Dashboard
    * **Interactive Cards**: Dynamic content with icons, values, hover effects, and "More Info" buttons.
    * **Analytics Dashboard**: Integrated data visualizations with various charts.
    * **Sales Area Chart**: Displays sales trends with customized tooltips and legends.
    * **Top Rating Employees (Pie Chart)**: Shows top performers with distinct colors and labeled tooltips.
    * **Top Selling Products (Bar Chart)**: Displays top-selling products with interactive tooltips and legends.
    * **Responsive Layout**: Optimized for all screen sizes using Bootstrap's grid system.
    * **Custom Styles**: Unique animations, hover effects, and a centrally aligned layout with custom buttons and font adjustments.
    * **User-Friendly Interface**: Clear headings, icons, and easy navigation for better usability.

* ## 📦Product Management
    * **CRUD Operations**: Create, read, update, and delete products with attributes like name, price, quantity, and category, including validation for required fields.
    * **Stock Management**: Automatically updates stock levels after each transaction and prevents transactions if stock is insufficient.
    * **Fetch Available Products**: Displays only products with stock greater than zero in transaction forms, ensuring smooth processing.

* ## 💳Transaction Management
    * **Transaction Creation**: Facilitates new transactions by linking products with employees, ensuring valid product availability and stock levels.
    * **Product-Transaction Management**: Tracks the quantity of each product sold in a many-to-many relationship between products and transactions.
    * **Stock Updates**: Automatically reduces product stock based on the quantities in each transaction, maintaining real-time consistency.
    * **PDF Receipt Generation**: Generates a detailed PDF receipt after each transaction, including product names, quantities, prices, total amounts, and the employee involved.
    * **Excel Export**: Optionally exports transaction records to Excel, ideal for reporting and sharing with stakeholders.
    * **Transaction Deletion**: Admin users can delete or archive transactions to manage database size, while maintaining data integrity.
    * **Employee-Linked Transactions**: Tracks which employee processed each transaction for accountability and performance tracking.
    * **Error Handling**: Ensures valid input, preventing negative quantities or invalid product selections, with user notifications for invalid actions.

* ## 🔑Role Management, Authorization & Authentication
    * **User Authentication**: Ensures secure login and access to the system through username and password validation.
    * **Role Assignment**: Supports creating and managing user roles, allowing different access levels and permissions based on job functions.
    * **Authorization Control**: Restricts access to specific features and data based on user roles, enhancing security and protecting sensitive information.
    * **Password Management**: Provides functionality for users to change their passwords, including options for resetting forgotten passwords.
    * **Session Management**: Maintains user sessions securely, logging users out after a period of inactivity to protect accounts.
    * **Error Handling**: Validates user inputs during login and registration, notifying users of authentication errors or unauthorized access attempts.

* ## 🧑‍💼Employee Management

    * **CRUD Operations**: Allows for the creation, reading, updating, and deletion of employee records, including attributes such as name, role, contact information, and employment status.
    * **Role Assignment**: Supports assigning roles and permissions to employees for access control and functionality within the system.
    * **Employee Performance Tracking**: Monitors employee performance and transaction history, facilitating accountability and evaluation.
    * **Authentication and Authorization**: Ensures secure access to the system, allowing employees to manage their accounts and change passwords as needed.
    * **Error Handling**: Validates inputs to prevent invalid data entry and notifies users of errors during employee record management.

* ## ⚠Stock Tracking

    * **Real-Time Alerts**: Notifies users of critical events such as low stock levels, pending transactions, or important system updates.
    * **Customizable Alert Settings**: Allows users to configure alert preferences based on their roles and responsibilities within the system.
    * **Alert History**: Maintains a log of past alerts for review and auditing purposes, ensuring accountability and transparency.
    * **Integration with Stock Management**: Automatically triggers alerts when stock levels fall below predefined thresholds, supporting proactive inventory management.
    * **Error Handling**: Ensures alerts are generated based on valid data and provides user notifications for any issues encountered.

* ## 🏢Supplier Management

    * **CRUD Operations**: Enables the creation, reading, updating, and deletion of supplier records, including details like name, contact information, and address.
    * **Supplier-Product Relationship**: Manages the relationship between suppliers and products, ensuring accurate tracking of product sources.
    * **Order Management**: Facilitates placing orders with suppliers when stock levels are low, helping to maintain product availability.
    * **Performance Tracking**: Monitors supplier performance based on delivery times, product quality, and reliability, aiding in informed decision-making.
    * **Error Handling**: Validates supplier data entries to ensure accuracy and notify users of potential issues.

## 🧑‍💻Authors

- [@Mohammed Hassan](https://www.linkedin.com/in/mohammed-bder/)
- [@Mohammed Belal](https://www.linkedin.com/in/mohamed-belall/)
- [@Mohammed Hazem](https://www.linkedin.com/in/mohamed-hazem-1bb4b1231/)
- [@Mohammed Alaa](https://www.linkedin.com/in/mohamed-gad-eng/)

## 📝Feedback

If you have any feedback, please reach out to us at MelioTech@gmail.com





## ⚖️License

[MIT](https://choosealicense.com/licenses/mit/)

