window.onload = function() {

var game = new Phaser.Game(800, 600, Phaser.AUTO, 'Html5Client', { preload: preload, create: create });

function preload() {
    //  We load a TexturePacker JSON file and image and show you how to make several unique sprites from the same file
    //game.load.atlas('atlas', 'elf.bmp', 'texturepacker_test.json');

    game.load.image('block', 'resources/block.png');
}

var chick;
var car;
var mech;
var robot;
var cop;

function create() {

    game.stage.backgroundColor = '#404040';

    var mapdata = [
      1,1,1,1,1,1,1,1,1,1,
      1,1,1,1,1,2,1,1,1,1,
      1,1,1,2,2,3,2,2,1,1,
      1,1,1,2,3,5,4,2,2,1,
      1,1,2,2,5,6,5,3,2,1,
      1,1,1,2,4,5,4,2,2,1,
      1,1,2,3,3,4,5,2,1,1,
      1,1,2,2,2,2,3,1,1,1,
      1,1,1,2,1,1,1,1,1,1,
      1,1,1,1,1,1,1,1,1,1,
    ];

    for (var x=0; x<10; x++){
    	for (var y=0; y<10; y++){
            for (var z=1; z<=mapdata[y*10+x]; z++){
                var block = game.add.sprite(400+(x-y)*32,128+(x+y)*16-z*21, 'block');
                block.anchor.setTo(0.5, 0.5);
            }
        }
    }
    //chick = game.add.sprite(64, 64, 'atlas');

    //  You can set the frame based on the frame name (which TexturePacker usually sets to be the filename of the image itself)
    //chick.frameName = 'budbrain_chick.png';

    //  Or by setting the frame index
    //chick.frame = 0;

    //cop = game.add.sprite(600, 64, 'atlas');
    //cop.frameName = 'ladycop.png';

    //robot = game.add.sprite(50, 300, 'atlas');
    //robot.frameName = 'robot.png';

    //car = game.add.sprite(100, 400, 'atlas');
    //car.frameName = 'supercars_parsec.png';

    //mech = game.add.sprite(250, 100, 'atlas');
    //mech.frameName = 'titan_mech.png';

}

};
