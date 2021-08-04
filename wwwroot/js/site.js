let robot = new Artyom();
robot.initialize({
    lang: 'en-GB',
    continuous: true,
    debug: true,
    listen: true
});

document.body.onunload = function () {
    robot.fatality();
}

function enable(target) {
    robot.say('Hello! Time to perform some commands.');
    document.getElementById(target).remove();
}

robot.addCommands({
    smart: true,
    indexes : [
        "Find me a *",
        "Find me *",
        "Find a *"
    ],
    action: function (i, wildcard) {
        robot.say(`looking for a ${wildcard}`);
        let image = document.getElementById("image");
        let title = document.getElementById("imageTitle");
        title.innerText = wildcard;
        image.src = "https://source.unsplash.com/random/640x480?" + wildcard;
    }
})