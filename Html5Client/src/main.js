window.onload = function() {

var game = new Phaser.Game(800, 600, Phaser.AUTO, 'Html5Client', { preload: preload, create: create });

var ProtoBuf = dcodeIO.ProtoBuf;
var builder = ProtoBuf.loadProtoFile("CommsMessages.proto")
var CommsMessages = builder.build("CommsMessages")
var Ping = CommsMessages.Ping;
var Header = CommsMessages.Header;
var Login = CommsMessages.Login;
var socket;

//var socket = new WebSocket("ws://localhost:8000/socket/server/startDaemon.php");
//
//socket.onopen = function(){
    //console.log("Socket has been opened!");
//}
//socket.onmessage = function(msg){
    //console.log(msg);
//}

/**
 * Creates a new Uint8Array based on two different ArrayBuffers
 *
 * @private
 * @param {ArrayBuffers} buffer1 The first buffer.
 * @param {ArrayBuffers} buffer2 The second buffer.
 * @return {ArrayBuffers} The new ArrayBuffer created out of the two.
 */
var _appendBuffer = function(buffer1, buffer2) {
  var tmp = new Uint8Array(buffer1.byteLength + buffer2.byteLength);
  tmp.set(new Uint8Array(buffer1), 0);
  tmp.set(new Uint8Array(buffer2), buffer1.byteLength);
  return tmp.buffer;
};

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
        var hdr = new Header({"msgtype":5, "from":"html5", "dest":"cloud" });
        var msg = new Login({"username":"sean", "password":"pass"});
        var hdrBuffer = hdr.encodeDelimited().toArrayBuffer();
        var msgBuffer = msg.encodeDelimited().toArrayBuffer();
        var data = _appendBuffer(hdrBuffer, msgBuffer); 
        socket.send(data);
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

    var sprites = [
      "null", // 0
      "grass_slope_n.png",  // 1
      "grass_slope_ne.png", // 2
      "grass_slope_nw.png", // 3
      "grass_slope_e.png",  // 4
      "grass_block.png",    // 5
      "grass_slope_w.png",  // 6
      "grass_slope_se.png", // 7
      "grass_slope_sw.png", // 8
      "grass_slope_s.png",  // 9
      "grass_slope_wse.png",// 10
      "grass_slope_nws.png",// 11
      "grass_slope_wne.png",// 12
      "grass_slope_nes.png",// 13
      "rock_outcrop_.png",  // 14
      "rock_slope_n.png",   // 15
      "rock_slope_nw.png",  // 16
      "rock_slope_e.png",   // 17
      "rock_block_1.png",   // 18
      "rock_block_2.png",   // 19
      "rock_slope_w.png",   // 20
      "rock_slope_se.png",  // 21
      "rock_slope_sw.png",  // 22
      "rock_slope_s.png",   // 23
    ]

    for (var x=0; x<10; x++){
        for (var y=0; y<10; y++){
            for (var z=1; z<=mapdata[y*10+x]; z++){
                var block = game.add.sprite(400+(x-y)*32,128+(x+y)*16-z*21, 'iso-outside');
                block.frameName = sprites[mapsprites[y*10+x]]
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
