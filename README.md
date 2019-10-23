# Mu-Yin
Need &amp; Memory based Utility System, powered by Unity Dots(Data Oriented Tech Stack)

# Introduction
I learned from untiy ecs more than half years, inspired by it's performance by default after the first peek, I said I would like to leverage it's power to a great AI system.
The first version was designed 5 months ago, which is ... terrible. I struggled with these whole new dod stuffs, had no idea how to communicate data or behaviours.
Anyway, I think this version is much more better, and even usable for others, so I make it public.

## Need based
Actually there are two meanings of it. 

### Need as consideration 
We as human, make thoughtful decision, which means we consider serveral state before making decision. For example, if you are hungry, you want to eat. However, if you found that there are not much foods in store, you probably more likely to find more food instead of just consume the last one.

### Hirerachy 
This is inspired by Maslow's hierarchy of needs.
Lv0 Survive        Attack/Escape
Lv1 Health         Eat/Drink/GetWater/GetFood/Heating/Heal/Sleep
Lv2 Safety         Earn money/Submission/Patrol
Lv3 Entertain      Play/Sing/Wander/Chat
Lv4 Recognition    Chat/Pursuit Love
Lv5 S-actualize    Help/Build/Work/

## UML
https://drive.google.com/file/d/1iVyVnKvRNnMo9BZtIWrACMlbPPUeruBY/view?usp=sharing

## Timer
I use timer in almost all calculation-intense systems, because we don't think as much as 60 times per second right :) ?

## Hybrid ECS
For now, the motion part & the custom actions part are all hybrid ecs, which implement ComponentSystem.

## Current Actions:
- Sleep

### NextStep
I would like to prototype on NPC to NPC actions, such as attack.

## Feature in future
- More actions
- NPC reaction to NPC
- Memory system
- Dialogus system
- Sensor system
