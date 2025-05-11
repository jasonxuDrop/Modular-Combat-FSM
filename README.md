# Modular Combat FSM

![Modular Combat State Machine Project Thumbnail](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Images/Thumbnail%2016_9.png)

​A modular Finite State Machine system used for a character with complex attack combos. The system also includes various editor tools to help debug, and help make changes faster. Due to the paid asset used, this repo only contains various code snippets I wrote, and is not a complete Unity project. 

- Demo: https://jasonxxr.itch.io/modular-combat-fsm-demo (itch.io)
- Documentation: https://kuma-alchemy.notion.site/Modular-Combat-FSM-1efbf26ea72b80a48567c88013d24021?pvs=74 (Notion)



## State Machine Architecture

![State Machine Components](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Images/State%20Machine%20Components.png)

![Inheritance Graph](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Images/Inheritence%20Graph.png)

Core Scripts
- [State Machine](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Assets/Scripts/State%20Machine/Core/StateMachine.cs) 
- [State](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Assets/Scripts/State%20Machine/Core/State.cs)
- [State Action](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Assets/Scripts/State%20Machine/Core/StateAction.cs)
- [State Transition](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Assets/Scripts/State%20Machine/Core/StateTransition.cs)
- [IPredicate](https://github.com/jasonxuDrop/Modular-Combat-FSM/blob/main/Assets/Scripts/Predicate/IPredicate.cs)



## Learn More

See the design rational, usage guide, and editor tools in the [documentation](https://kuma-alchemy.notion.site/Modular-Combat-FSM-1efbf26ea72b80a48567c88013d24021?pvs=74). 

Note that some of the scripts requires assets like Odin Inspector and More Mountain Tools to function properly. 
