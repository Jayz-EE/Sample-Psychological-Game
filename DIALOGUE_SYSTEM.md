# 💬 Enhanced Dialogue System

## Overview

The dialogue system has been redesigned to be **role-agnostic**, preventing players from deducing NPC roles through conversation patterns. All NPCs can use any dialogue, creating ambiguity and strategic depth.

## Key Improvements

### 1. Role-Agnostic Dialogues

**Before:**
- Dialogues were tied to specific roles (e.g., Detective-only lines)
- Players could identify roles by dialogue patterns
- Limited dialogue pool

**After:**
- All dialogues available to all NPCs
- No role-specific filtering
- Observations are ambiguous and could apply to any role

### 2. Expanded Dialogue Pool

**Total Dialogues: 35+**

| Context | Count | Purpose |
|---------|-------|---------|
| Neutral | 5 | Casual conversations |
| Suspicious | 8 | Questioning without revealing role |
| Fearful | 6 | Panic and anxiety |
| Trusting | 5 | Sharing information |
| Aggressive | 4 | Hostility |
| Rumor | 5 | Information spreading |

### 3. Ambiguous Observations

#### Example Scenarios

**Scenario 1: "I saw Jake outside last night"**
- **Could be said by:**
  - Detective (tracking movements)
  - Butcher (scouting victims)
  - Vagabond (sleeping outdoors)
  - Doctor (checking on patients)
  - Farmer (protecting crops)
  - Any role that was outside

**Scenario 2: "I found blood near the church"**
- **Could be said by:**
  - Detective (investigating)
  - Butcher (left evidence)
  - Doctor (treating wounds)
  - Anyone who passed by

**Scenario 3: "Someone was near Anna's house"**
- **Could be:**
  - Butcher targeting Anna
  - Doctor protecting Anna
  - Detective watching Anna
  - Vagabond wandering
  - Anyone with suspicions

## Dialogue Contexts

### Neutral Context (5 dialogues)
Everyday conversations that don't reveal roles:
- "Good morning. Another day in this cursed village."
- "I barely slept last night. Did you hear those strange noises?"
- "How are you holding up? These are difficult times."
- "I can't remember the last time I felt safe here."
- "This village used to be peaceful. What happened to us?"

### Suspicious Context (8 dialogues)
Questioning without role reveals:
- "Where were you last night? I didn't see you at home."
- "I saw you near the forest after dark. What were you doing there?"
- "Your story doesn't add up. You said you were home, but someone saw you outside."
- "I noticed you've been avoiding the council meetings. Why is that?"
- "You've been acting strange lately. Is there something you're not telling us?"
- "I found blood near your house. Care to explain?"
- "Several people mentioned seeing you wandering at night. What's going on?"
- "Don't think I haven't noticed. You're always around when something bad happens."

### Fearful Context (6 dialogues)
Panic that any role could express:
- "Someone is killing us! We need to do something before we're all dead!"
- "I can't sleep anymore. Every sound makes me jump. Who's next?"
- "We should leave this village. Nothing good will come from staying here."
- "I don't trust anyone anymore. How do we know who the killer is?"
- "I heard screaming last night. Did you hear it too?"
- "Three people dead already. When will this nightmare end?"

### Trusting Context (5 dialogues)
Sharing information without revealing role:
- "I saw something last night. I think I know who might be involved."
- "I'm glad I can talk to you. I don't know who else to trust."
- "Thank you for being honest with me. We need to stick together."
- "Between you and me, I think I know who we should be watching."
- "Whatever happens, I've got your back. We'll figure this out together."

### Aggressive Context (4 dialogues)
Hostility without role identification:
- "You better watch yourself. Accidents happen around here."
- "Stay away from me. I know what you've been doing."
- "Don't lie to me! I saw you with my own eyes!"
- "You think you're clever, but I'm watching you."

### Rumor Context (5 dialogues)
Information spreading:
- "I heard from someone that there was blood found near the church."
- "People are saying they saw someone sneaking around last night."
- "I'm not sure if it's true, but I heard someone was seen near the victim's house."
- "Word is going around that someone's been stealing food at night."
- "Everyone's talking about the footprints found near the forest."

