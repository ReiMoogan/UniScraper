# UniScraper - Scraping UC Merced's Classes
It's basically BobcatCourses's backend but made by some naïve freshman.

## But why?
Why not?

Well, Ellucian Banner decided to update to a slightly more modern interface, meaning you *kinda* get JSON output.
Better than HTML parsing, until you realize they mixed that in.
Honestly, I developed this repo because I wanted play with databases.

## What's included

This contains the API (and hopefully a UI eventually) written in ASP.NET Core. It fetches data from SQL Server and will sort out data in JSON.
See the controllers for routing info, or the Swagger docs when running. It also fetches data from RateMyProfessor, which is isolated in its own class library.

V1 of the API is an attempt to be backwards-compatible with BobcatCourses. If I ever get to a V2, it will implement JSON properly instead of
[whatever this is](https://cdn.discordapp.com/attachments/601998175301140480/981057295679127552/unknown.png).
Due to the different data sources, the API is a from-scratch rewrite, so data differences will likely occur.

There also contains a Visual Studio Database Project, which contains the table structures and Stored Procedures/Functions used.
This can be used to set up a database if the project is to be replicated. Note that this is designed in SQL Server (with full-text search).
Porting to MySQL, MongoDB, or any other DB system is not my top priority.

## Thanks to...
- [HelloAndrew](https://github.com/classAndrew), who figured out how to set up cookies automatically for [Moogan](https://github.com/classAndrew/cow).
- [Miguel Hernandez](https://github.com/miguelHx) and [dragonbone81](https://github.com/dragonbone81), who have solved this problem before.
  - See their [backend](https://github.com/dragonbone81/bobcat-courses-backend) and [frontend](https://github.com/miguelHx/bobcat-courses).
