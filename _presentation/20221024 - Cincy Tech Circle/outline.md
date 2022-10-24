# Cincyville Tech Circle Monthly Meetup
## Abstract
How we design modern systems is changing fast. It is creating exciting new capabilities but it also creating a few headaches. The Venn diagrams of business-application development-data are overlapping more and more. Let's spend some time talking about what that means for us at Centric and how we have leveraged these new patterns at clients and what new creative ways we can employ in the future

## Outline
### Introduction
* The "tension" created over the last dozen or so years by agility movements
* each advance has created work and capability
### What is Integration Architecture?
#### Integration Styles
* Data-centric
* Application-centric
* Event-centric
#### Reference Architecture
* Quick History of Architecture
  * Definition: Subsystems (app, data, integrations)
  * Monolith
  * SOA
  * ESB
  * Modern Architecture Patterns
    * A focus on smaller subsystems (see definition above)
    * Guiding Principles
      * Business == Technical == Team
        * Business Capability
          * agility
          * Speed to market/value
          * Governable
          * Intelligence
        * Technical
          * Loosely-coupled
          * Composability
          * Design simplicity
          * Scalabilty
          * Robust (async and sync)
          * Telemetry/Instrumentation
          * Operational Simplicity
          * Testable
          * Easy to upgrade/change
        * Team
          * Leverages team strengths
          * Leverages team skills
          * Incremental rollout
          * Non-invaside
          * Non-technical admin
      * Guiding principles show how all are stakeholders
      * Example: Conway's Law
        * ???
    * A Modern Architecture
      * Key Components
        * API Gateway
        * Event Stream
      * Patterns
        * Event-driven Architecture
          * Choreograpy (produce and consume) vs Orchestration (object control)
        * Data in motion
          * have to think about our data differently
          * data is always online