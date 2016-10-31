# Praecapatus
Work with Unity 5.4.1f

NOTES ON NOTES:
 - 1 +: start of impl
 - 2 +: fully functional
 - 3 +: manageable from interface
 
NEXT SESSION TASKS:
 - deal with time mechanics 	+
 - implement GameSaver			+
	- item database				+
	- per character saved		+
		- save inventory		+
			- equipment			++
		- save interactions		++
		- save character states	+
			- debuffs			+
 - items
	- equipment					+
	- usables					
 - improve loading/saving		+
	- characters				+
	- map						
 - implement Quest Mechanics	+
	- setting up syntax			+
	- write goal parser			+|
	- GATHER/DELIVER			+
	- CON_NODE					+
	- FIND						+
 - DE/BUFF mechanics			+
	- new class AbstractBuff	+
 - shared animation controller

NOTES ON GAME CONTROL
- Abilities can be used in 2 major ways:
	(1) story mode
		- abis can be called with ++ or without keyboard
		- abis called by keyboard will show version selection screen [by default]
			- this behaviour should be changeable as an option ('disable version selection from shortkeys outside fights') +
	(2) fight mode
		- abis will be used as saved using shortkies (preset)
		- the shortkey includes the specific version, unless better control of ability is possible
		[idea]
		- every X KR, the player is allowed to change his shortkeys
- world rotation +


NOTES ON GAME MECHANICS
- Realtime vs. Gametime ???											+
~ any valid object in the screne contains a PraeObject component	+
- INTERACTIONS																		+
	- interactions are controlled by 'InteractionComponents'						+
		-> any object with this component are subject to interaction				+	
		- any object can become/loose interaction by adding/removing this component	++
		- entities alway have interaction components								++
	~ praeobjects are not PraeObject by default										++
	- shop UI																		++
		- loading, saving															++
	- Talk UI 																		+
		- basic conversation														++
		- interaction events														+
			- open shopUI															++
- CONVERSATIONS
	- conversations can be preset using GraphML based graphs (i.e. using Gephi) ++
	- graphml files can be loaded as TextAssets with ending .txt or .xml	  	++
	- the loader can be found as a method inside class 'Conversation'		  	++
- MAP
	- loading, saving
	- dynmaic generation
	- optimized rotation
- INFAMIE
- RUHM
- ITEMS
	- items can be saved and loaded as xml					++
	- items can be added to /removed from the inventory		++
	- items of a certain class can be equipped				+
	- items can be assigned a uniq id to identify them		++
- PERFORMANCE OPTIMIZATION
	- shared anim controller for background elements like forests
- QUESTS
	- saved as graphml											++
	- the name of the save file is also the name of the quest 	++
	- encoding with specified syntax (see below)				+
	- loading/saving of progress using manager and xml			+


NOTES ON STORY
- the player chooses either 'IndustrieAdept' or 'Technici' as class [Logenmagier]
- magic should have varies useages and version


NOTES ON ABILITIES
- (extremely) prolonged abilities may be handled my making instances of 'ProlongedAbility' (PA)
	- in this case, PA will be a class, that might be independet of 'AbstractAbility'
- fundamential casts
	- Biomantie:		Wachstum [complex]
	- Chronomantie:		Zeitblase
	- Curanistik:		Heilung
	- Evokation:		Astralbelebung [complex]	+
	- Illusionstik:		Illusionsball [complex]
	- Metamagie:		Analyse [complex]
	- Okkultismus:		Pech [complex]
	- Protegonik:		Schnelle Abwehr
	- Pugnistik:		Astralangriff
	- Telekinese:		Telekinetische Blase
	- (Telepathie:		Gefühlsteilung)
- fundamential melee attack
- fundamential range attack

LINES: 9136
 find . -name '*.cs' | xargs wc -l

SYNTAX DEFINITIONS
	QUEST ENCODING
		- node.Attributes = label, description, GoalDesc, GoalType, GoalData
			- string label: abbrevation of the current goal
			- string description: the main description of the quest
		
		- edge.Attributes = label, Alignment, GoalDesc, GoalType, GoalData
			- string label: will be used as sub-title for the side/alignment goal
			- int Alignment: indicates the accumulation of ruhm and infamie for the completion of the quest
		
		- GoalDesc, GoalType and GoalData should always have the same length
			- array separator: " | "
			- string[] GoalDesc: array of descriptions encoding each sub-goal. Can be obsolete for some trivial gaols like gathering
				- automated replacements
					-  @C: replaced by character name based on goal data
			- string[] GoalType:
				- CON_NODE: reach a certain conversation node (through dialogue)
				- DELIVER: bring certain items to another entity by interacting with item
				- GATHER: acquire a certain item or collection of items
				- FIND: acquire of find a certain item in range				
			- string[] GoalData:
				- items: ITEM "item by id" "number to have"
				- chars: CHAR "character id"
				- cons:	 CON "conversation by name" "con node by id"