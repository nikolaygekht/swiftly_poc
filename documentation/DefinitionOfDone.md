# Definition of Done

In order to be considered done, all the code in this product must conform the following conditions:

1) The code must be reviewed by another team member to enure following SOLID principles. 

NOTE: if you don't understand SOLID or not sure whether the code is SOLID, please feel free to discuss it with CTO. 

2) The code must pass static analysis for obeying the best practices. The static analysis tool for each language to be defined. (e.g. C# must use sonar cube)

3) The code must be covered by automated testing. Automated test must satisfy the following criteria:

  * At least 80% of the code must be covered.
  * At least 95% of code directly involved into business usage scenario must be covered. 
  * The test structure must follow the test pyramid principles. 60% or more of testing   
     must be done at the unit-test level, no more than 30% must be done at integration level and no more than 15% must be done at acceptance-level

4) Any code committed to the git must be committed that way to the most recent version of the trunk must be possible to build into a working software. 

