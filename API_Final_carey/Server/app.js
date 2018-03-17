var http = require('http');
var path = require('path');
var express = require('express');
var io = require('socket.io')(process.envPort || 3001);
var shortid = require('shortid');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://localhost:27017/";

var app = express();


app.set('views', path.resolve(__dirname, 'views'));
app.set('view engine', 'ejs');


console.log("Server Started");
//console.log(shortid.generate());

var questions = [];

var dbObj;

MongoClient.connect(url, function (err, client) {
    if (err) throw err;

    dbObj = client.db("users");


    dbObj.collection("questionData").find({}).toArray(function (err, res) {
        if (err) throw err;
        questions = res;
        console.log(questions);
        //client.close();

    });
});



io.on('connection', function (socket) {

    var thisPlayerId = shortid.generate();

    //players.push(thisPlayerId);

    console.log('client Connected');

    //socket.broadcast.emit('spawn player', {id:thisPlayerId});

    socket.emit('connected', { id: thisPlayerId });



    socket.on('connected',
        function (data) {
            console.log('entered connected function.... Contains data - ', data);

            questions.forEach(
                function (question)
                {
                    socket.emit('addQuestion', question);
                });

            socket.emit('initQuestions');
        }
    );


    socket.on('send data',
        function (data) {
            console.log(JSON.stringify(data));
            dbObj.collection("questionData").save(data,
                function (err, res) {
                    if (err) throw err;
                    console.log("data saved to MongoDB");
                });
        });

});


app.get("/", function (request, response) {
    MongoClient.connect(url, function (err, db) {
        if (err) throw err;
        var dbObj = db.db("users");

        dbObj.collection("questionData").find().toArray(function (err, results) {
            console.log("Site Served");
            db.close();
            response.render("index", { questionData: results });
        });

    });

});


app.get("/new-entry", function (request, response) {
    response.render("new-entry");
});



app.post("/new-entry", function (request, response) {
    if (!request.body.title || !request.body.body) {
        response.status(400).send("Entries must have some text!");
        return;
    }
    //connected to our database and saved the games
    MongoClient.connect(url, function (err, db) {
        if (err) throw err;

        var dbObj = db.db("games");

        dbObj.collection("games").save(request.body, function (err, result) {
            console.log("data saved");
            db.close();
            response.redirect("/");
        });

    });

});




http.createServer(app).listen(3000, function () {
    console.log("Game library server started on port 3000");
});



	//socket.on('move', function(data){
	//	data.id = thisPlayerId;
	//	console.log("Player position is: ", JSON.stringify(data));
	//	socket.broadcast.emit('move', data);
	//});

	//socket.on('disconnect', function(){
	//	console.log("Player Disconnected");
	//	players.splice(players.indexOf(thisPlayerId),1);
	//	socket.broadcast.emit('disconnected', {id:thisPlayerId});
	//});

	//socket.on('send data', function(data){
	//	console.log(JSON.stringify(data));

	//	dbObj.collection("playerData").save(data, function(err, res){
	//		if(err)throw err;
	//		console.log("data saved to MongoDB");
	//	});
	//});
