All Parasite NPCs will inherit from either ParasiteBaseNPC or ParasiteAdaptableNPC (latter has not been made yet), rather than ModNPC
    Cont; ParasiteAdaptableNPC inherits from ParasiteBaseNPC, but allows that NPC to adapt to damage sources (Not made yet)

All Parasites will be sorted and sorted in their respective tier folders, Rupter in inborn, pri yellow eye in Primitive, etc.

Make sure the namespace lines up, and please implement a config for the mob! (Each tier will have their own config folder, each mob with their own Header)

Before overriding any methods for NPCs that inhert ParasiteBaseNPC or ParasiteAdaptableNPC, please check out their respective classes to make sure that important methods stil get run
    Cont; Also make sure to set any variables from either of those base classes in the new NPC, those are important!