window.onload = function() {

var game = new Phaser.Game(800, 600, Phaser.AUTO, 'Html5Client', { preload: preload, create: create });

var ProtoBuf = dcodeIO.ProtoBuf;
var builder = ProtoBuf.loadProtoFile("CommsMessages.proto")
var CommsMessages = builder.build("CommsMessages")
var Ping = CommsMessages.Ping;
var Header = CommsMessages.Header;
var NewUser = CommsMessages.NewUser;
var socket;

//var socket = new WebSocket("ws://localhost:8000/socket/server/startDaemon.php");
//
//socket.onopen = function(){
    //console.log("Socket has been opened!");
//}
//socket.onmessage = function(msg){
    //console.log(msg);
//}

function connect() {
    try {
        var host = "ws://zen:8081";// /socket/server/startDaemon.php";
        var socket = new WebSocket(host);

        console.log('Socket Status: '+socket.readyState);

        socket.onopen = function() {
            console.log('Socket Status: '+socket.readyState+' (open)');
            login(socket);
        }

        socket.onmessage = function(msg) {
            console.log('Received: '+msg.data);
        }

        socket.onclose = function() {
            console.log('Socket Status: '+socket.readyState+' (Closed)');
        }            
    } catch(exception) {
        console.log('Error'+exception);
    }
}

function login(socket) {
    try {
        console.log('Login...');
        var hdr = new Header({"msgtype":4, "from":"html5", "dest":"cloud" });
        var msg = new NewUser({"username":"sean", "password":"pass", "name":"Sean"});
        var hdrBuffer = hdr.encode().toArrayBuffer();
        var msgBuffer = msg.encode().toArrayBuffer();
        var array = Buffer.concat(hdrBuffer, msgBuffer);
        socket.send(array);
        console.log('Sent: '+array)
    } catch(exception) {
       console.log('Error:' + exception);
    }
}

//socket.close();


function preload() {
    //  We load a TexturePacker JSON file and image and show you how to make several unique sprites from the same file
    game.load.atlas('iso-outside', 'resources/iso-64x64-outside.png', 'resources/iso-64x64-outside.json');

    //game.load.image('block', 'resources/block.png');
}

var chick;
var car;
var mech;
var robot;
var cop;

function create() {

    connect();

    game.stage.backgroundColor = '#404040';

    var mapsprites = [
      1,1,1,1,1,1,1,1,1,1,
      1,1,1,1,1,2,1,1,1,1,
      1,1,1,2,2,3,2,2,1,1,
      1,1,1,2,3,3,3,2,2,1,
      1,1,2,1,3,3,3,3,2,1,
      1,1,1,2,3,1,3,2,2,1,
      1,1,2,3,1,1,3,2,1,1,
      1,1,2,2,2,2,3,1,1,1,
      1,1,1,2,1,1,1,1,1,1,
      1,1,1,1,1,1,1,1,1,1,
    ];

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
                var block = game.add.sprite(400+(x-y)*32,128+(x+y)*16-z*21, 'iso-outside');
                switch (mapsprites[y*10+x]) {
                    case 1:
                        block.frameName = 'dirt.png';
                        break;
                    case 2:
                        block.frameName = 'rock.png';
                        break;
                    case 3:
                        block.frameName = 'rock.png';
                        break;
                }
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
