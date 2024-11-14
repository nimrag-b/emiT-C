# emiT-C
 A time travelling programming language

emiT is a language all about parallel timelines. At any given point you can send a variable back in time, and make it change things about the past, starting a new timeline where the result is different.
You can kill variables, which destroys them permanantly- at least until you send another variable back in time to kill the variable doing the killing.
This very quickly leads to a lot of confusion, with a constantly changing source code and the very easy possibility of creating a paradox or a time loop. Remember, the timeline doesnt reset when you go back, any changes made before will remain until you go back even further to stop them from happening.

# About
This is a small hobby project, most of which was written in one afternoon, so if you encounter any bugs or anything, please dont hate me too much.

Feel free to branch or edit any of the source, and if you end up making anything cool with it, id love to see :)

join the discord! https://discord.gg/wVy3pXSsF2

elsolang wiki link: https://esolangs.org/wiki/EmiT

syntax highlighting can be found here: https://github.com/nimrag-b/emiT-syntax

# Writing Programs

First I would recommend checking out the spec folder for the current list of language features (and upcoming ones), as well as this README. Then look through the examples in the examples folder to see code in action. Also make sure to check out the VScode syntax highlighter linked above. To run programs, simple just open them with the exe and watch them run.


# Features

Time Warping - By warping back to a point definded earlier in the source, a variable can go back in time and change the source of the project within a new timeline.

Living variables - Every variable is either alive or dead, and has the ability to kill other variables too. Dead variables can no longer affect the program, and the only way to make them not dead is to send another variable back in time to prevent its murder.

Paradoxes - Its easy to make a paradox, wether its by meeting yourself in the past, or trying to access something that has never existed in this timeline. Encountering a paradox will cause the currently timeline to collapse, so make sure you are careful not to cause anything. Consider it an extreme form of error handling.

# Planned Features

Waiting for something to happen after warping, so that you can insert other code snippets in places other than directly after travelling
Proper conflict between two variables being alive at the same time
a Not operator - its more annoying coding without one than i thought.


# Keywords

create - makes a new variable

kills - kills a variable, taking it permanantly out of action

warps - time travels to a point - creating a new timeline splitting off from the original

time - define a new time point in the current timeline;

dead - if a variable has been killed/ doenst exist yet

alive - if a variable hasnt been killed and currently exists

exists - if a variable has ever existed in this timeline

