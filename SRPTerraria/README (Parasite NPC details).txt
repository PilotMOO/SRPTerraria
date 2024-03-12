All Parasite NPCs will inherit from either ParasiteBaseNPC or ParasiteAdaptableNPC (latter has not been made yet), rather than ModNPC

All Parasites will be sorted and sorted in their respective tier folders, Rupter in inborn, pri yellow eye in Primitive, etc.

Make sure the namespace lines up, and please implement a config for the mob! (Each tier will have their own config folder, each with their own Header)

At the start of PreAI() for all ParasiteBaseNPC/ParasiteAdaptableNPC, please run the method `this.FireWeakness(NPC);` to make the parasite take more damage from fire