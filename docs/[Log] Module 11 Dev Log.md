<!-- Markdown Docs: https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax -->
## Name: Abiodun Odufuye
### Module: Dev Log 11

<!-- Repeat the below as needed-->
### Date: [05/07/2026]

#### Goals for this Module
<!-- Example Template (include the brackets to make a checklist, fill them in as appropriate
- [ To add a win condition in my project to complete the remaining main game play loop that's currently missing ] Goal 1
- [ To complete the USE and STORE functions for my player tokens ] Goal 2
- [ To complete the GDD for my game ] Goal 3
-->

#### Progress
- **What I accomplished**:
  - Summarize completed tasks or progress made.
    - I managed to finish the remaining features of my prototype to reach a satisfying conclusion which now includes a USE button for player tokens to use the card they've received on their turn, a STORE/INVENTORY button allowing player tokens to store their cards in an inventory and pull up said inventory on their own turn, a win condition hidden underneath any random tile on the map that grants whichever player token who mines it victory, and along with other miscellaneous features tailored to the completion of the prototype
- **Challenges faced**:
  - Describe blockers, bugs, or issues encountered.
    -  I encountered these problems during the last stretch of development: InventoryPanel refusing to show up in the game scene and sometimes won't even display the stored cards by the player tokens; Tile Defense Mechanism wasn't working in that cards no matter the damage value would one-shot any tile disregrading their current defense value, and Card Draw Chance not properly the right cards based on their probability 
- **Solutions**:
  - Detail how you addressed challenges or your thought process.
    -  I fixed the first problem by losing the original CardButtonPrefab I set up and with a simpler approach to have the cards appear via a grid system showing the image of the stored cards in an array. The 2nd fix was me adding tileInMap.SetTileType(tileInMap.tileType); inside the showMap() loop right before tileManager.Initialize() because SetTileType was never called when tiles were created, and finally the last fix was modiifying the InitializeDefaultCards() method as to not override the draw chance values located in the Inspector

#### Learnings
- Key insights, techniques, or concepts explored.
    -  I learned the concept of the cutting room floor and how heartbreaking it is to code barely functioning features where they must remain that way due to time constraints and you have to solider onto easier and necessary features in order to push a conceivable prototype out in the world. I also realized how important it is to constantly check back in your code and the Unity elements to see them in tandem instead of overriding each other to an extent of making some in-game features inoperable.

#### Free Thinking
- Brainstorm or reflect on design ideas, architecture patterns, or potential improvements.
    - I wonder what a fully realized vision my project would be had it not be within the time constraints of a college course
    - I wonder if type of clarity I've given to my game is enough for anyone to pick up, play, and not be confused by it
<!--

- Example prompts:
  - "What if the player interactions were asynchronous instead of real-time?"
  - "How could ECS improve performance in this system?"
  - "Does my current design support scalability? How can it improve?"
  
-->

#### Next Steps
- Tasks or experiments to focus on during the next session.
    -  I'm currently on the road of finishing my assignments for the remaining courses I have for this semester including this one to eventually get the degree I've been yearning for.