## Player Response Options

### Expanded to 5-6 options per context

#### Suspicious Context Responses:
1. "I stayed inside my house all night." (Trust +5, Suspicion -10)
2. "I was walking around. I couldn't sleep." (Suspicion +5, Trust +3)
3. "That's none of your concern." (Trust -15, Suspicion +20, Spreads Rumor)
4. "I could ask you the same question." (Suspicion +8, Trust -5)
5. "I heard noises and went to investigate." (Suspicion +3, Trust +5)
6. "Why are you so interested in my whereabouts?" (Suspicion +10, Trust -8)

#### Fearful Context Responses:
1. "We'll get through this together. Stay calm." (Trust +15, Fear -10)
2. "You should be afraid. We all should." (Fear +15, Trust -5)
3. "Panicking won't help anyone." (Fear -5, Trust +8)
4. "Have you noticed anything suspicious?" (Trust +5, Suspicion +5)
5. "Maybe we should leave the village." (Fear +10, Trust +3)

#### Trusting Context Responses:
1. "Thank you for trusting me. What did you see?" (Trust +15)
2. "I appreciate your honesty. We need allies." (Trust +12)
3. "Tell me everything you know." (Trust +10, Suspicion +3)
4. "I have information too. Let's share." (Trust +18)
5. "Are you sure we can trust each other?" (Trust -5, Suspicion +8)

#### Aggressive Context Responses:
1. "Are you threatening me?" (Suspicion +15, Trust -10, Fear +5)
2. "Back off. I haven't done anything wrong." (Suspicion +10, Trust -8)
3. "Maybe you're the one we should be watching." (Suspicion +20, Trust -15, Spreads Rumor)
4. "Let's calm down and talk rationally." (Trust +5, Fear -5)
5. "I don't have to explain myself to you." (Suspicion +18, Trust -12)

#### Rumor Context Responses:
1. "Tell me more. Who told you this?" (Trust +5, Spreads Rumor)
2. "I heard something similar from someone else." (Spreads Rumor, Suspicion +5)
3. "We shouldn't spread unconfirmed information." (Trust +8, Fear -3)
4. "That's interesting. I'll keep an eye out." (Trust +5, Suspicion +3)
5. "Sounds like gossip to me." (Trust -5)

#### Neutral Context Responses:
1. "How are you holding up?" (Trust +5)
2. "Have you seen anything unusual lately?" (Suspicion +3, Trust +3)
3. "We should stick together in these times." (Trust +8)
4. "I'm worried about what's happening here." (Trust +5, Fear +5)
5. "Let's talk later. I have things to do." (Trust -3)

## Council Statements

### Ambiguous Observations (20+ variations)

**Death Comments:**
- "We've lost X people. This madness has to stop!"
- "X dead already. Who will be next?"
- "Another death. We need to find who's responsible."
- "How many more must die before we act?"
- "The killer is among us. We must be vigilant."

**Evidence Comments:**
- "I found [evidence] near [location]. Someone was there."
- "There's [evidence] at [location]. We should investigate."
- "I noticed [evidence] by [location] this morning."
- "Has anyone else seen the [evidence] near [location]?"
- "The [evidence] at [location] wasn't there yesterday."

**NPC Observations:**
- "I saw [Name] outside last night."
- "[Name] was near [location] after dark."
- "I noticed [Name] acting strangely yesterday."
- "[Name] wasn't home when I checked."
- "I heard footsteps near [Name]'s house."
- "[Name] has been avoiding me lately."
- "I saw [Name] talking to someone in secret."
- "[Name] was carrying something suspicious."
- "I found [Name] near the scene this morning."

**General Suspicion:**
- "Someone here knows more than they're saying."
- "The killer is sitting among us right now."
- "We need to be more careful about who we trust."
- "I don't feel safe anymore."
- "Someone is lying. I can feel it."
- "We should watch each other more closely."
- "There's a pattern to these deaths."
- "The evidence points to someone in this room."
- "We're running out of time to find the truth."

