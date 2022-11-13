# Goals

The goal of the first phase of the development is to provide a version of the application that

* Is able to handle up to 2000 clients

* Is able to handle up to 20000 requests/day with peak load of 100 requests/minute.

* Demonstrates the ability for further development and scaling.

* Demonstrates the best architecture and development practices.

* The labour intensity is limited to 6 man-months total.

* The first version of the system must be ready to deploy by January, 1 2023.

# Limitations

* First step functionality is limited to (minimum viable product/proof of concept):
  * Minimum backoffice to:
    * See the list of the users/change password/activate or deactivate user;
    * See all customers;
    * See all drivers;
    * See all current and historical requests;
  * Minimum front-end:
    * Register as a customer;
    * Register as a driver;
    * Enter and track request;
    * See requests as a driver and accept it;
    * Update request as a driver;
* Initial front-end is designed to work on browsers.
* On design, optimized primarily to mobile devices is enough.
* The system supports only English and only US locale.

# Process Concept

In order to facilitate fast delivery and maximum ability to adjust the course of action, Agile/Lean based process should be used.

1) The whole work must be split into "domains". Each domain is developed and released independently. 

2) The work is build around Trunk-Based development process. 

The developers must commit working increment daily. 

3) The work is build around Test-Driven development process. All the code written must be covered and tested on the same day basis. 

4) The progress toward iteration goal/overall goal must be shared on the daily basis. 

5) Not later than December 1, we start to target for having continuously running public preview for testing and demonstration purposes. 


The team composition and team agreement on the work details TBD. 