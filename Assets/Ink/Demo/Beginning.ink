->A1
VAR Deduction = 0

=== A1 ===
Animals move through the skyscrapers of the city. On the subway, a hardworking bee gets a call: "Hey, Scrum Master, please come to the studio immediately to meet the client..."
Our client is here. Mr. Sloth, our Product Owner, just started eating and says he’ll need about five more hours, so you’ll have to fill in for him. #C:Squirrel:0

*Okay, no problem!#C:Bee:0
->A2
*Ugh, again!#C:Bee:0
->A2

= A2
I want a management game with a vegetarian restaurant theme. #C:Sheep:0

*A vegetarian restaurant? Great idea!#C:Bee:0
->A3
*Why not make it KFC instead?#C:Bee:0
->A2a
*We've made thousands of games like this.#C:Bee:0
->A2b

=A2a
A friend of mine said that Kalev Fried Chicken tasted really weird, and you know, we sheep eat veggies. #C:Sheep:0
Well, I haven't tried it yet. But my job lately has made me want to become a carnivore... Please continue. #C:Bee:0
->A3

=A2b
What's that supposed to mean? My idea isn't innovative enough? #C:Sheep:0
I think you've got it wrong. I'm the client, me!#C:Sheep:0
Sorry, that's not what I meant. Please continue.#C:Bee:0
->A3

=A3
Now I'd like to talk about what I expect from this game, so please understand me carefully. If you misinterpret me, you will be docked performance points.#C:Sheep:0
First, the main function.#C:Sheep:0
The main character is a sheep who runs a restaurant with a few sheep employees. Every day, the menu needs to be set up and displayed outside so other sheep can see it and come in to eat. #C:Sheep:0
After they finish, the player taps the money left on the tables.#C:Sheep:0

*Sure. A payment system.#C:Bee:0
~Deduction ++
->A3z
*Sure. A restaurant operation system.#C:Bee:0
->A4

*Sure. A cooking system#C:Bee:0
~Deduction ++
->A3z

=A3z
Ugh...No, I mean a restaurant operating system. You've been deducted {Deduction}. #C:Sheep:0
Okay, let me continue.#C:Sheep:0
->A4

=A4
Players also freely mix a few ingredients to create new dishes. If a new dish becomes popular, it boosts the restaurant’s reputation, attracting more sheep. #C:Sheep:0

*Got it, cooking system.#C:Bee:0
~Deduction ++
->A4z

*Got it, ingredients creation.#C:Bee:0
~Deduction ++
->A4z

*Got it, recipe creation.#C:Bee:0
->A5

=A4z
Ugh...No, I mean a recipe creation system. You've been deducted {Deduction}. #C:Sheep:0
Okay, let me continue.#C:Sheep:0
->A5

=A5
By the way, an interactive interface is the basis for a working game.#C:Sheep:0
I will need a playable prototype of the game, as you should be well aware, so I won't go into that.#C:Sheep:0
OK, the main features have been covered. Now I'm going to talk about some other features.#C:Sheep:0
Of course, if a restaurant wants to grow, it can’t just stick to the basics.#C:Sheep:0
Players could do some marketing, like buying ads or inviting influencers, yeah, they call them Lead Sheeps, to make vlogs.#C:Sheep:0
You know, lots of sheep just follow whatever the Lead Sheeps do. That's what sheeps do.#C:Sheep:0

*Understood. Influencer recruitment.#C:Bee:0
~Deduction ++
->A5z
*Understood. Marketing features.#C:Bee:0
->A6
*Understood. Bribe the leaders.#C:Bee:0
~Deduction ++
->A5z

=A5z
No, what? I meant to add a marketing feature to the game. You've been deducted {Deduction}. #C:Sheep:0
Now let me continue..#C:Sheep:0
->A6

=A6
With enough money, they could upgrade the restaurant or renovate it. Oh, and I’d love cozy, relaxing grassland music with cute sound effects. #C:Sheep:0

*upgrades & renovation, music & sound.#C:Bee:0
->A7
*Resto relocation, music & sound.#C:Bee:0
~Deduction ++
->A6z
*Cute and cozy decoration with grass.#C:Bee:0
~Deduction ++
->A6z

=A6z
What I was talking about is that players can upgrade and refurbish restaurants. What are you talking about...?#C:Sheep:0
You've been deducted {Deduction}. Okay, let me continue.#C:Sheep:0
->A7

=A7
Lastly, I'd like to introduce some additional features. These don’t affect gameplay much, but I think they’d make things more fun. #C:Sheep:0
I’d like the visuals to change with the weather.#C:Sheep:0
Got it. Weather-based visuals.#C:Bee:0
And players should be able to dye their employees’ wool, or dress them in stylish clothes. #C:Sheep:0
Sometimes I’d love to see a restaurant full of blue gothic sheep. how funny would that be? #C:Sheep:0

*Yeah, blue gothic sheeps.#C:Bee:0
~Deduction ++
->A7z
*Yeah, customizable employee appearance.#C:Bee:0
->A8
*Yeah, main character dress-up system.#C:Bee:0
~Deduction ++
->A7z

=A7z
I think you're wrong. I mean, I want to define how the employees look. #C:Sheep:0
You've been deducted {Deduction}. Now let me continue.#C:Sheep:0
->A8

=A8
Umm...Holiday events like Grass Festival could give players a mystery recipe and cash rewards, like Grass Cake.#C:Sheep:0

*Okay, gifts from the Mystic.#C:Bee:0
~Deduction ++
->A8z
*Okay, create a mystery recipe.#C:Bee:0
~Deduction ++
->A8z
*Okay, holiday rewards.#C:Bee:0
->A9

=A8z
No, I mean players should receive gifts for the holidays. It's called a holiday reward.#C:Sheep:0
In total, You've been deducted {Deduction}. Not bad, but hopefully you'll get better results afterward.#C:Sheep:0
->A9

=A9
I think that’s all. Looking forward to your work. Good luck! #C:Sheep:0
-> END