## Accusation Reasons

### Ambiguous Justifications (20+ variations)

**Evidence-Based:**
- "Found near suspicious evidence at [location]"
- "Was seen at [location] where evidence was discovered"
- "Too many coincidences linking them to [location]"
- "Evidence at [location] points in their direction"

**Rumor-Based:**
- "Multiple reports of suspicious behavior"
- "Too many people have seen them acting strange"
- "The rumors about them keep piling up"
- "Everyone's talking about their suspicious activities"
- "I've heard too many concerning things about them"

**Behavioral:**
- "Always wandering around at night"
- "Avoiding eye contact and acting nervous"
- "Their story keeps changing"
- "They were outside when they claimed to be home"
- "Too defensive when questioned"
- "Acting suspiciously during council meetings"
- "Seen near multiple crime scenes"
- "Can't account for their whereabouts"
- "Others have noticed their strange behavior"
- "They know too much about the deaths"
- "Always has an excuse ready"
- "Trying too hard to deflect suspicion"
- "Their alibis don't check out"
- "Caught in multiple lies"

**Intuition-Based:**
- "Something about them doesn't feel right"
- "My instincts tell me they're involved"
- "Process of elimination points to them"
- "They're the most suspicious person here"
- "I have a bad feeling about them"

## Rumor Contexts

### 20 Ambiguous Contexts

1. "Seen near the forest at night"
2. "Acting suspiciously"
3. "Heard strange noises from their direction"
4. "Wandering outside after dark"
5. "Seen near someone's house"
6. "Found near the crime scene"
7. "Acting nervous and avoiding people"
8. "Carrying something in the dark"
9. "Whispering with someone secretly"
10. "Leaving their house at odd hours"
11. "Seen with blood on their clothes"
12. "Acting differently lately"
13. "Avoiding the council meetings"
14. "Lying about their whereabouts"
15. "Seen arguing with the victim"
16. "Found in a restricted area"
17. "Behaving erratically"
18. "Seen running from somewhere"
19. "Hiding something"
20. "Making suspicious trades"

## Strategic Implications

### For Players

**Deduction Challenges:**
- Can't identify roles by dialogue alone
- Must cross-reference multiple sources
- Behavioral patterns more important than words
- Context matters more than content

**Social Manipulation:**
- Any response could be strategic
- Defensive behavior doesn't confirm guilt
- Helpful behavior doesn't confirm innocence
- Trust must be earned through actions, not words

### For NPCs

**Realistic Behavior:**
- All NPCs can make similar observations
- Vagabonds blend in naturally
- Butchers don't stand out in conversation
- Detectives don't reveal themselves

**Emergent Storytelling:**
- Unique narratives each playthrough
- Unpredictable accusation patterns
- Complex social dynamics
- Realistic paranoia and mistrust

## Example Gameplay Scenario

### Night 2 - Multiple Observations

**Anna (Detective):** "I saw Jake outside last night near Jenna's house."
**Jake (Vagabond):** "I was just walking around. I couldn't sleep."
**Marcus (Butcher):** "I saw someone near there too. Very suspicious."
**Jenna (Doctor):** "I heard footsteps outside my window."

### Analysis

**Who could be telling the truth?**
- Anna: Could be investigating OR stalking
- Jake: Could be innocent OR scouting
- Marcus: Could be deflecting OR genuinely observant
- Jenna: Could be a victim OR lying

**No role is revealed** - all statements are plausible for multiple roles.

## Benefits

### 1. Enhanced Mystery
- Roles remain hidden longer
- More strategic gameplay
- Increased replayability

### 2. Realistic Social Dynamics
- Natural conversations
- Believable accusations
- Organic suspicion building

### 3. Strategic Depth
- Players must analyze patterns
- Context becomes crucial
- Multiple interpretations possible

### 4. Balanced Gameplay
- No role has dialogue advantage
- Vagabonds naturally suspicious
- All roles equally viable

---

**The dialogue system now creates true social deduction gameplay where words alone cannot reveal the truth. 🎭**
