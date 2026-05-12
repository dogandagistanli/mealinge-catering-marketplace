# Mealinge - CENG 382 Web Project Report

## Project Summary

Mealinge is an ASP.NET Core MVC web application built with Entity Framework Core and ASP.NET Identity. The project models a catering marketplace with three roles: Admin, Caterer, and User. Users browse nearby caterers, customize menu items, use a session-based cart, simulate payment, receive order notifications, download dynamic PDFs, rate completed orders, and use an order-connected live call after purchase.

## Main Technologies

- ASP.NET Core MVC
- Entity Framework Core with SQL Server
- ASP.NET Identity role authentication
- Session-based cart storage
- Google Maps JavaScript API and browser geolocation
- SMTP email notification service
- Dynamic PDF generation service
- SignalR and WebRTC for live order calls

## Seeded Demo Accounts

| Role | Email | Password |
|---|---|---|
| Admin | admin@mealinge.com | 123456 |
| Caterer | caterer1@mealinge.com | 123456 |
| Caterer | caterer2@mealinge.com | 123456 |
| Caterer | caterer3@mealinge.com | 123456 |
| User | user1@mealinge.com | 123456 |

## Implemented Required Features

- Admin, Caterer, and User roles are seeded with ASP.NET Identity.
- Role-based dashboards show statistics relevant to each role.
- Caterers and admins can create, edit, delete, and manage menu items.
- Menu item image upload is supported with basic file validation.
- Menu items support grouped customization options, removable ingredients, optional additions, and price changes.
- Users can add customized menu items to a session cart, update quantity, remove items, and see calculated totals.
- Payment is simulated and creates paid orders with order item details.
- Users and caterers receive order notification records after payment.
- Dynamic receipt and agreement PDFs are generated from order data.
- Ratings are restricted to completed paid orders and tied to order/order item data.
- Google Maps and browser geolocation are used for nearby caterer discovery.
- A saved user location can filter the menu list to nearby caterers.
- Application events are stored in the logging table and shown to admins.
- Logs, emails, orders, menu browsing, and menu management include filtering or pagination where appropriate.
- The UI includes custom Mealinge branding, responsive styling, and light/dark theme support.

## Implemented Bonus Features

### Email-Based Two-Factor Authentication

Users can enable or disable email-based 2FA from the Security page. When enabled, login generates a time-limited email code. The user must verify the code before being signed in.

### Live Call System After Purchase

Completed paid orders expose a Live Call page. Access is restricted to the purchasing user and the caterer connected to that order. SignalR is used for call signaling and WebRTC is used for browser audio communication.

## Demo Flow

1. Sign in as a user.
2. Open Nearby Kitchens, find location, and browse nearby menus.
3. Open a menu item detail page.
4. Select customizations and add the item to the cart.
5. Update quantity or remove items from the cart if needed.
6. Complete the payment simulation.
7. Open the success page links for receipt, agreement, and live call.
8. Rate individual completed order items from the Orders page.
9. Sign in as a caterer and review caterer dashboard/order/live call access.
10. Sign in as admin and review dashboards, logs, and email records.

## Notes

The project uses `appsettings.example.json` to show the required configuration keys. Real Google Maps and SMTP credentials should be managed with user secrets or environment variables before public submission.
