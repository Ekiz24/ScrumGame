->A1
VAR Deduction = 0

=== A1 ===
Animals move through the skyscrapers of the city. On the subway, a hardworking sheepdog gets a call: "Hey, Scrum Master, please come to the studio immediately to meet the client..."
Our client is here. Mr. Sloth, our Product Owner, just started eating and says he’ll need about five more hours, so you’ll have to fill in for him. #C:Squirrel:0
Okay, no problem!#C:Dog:0
->A2

= A2
I want a management game with a vegetarian restaurant theme. #C:Sheep:0
A vegetarian restaurant? Good idea!#C:Dog:0
->A3

=A3
Now I'd like to talk about what I expect from this game, so please understand me carefully. If you misinterpret me, you will be docked points.#C:Sheep:0
First, the main function.#C:Sheep:0
The main character is a sheep who runs a restaurant with a few sheep employees. Every day, the menu needs to be set up and displayed outside so other sheep can see it and come in to eat. #C:Sheep:0
After they finish, the player taps the money left on the tables.#C:Sheep:0

*So, a payment system?#C:Dog:0
~Deduction ++
->A3z
*So, a restaurant operation system?#C:Dog:0
->A3a

*So, a cooking system?#C:Dog:0
~Deduction ++
->A3z

=A3a
Yes, you're correct.#C:Sheep:0
->A4

=A3z
Ugh...No, I mean a restaurant operating system. You've been deducted {Deduction}. #C:Sheep:0
Okay, let me continue.#C:Sheep:0
->A4

=A4
Players also freely mix a few ingredients to create new dishes. If a new dish becomes popular, it boosts the restaurant’s reputation, attracting more sheep. #C:Sheep:0

*So, cooking system?#C:Dog:0
~Deduction ++
->A4z

*So, ingredients creation?#C:Dog:0
~Deduction ++
->A4z

*So, recipe creation?#C:Dog:0
->A4a

=A4a
Yeah, that's what I meant.#C:Sheep:0
->A5

=A4z
Ugh...No, I mean a recipe creation system. You've been deducted {Deduction}. #C:Sheep:0
Okay, let me continue.#C:Sheep:0
->A5

=A5
By the way, an interactive interface is the basis for a working game.#C:Sheep:0
I will need a playable prototype of the game, with suitable interface and smooth operation, as you should be well aware.#C:Sheep:0
*So, UI and UX design?#C:Dog:0
->A55a

*So, an operating system?#C:Dog:0
~Deduction ++
->A55z
*So, a gorgeous interface?#C:Dog:0
~Deduction ++
->A55z

=A55a
Yes, that's what I meant.#C:Sheep:0
->A555

=A55z
No, I mean a good UI and UX design is important for this game. You've been deducted {Deduction}.#C:Sheep:0
Let me continue.#C:Sheep:0
->A555

=A555
So the main features have been covered. Now I'm going to talk about some other features.#C:Sheep:0
Of course, if a restaurant wants to grow, it can’t just stick to the basics.#C:Sheep:0
Players could do some marketing, like buying ads or inviting influencers, yeah, they call them Lead Sheeps, to make vlogs.#C:Sheep:0
Yeah, I know. I'm a sheepdog.#C:Dog:0
So you know, lots of sheep just follow whatever the Lead Sheeps do. That's what sheeps do.#C:Sheep:0

*So you want influencer recruitment?#C:Dog:0
~Deduction ++
->A5z
*So you want a marketing feature?#C:Dog:0
->A5a
*So you want bribing the leaders?#C:Dog:0
~Deduction ++
->A5z

=A5a
Yes, that's what I meant.#C:Sheep:0
->A6

=A5z
No, what? I meant to add a marketing feature to the game. You've been deducted {Deduction}. #C:Sheep:0
Now let me continue..#C:Sheep:0
->A6

=A6
With enough money, players can bring the restaurant up to a higher level and renovate it.#C:Sheep:0

*So, upgrades & renovation?#C:Dog:0
->A6a
*So, resto relocation?#C:Dog:0
~Deduction ++
->A6z
*So, cute and cozy decoration with grass?#C:Dog:0
~Deduction ++
->A6z

=A6a
Yes, exactly.#C:Sheep:0
->A7

=A6z
What I was talking about is that players can upgrade and renovate the restaurant. What are you talking about...?#C:Sheep:0
You've been deducted {Deduction}. Okay, let me continue.#C:Sheep:0
->A7

=A66
Oh, and I’d love cozy, relaxing grassland music with cute sound effects.#C:Sheep:0


*So, you want cute decoration with grass?#C:Dog:0
~Deduction ++
->A66z
*So, you want an album?#C:Dog:0
~Deduction ++
->A66z
*So, you want music and sounds?#C:Dog:0
->A66a

=A66z
No, what? I just want a little music and sound effects. You've been deducted {Deduction}.#C:Sheep:0
Now let me continue.#C:Sheep:0
->A7

=A66a
Yes, that's what I want.#C:Sheep:0
->A7

=A7
Lastly, I'd like to introduce some additional features. These don’t affect gameplay much, but I think they’d make things more fun. #C:Sheep:0
I’d like the visuals to change with the weather.#C:Sheep:0

*So you mean weather-based visuals?#C:Dog:0
->A77a
*So you mean customized weather?#C:Dog:0
~Deduction ++
->A77z
*So you mean a nice background image?#C:Dog:0
~Deduction ++
->A77z

=A77a
Yes, you're correct.#C:Sheep:0
->A777

=A77z
Ugh...No, I mean it should have weather-based visuals. You've been deducted {Deduction}. #C:Sheep:0
->A777

=A777
And players should be able to dye their employees’ wool, or dress them in stylish clothes. #C:Sheep:0
Sometimes I’d love to see a restaurant full of blue gothic sheep. how funny would that be? #C:Sheep:0

*So you want blue gothic sheeps?#C:Dog:0
~Deduction ++
->A7z
*So you want customizable employee appearance?#C:Dog:0
->A7a
*So you want a main character dress-up system?#C:Dog:0
~Deduction ++
->A7z

=A7a
Yes, exactly.#C:Sheep:0
->A8

=A7z
I think you're wrong. I mean, I want to define how the employees look. #C:Sheep:0
You've been deducted {Deduction}. Now let me continue.#C:Sheep:0
->A8

=A8
Umm...Holiday events like Grass Festival could give players a mystery recipe and cash rewards, like Grass Cake.#C:Sheep:0

*You mean gifts from the Mystic?#C:Dog:0
~Deduction ++
->A8z
*You mean creating a mystery recipe?#C:Dog:0
~Deduction ++
->A8z
*You mean holiday rewards?#C:Dog:0
->A8a

=A8a
Yes, that's what I meant.#C:Sheep:0
->A9

=A8z
No, I mean players should receive gifts for the holidays. It's called a holiday reward.#C:Sheep:0
->A9

=A9
I think that’s all. In total, You've been deducted {Deduction}. Looking forward to your work. Good luck! #C:Sheep:0
-> END
